using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoleDot : MonoBehaviour {
    public GameObject DotPF;
    public GameObject EditButtonPF;
    public GameObject dot;
    public GameObject editButton;
    public GameObject up;
    public GameObject down;
    public GameObject left;
    public GameObject right;
    public bool isUsedBySolution = false;
    public bool isUsedByPlayer = false;
    public bool hasPoint = false;
    public Elements point;
    public int posX;
    public int posY;
    
    public void AddLine(GameObject newLine, GameObject anotherDot)
    {
        if (posX < anotherDot.GetComponent<PoleDot>().posX)
        {
            right = newLine;
        }
        else if (posX > anotherDot.GetComponent<PoleDot>().posX)
        {
            left = newLine;
        }
        else if (posY < anotherDot.GetComponent<PoleDot>().posY)
        {
            down = newLine;
        }
        else if (posY > anotherDot.GetComponent<PoleDot>().posY)
        {
            up = newLine;
        }
    }
    public bool AllowedToUp()
    {
        return (up != null);
    }
    public bool AllowedToDown()
    {
        return (down != null);
    }
    public bool AllowedToLeft()
    {
        return (left != null);
    }
    public bool AllowedToRight()
    {
        return (right != null);
    }

    public void CreateDot()
    {
        if (dot == null)
            dot = Instantiate(DotPF, transform);
    }

    public void ShowEditButton()
    {
        editButton = Instantiate(EditButtonPF, transform);
    }

    public void HideEditButton()
    {
        Destroy(editButton);
    }
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
