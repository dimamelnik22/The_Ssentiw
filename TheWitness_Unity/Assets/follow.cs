using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class follow : MonoBehaviour {


    private readonly float _speed = 0.4f;
    private Vector2 _startPos;
    public Material blue;
    public Material grey;
    private float eps = 0.5f;
    public GameObject dot;
    public GameObject nextdot;
    public GameObject line;
    public GameObject nearestDot;
    public bool onDot = true;
    public bool moveHor = true;
    Vector3 lastpos;
    Vector3 dista = new Vector3(10f,0f,10f);
    

    // Use this for initialization
    void Start () {
        lastpos = Input.mousePosition;
	}
	
	// Update is called once per frame
	void Update () {
        if (nearestDot == null) nearestDot = dot;
        if (Input.GetMouseButton(0))
        {
            Vector3 dir = 0.03f * (Input.mousePosition - lastpos);
            lastpos = Input.mousePosition;
            Debug.Log(Mathf.Abs(dir.x) + "    " + Mathf.Abs(dir.y));
            if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
            {
                if (moveHor)
                {
                    transform.Translate(new Vector3(dir.x, 0f, 0f));
                }
                else
                {
                    foreach (GameObject go in GameObject.FindGameObjectsWithTag("PoleDot"))
                    {
                        if (Mathf.Sqrt(Mathf.Abs(go.transform.position.x - transform.position.x) * Mathf.Abs(go.transform.position.x - transform.position.x) + Mathf.Abs(go.transform.position.z - transform.position.z) * Mathf.Abs(go.transform.position.z - transform.position.z)) <
                            Mathf.Sqrt(Mathf.Abs(dista.x) * Mathf.Abs(dista.x) + Mathf.Abs(dista.z) * Mathf.Abs(dista.z)))
                        {
                            dista = go.transform.position - transform.position;
                            nearestDot.GetComponent<Renderer>().material.Lerp(blue, grey, 20f);
                            nearestDot = go;
                            nearestDot.GetComponent<Renderer>().material.Lerp(grey, blue, 20f);
                        }
                    }
                    dista = new Vector3(10f, 0f, 10f);
                    if (Mathf.Abs(transform.position.z - nearestDot.transform.position.z) < eps)
                    {
                        if (dir.x > 0 && nearestDot.GetComponent<PoleDot>().AllowedToRight())
                        {
                            transform.position = nearestDot.transform.position;
                            transform.Translate(new Vector3(0f, 1f, 0f));
                            moveHor = !moveHor;
                        }
                        else if (dir.x < 0 && nearestDot.GetComponent<PoleDot>().AllowedToLeft())
                        {
                            transform.position = nearestDot.transform.position;
                            transform.Translate(new Vector3(0f, 1f, 0f));
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
                    foreach (GameObject go in GameObject.FindGameObjectsWithTag("PoleDot"))
                    {
                        if (Mathf.Sqrt(Mathf.Abs(go.transform.position.x - transform.position.x) * Mathf.Abs(go.transform.position.x - transform.position.x) + Mathf.Abs(go.transform.position.z - transform.position.z) * Mathf.Abs(go.transform.position.z - transform.position.z)) <
                            Mathf.Sqrt(Mathf.Abs(dista.x) * Mathf.Abs(dista.x) + Mathf.Abs(dista.z) * Mathf.Abs(dista.z)))
                        {
                            dista = go.transform.position - transform.position;
                            nearestDot.GetComponent<Renderer>().material.Lerp(blue, grey, 20f);
                            nearestDot = go;
                            nearestDot.GetComponent<Renderer>().material.Lerp(grey, blue, 20f);
                        }
                    }
                    dista = new Vector3(10f, 0f, 10f);
                    if (Mathf.Abs(transform.position.x - nearestDot.transform.position.x) < eps)
                    {
                        if (dir.y > 0 && nearestDot.GetComponent<PoleDot>().AllowedToUp())
                        {
                            transform.position = nearestDot.transform.position;
                            transform.Translate(new Vector3(0f, 1f, 0f));
                            moveHor = !moveHor;
                        }
                        else if (dir.y < 0 && nearestDot.GetComponent<PoleDot>().AllowedToDown())
                        {
                            transform.position = nearestDot.transform.position;
                            transform.Translate(new Vector3(0f, 1f, 0f));
                            moveHor = !moveHor;
                        }
                    }
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
                        if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
                        {
                            if (moveHor)
                            {
                                transform.Translate(new Vector3(dir.x, 0f, 0f));
                            }
                            else
                            {
                                foreach (GameObject go in GameObject.FindGameObjectsWithTag("PoleDot"))
                                {
                                    if (Mathf.Sqrt(Mathf.Abs(go.transform.position.x - transform.position.x) * Mathf.Abs(go.transform.position.x - transform.position.x) + Mathf.Abs(go.transform.position.z - transform.position.z) * Mathf.Abs(go.transform.position.z - transform.position.z)) <
                                        Mathf.Sqrt(Mathf.Abs(dista.x) * Mathf.Abs(dista.x) + Mathf.Abs(dista.z) * Mathf.Abs(dista.z)))
                                    {
                                        dista = go.transform.position - transform.position;
                                        nearestDot.GetComponent<Renderer>().material.Lerp(blue, grey, 20f);
                                        nearestDot = go;
                                        nearestDot.GetComponent<Renderer>().material.Lerp(grey, blue, 20f);
                                    }
                                }
                                dista = new Vector3(100f, 0f, 100f);
                                if (Mathf.Abs(transform.position.z - nearestDot.transform.position.z) < eps)
                                {
                                    if (dir.x > 0 && nearestDot.GetComponent<PoleDot>().AllowedToRight())
                                    {
                                        transform.position = nearestDot.transform.position;
                                        transform.Translate(new Vector3(0f, 1f, 0f));
                                        moveHor = !moveHor;
                                    }
                                    else if (dir.x < 0 && nearestDot.GetComponent<PoleDot>().AllowedToLeft())
                                    {
                                        transform.position = nearestDot.transform.position;
                                        transform.Translate(new Vector3(0f, 1f, 0f));
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
                                foreach (GameObject go in GameObject.FindGameObjectsWithTag("PoleDot"))
                                {
                                    if (Mathf.Sqrt(Mathf.Abs(go.transform.position.x - transform.position.x) * Mathf.Abs(go.transform.position.x - transform.position.x) + Mathf.Abs(go.transform.position.z - transform.position.z) * Mathf.Abs(go.transform.position.z - transform.position.z)) <
                                        Mathf.Sqrt(Mathf.Abs(dista.x) * Mathf.Abs(dista.x) + Mathf.Abs(dista.z) * Mathf.Abs(dista.z)))
                                    {
                                        dista = go.transform.position - transform.position;
                                        nearestDot.GetComponent<Renderer>().material.Lerp(blue, grey, 20f);
                                        nearestDot = go;
                                        nearestDot.GetComponent<Renderer>().material.Lerp(grey, blue, 20f);
                                    }
                                }
                                dista = new Vector3(100f, 0f, 100f);
                                if (Mathf.Abs(transform.position.x - nearestDot.transform.position.x) < eps)
                                {
                                    if (dir.y > 0 && nearestDot.GetComponent<PoleDot>().AllowedToUp())
                                    {
                                        transform.position = nearestDot.transform.position;
                                        transform.Translate(new Vector3(0f, 1f, 0f));
                                        moveHor = !moveHor;
                                    }
                                    else if (dir.y < 0 && nearestDot.GetComponent<PoleDot>().AllowedToDown())
                                    {
                                        transform.position = nearestDot.transform.position;
                                        transform.Translate(new Vector3(0f, 1f, 0f));
                                        moveHor = !moveHor;
                                    }
                                }
                            }
                        }
                        break;
                }
                
            
        }
    }
}
