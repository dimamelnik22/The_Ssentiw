using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Path : MonoBehaviour
{
    private float countdown = 3f;
    public bool fade = false;
    public Material WronColor;
    public Material FadeColor;
    public void Update()
    {
        if (fade)
        {
            if (countdown > 0f)
            {
                GetComponent<Renderer>().material.Lerp(FadeColor, WronColor, countdown/3f);
            }
            countdown -= Time.deltaTime;
        }
    }
}
