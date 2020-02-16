using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasScript : MonoBehaviour
{
    public GameObject WelcomeTextPF;
    public GameObject EditorButtonPF;
    
    public void ShowText()
    {
        Instantiate(WelcomeTextPF, transform);
    }
    public void ShowEditorButton()
    {
        Instantiate(EditorButtonPF, transform);
    }
}
