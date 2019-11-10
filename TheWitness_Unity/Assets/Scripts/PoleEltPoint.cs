using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoleEltPoint : Elements {
    public bool down = false;
    public bool right = false;
    GameObject attachedDot;
    GameObject attachedLine;
    public void SetDot(GameObject dot)
    {
        attachedDot = dot;
        x = dot.GetComponent<PoleDot>().posX;
        y = dot.GetComponent<PoleDot>().posY;

    }
    public void SetLine(GameObject line)
    {
        attachedLine = line;
        if(line.GetComponent<PoleLine>().isHorizontal)
        {
            right = true;
            x = line.GetComponent<PoleLine>().left.GetComponent<PoleDot>().posX;
            y = line.GetComponent<PoleLine>().left.GetComponent<PoleDot>().posY;
        }
        else
        {
            down  = true;
            x = line.GetComponent<PoleLine>().up.GetComponent<PoleDot>().posX;
            y = line.GetComponent<PoleLine>().up.GetComponent<PoleDot>().posY;
        }
    }
    public bool IsSolvedByPlayer()
    {
        if (attachedDot != null) return attachedDot.GetComponent<PoleDot>().isUsedByPlayer;
        else return attachedLine.GetComponent<PoleLine>().isUsedByPlayer;
    }
    void Start()
    {
    }

    void Update () {
        
    }
}
