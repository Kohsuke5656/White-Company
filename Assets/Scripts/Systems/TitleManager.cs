using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleManager : MonoBehaviour {

	private StageManager sm;
    //public AudioSource SelectSound;

	// Use this for initialization
	void Start () {
		sm = GameObject.Find ("StageManager").GetComponent<StageManager>();
	}

	// Update is called once per frame
	void Update () {

		if (Input.GetKeyDown(KeyCode.Space)) {
            //SelectSound.Play();
			sm.NextScene();
		}

	}
}
