using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditSquare : MonoBehaviour
{
    public GameObject square;

    void OnMouseDown()
    {
        GameObject.FindGameObjectWithTag("Editor").GetComponent<Editor>().EditSquare(square);
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
