using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditLine : MonoBehaviour
{
    [HideInInspector]
    public GameObject line;
    void OnMouseDown()
    {
        GameObject.FindGameObjectWithTag("Editor").GetComponent<Editor>().EditLine(line);
    }
}
