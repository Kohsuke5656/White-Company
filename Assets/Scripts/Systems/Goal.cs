using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour {

	private StageManager sm;
	public bool isReturnToTitle;

	//public GameObject timer;
    //public AudioSource victory;

	// Use this for initialization
	void Start () {
        if(GameObject.Find("StageManager") != null)
		    sm = GameObject.Find ("StageManager").GetComponent<StageManager>();
	}

	// Update is called once per frame
	void Update () {

		//if (timer.GetComponent<DemoTest> ().isGameClear) {
		//	if (Input.GetKeyDown(KeyCode.Space) || Input.GetButton("Maru")) {
  //              timer.GetComponent<AudioSource>().Play();
		//		if (isReturnToTitle) {
		//			sm.BackToTitle ();
		//		} else {
		//			sm.NextScene ();
		//		}
		//	}
		//}

	}

    void OnTriggerEnter(Collider other)
    {
        //プレイヤーがゴールに触れる
        if (other.gameObject.name == "Player")
        {
            //timer.GetComponent<DemoTest> ().isGameClear = true;
            GameObject camera = GameObject.Find("Main Camera");
            //camera.GetComponent<AudioSource>().Stop();
            //victory.Play();

            //最後のステージならタイトルへ
            if (isReturnToTitle)
            {
                sm.BackToTitle();
            }
            //それ以外はつぎのステージ
            else
            {
                sm.NextScene();
            }
        }
    }
}
