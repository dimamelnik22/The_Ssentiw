using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoleSquare : MonoBehaviour {
    
    [Header("Element")]
	public Elements element;

    [Header("Position")]
    public int indexI;
    public int indexJ;

    [Header("Navigation")]
    public GameObject up;
	public GameObject down;
	public GameObject left;
	public GameObject right;

    [Header("Prefabs")]
    public GameObject EditButtonPF;
    public GameObject ShapeBoolPF;

    [HideInInspector]
	public bool hasElem = false;

    private GameObject editButton;
    private GameObject shapeBool;

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
}
