using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ScrText : MonoBehaviour {
    public Transform Master;
	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void Set()
    {
        if (Master)
        {
            GetComponent<InputField>().text = Master.GetComponent<Slider>().value.ToString();
        }
    }
}
