using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

	// Use this for initialization
	void Start () {
        
        //transform.position = new Vector3((Core.PolePreferences.poleSize - 1) * 2.5f, -(Core.PolePreferences.poleSize - 1) * 2.5f, -5.5f * Core.PolePreferences.poleSize);
    }
	
	// Update is called once per frame
	void Update () {
        Pole pole = GameObject.FindGameObjectWithTag("Pole").GetComponent<Pole>();
        int size = Mathf.Max(pole.height, pole.width);
        GetComponent<Camera>().orthographicSize = 13 + 2.5f * (size - 5);
        transform.position = new Vector3((pole.width - 1) * 2.5f, -(pole.height - 1) * 2.5f, -5.5f * size);
    }
}
