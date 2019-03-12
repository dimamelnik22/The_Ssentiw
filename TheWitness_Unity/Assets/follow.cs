using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class follow : MonoBehaviour {


    private readonly float _speed = 0.4f;
    private Vector2 _startPos;
    public Material blue;
    public Material grey;
    public Material green;
    private float eps = 0.9f;
    public GameObject dot;
    public GameObject nextdot;
    public GameObject currentLine;
    public GameObject nearestDot;
    public bool onDot = true;
    public bool moveHor = true;
    Vector3 lastpos;
    Vector3 dista = new Vector3(10f,0f,10f);
    private float distanceToDot;
    public Text debugtext;

    private float upLimit = 0f;
    private float downLimit = -20f;
    private float leftLimit = 0f;
    private float rightLimit = 20f;




    public List<GameObject> pathDots;

    // Use this for initialization
    void Start () {
        lastpos = Input.mousePosition;
	}
	
	// Update is called once per frame
	void Update () {

        upLimit = 0f;
        downLimit = -20f;
        leftLimit = 0f;
        rightLimit = 20f;
        foreach (GameObject dot in pathDots)
        {
            if (dot != pathDots[pathDots.Count - 1] && dot.transform.position.z == transform.position.z && dot.transform.position.x > transform.position.x && dot.transform.position.x <= rightLimit)
            {
                rightLimit = dot.transform.position.x - 1f;
            }
            if (dot != pathDots[pathDots.Count - 1] && dot.transform.position.z == transform.position.z && dot.transform.position.x < transform.position.x && dot.transform.position.x >= leftLimit)
            {
                leftLimit = dot.transform.position.x + 1f;
            }
            if (dot != pathDots[pathDots.Count - 1] && dot.transform.position.x == transform.position.x && dot.transform.position.z > transform.position.z && dot.transform.position.z <= upLimit)
            {
                upLimit = dot.transform.position.z - 1f;
            }
            if (dot != pathDots[pathDots.Count - 1] && dot.transform.position.x == transform.position.x && dot.transform.position.z < transform.position.z && dot.transform.position.z >= downLimit)
            {
                downLimit = dot.transform.position.z + 1f;
            }
        }


        if (Input.GetMouseButton(0))
        {
            foreach (GameObject go in GameObject.FindGameObjectsWithTag("PoleDot"))
            {
                if (Mathf.Sqrt(Mathf.Abs(go.transform.position.x - transform.position.x) * Mathf.Abs(go.transform.position.x - transform.position.x) + Mathf.Abs(go.transform.position.z - transform.position.z) * Mathf.Abs(go.transform.position.z - transform.position.z)) <
                    Mathf.Sqrt(Mathf.Abs(dista.x) * Mathf.Abs(dista.x) + Mathf.Abs(dista.z) * Mathf.Abs(dista.z)))
                {
                    dista = go.transform.position - transform.position;
                   // nearestDot.GetComponent<Renderer>().material.Lerp(blue, grey, 20f);
                    nearestDot = go;
                    //nearestDot.GetComponent<Renderer>().material.Lerp(grey, blue, 20f);
                }
            }
            dista = new Vector3(10f, 0f, 10f);
            foreach (GameObject go in GameObject.FindGameObjectsWithTag("PolePart"))
            {
                if (Mathf.Sqrt(Mathf.Abs(go.transform.position.x - transform.position.x) * Mathf.Abs(go.transform.position.x - transform.position.x) + Mathf.Abs(go.transform.position.z - transform.position.z) * Mathf.Abs(go.transform.position.z - transform.position.z)) <
                    Mathf.Sqrt(Mathf.Abs(dista.x) * Mathf.Abs(dista.x) + Mathf.Abs(dista.z) * Mathf.Abs(dista.z)))
                {
                    dista = go.transform.position - transform.position;
                    //currentLine.GetComponent<Renderer>().material.Lerp(blue, grey, 20f);
                    currentLine = go;
                    //currentLine.GetComponent<Renderer>().material.Lerp(grey, blue, 20f);
                }
            }
            dista = new Vector3(10f, 0f, 10f);
            Vector3 dir = 0.03f * (Input.mousePosition - lastpos);
            lastpos = Input.mousePosition;
            //Debug.Log(Mathf.Abs(dir.x) + "    " + Mathf.Abs(dir.y));
            if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
            {
                if (moveHor)
                {
                    transform.Translate(new Vector3(dir.x, 0f, 0f));
                    
                }
                else
                {
                    //debugtext.text = GameObject.FindGameObjectsWithTag("PoleDot").Length.ToString();
                    
                    if (Mathf.Abs(transform.position.z - nearestDot.transform.position.z) < eps)
                    {
                        if (dir.x > 0 && nearestDot.GetComponent<PoleDot>().AllowedToRight())
                        {
                            transform.position = nearestDot.transform.position;
                            transform.Translate(new Vector3(0f, 1f, 0f));
                            //currentLine = nearestDot.GetComponent<PoleDot>().right;
                            moveHor = !moveHor;
                        }
                        else if (dir.x < 0 && nearestDot.GetComponent<PoleDot>().AllowedToLeft())
                        {
                            transform.position = nearestDot.transform.position;
                            transform.Translate(new Vector3(0f, 1f, 0f));
                           // currentLine = nearestDot.GetComponent<PoleDot>().left;
                            moveHor = !moveHor;
                        }
                    }
                    else transform.Translate(new Vector3(0f, 0f, dir.y));
                }
            }
            else
            {
                if (!moveHor)
                {
                    transform.Translate(new Vector3(0f, 0f, dir.y));
                    
                }
                else
                {
                    //debugtext.text = GameObject.FindGameObjectsWithTag("PoleDot").Length.ToString();
                    
                    if (Mathf.Abs(transform.position.x - nearestDot.transform.position.x) < eps)
                    {
                        if (dir.y > 0 && nearestDot.GetComponent<PoleDot>().AllowedToUp())
                        {
                            transform.position = nearestDot.transform.position;
                            transform.Translate(new Vector3(0f, 1f, 0f));
                            //currentLine.GetComponent<Renderer>().material.Lerp(green, grey, 2f);
                            currentLine = nearestDot.GetComponent<PoleDot>().up;
                            //currentLine.GetComponent<Renderer>().material.Lerp(grey, green, 2f);
                            moveHor = !moveHor;
                        }
                        else if (dir.y < 0 && nearestDot.GetComponent<PoleDot>().AllowedToDown())
                        {
                            transform.position = nearestDot.transform.position;
                            transform.Translate(new Vector3(0f, 1f, 0f));
                            //currentLine.GetComponent<Renderer>().material.Lerp(green, grey, 2f);
                            currentLine = nearestDot.GetComponent<PoleDot>().down;
                            //currentLine.GetComponent<Renderer>().material.Lerp(grey, green, 2f);
                            moveHor = !moveHor;
                        }
                    }
                    else transform.Translate(new Vector3(dir.x, 0f, 0f));
                }
            }
        }
        else
        {
            lastpos = Input.mousePosition;
        }
        
        



        if (Input.touchCount > 0)
        {
            

                var touch = Input.GetTouch(0);

                switch (touch.phase)
                {
                    
                    case TouchPhase.Began:
                        _startPos = touch.position;
                        break;

                    case TouchPhase.Moved:
                        
                        //Vector3 pos = transform.position;

                        Vector2 dir = 0.02f * (touch.position - _startPos);
                        _startPos = touch.position;
                    //Debug.Log(Mathf.Abs(dir.x) + "    " + Mathf.Abs(dir.y));
                    foreach (GameObject go in GameObject.FindGameObjectsWithTag("PoleDot"))
                    {
                        if (Mathf.Sqrt(Mathf.Abs(go.transform.position.x - transform.position.x) * Mathf.Abs(go.transform.position.x - transform.position.x) + Mathf.Abs(go.transform.position.z - transform.position.z) * Mathf.Abs(go.transform.position.z - transform.position.z)) <
                            Mathf.Sqrt(Mathf.Abs(dista.x) * Mathf.Abs(dista.x) + Mathf.Abs(dista.z) * Mathf.Abs(dista.z)))
                        {
                            dista = go.transform.position - transform.position;
                            //nearestDot.GetComponent<Renderer>().material.Lerp(blue, grey, 20f);
                            nearestDot = go;
                            //nearestDot.GetComponent<Renderer>().material.Lerp(grey, blue, 20f);
                        }
                    }
                    dista = new Vector3(10f, 0f, 10f);
                    foreach (GameObject go in GameObject.FindGameObjectsWithTag("PolePart"))
                    {
                        if (Mathf.Sqrt(Mathf.Abs(go.transform.position.x - transform.position.x) * Mathf.Abs(go.transform.position.x - transform.position.x) + Mathf.Abs(go.transform.position.z - transform.position.z) * Mathf.Abs(go.transform.position.z - transform.position.z)) <
                            Mathf.Sqrt(Mathf.Abs(dista.x) * Mathf.Abs(dista.x) + Mathf.Abs(dista.z) * Mathf.Abs(dista.z)))
                        {
                            dista = go.transform.position - transform.position;
                            //currentLine.GetComponent<Renderer>().material.Lerp(blue, grey, 20f);
                            currentLine = go;
                           // currentLine.GetComponent<Renderer>().material.Lerp(grey, blue, 20f);
                        }
                    }
                    dista = new Vector3(10f, 0f, 10f);
                    //Vector3 dir = 0.03f * (Input.mousePosition - lastpos);
                    lastpos = Input.mousePosition;
                    //Debug.Log(Mathf.Abs(dir.x) + "    " + Mathf.Abs(dir.y));
                    if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
                    {
                        if (moveHor)
                        {
                            transform.Translate(new Vector3(dir.x, 0f, 0f));
                        }
                        else
                        {
                            //debugtext.text = GameObject.FindGameObjectsWithTag("PoleDot").Length.ToString();

                            if (Mathf.Abs(transform.position.z - nearestDot.transform.position.z) < eps)
                            {
                                if (dir.x > 0 && nearestDot.GetComponent<PoleDot>().AllowedToRight())
                                {
                                    transform.position = nearestDot.transform.position;
                                    transform.Translate(new Vector3(0f, 1f, 0f));
                                    //currentLine = nearestDot.GetComponent<PoleDot>().right;
                                    moveHor = !moveHor;
                                }
                                else if (dir.x < 0 && nearestDot.GetComponent<PoleDot>().AllowedToLeft())
                                {
                                    transform.position = nearestDot.transform.position;
                                    transform.Translate(new Vector3(0f, 1f, 0f));
                                    // currentLine = nearestDot.GetComponent<PoleDot>().left;
                                    moveHor = !moveHor;
                                }
                            }
                        }
                    }
                    else
                    {
                        if (!moveHor)
                        {
                            transform.Translate(new Vector3(0f, 0f, dir.y));
                        }
                        else
                        {
                            //debugtext.text = GameObject.FindGameObjectsWithTag("PoleDot").Length.ToString();

                            if (Mathf.Abs(transform.position.x - nearestDot.transform.position.x) < eps)
                            {
                                if (dir.y > 0 && nearestDot.GetComponent<PoleDot>().AllowedToUp())
                                {
                                    transform.position = nearestDot.transform.position;
                                    transform.Translate(new Vector3(0f, 1f, 0f));
                                   // currentLine.GetComponent<Renderer>().material.Lerp(green, grey, 2f);
                                    currentLine = nearestDot.GetComponent<PoleDot>().up;
                                   // currentLine.GetComponent<Renderer>().material.Lerp(grey, green, 2f);
                                    moveHor = !moveHor;
                                }
                                else if (dir.y < 0 && nearestDot.GetComponent<PoleDot>().AllowedToDown())
                                {
                                    transform.position = nearestDot.transform.position;
                                    transform.Translate(new Vector3(0f, 1f, 0f));
                                    //currentLine.GetComponent<Renderer>().material.Lerp(green, grey, 2f);
                                    currentLine = nearestDot.GetComponent<PoleDot>().down;
                                   // currentLine.GetComponent<Renderer>().material.Lerp(grey, green, 2f);
                                    moveHor = !moveHor;
                                }
                            }
                        }
                    }
                    break;
                }
                
            
        }
        transform.position = new Vector3(Mathf.Min(rightLimit, Mathf.Max(leftLimit, transform.position.x)), 1f, Mathf.Min(upLimit, Mathf.Max(downLimit, transform.position.z)));
    }
}
