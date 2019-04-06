using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuCamera : MonoBehaviour {
    public GameObject Menu;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 pos = Menu.GetComponent<MenuManager>().menuPole.transform.position;
        transform.position = new Vector3(pos.x + 10f, pos.y - 10f, -35f);
	}
}
