using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditDot : MonoBehaviour
{

    void OnMouseDown()
    {
        GameObject.FindGameObjectWithTag("Editor").GetComponent<Editor>().AddElementToDot(GetComponentsInParent<PoleDot>()[0].posX, GetComponentsInParent<PoleDot>()[0].posY);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
