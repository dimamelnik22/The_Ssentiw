using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EltClrRing : Elements {
    public bool colorlerping = false;
    public void ShowUnsolvedColor()
    {
        colorlerping = true;
        StartCoroutine(Do());
    }
    public void ShowNormalizedColor()
    {
        colorlerping = false;
        GetComponent<Renderer>().material.color = c;
    }
    
    void Start ()
    {
	}
	
	// Update is called once per frame
	void Update ()
    {

    }
    IEnumerator Do()
    {
        float countdown = 0.5f;
        bool tored = true;
        while (colorlerping)
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
            yield return null;
        }
    }
}
