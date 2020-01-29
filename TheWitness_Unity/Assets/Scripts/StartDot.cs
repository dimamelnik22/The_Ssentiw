using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartDot : MonoBehaviour
{


    public GameObject dot;
    public void LinkDot(GameObject start)
    {
        dot = start;
        dot.GetComponent<PoleDot>().startFinish = this.gameObject;
    }

    void OnMouseDown()
    {
        if (GameObject.FindGameObjectWithTag("Editor") != null)
            return;
        GameObject activePath = null;
        if (GameObject.FindGameObjectWithTag("Core").GetComponent<Core>() != null)
        {
            if (GameObject.FindGameObjectWithTag("Core").GetComponent<Core>().pathIsShown)
                return;
            activePath = GameObject.FindGameObjectWithTag("Core").GetComponent<Core>().activePath;
        }
        if (GameObject.FindGameObjectWithTag("Core").GetComponent<MenuManager>() != null)
            activePath = GameObject.FindGameObjectWithTag("Core").GetComponent<MenuManager>().activePath;
        //activePath.GetComponent<ActivePath>().pointer.SetActive(true);
        activePath.GetComponent<ActivePath>().NewStart(dot);
    }
}
