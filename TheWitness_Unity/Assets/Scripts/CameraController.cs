﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

	// Use this for initialization
	void Start () {
        GetComponent<Camera>().orthographicSize = 13 + 2.5f * (Core.PolePreferences.poleSize - 5);
        //transform.position = new Vector3((Core.PolePreferences.poleSize - 1) * 2.5f, -(Core.PolePreferences.poleSize - 1) * 2.5f, -5.5f * Core.PolePreferences.poleSize);
    }
	
	// Update is called once per frame
	void Update () {
        transform.position = new Vector3((Core.PolePreferences.poleSize - 1) * 2.5f, -(Core.PolePreferences.poleSize - 1) * 2.5f, -5.5f * Core.PolePreferences.poleSize);
    }
}
