using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoleEltPoint : Elements {

    public void Attach(GameObject parentGameObject)
    {
        location = parentGameObject;
    }
    public bool IsSolvedByPlayer()
    {
        if (location.GetComponent<PoleLine>() != null)
            return location.GetComponent<PoleLine>().isUsedByPlayer;
        else if (location.GetComponent<PoleDot>() != null)
            return location.GetComponent<PoleDot>().isUsedByPlayer;
        else
        {
            Debug.LogError("MissAttachedDotError");
            return false;
        }
    }
    void Start()
    {
        c = this.GetComponent<Renderer>().material.color;
    }
}
