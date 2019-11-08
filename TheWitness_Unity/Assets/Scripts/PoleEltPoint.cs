using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoleEltPoint : Elements {

    GameObject attachedDot;
    GameObject attachedLine;
    public bool tored = true;
    public void SetDot(GameObject dot)
    {
        attachedDot = dot;
    }
    public void SetLine(GameObject line)
    {
        attachedLine = line;
    }
    public bool IsSolvedByPlayer()
    {
        if (attachedDot != null) return attachedDot.GetComponent<PoleDot>().isUsedByPlayer;
        else return attachedLine.GetComponent<PoleLine>().isUsedByPlayer;
    }
    void Start()
    {
        c = new Color(45/255,104/255,1);
        GetComponent<Renderer>().material.color = c;
        Debug.Log(GetComponent<Renderer>().material.color);
        Debug.Log(c);
    }

    void Update () {
        
    }
}
