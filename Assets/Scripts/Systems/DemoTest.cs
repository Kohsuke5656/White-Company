using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoTest : MonoBehaviour {


    public bool isGameClear;
    public bool isGameOver;

	// Use this for initialization
	void Start () {
        isGameClear = false;
        isGameOver = false;
    }
	
	// Update is called once per frame
	void Update () {

 //       if(isGameOver)
 //       {
 //           gameObject.GetComponent<Timer>().isTimeStart = false;
 //           gameObject.GetComponent<Timer>().cnt.fontSize = 100;
 //           gameObject.GetComponent<Timer>().cnt.text = "Game Over +_+";
 //       }
//	if(isGameClear)
 //       {
 //           int tmpClearTime = (int)(gameObject.GetComponent<Timer>().nowTime * 100);
 //           int tmpClearS = tmpClearTime / 100;
 //           int tmpClearMs = tmpClearTime % 100;
 //           gameObject.GetComponent<Timer>().isTimeStart = false;
 //           gameObject.GetComponent<Timer>().cnt.fontSize = 80;
 //           if (tmpClearS < 5.0f)
 //           {
 //               gameObject.GetComponent<Timer>().cnt.text = "S\n" + tmpClearS.ToString() + "." + tmpClearMs.ToString() + "\n" + "Clear";
 //           }
 //           else if (tmpClearS < 8.0f)
 //           {
 //               gameObject.GetComponent<Timer>().cnt.text = "A\n" + tmpClearS.ToString() + "." + tmpClearMs.ToString() + "\n" + "Clear";
 //           }
 //           else if (tmpClearS < 11.0f)
 //           {
 //               gameObject.GetComponent<Timer>().cnt.text = "B\n" + tmpClearS.ToString() + "." + tmpClearMs.ToString() + "\n" + "Clear";
 //           }
 //           else if (tmpClearS < 15.0f)
 //           {
 //               gameObject.GetComponent<Timer>().cnt.text = "C\n" + tmpClearS.ToString() + "." + tmpClearMs.ToString() + "\n" + "Clear";
 //           }
 //           else
 //           {
 //               gameObject.GetComponent<Timer>().cnt.text = "D\n" + tmpClearS.ToString() + "." + tmpClearMs.ToString() + "\n" + "Clear";
 //           }
 //       }
	}
}
