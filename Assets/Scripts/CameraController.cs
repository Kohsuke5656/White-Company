using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    private GameObject Player = null;

	// Use this for initialization
	void Start () {
        Player = GameObject.FindGameObjectWithTag("Player");
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 newPosition = transform.position;
        newPosition.x = Player.transform.position.x;
        newPosition.y = Player.transform.position.y + 0.80f;

        //transform.position = newPosition;
        transform.position =
            new Vector3(Player.transform.position.x, newPosition.y, newPosition.z);
	}
}
