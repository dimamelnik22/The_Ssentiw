using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasScript : MonoBehaviour
{
    public GameObject WelcomeTextPF;
    public GameObject EditorButtonPF;
    public GameObject NextButtonPF;

    private GameObject editButton;
    private GameObject nextButton;
    
    public void ShowText()
    {
        Instantiate(WelcomeTextPF, transform);
    }
    public void ShowEditorButton()
    {
        if (editButton == null)
            editButton = Instantiate(EditorButtonPF, transform);
    }
    public void ShowNextButton()
    {
        if (nextButton == null)
            nextButton = Instantiate(NextButtonPF, transform);
    }
}
