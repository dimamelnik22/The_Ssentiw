using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuButton : MonoBehaviour
{
    public GameObject menu;
    public int index = -1;
    void OnMouseDown()
    {
        menu.GetComponent<MenuManager>().PressButton(index);
        Debug.Log("pressed");
    }
    void Start()
    {
        menu = GameObject.FindGameObjectWithTag("Core");
    }
}
