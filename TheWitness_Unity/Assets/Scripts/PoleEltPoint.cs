using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoleEltPoint : MonoBehaviour {

    GameObject attachedDot;
    GameObject attachedLine;
    public Material NormalColor;
    public Material UnsolvedColor;
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
    public void ShowUnsolved()
    {
        GetComponent<Renderer>().material.Lerp(GetComponent<Renderer>().material, UnsolvedColor, 20f);
    }
    public void NormalizeColor()
    {
        GetComponent<Renderer>().material.Lerp(GetComponent<Renderer>().material, NormalColor, 20f);
    }

    // Update is called once per frame
    void Update () {
        
    }
}
