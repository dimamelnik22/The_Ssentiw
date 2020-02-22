using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivePath : MonoBehaviour
{
    [Header("Clone Path")]
    public GameObject clone;
    [Header("Prefabs")]
    public GameObject PointerPF;
    public GameObject PathDotPF;
    public GameObject PathLinePF;
    public GameObject PathStartPF;
    public GameObject PathFinishPF;
    public GameObject ActivePathPF;

    [HideInInspector]
    public static List<GameObject> allPathDots = new List<GameObject>();
    [HideInInspector]
    public List<GameObject> dotsOnPole = new List<GameObject>();
    [HideInInspector]
    public List<GameObject> linesOnPole = new List<GameObject>();
    [HideInInspector]
    public bool isFinished = false;
    //[HideInInspector]
    public GameObject pointer;
    [HideInInspector]
    public List<GameObject> finishes = new List<GameObject>();
    [HideInInspector]
    public GameObject currentFinishOnPole;
    [HideInInspector]
    public GameObject currentFinish;
    [HideInInspector]
    public bool isStarted = false;
    [HideInInspector]
    public bool isMirroredVert = false;
    [HideInInspector]
    public bool isMirroredHor = false;
    [HideInInspector]
    public bool isSymmetric = false;
    [HideInInspector]
    public GameObject pole;

    private readonly List<GameObject> dots = new List<GameObject>();
    private readonly List<GameObject> lines = new List<GameObject>();
    private List<GameObject> starts;
    private GameObject leadDot;
    private GameObject currentLine;
    private readonly float eps = 1f;


    private Vector3 stepz = new Vector3(0f, 0f, -0.5f);

    public void OnDestroy()
    {
        //foreach (GameObject dot in dots) Destroy(dot);
        //foreach (GameObject line in lines) Destroy(line);
        //Destroy(pointer);
        for (int i = transform.childCount - 1; i >= 0; --i)
        {
            Destroy(transform.GetChild(i));
        }
        Destroy(leadDot);
    }

    public void Init(GameObject _pole, List<GameObject> _start, List<GameObject> _finishes)
    {
        pole = _pole;
        finishes = _finishes;
        starts = _start;

    }
    public void InitWithClone(GameObject _pole, List<GameObject> _start, List<GameObject> _finishes)
    {
        pole = _pole;
        finishes = _finishes;
        starts = _start;
        clone = Instantiate(ActivePathPF);
        clone.GetComponent<ActivePath>().Init(pole, starts, finishes);
    }

    public void Restart(List<GameObject> start, List<GameObject> _finishes)
    {
        if (!isMirroredHor && !isMirroredVert && !isSymmetric)
            pole.GetComponent<Pole>().NormalizeColors();
        isStarted = !isStarted;
        foreach (GameObject obj in dots) Destroy(obj);
        foreach (GameObject obj in lines) Destroy(obj);
        Destroy(leadDot);
        Destroy(pointer);
        dots.Clear();
        lines.Clear();
        allPathDots.Clear();

        dotsOnPole.Clear();
        linesOnPole.Clear();
        Init(pole, start, _finishes);
    }

    public void NewStart(GameObject _start)
    {
        Restart(starts, finishes);
        if (clone != null)
        {
            if (pole.GetComponent<Pole>().starts.Contains(pole.GetComponent<Pole>().poleDots[pole.GetComponent<Pole>().height - 1 - _start.GetComponent<PoleDot>().posY][pole.GetComponent<Pole>().width - 1 - _start.GetComponent<PoleDot>().posX]))
            {
                clone.GetComponent<ActivePath>().isSymmetric = true;
                clone.GetComponent<ActivePath>().NewStart(pole.GetComponent<Pole>().poleDots[pole.GetComponent<Pole>().height - 1 - _start.GetComponent<PoleDot>().posY][pole.GetComponent<Pole>().width - 1 - _start.GetComponent<PoleDot>().posX]);
            }
            else if (pole.GetComponent<Pole>().starts.Contains(pole.GetComponent<Pole>().poleDots[_start.GetComponent<PoleDot>().posY][pole.GetComponent<Pole>().width - 1 - _start.GetComponent<PoleDot>().posX]))
            {
                clone.GetComponent<ActivePath>().isMirroredVert = true;
                clone.GetComponent<ActivePath>().NewStart(pole.GetComponent<Pole>().poleDots[_start.GetComponent<PoleDot>().posY][pole.GetComponent<Pole>().width - 1 - _start.GetComponent<PoleDot>().posX]);
            }
            else if (pole.GetComponent<Pole>().starts.Contains(pole.GetComponent<Pole>().poleDots[pole.GetComponent<Pole>().height - 1 - _start.GetComponent<PoleDot>().posY][_start.GetComponent<PoleDot>().posX]))
            {
                clone.GetComponent<ActivePath>().isMirroredHor = true;
                clone.GetComponent<ActivePath>().NewStart(pole.GetComponent<Pole>().poleDots[pole.GetComponent<Pole>().height - 1 - _start.GetComponent<PoleDot>().posY][_start.GetComponent<PoleDot>().posX]);
            }
        }
        dotsOnPole.Add(_start);
        var dot = Instantiate(PathStartPF, _start.transform.position + stepz, PathStartPF.transform.rotation);
        dots.Add(dot);
        allPathDots.Add(dot);
        leadDot = Instantiate(PathDotPF, _start.transform.position + stepz, PathDotPF.transform.rotation);
        pointer = Instantiate(PointerPF, _start.transform.position + stepz, PointerPF.transform.rotation);
        pointer.transform.parent = this.transform;
        //if (Input.touchCount > 0)
        //    pointer.GetComponent<follow>().lastpos = Input.GetTouch(0).position;
        pointer.GetComponent<Follow>().poleDots = pole.GetComponent<Pole>().poleDots;
        pointer.GetComponent<Follow>().path = this;
        pointer.GetComponent<Follow>().nearestDot = _start;
        if (_start.GetComponent<PoleDot>().left != null)
            pointer.GetComponent<Follow>().currentLine = _start.GetComponent<PoleDot>().left;
        else if (_start.GetComponent<PoleDot>().right != null)
            pointer.GetComponent<Follow>().currentLine = _start.GetComponent<PoleDot>().right;
        else if (_start.GetComponent<PoleDot>().up != null)
            pointer.GetComponent<Follow>().currentLine = _start.GetComponent<PoleDot>().up;
        else if (_start.GetComponent<PoleDot>().down != null)
            pointer.GetComponent<Follow>().currentLine = _start.GetComponent<PoleDot>().down;
        pointer.GetComponent<Follow>().pathDots = dots;
        pointer.GetComponent<Follow>().allPathDots = allPathDots;
        Vector3 pos = dots[dots.Count - 1].transform.position + 0.5f * (pointer.transform.position - dots[dots.Count - 1].transform.position);
        currentLine = Instantiate(PathLinePF, pos, PathLinePF.transform.rotation);
        dots[dots.Count - 1].transform.parent = this.transform;
        currentLine.transform.parent = this.transform;
        lines.Add(currentLine);
        isStarted = true;
    }

    private void LineScaleDestroy()
    {
        Destroy(currentLine);
        lines.RemoveAt(lines.Count - 1);
        Destroy(dots[dots.Count - 1]);
        allPathDots.Remove(dots[dots.Count - 1]);
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
            if (dotsOnPole[dotsOnPole.Count - 1] != currentFinishOnPole)
            {
                GameObject dot = dotsOnPole[dotsOnPole.Count - 1];
                dotsOnPole.Add(currentFinishOnPole);
                
                if (dot.GetComponent<PoleDot>().posX < currentFinishOnPole.GetComponent<PoleDot>().posX)
                {
                    linesOnPole.Add(dot.GetComponent<PoleDot>().right);
                    currentLine.transform.localScale = new Vector3(5f, 1f, 0.1f);
                }
                else if (dot.GetComponent<PoleDot>().posX > currentFinishOnPole.GetComponent<PoleDot>().posX)
                {
                    linesOnPole.Add(dot.GetComponent<PoleDot>().left);
                    currentLine.transform.localScale = new Vector3(5f, 1f, 0.1f);
                }
                else if (dot.GetComponent<PoleDot>().posY < currentFinishOnPole.GetComponent<PoleDot>().posY)
                {
                    linesOnPole.Add(dot.GetComponent<PoleDot>().down);
                    currentLine.transform.localScale = new Vector3(1f, 5f, 0.1f);
                }
                else if (dot.GetComponent<PoleDot>().posY > currentFinishOnPole.GetComponent<PoleDot>().posY)
                {
                    linesOnPole.Add(dot.GetComponent<PoleDot>().up);
                    currentLine.transform.localScale = new Vector3(1f, 5f, 0.1f);
                }
                currentLine.transform.position = linesOnPole[linesOnPole.Count - 1].transform.position + stepz;
                pointer.transform.position = currentFinishOnPole.transform.position + stepz;
                var tempdot = Instantiate(PathDotPF, currentFinishOnPole.transform.position + stepz, PathDotPF.transform.rotation);
                allPathDots.Add(tempdot);
                dots.Add(tempdot);
                currentLine = Instantiate(PathLinePF, dots[dots.Count - 1].transform.position, PathLinePF.transform.rotation);
                currentLine.transform.parent = this.transform;
                lines.Add(currentLine);
            }
        }
    }


    public void SystemStep(GameObject dot)
    {
        dotsOnPole.Add(dot);
        var tempdot = Instantiate(PathDotPF, dotsOnPole[dotsOnPole.Count - 1].transform.position + stepz, PathDotPF.transform.rotation);
        dots.Add(dot);
        allPathDots.Add(tempdot);
        if (dotsOnPole[dotsOnPole.Count - 1].GetComponent<PoleDot>().right != null && dotsOnPole[dotsOnPole.Count - 1].GetComponent<PoleDot>().right.GetComponent<PoleLine>().right == dotsOnPole[dotsOnPole.Count - 2])
        {
            linesOnPole.Add(dotsOnPole[dotsOnPole.Count - 1].GetComponent<PoleDot>().right);
        }
        else if (dotsOnPole[dotsOnPole.Count - 1].GetComponent<PoleDot>().left != null && dotsOnPole[dotsOnPole.Count - 1].GetComponent<PoleDot>().left.GetComponent<PoleLine>().left == dotsOnPole[dotsOnPole.Count - 2])
        {
            linesOnPole.Add(dotsOnPole[dotsOnPole.Count - 1].GetComponent<PoleDot>().left);
        }
        else if (dotsOnPole[dotsOnPole.Count - 1].GetComponent<PoleDot>().up != null && dotsOnPole[dotsOnPole.Count - 1].GetComponent<PoleDot>().up.GetComponent<PoleLine>().up == dotsOnPole[dotsOnPole.Count - 2])
        {
            linesOnPole.Add(dotsOnPole[dotsOnPole.Count - 1].GetComponent<PoleDot>().up);
        }
        else if (dotsOnPole[dotsOnPole.Count - 1].GetComponent<PoleDot>().down != null && dotsOnPole[dotsOnPole.Count - 1].GetComponent<PoleDot>().down.GetComponent<PoleLine>().down == dotsOnPole[dotsOnPole.Count - 2])
        {
            linesOnPole.Add(dotsOnPole[dotsOnPole.Count - 1].GetComponent<PoleDot>().down);
        }

        currentLine.transform.localScale = new Vector3(linesOnPole[linesOnPole.Count - 1].GetComponent<PoleLine>().Line.transform.localScale.x, linesOnPole[linesOnPole.Count - 1].GetComponent<PoleLine>().Line.transform.localScale.y, 0.1f);
        currentLine.transform.position = linesOnPole[linesOnPole.Count - 1].transform.position + stepz;
        leadDot.transform.position = dots[dots.Count - 1].transform.position;
        currentLine = Instantiate(PathLinePF, dots[dots.Count - 1].transform.position, PathLinePF.transform.rotation);
        dots[dots.Count - 1].transform.parent = this.transform;
        currentLine.transform.parent = this.transform;
        lines.Add(currentLine);
        pointer.transform.position = leadDot.transform.position;

    }
    public void Update()
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
                        currentFinish = Instantiate(PathFinishPF, finish.transform.position + stepz, PathFinishPF.transform.rotation);
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
                    //if (lines.Count < dots.Count) dotsOnPole.Remove(currentFinishOnPole);
                    Destroy(currentFinish);
                }
            }
            Vector3 pos = dots[dots.Count - 1].transform.position;
            pos+= 0.5f * (pointer.transform.position - dots[dots.Count - 1].transform.position);
            currentLine.transform.position = new Vector3(pos.x, pos.y, 0f) + stepz;
            pos -= dots[dots.Count - 1].transform.position;
            if (Mathf.Abs(pos.x) > 0f)
            {
                currentLine.transform.localScale = new Vector3(Mathf.Abs(pos.x * 2f), 1f, 0.1f);
                leadDot.transform.position = currentLine.transform.position + new Vector3(pos.x, 0f, 0f);
            }
            else if (Mathf.Abs(pos.y) > 0f)
            {
                currentLine.transform.localScale = new Vector3(1f, Mathf.Abs(pos.y * 2f), 0.1f);
                leadDot.transform.position = currentLine.transform.position + new Vector3(0f, pos.y, 0f);
            }
            else
            {
                currentLine.transform.localScale = new Vector3(0f, 0f, 0.1f);
                leadDot.transform.position = currentLine.transform.position;
            }


            //creating new dot and line
            float dist = pointer.transform.position.x - dots[dots.Count - 1].transform.position.x;
            while (Mathf.Abs(dist) > 5f /*&& CheckBorder()*/)
            {
                //Debug.Log("auto new by x "+ lines.Count);
                if (dist > 0)
                {
                    if (lines.Count > 1 && lines[lines.Count - 2].transform.position.x > dots[dots.Count - 1].transform.position.x)
                    {
                        LineScaleDestroy();
                    }
                    else
                    {
                        currentLine.transform.localScale = new Vector3(5f, 1f, 0.1f);
                        dotsOnPole.Add(dotsOnPole[dotsOnPole.Count - 1].GetComponent<PoleDot>().right.GetComponent<PoleLine>().right);
                        var dot = Instantiate(PathDotPF, dotsOnPole[dotsOnPole.Count - 1].transform.position + stepz, PathDotPF.transform.rotation);
                        dots.Add(dot);
                        allPathDots.Add(dot);

                        linesOnPole.Add(dotsOnPole[dotsOnPole.Count - 2].GetComponent<PoleDot>().right);
                        currentLine.transform.position = linesOnPole[linesOnPole.Count - 1].transform.position + stepz;
                        leadDot.transform.position = dots[dots.Count - 1].transform.position;
                        currentLine = Instantiate(PathLinePF, dots[dots.Count - 1].transform.position, PathLinePF.transform.rotation);
                        dots[dots.Count - 1].transform.parent = this.transform;
                        currentLine.transform.parent = this.transform;
                        lines.Add(currentLine);

                    }
                }
                else
                {
                    if (lines.Count > 1 && lines[lines.Count - 2].transform.position.x < dots[dots.Count - 1].transform.position.x)
                    {
                        LineScaleDestroy();
                    }
                    else
                    {
                        currentLine.transform.localScale = new Vector3(5f, 1f, 0.1f);
                        dotsOnPole.Add(dotsOnPole[dotsOnPole.Count - 1].GetComponent<PoleDot>().left.GetComponent<PoleLine>().left);
                        var dot = Instantiate(PathDotPF, dotsOnPole[dotsOnPole.Count - 1].transform.position + stepz, PathDotPF.transform.rotation);
                        dots.Add(dot);
                        allPathDots.Add(dot);
                        linesOnPole.Add(dotsOnPole[dotsOnPole.Count - 2].GetComponent<PoleDot>().left);
                        currentLine.transform.position = linesOnPole[linesOnPole.Count - 1].transform.position + stepz;
                        leadDot.transform.position = dots[dots.Count - 1].transform.position;
                        currentLine = Instantiate(PathLinePF, dots[dots.Count - 1].transform.position, PathLinePF.transform.rotation);
                        lines.Add(currentLine);
                        dots[dots.Count - 1].transform.parent = this.transform;
                        currentLine.transform.parent = this.transform;
                    }
                }
                dist = pointer.transform.position.x - dots[dots.Count - 1].transform.position.x;
            }
            dist = pointer.transform.position.y - dots[dots.Count - 1].transform.position.y;
            while (Mathf.Abs(dist) > 5f /*&& CheckBorder()*/)
            {
                //Debug.Log("auto new  by y "+ lines.Count);
                if (dist > 0)
                {
                    if (lines.Count > 1 && lines[lines.Count - 2].transform.position.y > dots[dots.Count - 1].transform.position.y)
                    {
                        LineScaleDestroy();
                    }
                    else
                    {
                        currentLine.transform.localScale = new Vector3(1f, 5f, 0.1f);
                        dotsOnPole.Add(dotsOnPole[dotsOnPole.Count - 1].GetComponent<PoleDot>().up.GetComponent<PoleLine>().up);
                        var dot = Instantiate(PathDotPF, dotsOnPole[dotsOnPole.Count - 1].transform.position + stepz, PathDotPF.transform.rotation);
                        dots.Add(dot);
                        allPathDots.Add(dot);
                        linesOnPole.Add(dotsOnPole[dotsOnPole.Count - 2].GetComponent<PoleDot>().up);
                        currentLine.transform.position = linesOnPole[linesOnPole.Count - 1].transform.position + stepz;
                        leadDot.transform.position = dots[dots.Count - 1].transform.position;
                        currentLine = Instantiate(PathLinePF, dots[dots.Count - 1].transform.position, PathLinePF.transform.rotation);
                        lines.Add(currentLine);
                        dots[dots.Count - 1].transform.parent = this.transform;
                        currentLine.transform.parent = this.transform;
                    }
                }
                else
                {
                    if (lines.Count > 1 && lines[lines.Count - 2].transform.position.y < dots[dots.Count - 1].transform.position.y)
                    {
                        LineScaleDestroy();
                    }
                    else
                    {
                        currentLine.transform.localScale = new Vector3(1f, 5f, 0.1f);
                        dotsOnPole.Add(dotsOnPole[dotsOnPole.Count - 1].GetComponent<PoleDot>().down.GetComponent<PoleLine>().down);
                        var dot = Instantiate(PathDotPF, dotsOnPole[dotsOnPole.Count - 1].transform.position + stepz, PathDotPF.transform.rotation);
                        dots.Add(dot);
                        allPathDots.Add(dot);
                        linesOnPole.Add(dotsOnPole[dotsOnPole.Count - 2].GetComponent<PoleDot>().down);
                        currentLine.transform.position = linesOnPole[linesOnPole.Count - 1].transform.position + stepz;
                        leadDot.transform.position = dots[dots.Count - 1].transform.position;
                        currentLine = Instantiate(PathLinePF, dots[dots.Count - 1].transform.position, PathLinePF.transform.rotation);
                        lines.Add(currentLine);
                        dots[dots.Count - 1].transform.parent = this.transform;
                        currentLine.transform.parent = this.transform;
                    }
                }
                dist = pointer.transform.position.x - dots[dots.Count - 1].transform.position.x;
            }

            //pointer.GetComponent<follow>().nearestDot = dotsOnPole[dotsOnPole.Count - 1];

            if ((Mathf.Abs(currentLine.transform.localScale.x) >= 5f || Mathf.Abs(currentLine.transform.localScale.y) >= 5f) && dotsOnPole[dotsOnPole.Count-1] !=pointer.GetComponent<Follow>().nearestDot)
            {
                //Debug.Log("new by scale" + lines.Count);
                dotsOnPole.Add(pointer.GetComponent<Follow>().nearestDot);

                if (Mathf.Abs(currentLine.transform.localScale.x) >= 5f) currentLine.transform.localScale = new Vector3(5f, 1f, 0.1f);
                else currentLine.transform.localScale = new Vector3(1f, 5f, 0.1f);
                //currentLine.transform.position = dotsOnPole[dotsOnPole.Count - 2].transform.position + 0.5f * (dotsOnPole[dotsOnPole.Count - 1].transform.position - dotsOnPole[dotsOnPole.Count - 2].transform.position) + stepz;
                
                var dot = Instantiate(PathDotPF, dotsOnPole[dotsOnPole.Count - 1].transform.position + stepz, PathDotPF.transform.rotation);
                dots.Add(dot);
                allPathDots.Add(dot);
                currentLine = Instantiate(PathLinePF, dots[dots.Count - 1].transform.position, PathLinePF.transform.rotation);
                leadDot.transform.position = dots[dots.Count - 1].transform.position;
                lines.Add(currentLine);
                // TUT DOLZHEN BYT' BAGOSIK   
                //linesOnPole.Add(pointer.GetComponent<follow>().currentLine);
                // TUT DOLZHEN BYT' BAGOSIK                    

                if (dotsOnPole[dotsOnPole.Count - 1].GetComponent<PoleDot>().right != null && dotsOnPole[dotsOnPole.Count - 1].GetComponent<PoleDot>().right.GetComponent<PoleLine>().right == dotsOnPole[dotsOnPole.Count - 2])
                {
                    linesOnPole.Add(dotsOnPole[dotsOnPole.Count - 1].GetComponent<PoleDot>().right);
                }
                else if (dotsOnPole[dotsOnPole.Count - 1].GetComponent<PoleDot>().left != null && dotsOnPole[dotsOnPole.Count - 1].GetComponent<PoleDot>().left.GetComponent<PoleLine>().left == dotsOnPole[dotsOnPole.Count - 2])
                {
                    linesOnPole.Add(dotsOnPole[dotsOnPole.Count - 1].GetComponent<PoleDot>().left);
                }
                else if (dotsOnPole[dotsOnPole.Count - 1].GetComponent<PoleDot>().up != null && dotsOnPole[dotsOnPole.Count - 1].GetComponent<PoleDot>().up.GetComponent<PoleLine>().up == dotsOnPole[dotsOnPole.Count - 2])
                {
                    linesOnPole.Add(dotsOnPole[dotsOnPole.Count - 1].GetComponent<PoleDot>().up);
                }
                else if (dotsOnPole[dotsOnPole.Count - 1].GetComponent<PoleDot>().down != null && dotsOnPole[dotsOnPole.Count - 1].GetComponent<PoleDot>().down.GetComponent<PoleLine>().down == dotsOnPole[dotsOnPole.Count - 2])
                {
                    linesOnPole.Add(dotsOnPole[dotsOnPole.Count - 1].GetComponent<PoleDot>().down);
                }
                lines[lines.Count - 2].transform.position = linesOnPole[linesOnPole.Count - 1].transform.position + stepz;
                dots[dots.Count - 1].transform.parent = this.transform;
                currentLine.transform.parent = this.transform;
            }

            if (lines.Count > 1 && Vector3.Distance(lines[lines.Count - 2].transform.position, pointer.transform.position) < 2.5f)
            {
                //Debug.Log("fuck go back");
                Destroy(currentLine);
                lines.RemoveAt(lines.Count - 1);
                currentLine = lines[lines.Count - 1];
                allPathDots.Remove(dots[dots.Count - 1]);
                Destroy(dots[dots.Count - 1]);
                dots.RemoveAt(dots.Count - 1);
                dotsOnPole.RemoveAt(dotsOnPole.Count - 1);
                linesOnPole.RemoveAt(linesOnPole.Count - 1);
            }

        }
            
        //foreach(GameObject g in dotsOnPole)
        //{
        //    g.GetComponent<Renderer>().material.color = new Color(1, 0, 0);
        //}
    }



}
