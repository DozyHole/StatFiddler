using UnityEngine;
using System.Collections;

public class Main : MonoBehaviour {
    public Transform CanvasMain;
    public Transform CanvasShare;

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void SwitchToShareCanvas()
    {
        CanvasMain.gameObject.SetActive(false);
       // CanvasShare.gameObject.SetActive(true);
    }
    public void SwitchToMainCanvas()
    {
        CanvasMain.gameObject.SetActive(true);
       // CanvasShare.gameObject.SetActive(false);
    }
}
