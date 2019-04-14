using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoleEltStar : MonoBehaviour
{
    public Color c;

    public void ShowUnsolvedColor()
    {
        GetComponent<Renderer>().material.color = Color.red;
    }
    public void ShowNormalizeColor()
    {
        GetComponent<Renderer>().material.color = c;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
