using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuItem : MonoBehaviour
{
    public Text Name;
    
    public void SetName(string name)
    {
        Name.text = name;
    }
}
