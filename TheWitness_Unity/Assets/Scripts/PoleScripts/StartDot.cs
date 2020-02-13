using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartDot : MonoBehaviour
{

    [Header("Location")]
    public GameObject dot;

    [Header("Prefabs")]
    public GameObject StartPF;

    private GameObject start;

    public void Create()
    {
        if (start == null)
            start = Instantiate(StartPF, transform);
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
