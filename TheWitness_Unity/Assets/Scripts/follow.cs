using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class follow : MonoBehaviour {

    private Vector2 _startPos;
    private float eps = 0.9f;
    public GameObject dot;
    public GameObject nextdot;
    public GameObject currentLine;
    public GameObject nearestDot;
    public ActivePath path;
    public bool onDot = true;
    public bool moveHor = true;
    public bool notActive = false;
    public Vector2 lastpos;
    Vector3 dista = new Vector3(10f,10f,10f);
    private float distanceToDot;
    public Text debugtext;
    Vector3 stepz = new Vector3(0f, 0f, -0.5f);
    public GameObject[][] poleDots;




    public List<GameObject> pathDots;

    // Use this for initialiyation
    void Start () {
        #if UNITY_EDITOR
        lastpos = Input.mousePosition;
        #endif
    }
	
	// Update is called once per frame
	void Update () {
        if (!Core.PolePreferences.isFrozen && !notActive)
        {
            float upLimit = transform.position.y;
            float downLimit = transform.position.y;
            float leftLimit = transform.position.x;
            float rightLimit = transform.position.x;
            for (int y = 0; y < poleDots.Length; y++)
                for (int x = 0; x < poleDots[y].Length; x++) 
                {
                    if (poleDots[y][x] !=null)
                    {
                        GameObject dot = poleDots[y][x];
                        if (dot.transform.position.y == transform.position.y && dot.transform.position.x > transform.position.x && dot.transform.position.x > rightLimit)
                        {
                            rightLimit = dot.transform.position.x;
                        }
                        if (dot.transform.position.y == transform.position.y && dot.transform.position.x < transform.position.x && dot.transform.position.x < leftLimit)
                        {
                            leftLimit = dot.transform.position.x;
                        }
                        if (dot.transform.position.x == transform.position.x && dot.transform.position.y > transform.position.y && dot.transform.position.y > upLimit)
                        {
                            upLimit = dot.transform.position.y;
                        }
                        if (dot.transform.position.x == transform.position.x && dot.transform.position.y < transform.position.y && dot.transform.position.y < downLimit)
                        {
                            downLimit = dot.transform.position.y;
                        }
                    }
                }
            foreach (GameObject dot in pathDots)
            {
                if (dot != pathDots[pathDots.Count - 1] && dot.transform.position.y == transform.position.y && dot.transform.position.x > transform.position.x && dot.transform.position.x <= rightLimit)
                {
                    rightLimit = dot.transform.position.x - 1f;
                }
                if (dot != pathDots[pathDots.Count - 1] && dot.transform.position.y == transform.position.y && dot.transform.position.x < transform.position.x && dot.transform.position.x >= leftLimit)
                {
                    leftLimit = dot.transform.position.x + 1f;
                }
                if (dot != pathDots[pathDots.Count - 1] && dot.transform.position.x == transform.position.x && dot.transform.position.y > transform.position.y && dot.transform.position.y <= upLimit)
                {
                    upLimit = dot.transform.position.y - 1f;
                }
                if (dot != pathDots[pathDots.Count - 1] && dot.transform.position.x == transform.position.x && dot.transform.position.y < transform.position.y && dot.transform.position.y >= downLimit)
                {
                    downLimit = dot.transform.position.y + 1f;
                }
            }

            Vector2 dir = new Vector2(0f, 0f);

            #if UNITY_EDITOR
            if (Input.GetMouseButton(0))
            {

                GetComponent<ParticleSystem>().Play();
                dir = 0.03f * MenuManager.MainSettings.speed * new Vector2(Input.mousePosition.x - lastpos.x, Input.mousePosition.y - lastpos.y);
                lastpos = Input.mousePosition;
            }
            else
            {
                GetComponent<ParticleSystem>().Stop();
                lastpos = Input.mousePosition;
            }
            #endif

            #if !UNITY_EDITOR
            if (Input.touchCount > 0)
            {

                GetComponent<ParticleSystem>().Play();
                var touch = Input.GetTouch(0);
                switch (touch.phase)
                {
                    case TouchPhase.Began:
                        lastpos = touch.position;
                        break;
                    case TouchPhase.Moved:
                        dir = 0.02f * MenuManager.MainSettings.speed * (touch.position - lastpos);
                        lastpos = touch.position;
                        break;
                }   
            }
            else
            {
                GetComponent<ParticleSystem>().Stop();
            }
            #endif

            foreach (GameObject go in GameObject.FindGameObjectsWithTag("PoleDot"))
            {
                if (Mathf.Sqrt(Mathf.Abs(go.transform.position.x - transform.position.x) * Mathf.Abs(go.transform.position.x - transform.position.x) + Mathf.Abs(go.transform.position.y - transform.position.y) * Mathf.Abs(go.transform.position.y - transform.position.y)) <
                    Mathf.Sqrt(Mathf.Abs(dista.x) * Mathf.Abs(dista.x) + Mathf.Abs(dista.y) * Mathf.Abs(dista.y)))
                {
                    dista = go.transform.position - transform.position;
                    nearestDot = go;
                }
            }
            dista = new Vector3(10f, 10f, 10f);
            foreach (GameObject go in GameObject.FindGameObjectsWithTag("PolePart"))
            {
                if (Mathf.Sqrt(Mathf.Abs(go.transform.position.x - transform.position.x) * Mathf.Abs(go.transform.position.x - transform.position.x) + Mathf.Abs(go.transform.position.y - transform.position.y) * Mathf.Abs(go.transform.position.y - transform.position.y)) <
                    Mathf.Sqrt(Mathf.Abs(dista.x) * Mathf.Abs(dista.x) + Mathf.Abs(dista.y) * Mathf.Abs(dista.y)))
                {
                    dista = go.transform.position - transform.position;
                    currentLine = go;
                }
            }
            dista = new Vector3(10f, 10f, 10f);
            if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
            {
                if (moveHor)
                {
                    transform.Translate(new Vector3(dir.x /*+ Mathf.Sign(dir.x) * Mathf.Abs(dir.y)*/, 0f, 0f));
                }
                else
                {
                    if (Mathf.Abs(transform.position.y - nearestDot.transform.position.y) < eps)
                    {
                        if (dir.x > 0 && nearestDot.GetComponent<PoleDot>().AllowedToRight())
                        {
                            transform.position = nearestDot.transform.position;
                            transform.Translate(stepz);
                            moveHor = !moveHor;
                        }
                        else if (dir.x < 0 && nearestDot.GetComponent<PoleDot>().AllowedToLeft())
                        {
                            transform.position = nearestDot.transform.position;
                            transform.Translate(stepz);
                            moveHor = !moveHor;
                        }
                    }
                    else transform.Translate(new Vector3(0f, dir.y + Mathf.Sign(nearestDot.transform.position.y - transform.position.y) * Mathf.Abs(dir.x), 0f));
                }
            }
            else
            {
                if (!moveHor)
                {
                    transform.Translate(new Vector3(0f, dir.y /*+ Mathf.Sign(dir.y) * Mathf.Abs(dir.x)*/, 0f));
                }
                else
                {
                    if (Mathf.Abs(transform.position.x - nearestDot.transform.position.x) < eps)
                    {
                        if (dir.y > 0 && nearestDot.GetComponent<PoleDot>().AllowedToUp())
                        {
                            transform.position = nearestDot.transform.position;
                            transform.Translate(stepz);
                            currentLine = nearestDot.GetComponent<PoleDot>().up;
                            moveHor = !moveHor;
                        }
                        else if (dir.y < 0 && nearestDot.GetComponent<PoleDot>().AllowedToDown())
                        {
                            transform.position = nearestDot.transform.position;
                            transform.Translate(stepz);
                            currentLine = nearestDot.GetComponent<PoleDot>().down;
                            moveHor = !moveHor;
                        }
                    }
                    else transform.Translate(new Vector3(dir.x + Mathf.Sign(nearestDot.transform.position.x - transform.position.x) * Mathf.Abs(dir.y), 0f, 0f));
                }
            }
            
            transform.position = new Vector3(Mathf.Min(rightLimit, Mathf.Max(leftLimit, transform.position.x)), Mathf.Min(upLimit, Mathf.Max(downLimit, transform.position.y)), 0f) + stepz;
        }
        
    }

}
