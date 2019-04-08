using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoleLine : MonoBehaviour {
    public bool isHorizontal = false;
    public GameObject Line;
    public int speed = 2;
	public GameObject up;
	public GameObject down;
	public GameObject left;
	public GameObject right;
    public bool isUsedBySolution = false;
    public bool isUsedByPlayer = false;
    public bool hasPoint = false;
    public GameObject point;

    // Use this for initialization
    void Start () {
        if (isHorizontal)
        {
            Line.transform.localScale = new Vector3(0f, 1f, 1f);
            Line.transform.localPosition =new Vector3(-2.5f, 0f, 0f);
        }
        else
        {
            Line.transform.localScale = new Vector3(1f, 0f, 1f);
            Line.transform.localPosition =  new Vector3(0f, 2.5f, 0f);
        }
    }
	
	// Update is called once per frame
	void Update () {
		if (isHorizontal)
        {
            if (Line.transform.localScale.x < 5f)
            {
                Line.transform.localScale = new Vector3(Line.transform.localScale.x + 2 * speed * Time.deltaTime, 1f, 1f);
                Line.transform.Translate(speed*Time.deltaTime, 0f, 0f);
            }
            else
            {
                Line.transform.localScale = new Vector3(5f, 1f, 1f);
                Line.transform.localPosition =new Vector3(0f, 0f, 0f);
            }
        }
        else
        {
            if (Line.transform.localScale.y < 5f)
            {
                Line.transform.localScale = new Vector3(1f, Line.transform.localScale.y + 2 * speed * Time.deltaTime, 1f);
                Line.transform.Translate(0f, -speed*Time.deltaTime, 0f);
            }
            else
            {
                Line.transform.localScale = new Vector3(1f, 5f, 1f);
                Line.transform.localPosition =new Vector3(0f, 0f, 0f);
            }
        }
    }
    private void OnDestroy()
    {
        if (isHorizontal)
        {
            if (Line.transform.localScale.x > 0f)
            {
                Line.transform.localScale = new Vector3(Line.transform.localScale.x - 2 * speed * Time.deltaTime, 1f, 1f);
                Line.transform.Translate(-speed * Time.deltaTime, 0f, 0f);
            }
            else
            {
                Line.transform.localScale = new Vector3(0f, 1f, 1f);
                Line.transform.localPosition = new Vector3(-2.5f, 0f, 0f);
            }
        }
        else
        {
            if (Line.transform.localScale.y >0f)
            {
                Line.transform.localScale = new Vector3(1f, Line.transform.localScale.y - 2 * speed * Time.deltaTime, 1f);
                Line.transform.Translate(0f, speed * Time.deltaTime, 0f);
            }
            else
            {
                Line.transform.localScale = new Vector3(1f, 0f, 1f);
                Line.transform.localPosition = new Vector3(0f,2.5f, 0f);
            }
        }
    }

    
}
