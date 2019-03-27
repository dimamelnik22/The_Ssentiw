using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuItem : MonoBehaviour
{
    public Text Name;

    // Start is called before the first frame update
    public void SetName(string name)
    {
        Name.text = name;
    }
}
