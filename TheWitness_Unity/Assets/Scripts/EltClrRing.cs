﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EltClrRing : MonoBehaviour {
    public Color c;

    public void ShowUnsolvedColor()
    {
        GetComponent<Renderer>().material.color = Color.red;
    }
    public void ShowNormalizeColor()
    {
        GetComponent<Renderer>().material.color = c;
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
