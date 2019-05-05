﻿using UnityEngine;
using System.Collections;
using SimpleSQL;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;
using UnityEngine.UI;

class TeamData
{
    public string name;
    public int position;
    public int positionAltered;
    public int points;
    public int pointsAltered;
    // for share page
    public int played;
    public int won;
    public int drawn;
    public int lost;
    public int goalsFor;
    public int goalsAgainst;
    public int diff;
}

public class Fixture
{
    public string   Div { get; set; }
    public string   Date { get; set; }
    public string   HomeTeam { get; set; }
    public string   AwayTeam { get; set; }
    public int      FTHG { get; set; } 
    public int      FTAG { get; set; }
    public string   FTR { get; set; }
    public int      HTHG { get; set; }
    public int      HTAG { get; set; }
    public string   HTR { get; set; }
    public string   Referee { get; set; }
    public int      HS { get; set; }        // home - shots
    public int      AS { get; set; }        // away - shots
    public int      HST { get; set; }       // home - shots on target
    public int      AST { get; set; }       // away - shots on target
    public int      HHW { get; set; }       // home - hit woodwork
    public int      AHW { get; set; }       // away - hit woodwork
    public int      HF { get; set; }        // home - fouls
    public int      AF { get; set; }        // away - fouls
} 



public class DataCompiler : MonoBehaviour {

    public SimpleSQL.SimpleSQLManager dbManager;
    public SimpleSQL.SimpleSQLManager[] dbManagerArr;
    SimpleSQL.SimpleSQLManager dbManagerCurr;

    // Main Canvas View
    public Transform ScrollViewCurrentContent;
    public Transform PointsForWinHomeInput;
    public Transform PointsForWinAwayInput;
    public Transform PointsForDrawHomeInput;
    public Transform PointsForDrawAwayInput;
    public Transform DropDownDivision;
    public Transform DropDownYear;
    public Transform CheckboxScoringFirstHalf;
    public Transform CheckboxScoringSecondHalf;
    public Transform CheckboxWoodwork;
    public Transform CheckboxShotOnTarget;
    public Transform CheckboxShot;
    public Transform CheckboxFoul;
    public Transform TotalGamesPlayedInput;
    public Transform TotalGamesPlayedSlider;

    // Share Canvas View
    public Transform[] TableTeams;
    public Transform[] TablePlayed;
    public Transform[] TablePoints;
    public Transform[] TableWon;
    public Transform[] TableLost;
    public Transform[] TableDrawn;
    public Transform[] TableGoalsFor;
    public Transform[] TableGoalsAgainst;
    public Transform[] TableDiff;

    public Transform SliderTableOffset;

    List<Fixture> fixtures;
    Dictionary<string, TeamData> map;
    List<KeyValuePair<string, TeamData>> listAltered;

    // Use this for initialization
    void Start () {
        map         = new Dictionary<string, TeamData>();
        // listAltered = new List<KeyValuePair<string, TeamData>>();
        // string sql  = "SELECT * FROM E0";
        //fixtures = dbManager.Query<Fixture>(sql);
        //CompileTable();
        UpdateDivision();
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public void OnDestroy(){
        dbManager.Close();
        dbManager.Dispose();

        foreach(SimpleSQL.SimpleSQLManager m in dbManagerArr)
        {
            m.Close();
            m.Dispose();
        }

    }

    public void UpdateDivision()
    {
        // Database - get country/year
        int year = DropDownYear.GetComponent<Dropdown>().value;
        dbManagerCurr = dbManagerArr[year];



        // EN
        int div = DropDownDivision.GetComponent<Dropdown>().value;
        string strDiv = "E0";
        switch(div)
        {
            case 0:
                strDiv = "E0";
                break;
            case 1:
                strDiv = "E1";
                break;
            case 2:
                strDiv = "E2";
                break;
            case 3:
                strDiv = "E3";
                break;
            case 4:
                strDiv = "EC";
                break;
        }
        string sql = "SELECT * FROM " + strDiv;
        fixtures = dbManagerCurr.Query<Fixture>(sql);
        CompileTable();
        // set max games
        // total games per team
        int totalTeams = map.Count;
        TotalGamesPlayedSlider.GetComponent<Slider>().maxValue = (totalTeams - 1) * 2;
        TotalGamesPlayedSlider.GetComponent<Slider>().value = (totalTeams - 1) * 2;

        // do again in case we are dont have all games in data (part way through season)
        CompileTable();
    }

    public void CompileTable()
    {
        map.Clear();

        int pointsForWinHome    = int.Parse(PointsForWinHomeInput.GetComponent<InputField>().text);
        int pointsForWinAway    = int.Parse(PointsForWinAwayInput.GetComponent<InputField>().text);
        int pointsForDrawHome   = int.Parse(PointsForDrawHomeInput.GetComponent<InputField>().text);
        int pointsForDrawAway   = int.Parse(PointsForDrawAwayInput.GetComponent<InputField>().text);
        bool scoringFirstHalf   = CheckboxScoringFirstHalf.GetComponent<Toggle>().isOn;
        bool scoringSecondHalf  = CheckboxScoringSecondHalf.GetComponent<Toggle>().isOn;
        bool woodwork           = CheckboxWoodwork.GetComponent<Toggle>().isOn;
        bool onTarget           = CheckboxShotOnTarget.GetComponent<Toggle>().isOn;
        bool shot               = CheckboxShot.GetComponent<Toggle>().isOn;
        bool foul               = CheckboxFoul.GetComponent<Toggle>().isOn;
        // if these remain 0 after all fixtures then we know they are wither not present in database or irrelevant anyway do we should disable the ui
        int totalShots      = 0;
        int totalOnTarget   = 0;
        int totalWoodwork   = 0;
        int totalFouls      = 0;

        //int count = CalculateGameCount(fixtures.Count);
        // get it from value of slider, not text box
        string strTotalGamesAllowed = TotalGamesPlayedInput.GetComponentInChildren<Text>().text;
        int totalGamesAllowed = (int)TotalGamesPlayedSlider.GetComponent<Slider>().value;    //int.Parse(strTotalGamesAllowed);

        Debug.Log(totalGamesAllowed);

        // create map of teams with points collected as value    
        foreach (Fixture fixture in fixtures)
        {

            if (!map.ContainsKey(fixture.HomeTeam)){
                TeamData data = new TeamData();
                data.position       = 0;
                data.points         = 0;
                data.pointsAltered  = 0;
                data.name           = fixture.HomeTeam;
                map.Add(fixture.HomeTeam, data);
            }
            if (!map.ContainsKey(fixture.AwayTeam)){
                TeamData data = new TeamData();
                data.position       = 0;
                data.points         = 0;
                data.pointsAltered  = 0;
                data.name           = fixture.AwayTeam;
                map.Add(fixture.AwayTeam, data);
            }


            if(map[fixture.HomeTeam].played >= totalGamesAllowed)
            {
                // skip this fixture
                continue;
            }
            if (map[fixture.AwayTeam].played >= totalGamesAllowed)
            {
                // skip this fixture
                continue;
            }

            totalShots += fixture.HS + fixture.AS;
            totalOnTarget += fixture.HST + fixture.AST;
            totalWoodwork += fixture.HHW + fixture.AHW;
            totalFouls += fixture.HF + fixture.AF;

            //bool useGoals = true;
            //if (!useGoals)
            //{
            //    if (fixture.FTR == "H")
            //    {
            //        // win
            //        map[fixture.HomeTeam].points = map[fixture.HomeTeam].points + 3;
            //        map[fixture.HomeTeam].pointsAltered = map[fixture.HomeTeam].pointsAltered + 3;
            //    }
            //    else if (fixture.FTR == "A")
            //    {
            //        // lose
            //        map[fixture.AwayTeam].points = map[fixture.AwayTeam].points + 3;
            //        map[fixture.AwayTeam].pointsAltered = map[fixture.AwayTeam].pointsAltered + 3;
            //    }
            //    else
            //    {
            //        // draw
            //        map[fixture.HomeTeam].points = map[fixture.HomeTeam].points + 1;
            //        map[fixture.AwayTeam].points = map[fixture.AwayTeam].points + 1;

            //        map[fixture.HomeTeam].pointsAltered = map[fixture.HomeTeam].pointsAltered + 1;
            //        map[fixture.AwayTeam].pointsAltered = map[fixture.AwayTeam].pointsAltered + 1;
            //    }
            //}
            //else
            //{

            int totalHomeGoals = 0;
            int totalAwayGoals = 0;
            if (scoringFirstHalf && scoringSecondHalf)
            {
                totalHomeGoals += fixture.FTHG; // goals
                totalAwayGoals += fixture.FTAG; // goals
            }
            else
            {
                if (scoringFirstHalf)
                {
                    totalHomeGoals += fixture.HTHG; // goals - first half
                    totalAwayGoals += fixture.HTAG; // goals - first half
                }
                if (scoringSecondHalf)
                {
                    totalHomeGoals += fixture.FTHG - fixture.HTHG; // goals - second half
                    totalAwayGoals += fixture.FTAG - fixture.HTAG; // goals - second half
                }
            }
            //}

            if (onTarget)
            {
                totalHomeGoals += fixture.HST; // shots on target
                totalAwayGoals += fixture.AST; // shots on target
            }

            if (shot)
            {
                totalHomeGoals += fixture.HS; // shots
                totalAwayGoals += fixture.AS; // shots
            }

            if (woodwork)
            {
                totalHomeGoals += fixture.HHW; // woodwork
                totalAwayGoals += fixture.AHW; // woodwork
            }

            if (foul)
            {
                totalHomeGoals += fixture.HF; // fouls
                totalAwayGoals += fixture.AF; // fouls
            }


            //** actual results **
            if (fixture.FTHG > fixture.FTAG)
            {
                // home win
                map[fixture.HomeTeam].points = map[fixture.HomeTeam].points + 3;
            }
            else if (fixture.FTHG < fixture.FTAG)
            {
                // away win
                map[fixture.AwayTeam].points = map[fixture.AwayTeam].points + 3;
            }
            else
            {
                // draw
                map[fixture.HomeTeam].points = map[fixture.HomeTeam].points + 1;
                map[fixture.AwayTeam].points = map[fixture.AwayTeam].points + 1;
            }

            //** altered results based on selections **

            map[fixture.HomeTeam].played++;
            map[fixture.AwayTeam].played++;
            map[fixture.HomeTeam].goalsFor      = map[fixture.HomeTeam].goalsFor + totalHomeGoals;
            map[fixture.HomeTeam].goalsAgainst  = map[fixture.HomeTeam].goalsAgainst + totalAwayGoals;

            map[fixture.AwayTeam].goalsFor      = map[fixture.AwayTeam].goalsFor + totalAwayGoals;
            map[fixture.AwayTeam].goalsAgainst  = map[fixture.AwayTeam].goalsAgainst + totalHomeGoals;

            if (totalHomeGoals > totalAwayGoals)
            {
                // home win
                map[fixture.HomeTeam].pointsAltered = map[fixture.HomeTeam].pointsAltered + pointsForWinHome;
                // share screen stats
                map[fixture.HomeTeam].won++;
                map[fixture.AwayTeam].lost++;
            }
            else if (totalHomeGoals < totalAwayGoals)
            {
                // away win
                map[fixture.AwayTeam].pointsAltered = map[fixture.AwayTeam].pointsAltered + pointsForWinAway;
                // share screen stats
                map[fixture.HomeTeam].lost++;
                map[fixture.AwayTeam].won++;
            }
            else
            {
                // draw
                map[fixture.HomeTeam].pointsAltered = map[fixture.HomeTeam].pointsAltered + pointsForDrawHome;
                map[fixture.AwayTeam].pointsAltered = map[fixture.AwayTeam].pointsAltered + pointsForDrawAway;
                // share screen stats
                map[fixture.HomeTeam].drawn++;
                map[fixture.AwayTeam].drawn++;

            }
            
        }

        // total games per team
        //int totalTeams = map.Count;
        //TotalGamesPlayedSlider.GetComponent<Slider>().maxValue = (totalTeams - 1) * 2;


        // disable ui if not used
        CheckboxWoodwork.GetComponent<Toggle>().interactable = true;
        CheckboxShot.GetComponent<Toggle>().interactable = true;
        CheckboxShotOnTarget.GetComponent<Toggle>().interactable = true;
        CheckboxFoul.GetComponent<Toggle>().interactable = true;
        if (totalWoodwork == 0)
        {
            CheckboxWoodwork.GetComponent<Toggle>().interactable = false;
        }
        if(totalShots == 0)
        {
            CheckboxShot.GetComponent<Toggle>().interactable = false;
        }
        if (totalOnTarget == 0)
        {
            CheckboxShotOnTarget.GetComponent<Toggle>().interactable = false;
        }
        if (totalFouls == 0)
        {
            CheckboxFoul.GetComponent<Toggle>().interactable = false;
        }
        // sort by value
        List<KeyValuePair<string, TeamData>> list = map.ToList();
        list.Sort(
            delegate (KeyValuePair<string, TeamData> pair1,
            KeyValuePair<string, TeamData> pair2)
            {
                if(pair1.Value == pair2.Value)
                {
                    // complex
                    // compare goal difference
                        // if same compare goals for
                            // if same compare head to head games
                }
                return pair2.Value.points.CompareTo(pair1.Value.points);
            }
        );

        //sort by value
        if(listAltered != null)
            listAltered.Clear();
        listAltered = map.ToList();
        listAltered.Sort(
            delegate (KeyValuePair<string, TeamData> pair1,
            KeyValuePair<string, TeamData> pair2)
            {
                if (pair1.Value == pair2.Value)
                {
                    // complex
                    // compare goal difference
                    // if same compare goals for
                    // if same compare head to head games
                }
                return pair2.Value.pointsAltered.CompareTo(pair1.Value.pointsAltered);
            }
        );

        AssignPositions(list, listAltered);

        // set league table view
        int listIndex = 0;
        foreach(Transform trans in ScrollViewCurrentContent)
        {
            bool haveData = true;
            if (listIndex >= listAltered.Count)
            {
                haveData = false;
            }
            Text txt = trans.GetComponent<Text>();
            if (txt)
            {
                if (haveData)
                {
                    txt.text = listIndex + 1 + ". " + listAltered.ElementAt(listIndex).Key;
                    int overBy = txt.text.Length - 13;
                    if (overBy > 1)
                    {
                        txt.text = txt.text.Substring(0, txt.text.Length - overBy);
                        txt.text = txt.text + "..";
                    }
                }
                else
                {
                    txt.text = "";
                }
            }
            if (trans.childCount == 1)
            {
                Transform transChild = trans.GetChild(0);
                if (transChild)
                {
                    Text txtValue = transChild.GetComponentInChildren<Text>();
                    if (txtValue)
                    {
                        if (haveData)
                        {
                            Transform transChildDiff = null;
                            if (txtValue.transform.childCount > 0)
                                transChildDiff = txtValue.transform.GetChild(0);

                            Text txtValueDiff = null; 
                            if(transChildDiff)
                                txtValueDiff = transChildDiff.GetComponentInChildren<Text>();

                            if (txtValueDiff)
                            {
                                txtValueDiff.text = "";
                            }

                            txtValue.text = listAltered.ElementAt(listIndex).Value.pointsAltered.ToString();
                            // colour
                            txtValue.color = Color.white;
                            int diff = listAltered.ElementAt(listIndex).Value.position - listAltered.ElementAt(listIndex).Value.positionAltered;
                            listAltered.ElementAt(listIndex).Value.diff = diff;
                            if (listAltered.ElementAt(listIndex).Value.positionAltered > listAltered.ElementAt(listIndex).Value.position)
                            {
                                txtValue.color = Color.red;
                                //int diff = listAltered.ElementAt(listIndex).Value.position - listAltered.ElementAt(listIndex).Value.positionAltered;
                                // txtValue.text = txtValue.text + " " + diff + "";
                                if (txtValueDiff)
                                {
                                    txtValueDiff.text = diff.ToString();
                                    txtValueDiff.color = Color.red;
                                }
                            }
                            if (listAltered.ElementAt(listIndex).Value.positionAltered < listAltered.ElementAt(listIndex).Value.position)
                            {
                                txtValue.color = Color.green;
                                //int diff = listAltered.ElementAt(listIndex).Value.position - listAltered.ElementAt(listIndex).Value.positionAltered;
                                //txtValue.text = txtValue.text + " +" + diff + "";
                                if (txtValueDiff)
                                {
                                    txtValueDiff.text = "+" + diff.ToString();
                                    txtValueDiff.color = Color.green;
                                }
                            }
                        }
                        else
                            txtValue.text = "";


                    }
                }
            }
            
            listIndex++;
        }
        // map.Clear();
        PopulateTableShare();
    }

    void AssignPositions(List<KeyValuePair<string, TeamData>> original, List<KeyValuePair<string, TeamData>> updated)
    {
        int index = 0;
        foreach(KeyValuePair<string, TeamData> kv in original)
        {
            kv.Value.position = index;
            index++;
        }
        index = 0;
        foreach (KeyValuePair<string, TeamData> kv in updated)
        {
            kv.Value.positionAltered = index;
            index++;
        }    
    }

    public void PopulateTableShare()
    {
        int count = listAltered.Count;
        int offset = 0;// count - 10;
        int index = 0;

        int sliderValue = (int)SliderTableOffset.GetComponent<Slider>().value;
        switch(sliderValue)
        {
            case 0:
                offset = 0;
                break;
            case 1:
                offset = (count / 2) - 5;
                break;
            case 2:
                offset = count - 10;
                break;
        }


        foreach(KeyValuePair<string, TeamData> kv in listAltered)
        {
            // we can offset table
            if (index < offset)
            {
                // dont start yet
                offset--;
                continue;
            }
            string keyTeam = kv.Key;
            TeamData valueData = kv.Value;

            if (TableTeams.Length > index)
            {
                // we have a text item in array
                // name
                Text txt = TableTeams[index].GetComponent<Text>();
                txt.text = keyTeam;
                txt.text = (valueData.positionAltered+1) + "." + txt.text;

                // played
                txt = TablePlayed[index].GetComponent<Text>();
                txt.text = valueData.played.ToString();

                // won
                txt = TableWon[index].GetComponent<Text>();
                txt.text = valueData.won.ToString();

                // lost
                txt = TableLost[index].GetComponent<Text>();
                txt.text = valueData.lost.ToString();

                // drawn
                txt = TableDrawn[index].GetComponent<Text>();
                txt.text = valueData.drawn.ToString();

                // goals for
                txt = TableGoalsFor[index].GetComponent<Text>();
                txt.text = valueData.goalsFor.ToString();

                // goals for
                txt = TableGoalsAgainst[index].GetComponent<Text>();
                txt.text = valueData.goalsAgainst.ToString();

                // points
                txt = TablePoints[index].GetComponent<Text>();
                txt.text = valueData.pointsAltered.ToString();

                // diff
                txt = TableDiff[index].GetComponent<Text>();
                txt.color = Color.white;
                txt.text = valueData.diff.ToString();
                if (valueData.diff > 0)
                {
                    txt.text = "+" + txt.text;
                    txt.color = Color.green;
                }
                if (valueData.diff == 0)
                {
                    txt.text = "";
                    txt.color = Color.green;
                }
                if (valueData.diff < 0)
                {
                    txt.color = Color.red;
                }
            }
            index++;
        }
    }

    // calculate how many games per team from total amount of fixtures
    int CalculateGameCount(int totalfixtureCount)
    {
        // each team plys twice so halve fixtures so we can work out amount of teams
        totalfixtureCount /= 2;

        int nGuess = 0;
        int loop = 4;
        while(nGuess != totalfixtureCount)
        {
            nGuess = Factorial(loop) / (2 * Factorial(loop - 2));
            loop++;
        }
        return (nGuess-1)*2; // play everyone twice
    }

    int Factorial(int value)
    {
        int total = 1;
        for(int i = 2; i <= value; i++)
        {
            total *= i;
        }
        return total;
    }
    
}


