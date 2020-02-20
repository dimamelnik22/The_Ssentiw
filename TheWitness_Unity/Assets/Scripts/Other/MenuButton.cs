using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuButton : MonoBehaviour
{
    [HideInInspector]
    public int index = -1;
    void OnMouseDown()
    {
        GameObject.FindGameObjectWithTag("Core").GetComponent<MenuManager>().PressButton(index);
    }
}
