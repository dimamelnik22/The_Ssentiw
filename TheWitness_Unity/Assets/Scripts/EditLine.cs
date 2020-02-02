using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditLine : MonoBehaviour
{
    public GameObject line;
    void OnMouseDown()
    {
        GameObject.FindGameObjectWithTag("Editor").GetComponent<Editor>().EditLine(line);
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
