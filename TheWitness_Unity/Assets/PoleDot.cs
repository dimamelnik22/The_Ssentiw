﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoleDot : MonoBehaviour {

    public GameObject up;
    public GameObject down;
    public GameObject left;
    public GameObject right;
    public bool isUsedBySolution = false;
    public bool isUsedByPlayer = false;
    public bool hasPoint = false;
    public GameObject point;
    public int position_x;
    public int position_y;
    
    public void AddLine(GameObject newLine, GameObject anotherDot)
    {
        if (position_x < anotherDot.GetComponent<PoleDot>().position_x)
        {
            right = newLine;
        }
        else if (position_x > anotherDot.GetComponent<PoleDot>().position_x)
        {
            left = newLine;
        }
        else if (position_y < anotherDot.GetComponent<PoleDot>().position_y)
        {
            down = newLine;
        }
        else if (position_y > anotherDot.GetComponent<PoleDot>().position_y)
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

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
