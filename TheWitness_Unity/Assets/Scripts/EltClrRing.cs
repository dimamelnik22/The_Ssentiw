using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EltClrRing : MonoBehaviour {
    public Color c;
    public float countdown = 0f;
    public bool colorlerping = false;
    public bool tored = true;
    public void ShowUnsolvedColor()
    {
        colorlerping = true;
        countdown = 0.5f;
    }
    public void ShowNormalizedColor()
    {
        colorlerping = false;
        GetComponent<Renderer>().material.color = c;
    }

    // Use this for initialization
    void Start () {

	}
	
	// Update is called once per frame
	void Update () {
        if (colorlerping)
        {
            if (countdown > 0f)
            {
                if (tored)
                    GetComponent<Renderer>().material.color = Color.Lerp(Color.red, c,2* countdown);
                else
                    GetComponent<Renderer>().material.color = Color.Lerp(c, Color.red,2* countdown);
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
