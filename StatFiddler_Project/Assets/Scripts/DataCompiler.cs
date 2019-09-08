using UnityEngine;
using System;
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
    public int playedReal;
    // for share page
    public int played;
    public int won;
    public int drawn;
    public int lost;
    public int goalsFor;
    public int goalsAgainst;
    public int diff;
}

public interface ICloneable<T>
{
    T Clone();
}


public class Fixture : ICloneable<Fixture>
{
    // Deep copy
    public Fixture Clone()
    {
        Fixture F = new Fixture();
        F.AF        = AF;
        F.Div       = Div;
        F.Date      = Date;
        F.HomeTeam  = HomeTeam;
        F.AwayTeam  = AwayTeam;
        F.FTHG      = FTHG;
        F.FTAG      = FTAG;
        F.FTR       = FTR;
        F.HTHG      = HTHG;
        F.HTAG      = HTAG;
        F.HTR       = HTR;
        F.Referee   = Referee;
        F.HS        = HS;
        F.AS        = AS;
        F.HST       = HST;
        F.AST       = AST;
        F.HHW       = HHW;
        F.AHW       = AHW;
        F.HF        = HF;
        F.AF        = AF;
        F.HY        = HY;
        F.AY        = AY;
        F.HR        = HR;
        F.AR        = AR;
        F.HC        = HC;
        F.AC        = AC;
        return F;
    }
    public string   Div         { get; set; }
    public string   Date        { get; set; }
    public string   HomeTeam    { get; set; }
    public string   AwayTeam    { get; set; }
    public int      FTHG        { get; set; } 
    public int      FTAG        { get; set; }
    public string   FTR         { get; set; }
    public int      HTHG        { get; set; }
    public int      HTAG        { get; set; }
    public string   HTR         { get; set; }
    public string   Referee     { get; set; }
    public int      HS          { get; set; }        // home - shots
    public int      AS          { get; set; }        // away - shots
    public int      HST         { get; set; }        // home - shots on target
    public int      AST         { get; set; }        // away - shots on target
    public int      HHW         { get; set; }        // home - hit woodwork
    public int      AHW         { get; set; }        // away - hit woodwork
    public int      HF          { get; set; }        // home - fouls
    public int      AF          { get; set; }        // away - fouls
    public int      HY          { get; set; }        // home - yellow
    public int      AY          { get; set; }        // away - yellow
    public int      HR          { get; set; }        // home - red
    public int      AR          { get; set; }        // away - red
    public int      HC          { get; set; }        // home - corner
    public int      AC          { get; set; }        // away - corner
}



public class DataCompiler : MonoBehaviour {

    public SimpleSQL.SimpleSQLManager dbManager;
    public SimpleSQL.SimpleSQLManager[] dbManagerArr;
    public SimpleSQL.SimpleSQLManager[] dbManagerArrItaly;
    public SimpleSQL.SimpleSQLManager[] dbManagerArrGermany;
    public SimpleSQL.SimpleSQLManager[] dbManagerArrScotland;
    public SimpleSQL.SimpleSQLManager[] dbManagerArrSpain;
    SimpleSQL.SimpleSQLManager dbManagerCurr;

    // Main Canvas View
    public Transform ScrollViewCurrentContent;
    public Transform PointsForWinHomeInput;
    public Transform PointsForWinAwayInput;
    public Transform PointsForDrawHomeInput;
    public Transform PointsForDrawAwayInput;
    public Transform PointsForRedInput;
    public Transform PointsForYellowInput;
    public Transform DropDownCountry;
    public Transform DropDownDivision;
    public Transform DropDownYear;
    public Transform CheckboxScoringFirstHalf;
    public Transform CheckboxScoringSecondHalf;
    public Transform CheckboxWoodwork;
    public Transform CheckboxShotOnTarget;
    public Transform CheckboxShot;
    public Transform CheckboxFoul;
    public Transform CheckboxCorner;
    public Transform TotalGamesPlayedInput;
    public Transform TotalGamesPlayedSlider;
    public Transform PointsForWinHomeSlider;
    public Transform PointsForWinAwaySlider;
    public Transform PointsForDrawHomeSlider;
    public Transform PointsForDrawAwaySlider;
    public Transform PointsForRedSlider;
    public Transform PointsForYellowSlider;
    public Transform DropDownSwapAttacksTeamA;
    public Transform DropDownSwapAttacksTeamB;
    public Transform DropDownSwapDefencesTeamA;
    public Transform DropDownSwapDefencesTeamB;
    public Transform DropDownRemoveRef;

    // new table
    public Transform[] TableRows;
    public Transform[] ChangesText;
    public Transform txtDivision;
    public Transform txtYear;
    public Transform SliderTableOffset;

    List<Fixture> fixtures;
    List<Fixture> fixturesDeepCopy;
    Dictionary<string, TeamData> map;
    List<KeyValuePair<string, TeamData>> listAltered;

    int SortDataBaseManagers(SimpleSQLManager a, SimpleSQLManager b)
    {
        return String.Compare(b.databaseFile.name, a.databaseFile.name);
    }

    // Use this for initialization
    void Start () {
        // sort so we can add new databases to array and always have most recent first in array
        Array.Sort<SimpleSQLManager>(dbManagerArr,      SortDataBaseManagers);
        Array.Sort<SimpleSQLManager>(dbManagerArrItaly, SortDataBaseManagers);
        Array.Sort<SimpleSQLManager>(dbManagerArrSpain, SortDataBaseManagers);
        Array.Sort<SimpleSQLManager>(dbManagerArrGermany, SortDataBaseManagers);
        Array.Sort<SimpleSQLManager>(dbManagerArrScotland, SortDataBaseManagers);
        var sorted = dbManagerArr.OrderBy(item => item.databaseFile.name); 
        map                 = new Dictionary<string, TeamData>();
        fixturesDeepCopy    = null;
        UpdateCountry();
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
        foreach (SimpleSQL.SimpleSQLManager m in dbManagerArrItaly)
        {
            m.Close();
            m.Dispose();
        }
        foreach (SimpleSQL.SimpleSQLManager m in dbManagerArrSpain)
        {
            m.Close();
            m.Dispose();
        }
        foreach (SimpleSQL.SimpleSQLManager m in dbManagerArrGermany)
        {
            m.Close();
            m.Dispose();
        }
        foreach (SimpleSQL.SimpleSQLManager m in dbManagerArrScotland)
        {
            m.Close();
            m.Dispose();
        }
    }

    void AddOptionsYears(List<string> optionsYear, int count)
    {
        List<String> potentialYears = new List<String>();
        potentialYears.Add("2018-2019");
        potentialYears.Add("2017-2018");
        potentialYears.Add("2016-2017");
        potentialYears.Add("2015-2016");
        potentialYears.Add("2014-2015");
        potentialYears.Add("2013-2014");
        potentialYears.Add("2012-2013");
        potentialYears.Add("2011-2012");
        potentialYears.Add("2010-2011");
        potentialYears.Add("2009-2010");
        potentialYears.Add("2008-2009");
        potentialYears.Add("2007-2008");
        potentialYears.Add("2006-2007");
        potentialYears.Add("2005-2006");
        potentialYears.Add("2004-2005");
        potentialYears.Add("2003-2004");
        potentialYears.Add("2002-2003");
        potentialYears.Add("2001-2002");
        potentialYears.Add("2000-2001");
        potentialYears.Add("1999-2000");
        potentialYears.Add("1998-1999");
        potentialYears.Add("1997-1998");
        potentialYears.Add("1996-1997");
        potentialYears.Add("1995-1996");
        potentialYears.Add("1994-1995");
        potentialYears.Add("1993-1994");

        for(int i = 0; i < count; i++)
        {
            optionsYear.Add(potentialYears[i]);
        }

    }

    public void ResetGui()
    {
        ResetSwaps();
        PointsForWinHomeSlider.GetComponent<Slider>().value = 3;
        PointsForWinAwaySlider.GetComponent<Slider>().value = 3;
        PointsForDrawHomeSlider.GetComponent<Slider>().value = 1;
        PointsForDrawAwaySlider.GetComponent<Slider>().value = 1;
        PointsForRedSlider.GetComponent<Slider>().value = 0;
        PointsForYellowSlider.GetComponent<Slider>().value = 0;

        CheckboxScoringFirstHalf.GetComponent<Toggle>().isOn = true; 
        CheckboxScoringSecondHalf.GetComponent<Toggle>().isOn = true;
        
        CheckboxWoodwork.GetComponent<Toggle>().isOn = false;
        CheckboxShotOnTarget.GetComponent<Toggle>().isOn = false;
        CheckboxShot.GetComponent<Toggle>().isOn = false;
        CheckboxFoul.GetComponent<Toggle>().isOn = false;
        CheckboxCorner.GetComponent<Toggle>().isOn = false;
        TotalGamesPlayedSlider.GetComponent<Slider>().value = TotalGamesPlayedSlider.GetComponent<Slider>().maxValue;
    }

    bool updateDivisionEnabled = true;
    bool refreshOptions = false;

    public void UpdateCountry()
    {
        updateDivisionEnabled = false;

        int country = DropDownCountry.GetComponent<Dropdown>().value;

        List<string> optionsDiv = new List<string>();
        List<string> optionsYear = new List<string>();

        if (country == 0)
        {
            // EN 
            dbManagerCurr = dbManagerArr[0];
            optionsDiv.Add("PREMIER LEAGUE");
            optionsDiv.Add("CHAMPIONSHIP");
            optionsDiv.Add("DIVISION 1");
            optionsDiv.Add("DIVISION 2");
            optionsDiv.Add("CONFERENCE");

            AddOptionsYears(optionsYear, 26);
        }
        if (country == 1)
        {
            // IT
            dbManagerCurr = dbManagerArrItaly[0];
            optionsDiv.Add("SERIE A");
            optionsDiv.Add("SERIE B");

            AddOptionsYears(optionsYear, 26);
        }
        if (country == 2)
        {
            // GE
            dbManagerCurr = dbManagerArrGermany[0];
            optionsDiv.Add("BUNDESLIGA");
            optionsDiv.Add("2.BUNDESLIGA");

            AddOptionsYears(optionsYear, 26);
        }
        if (country == 3)
        {
            // SC
            dbManagerCurr = dbManagerArrScotland[0];
            optionsDiv.Add("PREMIERSHIP");
            optionsDiv.Add("CHAMPIONSHIP");

            AddOptionsYears(optionsYear, 25);
        }
        DropDownDivision.GetComponent<Dropdown>().ClearOptions();
        DropDownYear.GetComponent<Dropdown>().ClearOptions();

        DropDownDivision.GetComponent<Dropdown>().AddOptions(optionsDiv);
        DropDownYear.GetComponent<Dropdown>().AddOptions(optionsYear);

        DropDownYear.GetComponent<Dropdown>().value = 0;
        DropDownDivision.GetComponent<Dropdown>().value = 0;

        updateDivisionEnabled = true;

        // just do once at the end
        refreshOptions = true;  // we need to rebuild options in case we are in division that does not exist for the new country
        UpdateDivision();
        refreshOptions = false;

    }

    string strDiv = "";
    public void UpdateDivision()
    {
        if (!updateDivisionEnabled)
            return;

        Debug.Log("Updating Division");
        // new year/division, different teams and number of teams
        ResetSwaps();

        // Database - get country/year
        int country = DropDownCountry.GetComponent<Dropdown>().value;
        int year = DropDownYear.GetComponent<Dropdown>().value;
       // string strDiv = "";
        int div = 0;

        Debug.Log("year " + year);
        Debug.Log("country " + country);

        div = DropDownDivision.GetComponent<Dropdown>().value;
        List<string> optionsDiv = new List<string>();
        if (country == 0)
        {
            // EN
            // how many divisions does each year have?    18
            int[] divCount = new int[] { 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4 };
            if (true) //refreshOptions)
            {
                DropDownDivision.GetComponent<Dropdown>().ClearOptions();
                optionsDiv.Add("PREMIER LEAGUE");
                optionsDiv.Add("CHAMPIONSHIP");
                optionsDiv.Add("DIVISION 1");
                optionsDiv.Add("DIVISION 2");
                optionsDiv.Add("CONFERENCE");
                DropDownDivision.GetComponent<Dropdown>().AddOptions(optionsDiv);

                if (divCount[year] < 5)
                    DropDownDivision.GetComponent<Dropdown>().options.RemoveAt(4);
            }
            dbManagerCurr = dbManagerArr[year];
            // deal with if we select a new year but division we are currently on does not exist for that year eg 'en conference 1999'
            while(div >= divCount[year])
            {
                div--;
                DropDownDivision.GetComponent<Dropdown>().value = div;
            }

            strDiv = "E0";
            switch (div)
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
        }
        else if (country == 1)
        {
            // Italy
            dbManagerCurr = dbManagerArrItaly[year];
            div = DropDownDivision.GetComponent<Dropdown>().value;
            strDiv = "I1";
            switch (div)
            {
                case 0:
                    strDiv = "I1";
                    break;
                case 1:
                    strDiv = "I2";
                    break;
            }
        }
        else if (country == 2)
        {
            // Germany
            dbManagerCurr = dbManagerArrGermany[year];
            div = DropDownDivision.GetComponent<Dropdown>().value;
            strDiv = "D1";
            switch (div)
            {
                case 0:
                    strDiv = "D1";
                    break;
                case 1:
                    strDiv = "D2";
                    break;
            }
        }
        else if (country == 3)
        {
            // Scotland
            // how many divisions does each year have?    25
            int[] divCount = new int[] {4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 2, 2, 2 };
            if (true) //refreshOptions)
            {
                DropDownDivision.GetComponent<Dropdown>().ClearOptions();
                optionsDiv.Add("PREMIERSHIP");
                optionsDiv.Add("CHAMPIONSHIP");
                optionsDiv.Add("LEAGUE ONE");
                optionsDiv.Add("LEAGUE TWO");
                DropDownDivision.GetComponent<Dropdown>().AddOptions(optionsDiv);

                if (divCount[year] == 2)
                {
                    DropDownDivision.GetComponent<Dropdown>().options.RemoveRange(2, 2);
                }
            }
            dbManagerCurr = dbManagerArrScotland[year];
            // deal with if we select a new year but division we are currently on does not exist for that year eg 'en conference 1999'
            while (div >= divCount[year])
            {
                div--;
                DropDownDivision.GetComponent<Dropdown>().value = div;
            }

            strDiv = "SC0";
            switch (div)
            {
                case 0:
                    strDiv = "SC0";
                    break;
                case 1:
                    strDiv = "SC1";
                    break;
                case 2:
                    strDiv = "SC2";
                    break;
                case 3:
                    strDiv = "SC3";
                    break;
            }

        }
        string sql = "SELECT * FROM " + strDiv;
        fixtures = dbManagerCurr.Query<Fixture>(sql);
        UpdateDataReal();

        // to do, get actual games played (partial season etc)
        // set max games
        // total games per team
        int totalTeams = map.Count;
        //TotalGamesPlayedSlider.GetComponent<Slider>().maxValue = (totalTeams - 1) * headToHeadCount;
        //TotalGamesPlayedSlider.GetComponent<Slider>().value = (totalTeams - 1) * headToHeadCount;
        int played = map.ElementAt(0).Value.playedReal;
        TotalGamesPlayedSlider.GetComponent<Slider>().maxValue = played;
        TotalGamesPlayedSlider.GetComponent<Slider>().value = played;


        Debug.Log(totalTeams);

        updateDivisionEnabled = false;
        DropDownDivision.GetComponent<Dropdown>().value = div;
        updateDivisionEnabled = true;

        txtYear.GetComponent<Text>().text = DropDownYear.GetComponent<Dropdown>().options[year].text;
        txtDivision.GetComponent<Text>().text = DropDownDivision.GetComponent<Dropdown>().options[div].text;

        TextAsset asset = new TextAsset();
        dbManager.databaseFile = asset;
        CompileTable();
    }

    void DoChanges(int pointsForWinHome, int pointsForWinAway, int pointsForDrawHome, int pointsForDrawAway, int pointsForRed,
        int pointsForYellow, bool scoringFirstHalf, bool scoringSecondHalf, bool woodwork, bool onTarget, bool shot, bool foul, bool corner, string referee, bool swapAttacks, bool swapDefences)
    {
        // list of rule changes user has made
        ArrayList arrChanges = new ArrayList();
        if (pointsForWinHome != 3)
            arrChanges.Add("* " + pointsForWinHome + " Points for home win");
        if (pointsForWinAway != 3)
            arrChanges.Add("* " + pointsForWinAway + " Points for away win");
        if (pointsForDrawHome != 1)
            arrChanges.Add("* " + pointsForWinHome + " Points for home draw");
        if (pointsForDrawAway != 1)
            arrChanges.Add("* " + pointsForWinAway + " Points for away draw");
        if (pointsForRed != 0)
            arrChanges.Add("* " + pointsForRed + " Points for red card");
        if (pointsForYellow != 0)
            arrChanges.Add("* " + pointsForYellow + " Points for yellow card");
        if (scoringFirstHalf && !scoringSecondHalf)
            arrChanges.Add("* First halves only");
        if (scoringSecondHalf && !scoringFirstHalf)
            arrChanges.Add("* Second halves only");
        if (woodwork)
            arrChanges.Add("* Goal for hitting woodwork");
        if (onTarget)
            arrChanges.Add("* Goal for shot on target");
        if (shot)
            arrChanges.Add("* Goal for shot");
        if (foul)
            arrChanges.Add("* Goal for foul");
        if (corner)
            arrChanges.Add("* Goal for corner");
        if(referee != "NONE")
            arrChanges.Add("* No ref: " + referee);
        if (swapAttacks)
            arrChanges.Add("* Attacks swapped");
        if (swapDefences)
            arrChanges.Add("* Defences swapped");
        int index = 0;
        foreach(Transform t in ChangesText)
        {
            Text txt = t.GetComponent<Text>();
            txt.text = "";
            if(arrChanges.Count > index)
            {
                txt.text = (string)arrChanges[index];
            }
            index++;
        }
    }

    HashSet<string> referees;

    // does refs too
    void AddTeamsToMap()
    {
        referees = new HashSet<string>();

        map.Clear();
        foreach (Fixture fixture in fixtures)
        {
            // refs
            if(fixture.Referee != null)
                referees.Add(fixture.Referee);

            if (fixture.HomeTeam == null || fixture.AwayTeam == null)
                continue;

            if (fixture.HomeTeam == "" || fixture.AwayTeam == "")
                continue;

            if (!map.ContainsKey(fixture.HomeTeam))
            {
                TeamData data = new TeamData();
                data.playedReal = 0;
                data.position = 0;
                data.points = 0;
                data.pointsAltered = 0;
                data.name = fixture.HomeTeam;
                map.Add(fixture.HomeTeam, data);
            }
            if (!map.ContainsKey(fixture.AwayTeam))
            {
                TeamData data = new TeamData();
                data.playedReal = 0;
                data.position = 0;
                data.points = 0;
                data.pointsAltered = 0;
                data.name = fixture.AwayTeam;
                map.Add(fixture.AwayTeam, data);
            }
        }
    }

    void DetermineMarketsAvailable()
    {
        // if these remain 0 after all fixtures then we know they are either not present in database or irrelevant anyway do we should disable the ui
        int totalShots = 0;
        int totalOnTarget = 0;
        int totalWoodwork = 0;
        int totalFouls = 0;
        int totalCorners = 0;
        foreach (Fixture fixture in fixtures)
        {
            totalShots += fixture.HS + fixture.AS;
            totalOnTarget += fixture.HST + fixture.AST;
            totalWoodwork += fixture.HHW + fixture.AHW;
            totalFouls += fixture.HF + fixture.AF;
            totalCorners += fixture.HC + fixture.AC;
        }
        // disable ui if not used
        CheckboxWoodwork.GetComponent<Toggle>().interactable = true;
        CheckboxShot.GetComponent<Toggle>().interactable = true;
        CheckboxShotOnTarget.GetComponent<Toggle>().interactable = true;
        CheckboxFoul.GetComponent<Toggle>().interactable = true;
        CheckboxCorner.GetComponent<Toggle>().interactable = true;
        if (totalWoodwork == 0)
        {
            CheckboxWoodwork.GetComponent<Toggle>().interactable = false;
        }
        if (totalShots == 0)
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
        if (totalCorners == 0)
        {
            CheckboxCorner.GetComponent<Toggle>().interactable = false;
        }
    }

    void SetRealData()
    {
        foreach (Fixture fixture in fixtures)
        {
            if (fixture == null || fixture.HomeTeam == null || fixture.AwayTeam == null)
                continue;

            if (fixture.HomeTeam == "" || fixture.AwayTeam == "")
                continue;

            map[fixture.HomeTeam].playedReal++;
            map[fixture.AwayTeam].playedReal++;
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
        }
    }

    void SortAndAssignReal()
    {
        // sort by value
        List<KeyValuePair<string, TeamData>> list = map.ToList();
        list.Sort(
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
                return pair2.Value.points.CompareTo(pair1.Value.points);
            }
        );
        AssignPositionsReal(list);
    }

    // does ref too
    void PopulateTeamSwap()
    {
        Dropdown teamA = DropDownSwapAttacksTeamA.GetComponent<Dropdown>();
        Dropdown teamB = DropDownSwapAttacksTeamB.GetComponent<Dropdown>();
        Dropdown teamC = DropDownSwapDefencesTeamA.GetComponent<Dropdown>();
        Dropdown teamD = DropDownSwapDefencesTeamB.GetComponent<Dropdown>();

        Dropdown ddRemoveRef = DropDownRemoveRef.GetComponent<Dropdown>();

        // clear options
        teamA.ClearOptions();
        teamB.ClearOptions();
        teamC.ClearOptions();
        teamD.ClearOptions();
        ddRemoveRef.ClearOptions();

        // populate
        // refs
        List<string> refs = referees.ToList<string>();  //new List<string>();
        refs.Insert(0, "NONE");
        ddRemoveRef.AddOptions(refs);
        ddRemoveRef.value = 0;

        // teams for swap
        List<string> teams = new List<string>();
        Dictionary<string, TeamData> mapTemp = new Dictionary<string, TeamData>(map); 
        foreach (KeyValuePair<string, TeamData> entry in mapTemp)
        {
            teams.Add(entry.Key); 
        }
        // order
        teams = teams.OrderBy(o => o).ToList();
        teams.Insert(0, "NONE");
        teamA.AddOptions(teams);
        teamB.AddOptions(teams);
        teamC.AddOptions(teams);
        teamD.AddOptions(teams);
    }

    void UpdateDataReal()
    {
        AddTeamsToMap();
        PopulateTeamSwap();
        // create new data with team swap values instead

        DetermineMarketsAvailable();
        SetRealData();
        SortAndAssignReal();
    }

    // and refs
    void ResetSwaps()
    {
        Dropdown ddAttackA  = DropDownSwapAttacksTeamA.GetComponent<Dropdown>();
        Dropdown ddAttackB  = DropDownSwapAttacksTeamB.GetComponent<Dropdown>();
        Dropdown ddDefenceA = DropDownSwapDefencesTeamA.GetComponent<Dropdown>();
        Dropdown ddDefenceB = DropDownSwapDefencesTeamB.GetComponent<Dropdown>();
        Dropdown ddRefs     = DropDownRemoveRef.GetComponent<Dropdown>();

        ddAttackA.value     = 0;
        ddAttackB.value     = 0;
        ddDefenceA.value    = 0;
        ddDefenceB.value    = 0;
        ddRefs.value        = 0;
    }

    void SwitchAttacks(bool switchAttacks, bool home, Fixture fixtureA, Fixture fixtureB)
    {
        if (switchAttacks)
        {
            if (home)
            {
                fixtureA.FTHG = fixtureB.FTHG;
            }
            else
            {
                fixtureA.FTAG = fixtureB.FTAG;
            }
        }
    }

    void SwitchDefences(bool switchDefences, bool home, Fixture fixtureA, Fixture fixtureB)
    {
        if (switchDefences)
        {
            if (home)
            {
                fixtureA.FTAG = fixtureB.FTAG;
            }
            else
            {
                fixtureA.FTHG = fixtureB.FTHG;
            }
        }
    }


    // problem - if team has apostrophe (nott'm forrest)
    void DoSwaps(bool attack, string A, string B, bool doHeadToHead)
    {
        foreach (Fixture fixture in fixturesDeepCopy)
        {
            string TeamSwapA = A;
            string TeamSwapB = B;
            // get all fixtures 
            // 2 special cases - fixtures betweeen TeamSwapA and TeamSwapB (skipped below)
            // only do this twice per update as swapping for attack then again for defence
            if (doHeadToHead && ((TeamSwapA == fixture.HomeTeam && TeamSwapB == fixture.AwayTeam) || (TeamSwapB == fixture.HomeTeam && TeamSwapA == fixture.AwayTeam)))
            {
                int tempGoalsHome = fixture.FTHG;
                fixture.FTHG = fixture.FTAG;
                fixture.FTAG = tempGoalsHome;
            }
            // 4 other cases - against other teams
            else if (fixture.HomeTeam == TeamSwapA)
            {
                // test for chelsea v chelsea etc
                if (TeamSwapB != fixture.AwayTeam)
                {
                    // get fixture of TeamSwapB vs fixture.AwayTeam 
                    string sql = "SELECT * FROM " + strDiv + " WHERE HomeTeam='" + TeamSwapB.Replace("'", "''") + "' AND AwayTeam='" + fixture.AwayTeam.Replace("'", "''") + "'";
                    List<Fixture> fix;
                    fix = dbManagerCurr.Query<Fixture>(sql);
                    if (fix.Count == 1)  // should always be 1
                    {
                        SwitchAttacks(attack, true, fixture, fix[0]);
                        SwitchDefences(!attack, true, fixture, fix[0]);
                    }
                }
            }
            else if (fixture.HomeTeam == TeamSwapB)
            {
                // test for chelsea v chelsea etc
                if (TeamSwapA != fixture.AwayTeam)
                {
                    // get fixture of TeamSwapA vs fixture.AwayTeam 
                    string sql = "SELECT * FROM " + strDiv + " WHERE HomeTeam='" + TeamSwapA.Replace("'", "''") + "' AND AwayTeam='" + fixture.AwayTeam.Replace("'", "''") + "'";
                    List<Fixture> fix;
                    fix = dbManagerCurr.Query<Fixture>(sql);
                    if (fix.Count == 1)  // should always be 1
                    {
                        SwitchAttacks(attack, true, fixture, fix[0]);
                        SwitchDefences(!attack, true, fixture, fix[0]);
                    }
                }
            }
            else if (fixture.AwayTeam == TeamSwapA)
            {
                // test for chelsea v chelsea etc
                if (TeamSwapB != fixture.HomeTeam)
                {
                    // get fixture of fixture.HomeTeam vs TeamSwapB
                    string sql = "SELECT * FROM " + strDiv + " WHERE HomeTeam='" + fixture.HomeTeam.Replace("'", "''") + "' AND AwayTeam='" + TeamSwapB.Replace("'", "''") + "'";
                    List<Fixture> fix;
                    fix = dbManagerCurr.Query<Fixture>(sql);
                    if (fix.Count == 1)  // should always be 1
                    {
                        SwitchAttacks(attack, false, fixture, fix[0]);
                        SwitchDefences(!attack, false, fixture, fix[0]);
                    }
                }
            }
            else if (fixture.AwayTeam == TeamSwapB)
            {
                // test for chelsea v chelsea etc
                if (TeamSwapA != fixture.HomeTeam)
                {
                    // get fixture of fixture.HomeTeam vs TeamSwapA
                    string sql = "SELECT * FROM " + strDiv + " WHERE HomeTeam='" + fixture.HomeTeam.Replace("'", "''") + "' AND AwayTeam='" + TeamSwapA.Replace("'", "''") + "'";
                    List<Fixture> fix;
                    fix = dbManagerCurr.Query<Fixture>(sql);
                    if (fix.Count == 1)  // should always be 1
                    {
                        SwitchAttacks(attack, false, fixture, fix[0]);
                        SwitchDefences(!attack, false, fixture, fix[0]);
                    }
                }
            }
        }
    }

    public void UpdateSwaps()
    {
        Dropdown    ddAttackA   = DropDownSwapAttacksTeamA.GetComponent<Dropdown>();
        Dropdown    ddAttackB   = DropDownSwapAttacksTeamB.GetComponent<Dropdown>();
        Dropdown    ddDefenceA  = DropDownSwapDefencesTeamA.GetComponent<Dropdown>();
        Dropdown    ddDefenceB  = DropDownSwapDefencesTeamB.GetComponent<Dropdown>();
        string      TeamSwapA   = ddAttackA.options[ddAttackA.value].text;
        string      TeamSwapB   = ddAttackB.options[ddAttackB.value].text;
        string      TeamSwapC   = ddDefenceA.options[ddDefenceA.value].text;
        string      TeamSwapD   = ddDefenceB.options[ddDefenceB.value].text;
        bool        swapAttack  = TeamSwapA != "NONE" && TeamSwapB != "NONE";
        bool        swapDefence = TeamSwapC != "NONE" && TeamSwapD != "NONE";
        fixturesDeepCopy = null;
        if (swapAttack || swapDefence)
        {
            // deep copy
            fixturesDeepCopy = fixtures.ConvertAll(fixture => fixture.Clone());

            bool doHeadTOHead = !(swapAttack && swapDefence);
            DoSwaps(true, TeamSwapA, TeamSwapB, true);
            DoSwaps(false, TeamSwapC, TeamSwapD, doHeadTOHead);
        }
        // something changed - even if 'none'
        CompileTable();
    }

    public void CompileTable()
    {
        int pointsForWinHome    = int.Parse(PointsForWinHomeInput.GetComponent<InputField>().text);
        int pointsForWinAway    = int.Parse(PointsForWinAwayInput.GetComponent<InputField>().text);
        int pointsForDrawHome   = int.Parse(PointsForDrawHomeInput.GetComponent<InputField>().text);
        int pointsForDrawAway   = int.Parse(PointsForDrawAwayInput.GetComponent<InputField>().text);
        int pointsForRed        = int.Parse(PointsForRedInput.GetComponent<InputField>().text);
        int pointsForYellow     = int.Parse(PointsForYellowInput.GetComponent<InputField>().text);

        bool scoringFirstHalf   = CheckboxScoringFirstHalf.GetComponent<Toggle>().isOn;
        bool scoringSecondHalf  = CheckboxScoringSecondHalf.GetComponent<Toggle>().isOn;
        bool woodwork           = CheckboxWoodwork.GetComponent<Toggle>().isOn;
        bool onTarget           = CheckboxShotOnTarget.GetComponent<Toggle>().isOn;
        bool shot               = CheckboxShot.GetComponent<Toggle>().isOn;
        bool foul               = CheckboxFoul.GetComponent<Toggle>().isOn;
        bool corner             = CheckboxCorner.GetComponent<Toggle>().isOn;
        // get it from value of slider, not text box
        int totalGamesAllowed = (int)TotalGamesPlayedSlider.GetComponent<Slider>().value;

        Dropdown ddReferee  = DropDownRemoveRef.GetComponent<Dropdown>();
        Dropdown ddTeamA    = DropDownSwapAttacksTeamA.GetComponent<Dropdown>();
        Dropdown ddTeamB    = DropDownSwapAttacksTeamB.GetComponent<Dropdown>();
        Dropdown ddTeamC    = DropDownSwapDefencesTeamA.GetComponent<Dropdown>();
        Dropdown ddTeamD    = DropDownSwapDefencesTeamB.GetComponent<Dropdown>();
        bool attacksSwapped = ddTeamA.captionText.text != "NONE" && ddTeamB.captionText.text != "NONE";
        bool defencesSwapped = ddTeamC.captionText.text != "NONE" && ddTeamD.captionText.text != "NONE";

        // bool fixtureSkipped = false;
        DoChanges(pointsForWinHome, pointsForWinAway, pointsForDrawHome, pointsForDrawAway, pointsForRed,
            pointsForYellow, scoringFirstHalf, scoringSecondHalf, woodwork, onTarget, shot, foul, corner, ddReferee.captionText.text, attacksSwapped, defencesSwapped);

        // if swap defense/goalie
        // go through fixtures
        // swap records


        // reset fiddled data
        foreach (KeyValuePair<string, TeamData> entry in map)
        {
            entry.Value.played = 0;
            entry.Value.pointsAltered = 0;
            entry.Value.positionAltered = 0;
            entry.Value.diff = 0;
            entry.Value.drawn = 0;
            entry.Value.goalsAgainst = 0;
            entry.Value.goalsFor = 0;
            entry.Value.lost = 0;
            entry.Value.won = 0;
        }

        // we use deep copy if attack/defence switches have been made to data
        List<Fixture> fixturesTemp;
        if (fixturesDeepCopy != null)
            fixturesTemp = fixturesDeepCopy;
        else
            fixturesTemp = fixtures;
       
        // create map of teams with points collected as value    
        foreach (Fixture fixture in fixturesTemp)
        {

            if (fixture.HomeTeam == null || fixture.AwayTeam == null)
                continue;

            if (fixture.HomeTeam == "" || fixture.AwayTeam == "")
                continue;

            if (map[fixture.HomeTeam].played >= totalGamesAllowed)
            {
                // skip this fixture
                //fixtureSkipped = true;
                continue;
            }
            if (map[fixture.AwayTeam].played >= totalGamesAllowed)
            {
                // skip this fixture
                //fixtureSkipped = true;
                continue;
            }



            //totalShots += fixture.HS + fixture.AS;
            //totalOnTarget += fixture.HST + fixture.AST;
            //totalWoodwork += fixture.HHW + fixture.AHW;
            //totalFouls += fixture.HF + fixture.AF;

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

            // TODO, make this fairer - average goals home/away etc
            // skip if we blacklisted referee
            //Dropdown ddReferee = DropDownRemoveRef.GetComponent<Dropdown>();
            if (fixture.Referee == ddReferee.captionText.text)
            {
                // skip this fixture
                //fixtureSkipped = true;
                float averageHomeGoals = 0.0f;
                float averageAwayGoals = 0.0f;
                // maybe get average points per game up to this point?
                if (map[fixture.HomeTeam].played > 0)
                {
                    averageHomeGoals = map[fixture.HomeTeam].goalsFor / map[fixture.HomeTeam].played;

                    if (map[fixture.AwayTeam].played > 0)
                    {
                        averageAwayGoals = map[fixture.AwayTeam].goalsFor / map[fixture.AwayTeam].played;
                    }
                }
                totalHomeGoals = (int)averageHomeGoals;
                totalAwayGoals = (int)averageAwayGoals;
                // map[fixture.HomeTeam].pointsAltered += (int)averageHome;
                // map[fixture.AwayTeam].pointsAltered += (int)averageAway;

                //continue;
            }




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

            if (corner)
            {
                totalHomeGoals += fixture.HC; // corners
                totalAwayGoals += fixture.AC; // corners
            }

            //** altered results based on selections **
            map[fixture.HomeTeam].played++;
            map[fixture.AwayTeam].played++;
            map[fixture.HomeTeam].goalsFor      = map[fixture.HomeTeam].goalsFor + totalHomeGoals;
            map[fixture.HomeTeam].goalsAgainst  = map[fixture.HomeTeam].goalsAgainst + totalAwayGoals;

            map[fixture.AwayTeam].goalsFor      = map[fixture.AwayTeam].goalsFor + totalAwayGoals;
            map[fixture.AwayTeam].goalsAgainst  = map[fixture.AwayTeam].goalsAgainst + totalHomeGoals;

            // points for yellow/red cards
            map[fixture.HomeTeam].pointsAltered = map[fixture.HomeTeam].pointsAltered + (fixture.HY * pointsForYellow);
            map[fixture.AwayTeam].pointsAltered = map[fixture.AwayTeam].pointsAltered + (fixture.AY * pointsForYellow);
            map[fixture.HomeTeam].pointsAltered = map[fixture.HomeTeam].pointsAltered + (fixture.HR * pointsForRed);
            map[fixture.AwayTeam].pointsAltered = map[fixture.AwayTeam].pointsAltered + (fixture.AR * pointsForRed);

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

        AssignPositionsAltered(listAltered);

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
                            if (transChildDiff)
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
                        {
                            txtValue.text = "";

                            Transform transChildDiff = null;
                            if (txtValue.transform.childCount > 0)
                                transChildDiff = txtValue.transform.GetChild(0);
                            Text txtValueDiff = null;
                            if (transChildDiff)
                                txtValueDiff = transChildDiff.GetComponentInChildren<Text>();
                            txtValueDiff.text = "";
                        }

                    }
                }
            }
            
            listIndex++;
        }
        // map.Clear();
        PopulateTableShare();
    }

    void AssignPositionsReal(List<KeyValuePair<string, TeamData>> list)//, List<KeyValuePair<string, TeamData>> updated)
    {
        int index = 0;
        foreach(KeyValuePair<string, TeamData> kv in list)
        {
            kv.Value.position = index;
            index++;
        }   
    }

    void AssignPositionsAltered(List<KeyValuePair<string, TeamData>> list)//, List<KeyValuePair<string, TeamData>> updated)
    {
        int index = 0;
        foreach (KeyValuePair<string, TeamData> kv in list)
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

            if (TableRows.Length > index)
            {
                // we have a text item in array
                // position
                Text txt = TableRows[index].Find("Panel_position").Find("Text").GetComponent<Text>();
                txt.text = (valueData.positionAltered + 1).ToString();

                // name
                txt = TableRows[index].Find("Panel_name").Find("Text").GetComponent<Text>();
                txt.text = " " + keyTeam;

                // played      
                txt = TableRows[index].Find("Panel_played").Find("Text").GetComponent<Text>();
                txt.text = valueData.played.ToString();

                // won
                txt = TableRows[index].Find("Panel_won").Find("Text").GetComponent<Text>();
                txt.text = valueData.won.ToString();

                // lost
                txt = TableRows[index].Find("Panel_lost").Find("Text").GetComponent<Text>();
                txt.text = valueData.lost.ToString();

                // drawn
                txt = TableRows[index].Find("Panel_drawn").Find("Text").GetComponent<Text>();
                txt.text = valueData.drawn.ToString();

                // goals for
                txt = TableRows[index].Find("Panel_goals_for").Find("Text").GetComponent<Text>();
                txt.text = valueData.goalsFor.ToString();

                // goals against
                txt = TableRows[index].Find("Panel_goals_against").Find("Text").GetComponent<Text>();
                txt.text = valueData.goalsAgainst.ToString();

                // points
                txt = TableRows[index].Find("Panel_points").Find("Text").GetComponent<Text>();
                txt.text = valueData.pointsAltered.ToString();

                // diff
                txt = TableRows[index].Find("Panel_diff").Find("Text").GetComponent<Text>();
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



    //public void PopulateTableShare()
    //{
    //    int count = listAltered.Count;
    //    int offset = 0;// count - 10;
    //    int index = 0;

    //    int sliderValue = (int)SliderTableOffset.GetComponent<Slider>().value;
    //    switch (sliderValue)
    //    {
    //        case 0:
    //            offset = 0;
    //            break;
    //        case 1:
    //            offset = (count / 2) - 5;
    //            break;
    //        case 2:
    //            offset = count - 10;
    //            break;
    //    }


    //    foreach (KeyValuePair<string, TeamData> kv in listAltered)
    //    {
    //        // we can offset table
    //        if (index < offset)
    //        {
    //            // dont start yet
    //            offset--;
    //            continue;
    //        }
    //        string keyTeam = kv.Key;
    //        TeamData valueData = kv.Value;

    //        if (TableTeams.Length > index)
    //        {
    //            // we have a text item in array
    //            // name
    //            Text txt = TableTeams[index].GetComponent<Text>();
    //            txt.text = keyTeam;
    //            txt.text = (valueData.positionAltered + 1) + "." + txt.text;

    //            // played
    //            txt = TablePlayed[index].GetComponent<Text>();
    //            txt.text = valueData.played.ToString();

    //            // won
    //            txt = TableWon[index].GetComponent<Text>();
    //            txt.text = valueData.won.ToString();

    //            // lost
    //            txt = TableLost[index].GetComponent<Text>();
    //            txt.text = valueData.lost.ToString();

    //            // drawn
    //            txt = TableDrawn[index].GetComponent<Text>();
    //            txt.text = valueData.drawn.ToString();

    //            // goals for
    //            txt = TableGoalsFor[index].GetComponent<Text>();
    //            txt.text = valueData.goalsFor.ToString();

    //            // goals for
    //            txt = TableGoalsAgainst[index].GetComponent<Text>();
    //            txt.text = valueData.goalsAgainst.ToString();

    //            // points
    //            txt = TablePoints[index].GetComponent<Text>();
    //            txt.text = valueData.pointsAltered.ToString();

    //            // diff
    //            txt = TableDiff[index].GetComponent<Text>();
    //            txt.color = Color.white;
    //            txt.text = valueData.diff.ToString();
    //            if (valueData.diff > 0)
    //            {
    //                txt.text = "+" + txt.text;
    //                txt.color = Color.green;
    //            }
    //            if (valueData.diff == 0)
    //            {
    //                txt.text = "";
    //                txt.color = Color.green;
    //            }
    //            if (valueData.diff < 0)
    //            {
    //                txt.color = Color.red;
    //            }
    //        }
    //        index++;
    //    }
    //}



}


