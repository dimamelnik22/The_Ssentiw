using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditSquare : MonoBehaviour
{
    [HideInInspector]
    public GameObject square;
    void OnMouseDown()
    {
        GameObject.FindGameObjectWithTag("Editor").GetComponent<Editor>().EditSquare(square);
    }
}
