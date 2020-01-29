using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishDot : MonoBehaviour
{

    public GameObject dot;
    public void LinkDot(GameObject finish)
    {
        dot = finish;
        dot.GetComponent<PoleDot>().startFinish = this.gameObject;
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
