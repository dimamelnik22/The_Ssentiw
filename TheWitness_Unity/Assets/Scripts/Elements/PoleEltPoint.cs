using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoleEltPoint : Elements {

    public int GetX()
    {
        if (location.GetComponent<PoleDot>() != null)
        {
            return location.GetComponent<PoleDot>().posX;
        }
        else
        {
            if (location.GetComponent<PoleLine>().isHorizontal)
            {
                return location.GetComponent<PoleLine>().left.GetComponent<PoleDot>().posX;
            }
            else
            {
                return location.GetComponent<PoleLine>().up.GetComponent<PoleDot>().posX;
            }

        }
    }
    public int GetY()
    {
        if (location.GetComponent<PoleDot>() != null)
        {
            return location.GetComponent<PoleDot>().posY;
        }
        else
        {
            if (location.GetComponent<PoleLine>().isHorizontal)
            {
                return location.GetComponent<PoleLine>().left.GetComponent<PoleDot>().posY;
            }
            else
            {
                return location.GetComponent<PoleLine>().up.GetComponent<PoleDot>().posY;
            }

        }
    }
    
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
        c = new Material(this.GetComponent<Renderer>().material);
    }
}
