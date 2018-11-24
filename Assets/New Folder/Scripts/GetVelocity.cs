using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetVelocity : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        Debug.Log("Velocity of " + name + " is = " + GetComponent<Rigidbody>().velocity);
		
	}
}
