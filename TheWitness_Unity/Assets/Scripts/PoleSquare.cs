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

    public GameObject ShapeBoolPF;
    public GameObject shapeBool;

    public void UpdateShapeBool(bool exists)
    {
        if (exists && shapeBool == null)
        {
            shapeBool = Instantiate(ShapeBoolPF, transform);
        }
        else if (!exists && shapeBool != null)
        {
            Destroy(shapeBool);
        }
    }
    public void ShowEditButton()
    {
        editButton = Instantiate(EditButtonPF, transform);
        editButton.GetComponent<EditSquare>().square = this.gameObject;
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
