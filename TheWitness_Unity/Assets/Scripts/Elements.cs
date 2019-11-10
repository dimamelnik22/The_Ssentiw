using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elements : MonoBehaviour
{
    //SP(num)s{indexIIindexJJheightHwidthWbitmap}

    //const string t = "SRP";



    public bool colorlerping = false;
    public float countdown = 0.5f;
    public int x;
    public int y;
    public Color c;
    public bool rotate = false;
    /*private char type;
    public char Type
    {
        set
        {
            for(int i = 0;i < t.Length;++i)
            {
                if (t[i] == value)
                {
                    type = value;
                    break;
                }
            }
            Debug.Log("wrong type!!!");
        }
        get { return type; }
    }*/
    public void ShowUnsolvedColor()
    {
        colorlerping = true;
        StartCoroutine(Do());
    }
    public void ShowNormalizedColor()
    {
        colorlerping = false;
        GetComponent<Renderer>().material.color = c;
    }
    public IEnumerator Do()
    {
        bool tored = true;
        while (colorlerping)
        {
            if (countdown > 0f)
            {
                if (tored)
                    GetComponent<Renderer>().material.color = Color.Lerp(Color.red, c, 2 * countdown);
                else
                    GetComponent<Renderer>().material.color = Color.Lerp(c, Color.red, 2 * countdown);
            }
            else
            {
                countdown = 0.5f;
                tored = !tored;
            }
            countdown -= Time.deltaTime;
            yield return null;
        }
    }
}
