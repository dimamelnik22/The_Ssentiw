using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartDot : MonoBehaviour
{


    public GameObject dot;
    public void LinkDot(GameObject start)
    {
        dot = start;
    }

    void OnMouseDown()
    {
        GameObject activePath = null;
        if (GameObject.FindGameObjectWithTag("Core").GetComponent<Core>() != null)
            activePath = GameObject.FindGameObjectWithTag("Core").GetComponent<Core>().activePath;
        if (GameObject.FindGameObjectWithTag("Core").GetComponent<MenuManager>() != null)
            activePath = GameObject.FindGameObjectWithTag("Core").GetComponent<MenuManager>().activePath;
        //activePath.GetComponent<ActivePath>().pointer.SetActive(true);
        activePath.GetComponent<ActivePath>().NewStart(dot);
    }
}
