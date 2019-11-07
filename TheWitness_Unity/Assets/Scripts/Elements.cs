using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elements : MonoBehaviour
{
    //SP(num)s{indexIIindexJJheightHwidthWbitmap}

    const string t = "SR";



    public int x;
    public int y;
    public Color c;
    public bool rotate = false;
    private char type;
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
    }
    public int getX()
    {
        return x;
    }
}
