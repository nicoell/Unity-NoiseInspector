using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Spinning : MonoBehaviour {

    public float speedX = 10f;
    public float speedY = 10f;
    public float speedZ = 10f;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	    transform.Rotate(Vector3.up, speedX * Time.deltaTime);
	    transform.Rotate(Vector3.left, speedY * Time.deltaTime);
	    transform.Rotate(Vector3.forward, speedZ * Time.deltaTime);
    }
}
