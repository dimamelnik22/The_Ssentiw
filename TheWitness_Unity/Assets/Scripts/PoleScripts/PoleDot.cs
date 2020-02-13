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

   // [HideInInspector]
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
        return (up != null && !up.GetComponent<PoleLine>().cut);
    }
    public bool AllowedToDown()
    {
        return (down != null && !down.GetComponent<PoleLine>().cut);
    }
    public bool AllowedToLeft()
    {
        return (left != null && !left.GetComponent<PoleLine>().cut);
    }
    public bool AllowedToRight()
    {
        return (right != null && !right.GetComponent<PoleLine>().cut);
    }

    public void CreateDot()
    {
        if (dot == null)
            dot = Instantiate(DotPF, transform);
    }
    public void DeleteDot()
    {
        if(hasPoint)
        {
            this.gameObject.GetComponentInParent<Transform>().GetComponentInParent<Pole>().eltsManager.points.Remove(point);
            Destroy(point.gameObject);
            hasPoint = false;
        }
        if(startFinish != null)
        {
            this.gameObject.GetComponentInParent<Transform>().GetComponentInParent<Pole>().starts.Remove(this.gameObject);
            this.gameObject.GetComponentInParent<Transform>().GetComponentInParent<Pole>().finishes.Remove(this.gameObject);
            //sf
            Destroy(startFinish);
        }
        if (dot != null)
            Destroy(dot);
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
