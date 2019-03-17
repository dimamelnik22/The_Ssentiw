using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivePath : MonoBehaviour {
    public GameObject pointerPF;
    

    public List<GameObject> dots = new List<GameObject>();
    public List<GameObject> lines = new List<GameObject>();
    public List<GameObject> dotsOnPole = new List<GameObject>();
    public List<GameObject> linesOnPole = new List<GameObject>();
    public List<GameObject> finishes = new List<GameObject>();
    public GameObject start;
    public GameObject pointer;
    public GameObject currentLine;
    public GameObject currentFinish;
    public GameObject currentFinishOnPole;
    public bool isStarted = false;
    public bool isFinished = false;
    private float eps = 1f;

    public GameObject PathDotPrefab;
    public GameObject PathLinePrefab;
    public GameObject PathStartPrefab;
    public GameObject PathFinishPrefab;
    private Vector3 stepz = new Vector3(0f, 0f, -0.5f);
    public GameObject pole;


    public void OnDestroy()
    {
        //foreach (GameObject dot in dots) Destroy(dot);
        //foreach (GameObject line in lines) Destroy(line);
        //Destroy(pointer);
        Debug.Log("Destroy ActivrPath");
        for (int i = transform.childCount - 1; i >=0; --i)
        {
            Destroy(transform.GetChild(i));
        }
    }

    public void Init(GameObject _pole, GameObject _start,List<GameObject> _finishes)
    {
        pole = _pole;
        finishes = _finishes;
        start = _start;
        dotsOnPole.Add(start);
        
    }

    public void Restart(GameObject start, List<GameObject> _finishes)
    {

        isStarted = !isStarted;
        foreach(GameObject obj in GameObject.FindGameObjectsWithTag("Path")) Destroy(obj);
        Destroy(pointer);
        dots.Clear();
        lines.Clear();

        dotsOnPole.Clear();
        linesOnPole.Clear();
        Init(pole,start,_finishes);
    }

    private void lineScaleDestroy()
    {
        Destroy(currentLine);
        lines.RemoveAt(lines.Count - 1);
        Destroy(dots[dots.Count - 1]);
        dots.RemoveAt(dots.Count - 1);
        linesOnPole.RemoveAt(linesOnPole.Count - 1);
        dotsOnPole.RemoveAt(dotsOnPole.Count - 1);
        currentLine = lines[lines.Count - 1];

        Vector3 pos = dots[dots.Count - 1].transform.position + 0.5f * (pointer.transform.position - dots[dots.Count - 1].transform.position);
        currentLine.transform.position = new Vector3(pos.x, pos.y, 0f) + stepz;
        pos -= dots[dots.Count - 1].transform.position;
        if (Mathf.Abs(pos.x) > 0f)
        {
            currentLine.transform.localScale = new Vector3(Mathf.Abs(pos.x * 2f), 1f, 0.1f);
        }
        else if (Mathf.Abs(pos.y) > 0f)
        {
            currentLine.transform.localScale = new Vector3(1f, Mathf.Abs(pos.y * 2f), 0.1f);
        }
        else currentLine.transform.localScale = new Vector3(0f, 0f, 0.1f);
    }

    public void EndSolution()
    {
        if (isFinished)
        {
            if (dotsOnPole[dotsOnPole.Count-1] != currentFinishOnPole)
            {
                GameObject dot = dotsOnPole[dotsOnPole.Count - 1];
                dotsOnPole.Add(currentFinishOnPole);
                if (dot.GetComponent<PoleDot>().posX < currentFinishOnPole.GetComponent<PoleDot>().posX) linesOnPole.Add(dot.GetComponent<PoleDot>().right);
                else if (dot.GetComponent<PoleDot>().posX > currentFinishOnPole.GetComponent<PoleDot>().posX) linesOnPole.Add(dot.GetComponent<PoleDot>().left);
                else if (dot.GetComponent<PoleDot>().posX < currentFinishOnPole.GetComponent<PoleDot>().posX) linesOnPole.Add(dot.GetComponent<PoleDot>().down);
                else if (dot.GetComponent<PoleDot>().posX > currentFinishOnPole.GetComponent<PoleDot>().posX) linesOnPole.Add(dot.GetComponent<PoleDot>().up);
            }
        }
    }

    private bool CheckBorder()
    {
        GameObject lastDot = dotsOnPole[dotsOnPole.Count - 1];
        if (transform.position.x > lastDot.transform.position.x && !lastDot.GetComponent<PoleDot>().AllowedToRight()
            || transform.position.x < lastDot.transform.position.x && !lastDot.GetComponent<PoleDot>().AllowedToLeft()
            || transform.position.y > lastDot.transform.position.x && !lastDot.GetComponent<PoleDot>().AllowedToUp()
            || transform.position.y < lastDot.transform.position.x && !lastDot.GetComponent<PoleDot>().AllowedToDown())
            return false;
        else return true;

    }

    void Update()
    {
        if (!Core.PolePreferences.isFrozen)
        {
            
            if (isStarted)
            {
                if (!isFinished)
                {
                    foreach (GameObject finish in finishes)
                    {
                        if (Mathf.Abs(pointer.transform.position.x - finish.transform.position.x) + Mathf.Abs(pointer.transform.position.y - finish.transform.position.y) < eps)
                        {
                            currentFinishOnPole = finish;
                            currentFinish = Instantiate(PathFinishPrefab, finish.transform.position + stepz, PathFinishPrefab.transform.rotation);
                            currentFinish.transform.parent = this.transform;
                            isFinished = true;
                        }
                    }
                }
                else
                {
                    if (Mathf.Abs(pointer.transform.position.x - currentFinishOnPole.transform.position.x) + Mathf.Abs(pointer.transform.position.y - currentFinishOnPole.transform.position.y) > eps)
                    {
                        isFinished = false;
                        Destroy(currentFinish);
                    }
                }
                Vector3 pos = dots[dots.Count - 1].transform.position + 0.5f * (pointer.transform.position - dots[dots.Count - 1].transform.position);
                currentLine.transform.position = new Vector3(pos.x, pos.y, 0f) + stepz;
                pos -= dots[dots.Count - 1].transform.position;
                if (Mathf.Abs(pos.x) > 0f)
                {
                    currentLine.transform.localScale = new Vector3(Mathf.Abs(pos.x * 2f), 1f, 0.1f);
                }
                else if (Mathf.Abs(pos.y) > 0f)
                {
                    currentLine.transform.localScale = new Vector3(1f, Mathf.Abs(pos.y * 2f), 0.1f);
                }
                else currentLine.transform.localScale = new Vector3(0f, 0f, 0.1f);


                //creating new dot and line
                float dist = pointer.transform.position.x - dots[dots.Count - 1].transform.position.x;
                while (Mathf.Abs(dist) >= 5f && CheckBorder())
                {
                    if (dist > 0)
                    {
                        if (lines.Count > 1 && lines[lines.Count - 2].transform.position.x > dots[dots.Count - 1].transform.position.x)
                        {
                            lineScaleDestroy();
                        }
                        else
                        {
                            currentLine.transform.localScale = new Vector3(5f, 1f, 0.1f);
                            dotsOnPole.Add(dotsOnPole[dotsOnPole.Count - 1].GetComponent<PoleDot>().right.GetComponent<PoleLine>().right);
                            dots.Add(Instantiate(PathDotPrefab, dotsOnPole[dotsOnPole.Count - 1].transform.position + stepz, PathDotPrefab.transform.rotation));
                            
                            linesOnPole.Add(dotsOnPole[dotsOnPole.Count - 2].GetComponent<PoleDot>().right);
                            currentLine.transform.position = linesOnPole[linesOnPole.Count - 1].transform.position + stepz;
                            currentLine = Instantiate(PathLinePrefab, dots[dots.Count - 1].transform.position, PathLinePrefab.transform.rotation);
                            dots[dots.Count - 1].transform.parent = this.transform;
                            currentLine.transform.parent = this.transform;
                            lines.Add(currentLine);

                        }
                    }
                    else
                    {
                        if (lines.Count > 1 && lines[lines.Count - 2].transform.position.x < dots[dots.Count - 1].transform.position.x)
                        {
                            lineScaleDestroy();
                        }
                        else
                        {
                            currentLine.transform.localScale = new Vector3(5f, 1f, 0.1f);
                            dotsOnPole.Add(dotsOnPole[dotsOnPole.Count - 1].GetComponent<PoleDot>().left.GetComponent<PoleLine>().left);
                            dots.Add(Instantiate(PathDotPrefab, dotsOnPole[dotsOnPole.Count - 1].transform.position + stepz, PathDotPrefab.transform.rotation));
                            linesOnPole.Add(dotsOnPole[dotsOnPole.Count - 2].GetComponent<PoleDot>().left);
                            currentLine.transform.position = linesOnPole[linesOnPole.Count - 1].transform.position + stepz;
                            currentLine = Instantiate(PathLinePrefab, dots[dots.Count - 1].transform.position, PathLinePrefab.transform.rotation);
                            lines.Add(currentLine);
                            dots[dots.Count - 1].transform.parent = this.transform;
                            currentLine.transform.parent = this.transform;
                        }
                    }
                    dist = pointer.transform.position.x - dots[dots.Count - 1].transform.position.x;
                }
                dist = pointer.transform.position.y - dots[dots.Count - 1].transform.position.y;
                while (Mathf.Abs(dist) >= 5f && CheckBorder())
                {
                    if (dist > 0)
                    {
                        if (lines.Count > 1 && lines[lines.Count - 2].transform.position.y > dots[dots.Count - 1].transform.position.y)
                        {
                            lineScaleDestroy();
                        }
                        else
                        {
                            currentLine.transform.localScale = new Vector3(1f, 5f, 0.1f);
                            dotsOnPole.Add(dotsOnPole[dotsOnPole.Count - 1].GetComponent<PoleDot>().up.GetComponent<PoleLine>().up);
                            dots.Add(Instantiate(PathDotPrefab, dotsOnPole[dotsOnPole.Count - 1].transform.position + stepz, PathDotPrefab.transform.rotation));
                            linesOnPole.Add(dotsOnPole[dotsOnPole.Count - 2].GetComponent<PoleDot>().up);
                            currentLine.transform.position = linesOnPole[linesOnPole.Count - 1].transform.position + stepz;
                            currentLine = Instantiate(PathLinePrefab, dots[dots.Count - 1].transform.position, PathLinePrefab.transform.rotation);
                            lines.Add(currentLine);
                            dots[dots.Count - 1].transform.parent = this.transform;
                            currentLine.transform.parent = this.transform;
                        }
                    }
                    else
                    {
                        if (lines.Count > 1 && lines[lines.Count - 2].transform.position.y < dots[dots.Count - 1].transform.position.y)
                        {
                            lineScaleDestroy();
                        }
                        else
                        {
                            currentLine.transform.localScale = new Vector3(1f, 5f, 0.1f);
                            dotsOnPole.Add(dotsOnPole[dotsOnPole.Count - 1].GetComponent<PoleDot>().down.GetComponent<PoleLine>().down);
                            dots.Add(Instantiate(PathDotPrefab, dotsOnPole[dotsOnPole.Count - 1].transform.position + stepz, PathDotPrefab.transform.rotation));
                            linesOnPole.Add(dotsOnPole[dotsOnPole.Count - 2].GetComponent<PoleDot>().down);
                            currentLine.transform.position = linesOnPole[linesOnPole.Count - 1].transform.position + stepz;
                            currentLine = Instantiate(PathLinePrefab, dots[dots.Count - 1].transform.position, PathLinePrefab.transform.rotation);
                            lines.Add(currentLine);
                            dots[dots.Count - 1].transform.parent = this.transform;
                            currentLine.transform.parent = this.transform;
                        }
                    }
                    dist = pointer.transform.position.x - dots[dots.Count - 1].transform.position.x;
                }

                //pointer.GetComponent<follow>().nearestDot = dotsOnPole[dotsOnPole.Count - 1];

                if (Mathf.Abs(currentLine.transform.localScale.x) >= 5f || Mathf.Abs(currentLine.transform.localScale.y) >= 5f)
                {
                    dotsOnPole.Add(pointer.GetComponent<follow>().nearestDot);
                    dots.Add(Instantiate(PathDotPrefab, dotsOnPole[dotsOnPole.Count - 1].transform.position + stepz, PathDotPrefab.transform.rotation));
                    currentLine = Instantiate(PathLinePrefab, dots[dots.Count - 1].transform.position, PathLinePrefab.transform.rotation);
                    lines.Add(currentLine);
                    linesOnPole.Add(pointer.GetComponent<follow>().currentLine);
                    dots[dots.Count - 1].transform.parent = this.transform;
                    currentLine.transform.parent = this.transform;
                }

                if (lines.Count > 1 && Vector3.Distance(lines[lines.Count - 2].transform.position, pointer.transform.position) < 2.5f)
                {

                    Destroy(currentLine);
                    lines.RemoveAt(lines.Count - 1);
                    currentLine = lines[lines.Count - 1];
                    Destroy(dots[dots.Count - 1]);
                    dots.RemoveAt(dots.Count - 1);
                    dotsOnPole.RemoveAt(dotsOnPole.Count - 1);
                    linesOnPole.RemoveAt(linesOnPole.Count - 1);
                }

            }
            else if ((Input.GetMouseButton(0) || Input.touchCount > 0) && !isStarted)
            {
                dots.Add(Instantiate(PathStartPrefab, start.transform.position + stepz, PathStartPrefab.transform.rotation));
                pointer = Instantiate(pointerPF, start.transform.position + stepz, pointerPF.transform.rotation);
                pointer.transform.parent = this.transform;
                pointer.GetComponent<follow>().poleDots = pole.GetComponent<Pole>().poleDots;
                pointer.GetComponent<follow>().path = this;
                pointer.GetComponent<follow>().nearestDot = start;
                if (start.GetComponent<PoleDot>().left != null)
                    pointer.GetComponent<follow>().currentLine = start.GetComponent<PoleDot>().left;
                else if (start.GetComponent<PoleDot>().right != null)
                    pointer.GetComponent<follow>().currentLine = start.GetComponent<PoleDot>().right;
                else if (start.GetComponent<PoleDot>().up != null)
                    pointer.GetComponent<follow>().currentLine = start.GetComponent<PoleDot>().up;
                else if (start.GetComponent<PoleDot>().down != null)
                    pointer.GetComponent<follow>().currentLine = start.GetComponent<PoleDot>().down;
                pointer.GetComponent<follow>().pathDots = dots;
                Vector3 pos = dots[dots.Count - 1].transform.position + 0.5f * (pointer.transform.position - dots[dots.Count - 1].transform.position);
                currentLine = Instantiate(PathLinePrefab, pos, PathLinePrefab.transform.rotation);
                dots[dots.Count - 1].transform.parent = this.transform;
                currentLine.transform.parent = this.transform;
                lines.Add(currentLine);
                isStarted = true;
            }
        }
        //foreach(GameObject g in dotsOnPole)
        //{
        //    g.GetComponent<Renderer>().material.color = new Color(1, 0, 0);
        //}
    }

	
	
}
