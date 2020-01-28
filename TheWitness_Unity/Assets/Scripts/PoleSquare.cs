using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoleSquare : MonoBehaviour {

    public GameObject EditButtonPF;
    public GameObject editButton;
    public GameObject up;
	public GameObject down;
	public GameObject left;
	public GameObject right;
	public bool hasElem = false;
	public Elements element;

    public int indexI;
    public int indexJ;

    public void ShowEditButton()
    {
        editButton = Instantiate(EditButtonPF, transform);
    }

    public void HideEditButton()
    {
        Destroy(editButton);
    }
    // Use this for initialization
    void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
