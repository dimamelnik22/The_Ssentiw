using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoleEltPoint : MonoBehaviour {

    GameObject attachedDot;
    GameObject attachedLine;
    public Material NormalColor;
    Color c;
    public float countdown = 0f;
    public bool colorlerping = false;
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
    public void ShowUnsolvedColor()
    {
        colorlerping = true;
        countdown = 0.5f;
    }
    public void ShowNormalizedColor()
    {
        colorlerping = false;
		tored = true;
        GetComponent<Renderer>().material.color = c;
    }
    private void Start()
    {
        c = NormalColor.color;
    }
    // Update is called once per frame
    void Update () {
        if (colorlerping)
        {
            if (countdown > 0f)
            {
                if (tored)
                    GetComponent<Renderer>().material.color = Color.Lerp(Color.red, c, 2 * countdown);
                else
                    GetComponent<Renderer>().material.color = Color.Lerp(c, Color.red, 2 * countdown);
            }
            else
            {
                countdown = 0.5f;
                tored = !tored;
            }
            countdown -= Time.deltaTime;
        }
    }
}
