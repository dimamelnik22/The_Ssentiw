using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditDot : MonoBehaviour
{
    [HideInInspector]
    public GameObject dot;
    void OnMouseDown()
    {
        GameObject.FindGameObjectWithTag("Editor").GetComponent<Editor>().EditDot(dot);
    }
}
