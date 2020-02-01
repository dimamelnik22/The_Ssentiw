using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoleDot : MonoBehaviour {
    [Header("Element")]
    public Elements point;

    [Header("Position")]
    public int posX;
    public int posY;

    [Header("Navigation")]
    public GameObject up;
    public GameObject down;
    public GameObject left;
    public GameObject right;

    [Header("Prefabs")]
    public GameObject PointPF;
    public GameObject DotPF;
    public GameObject EditButtonPF;
    public GameObject StartPF;
    public GameObject FinishPF;

    [HideInInspector]
    public GameObject startFinish;
    [HideInInspector]
    public bool isUsedBySolution = false;
    [HideInInspector]
    public bool isUsedByPlayer = false;
    [HideInInspector]
    public bool hasPoint = false;
    [HideInInspector]
    public GameObject dot;

    private GameObject editButton;
    
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

    public void AddStart()
    {
        startFinish = Instantiate(StartPF, transform);
        startFinish.GetComponent<StartDot>().dot = this.gameObject;
    }

    public void AddFinish()
    {
        startFinish = Instantiate(FinishPF, transform);
        startFinish.GetComponent<FinishDot>().dot = this.gameObject;
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
    public void CreateObject()
    {
        if (hasPoint)
        {
            point = Instantiate(PointPF, transform).GetComponent<Elements>();
            point.GetComponent<PoleEltPoint>().Attach(this.gameObject);
            GameObject.FindGameObjectWithTag("Pole").GetComponent<Pole>().eltsManager.points.Add(point);
        }
        else if (startFinish != null)
            if (startFinish.GetComponent<FinishDot>() != null)
                startFinish.GetComponent<FinishDot>().Create();
            else if (startFinish.GetComponent<StartDot>() != null)
                startFinish.GetComponent<StartDot>().Create();
    }

    public void ShowEditButton()
    {
        editButton = Instantiate(EditButtonPF, transform);
        editButton.GetComponent<EditDot>().dot = this.gameObject;
    }

    public void HideEditButton()
    {
        Destroy(editButton);
    }
}
