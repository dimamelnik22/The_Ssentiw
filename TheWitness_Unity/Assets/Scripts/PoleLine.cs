using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoleLine : MonoBehaviour {
    public GameObject pole;
    public bool isHorizontal = false;
    public GameObject Line;
    public readonly int speed = 8;
	public GameObject up;
	public GameObject down;
	public GameObject left;
	public GameObject right;
    public bool isUsedBySolution = false;
    public bool isUsedByPlayer = false;
    public bool hasPoint = false;
    public GameObject point;
    private int dir = 1;
    private bool isScaling = false;
    public bool scalingIsFinished = false;

    //dots order
    public GameObject first;
    public GameObject second;

    public void StartScaling(GameObject dot)
    {
        first = dot;
        if (Line.transform.localScale.x + Line.transform.localScale.y < 2f)
        {
            
            if (dot == up)
            {
                Line.transform.position = dot.transform.position;
                dir = 1;
                isHorizontal = false;
                second = down;
            }
            else if (dot == down)
            {
                Line.transform.position = dot.transform.position;
                dir = -1;
                isHorizontal = false;
                second = up;
            }
            else if (dot == left)
            {
                Line.transform.position = dot.transform.position;
                dir = 1;
                isHorizontal = true;
                second = right;
            }
            else if (dot == right)
            {
                Line.transform.position = dot.transform.position;
                dir = -1;
                isHorizontal = true;
                second = left;
            }
            isScaling = true;
        }
    }

    // Use this for initialization
    void Start () {
        pole = GameObject.FindGameObjectWithTag("Pole");
        //if (isHorizontal)
        //{
        //    Line.transform.localScale = new Vector3(0f, 1f, 1f);
        //    Line.transform.localPosition =new Vector3(-2.5f, 0f, 0f);
        //}
        //else
        //{
        //    Line.transform.localScale = new Vector3(1f, 0f, 1f);
        //    Line.transform.localPosition =  new Vector3(0f, 2.5f, 0f);
        //}
    }
	
	// Update is called once per frame
	void Update () {
        if (isScaling)
        {
            if (isHorizontal)
            {
                if (Line.transform.localScale.x < 5f)
                {
                    Line.transform.localScale = new Vector3(Line.transform.localScale.x + 2 * speed * Time.deltaTime, 1f, 1f);
                    Line.transform.Translate(speed * Time.deltaTime * dir, 0f, 0f);
                }
                else
                {
                    Line.transform.localScale = new Vector3(5f, 1f, 1f);
                    Line.transform.localPosition = new Vector3(0f, 0f, 0f);
                    isScaling = false;
                    scalingIsFinished = true;
                    second.GetComponent<PoleDot>().CreateDot();
                    pole.GetComponent<Pole>().StartScaling(second);
                }
            }
            else
            {
                if (Line.transform.localScale.y < 5f)
                {
                    Line.transform.localScale = new Vector3(1f, Line.transform.localScale.y + 2 * speed * Time.deltaTime, 1f);
                    Line.transform.Translate(0f, -speed * Time.deltaTime * dir, 0f);
                }
                else
                {
                    Line.transform.localScale = new Vector3(1f, 5f, 1f);
                    Line.transform.localPosition = new Vector3(0f, 0f, 0f);
                    isScaling = false;
                    scalingIsFinished = true;
                    second.GetComponent<PoleDot>().CreateDot();
                    pole.GetComponent<Pole>().StartScaling(second);
                }
            }
        }
    }
    //private void OnDestroy()
    //{
    //    if (isHorizontal)
    //    {
    //        if (Line.transform.localScale.x > 0f)
    //        {
    //            Line.transform.localScale = new Vector3(Line.transform.localScale.x - 2 * speed * Time.deltaTime, 1f, 1f);
    //            Line.transform.Translate(-speed * Time.deltaTime, 0f, 0f);
    //        }
    //        else
    //        {
    //            Line.transform.localScale = new Vector3(0f, 1f, 1f);
    //            Line.transform.localPosition = new Vector3(-2.5f, 0f, 0f);
    //        }
    //    }
    //    else
    //    {
    //        if (Line.transform.localScale.y >0f)
    //        {
    //            Line.transform.localScale = new Vector3(1f, Line.transform.localScale.y - 2 * speed * Time.deltaTime, 1f);
    //            Line.transform.Translate(0f, speed * Time.deltaTime, 0f);
    //        }
    //        else
    //        {
    //            Line.transform.localScale = new Vector3(1f, 0f, 1f);
    //            Line.transform.localPosition = new Vector3(0f,2.5f, 0f);
    //        }
    //    }
    //}

    
}
