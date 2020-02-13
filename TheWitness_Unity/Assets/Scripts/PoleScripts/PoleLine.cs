using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoleLine : MonoBehaviour {

    [Header("Element")]
    public Elements point;

    [Header("Navigation")]
    public GameObject up;
	public GameObject down;
	public GameObject left;
	public GameObject right;

    [Header("Scale speed")]
    public readonly int speed = 20;

    [Header("Prefabs")]
    public GameObject EditButtonPF;
    public GameObject PointPF;

    [HideInInspector]
    public GameObject pole;
    [HideInInspector]
    public GameObject Line;
    [HideInInspector]
    public bool isUsedBySolution = false;
    [HideInInspector]
    public bool isUsedByPlayer = false;
    [HideInInspector]
    public bool hasPoint = false;
    [HideInInspector]
    public bool scalingIsFinished = false;
    [HideInInspector]
    public bool isHorizontal = false;
    [HideInInspector]
    public bool cut = false;

    private GameObject scaleEndDot;
    private GameObject editButton;
    private int dir = 1;
    private bool isScaling = false;


    public void StartScaling(GameObject dot)
    {
        if (Line.transform.localScale.x + Line.transform.localScale.y < 2f && !cut)
        {
            if (dot == up)
            {
                Line.transform.position = dot.transform.position;
                dir = 1;
                isHorizontal = false;
                scaleEndDot = down;
            }
            else if (dot == down)
            {
                Line.transform.position = dot.transform.position;
                dir = -1;
                isHorizontal = false;
                scaleEndDot = up;
            }
            else if (dot == left)
            {
                Line.transform.position = dot.transform.position;
                dir = 1;
                isHorizontal = true;
                scaleEndDot = right;
            }
            else if (dot == right)
            {
                Line.transform.position = dot.transform.position;
                dir = -1;
                isHorizontal = true;
                scaleEndDot = left;
            }
            isScaling = true;
        }
    }

    public void CreatePoint()
    {
        if (hasPoint)
        {
            point = Instantiate(PointPF, transform).GetComponent<Elements>();
            point.GetComponent<PoleEltPoint>().Attach(this.gameObject);
            GameObject.FindGameObjectWithTag("Pole").GetComponent<Pole>().eltsManager.points.Add(point);
        }
    }

    public void CutOrCreateLine()
    {
        cut = !cut;
        if (cut)
        {
            Line.transform.localPosition = new Vector3(0f, 0f, 0f);
            Line.transform.localScale = new Vector3(0f, 0f, 0f);
            if (isHorizontal)
            {
                var dot = left.GetComponent<PoleDot>();
                if (!dot.AllowedToDown() && !dot.AllowedToLeft() && !dot.AllowedToRight() && !dot.AllowedToUp())
                {
                    dot.DeleteDot();
                }
                dot = right.GetComponent<PoleDot>();
                if (!dot.AllowedToDown() && !dot.AllowedToLeft() && !dot.AllowedToRight() && !dot.AllowedToUp())
                {
                    dot.DeleteDot();
                }
            }
            else
            {
                var dot = up.GetComponent<PoleDot>();
                if (!dot.AllowedToDown() && !dot.AllowedToLeft() && !dot.AllowedToRight() && !dot.AllowedToUp())
                {
                    dot.DeleteDot();
                }
                dot = down.GetComponent<PoleDot>();
                if (!dot.AllowedToDown() && !dot.AllowedToLeft() && !dot.AllowedToRight() && !dot.AllowedToUp())
                {
                    dot.DeleteDot();
                }
            }
        }
        else
        {
            if (isHorizontal)
            {
                Line.transform.localScale = new Vector3(5f, 1f, 1f);
                Line.transform.localPosition = new Vector3(0f, 0f, 0f);
                right.GetComponent<PoleDot>().CreateDot();
                left.GetComponent<PoleDot>().CreateDot();
            }
            else
            {
                Line.transform.localScale = new Vector3(1f, 5f, 1f);
                Line.transform.localPosition = new Vector3(0f, 0f, 0f);
                up.GetComponent<PoleDot>().CreateDot();
                down.GetComponent<PoleDot>().CreateDot();
            }
        }
    }

    public void ShowEditButton()
    {
        editButton = Instantiate(EditButtonPF, transform);
        editButton.GetComponent<EditLine>().line = this.gameObject;
    }

    public void HideEditButton()
    {
        Destroy(editButton);
    }

	void Update () {
        if (isScaling)
        {
            if (isHorizontal)
            {
                if (Line.transform.localScale.x + 2 * speed * Time.deltaTime < 5f)
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
                    //scaleEndDot.GetComponent<PoleDot>().CreateDot();
                    //scaleEndDot.GetComponent<PoleDot>().CreateObject();
                    pole.GetComponent<Pole>().StartScaling(scaleEndDot);
                    CreatePoint();
                }
            }
            else
            {
                if (Line.transform.localScale.y + 2 * speed * Time.deltaTime < 5f)
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
                    //scaleEndDot.GetComponent<PoleDot>().CreateDot();
                    //scaleEndDot.GetComponent<PoleDot>().CreateObject();
                    pole.GetComponent<Pole>().StartScaling(scaleEndDot);
                    CreatePoint();
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
