using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Follow : MonoBehaviour {

    [HideInInspector]
    public GameObject currentLine;
    [HideInInspector]
    public GameObject nearestDot;
    [HideInInspector]
    public ActivePath path;
    [HideInInspector]
    public GameObject[][] poleDots;
    [HideInInspector]
    public List<GameObject> pathDots;
    [HideInInspector]
    public List<GameObject> allPathDots;
    public float upLimit;
    public float downLimit;
    public float leftLimit;
    public float rightLimit;

    private readonly float eps = 0.9f;
    private bool moveHor = true;
    public Vector2 lastPos;
    private Vector3 maxDistance = new Vector3(10f, 10f, 10f);
    private Vector3 stepz = new Vector3(0f, 0f, -0.5f);

    void Start() {
#if UNITY_EDITOR
        lastPos = Input.mousePosition;
#else
        if (Input.touchCount > 0)
            {

                GetComponent<ParticleSystem>().Play();
                var touch = Input.GetTouch(0);
                lastPos = touch.position;
            }
#endif
    }
    void OnCollisionEnter(Collision col)
    {

    }
    void Update() {
        var nextDot = path.dotsOnPole[path.dotsOnPole.Count - 1];
        upLimit = nextDot.transform.position.y;
        downLimit = nextDot.transform.position.y;
        leftLimit = nextDot.transform.position.x;
        rightLimit = nextDot.transform.position.x;
        while (nextDot.GetComponent<PoleDot>().AllowedToUp())
        {
            nextDot = nextDot.GetComponent<PoleDot>().up.GetComponent<PoleLine>().up;
            upLimit = nextDot.transform.position.y;
        }
        nextDot = path.dotsOnPole[path.dotsOnPole.Count - 1];
        while (nextDot.GetComponent<PoleDot>().AllowedToDown())
        {
            nextDot = nextDot.GetComponent<PoleDot>().down.GetComponent<PoleLine>().down;
            downLimit = nextDot.transform.position.y;
        }
        nextDot = path.dotsOnPole[path.dotsOnPole.Count - 1];
        while (nextDot.GetComponent<PoleDot>().AllowedToRight())
        {
            nextDot = nextDot.GetComponent<PoleDot>().right.GetComponent<PoleLine>().right;
            rightLimit = nextDot.transform.position.x;
        }
        nextDot = path.dotsOnPole[path.dotsOnPole.Count - 1];
        while (nextDot.GetComponent<PoleDot>().AllowedToLeft())
        {
            nextDot = nextDot.GetComponent<PoleDot>().left.GetComponent<PoleLine>().left;
            leftLimit = nextDot.transform.position.x;
        }
        if (path.clone != null && (path.clone.GetComponent<ActivePath>().isMirroredHor || path.clone.GetComponent<ActivePath>().isMirroredVert || path.clone.GetComponent<ActivePath>().isSymmetric))
        {
            GameObject dot = path.clone.GetComponent<ActivePath>().pointer;
            if (dot.transform.position.y == transform.position.y && dot.transform.position.x > transform.position.x && dot.transform.position.x <= rightLimit)
            {
                rightLimit = (dot.transform.position.x + this.transform.position.x) / 2 - 0.6f;
            }
            if (dot.transform.position.y == transform.position.y && dot.transform.position.x < transform.position.x && dot.transform.position.x >= leftLimit)
            {
                leftLimit = (dot.transform.position.x + this.transform.position.x) / 2 + 0.6f;
            }
            if (dot.transform.position.x == transform.position.x && dot.transform.position.y > transform.position.y && dot.transform.position.y <= upLimit)
            {
                upLimit = (dot.transform.position.y + this.transform.position.y) / 2 - 0.6f;
            }
            if (dot.transform.position.x == transform.position.x && dot.transform.position.y < transform.position.y && dot.transform.position.y >= downLimit)
            {
                downLimit = (dot.transform.position.y + this.transform.position.y) / 2 + 0.6f;
            }
        }
        foreach (GameObject dot in allPathDots)
        {
            if (dot != pathDots[pathDots.Count - 1] && dot.transform.position.y == transform.position.y && dot.transform.position.x > transform.position.x && dot.transform.position.x <= rightLimit)
            {
                rightLimit = dot.transform.position.x - 1f;
                if (path.clone != null && (path.clone.GetComponent<ActivePath>().isMirroredHor || path.clone.GetComponent<ActivePath>().isMirroredVert || path.clone.GetComponent<ActivePath>().isSymmetric))
                {
                    var list = path.clone.GetComponent<ActivePath>().pointer.GetComponent<Follow>().pathDots;
                    if (dot == list[list.Count - 1])
                    {
                        rightLimit -= 2f;
                    }
                }
            }
            if (dot != pathDots[pathDots.Count - 1] && dot.transform.position.y == transform.position.y && dot.transform.position.x < transform.position.x && dot.transform.position.x >= leftLimit)
            {
                leftLimit = dot.transform.position.x + 1f;
                if (path.clone != null && (path.clone.GetComponent<ActivePath>().isMirroredHor || path.clone.GetComponent<ActivePath>().isMirroredVert || path.clone.GetComponent<ActivePath>().isSymmetric))
                {
                    var list = path.clone.GetComponent<ActivePath>().pointer.GetComponent<Follow>().pathDots;
                    if (dot == list[list.Count - 1])
                    {
                        leftLimit += 2f;
                    }
                }
            }
            if (dot != pathDots[pathDots.Count - 1] && dot.transform.position.x == transform.position.x && dot.transform.position.y > transform.position.y && dot.transform.position.y <= upLimit)
            {
                upLimit = dot.transform.position.y - 1f;
                if (path.clone != null && (path.clone.GetComponent<ActivePath>().isMirroredHor || path.clone.GetComponent<ActivePath>().isMirroredVert || path.clone.GetComponent<ActivePath>().isSymmetric))
                {
                    var list = path.clone.GetComponent<ActivePath>().pointer.GetComponent<Follow>().pathDots;
                    if (dot == list[list.Count - 1])
                    {
                        upLimit -= 2f;
                    }
                }
            }
            if (dot != pathDots[pathDots.Count - 1] && dot.transform.position.x == transform.position.x && dot.transform.position.y < transform.position.y && dot.transform.position.y >= downLimit)
            {
                downLimit = dot.transform.position.y + 1f;
                if (path.clone != null && (path.clone.GetComponent<ActivePath>().isMirroredHor || path.clone.GetComponent<ActivePath>().isMirroredVert || path.clone.GetComponent<ActivePath>().isSymmetric))
                {
                    var list = path.clone.GetComponent<ActivePath>().pointer.GetComponent<Follow>().pathDots;
                    if (dot == list[list.Count - 1])
                    {
                        downLimit += 2f;
                    }
                }
            }
            
        }
        

        Vector2 dir = new Vector2(0f, 0f);

#if UNITY_EDITOR
        if (Input.GetMouseButton(0))
        {

            GetComponent<ParticleSystem>().Play();
            dir = 0.025f * MenuManager.MainSettings.speed * new Vector2(Input.mousePosition.x - lastPos.x, Input.mousePosition.y - lastPos.y);
            lastPos = Input.mousePosition;
        }
        else
        {
            GetComponent<ParticleSystem>().Stop();
            lastPos = Input.mousePosition;
        }
#else
        if (Input.touchCount > 0)
        {

            GetComponent<ParticleSystem>().Play();
            var touch = Input.GetTouch(0);
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    lastPos = touch.position;
                    break;
                case TouchPhase.Moved:
                    dir = 0.02f * MenuManager.MainSettings.speed * (touch.position - lastPos);
                    lastPos = touch.position;
                    break;
            }   
        }
        else
        {
            GetComponent<ParticleSystem>().Stop();
        }
#endif

        float camerarot = GameObject.FindGameObjectWithTag("MainCamera").transform.rotation.eulerAngles.z;
        Vector2 tmp = dir;
        dir.x = (tmp.x * Mathf.Cos(camerarot / 180f * Mathf.PI) - tmp.y * Mathf.Sin(camerarot / 180f * Mathf.PI));
        dir.y = (tmp.x * Mathf.Sin(camerarot / 180f * Mathf.PI) + tmp.y * Mathf.Cos(camerarot / 180f * Mathf.PI));
        foreach (GameObject go in GameObject.FindGameObjectsWithTag("PoleDot"))
        {
            if (Mathf.Sqrt(Mathf.Abs(go.transform.position.x - transform.position.x) * Mathf.Abs(go.transform.position.x - transform.position.x) + Mathf.Abs(go.transform.position.y - transform.position.y) * Mathf.Abs(go.transform.position.y - transform.position.y)) <
                Mathf.Sqrt(Mathf.Abs(maxDistance.x) * Mathf.Abs(maxDistance.x) + Mathf.Abs(maxDistance.y) * Mathf.Abs(maxDistance.y)))
            {
                maxDistance = go.transform.position - transform.position;
                nearestDot = go;
            }
        }
        maxDistance = new Vector3(10f, 10f, 10f);
        foreach (GameObject go in GameObject.FindGameObjectsWithTag("PolePart"))
        {
            if (Mathf.Sqrt(Mathf.Abs(go.transform.position.x - transform.position.x) * Mathf.Abs(go.transform.position.x - transform.position.x) + Mathf.Abs(go.transform.position.y - transform.position.y) * Mathf.Abs(go.transform.position.y - transform.position.y)) <
                Mathf.Sqrt(Mathf.Abs(maxDistance.x) * Mathf.Abs(maxDistance.x) + Mathf.Abs(maxDistance.y) * Mathf.Abs(maxDistance.y)))
            {
                maxDistance = go.transform.position - transform.position;
                currentLine = go;
            }
        }
        maxDistance = new Vector3(10f, 10f, 10f);
        if (path.isSymmetric || path.isMirroredVert || path.isMirroredHor)
        {
            Vector3 orig = GameObject.FindGameObjectWithTag("Core").GetComponent<Core>().activePath.GetComponent<ActivePath>().pointer.transform.position;
            if (path.isMirroredHor)
            {
                transform.position = new Vector3(orig.x, - (path.pole.GetComponent<Pole>().height - 1) * 5f - orig.y, orig.z);
            }
            else if (path.isMirroredVert)
            {
                transform.position = new Vector3((path.pole.GetComponent<Pole>().width - 1) * 5f - orig.x, orig.y, orig.z);
            }
            else if (path.isSymmetric)
            {
                transform.position = new Vector3((path.pole.GetComponent<Pole>().width - 1) * 5f - orig.x, - (path.pole.GetComponent<Pole>().height - 1) * 5f - orig.y, orig.z);
            }
        }
        else
        {
            if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
            {
                if (moveHor)
                {
                    transform.Translate(new Vector3(dir.x /*+ Mathf.Sign(dir.x) * Mathf.Abs(dir.y)*/, 0f, 0f));
                }
                else
                {
                    if (Mathf.Abs(transform.position.y - nearestDot.transform.position.y) < eps && (nearestDot.GetComponent<PoleDot>().AllowedToRight() || nearestDot.GetComponent<PoleDot>().AllowedToLeft()))
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
                    else transform.Translate(new Vector3(0f, dir.y + Mathf.Sign(dir.y) * Mathf.Abs(dir.x) * 0.5f, 0f));
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
                    if (Mathf.Abs(transform.position.x - nearestDot.transform.position.x) < eps && (nearestDot.GetComponent<PoleDot>().AllowedToUp() || nearestDot.GetComponent<PoleDot>().AllowedToDown()))
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
                    else transform.Translate(new Vector3(dir.x + Mathf.Sign(dir.x) * Mathf.Abs(dir.y) * 0.5f, 0f, 0f));
                }
            }

            transform.position = new Vector3(Mathf.Min(rightLimit, Mathf.Max(leftLimit, transform.position.x)), Mathf.Min(upLimit, Mathf.Max(downLimit, transform.position.y)), 0f) + stepz;
        }
        
    }

}
