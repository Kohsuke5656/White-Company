using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InjectArea : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
    }

    public static bool OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.tag == "Block")
            return true;

        return false;
    }
}
