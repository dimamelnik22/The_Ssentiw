using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivePath : MonoBehaviour {
    public GameObject pointerPF;

    public List<GameObject> dots = new List<GameObject>();
    public List<GameObject> lines = new List<GameObject>();
    public List<GameObject> dotsOnPole = new List<GameObject>();
    public List<GameObject> linesOnPole = new List<GameObject>();
    public GameObject pointer;
    public GameObject currentLine;
    public bool isScaling = false;

    public GameObject PathDotPrefab;
    public GameObject PathLinePrefab;


    public void Init(GameObject start)
    {
        dotsOnPole.Add(start);
        dots.Add(Instantiate(PathDotPrefab,start.transform.position + new Vector3(0f,1f,0f),PathDotPrefab.transform.rotation));
        pointer = Instantiate(pointerPF, start.transform.position + new Vector3(0f, 2f, 0f), pointerPF.transform.rotation);
        pointer.GetComponent<follow>().nearestDot = start;
        pointer.GetComponent<follow>().currentLine = start.GetComponent<PoleDot>().left;
        pointer.GetComponent<follow>().pathDots = dots;
    }

    public void Restart(GameObject start)
    {
        isScaling = !isScaling;
        foreach(GameObject obj in GameObject.FindGameObjectsWithTag("Path")) Destroy(obj);
        Destroy(pointer);
        dots.Clear();
        lines.Clear();

        dotsOnPole.Clear();
        linesOnPole.Clear();
        Init(start);
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
        currentLine.transform.position = new Vector3(pos.x, 1f, pos.z);
        pos -= dots[dots.Count - 1].transform.position;
        if (Mathf.Abs(pos.x) > 0f)
        {
            currentLine.transform.localScale = new Vector3(Mathf.Abs(pos.x * 2f), 0.1f, 1f);
        }
        else if (Mathf.Abs(pos.z) > 0f)
        {
            currentLine.transform.localScale = new Vector3(1f, 0.1f, Mathf.Abs(pos.z * 2f));
        }
        else currentLine.transform.localScale = new Vector3(0f, 0.1f, 0f);
    }

    void Update()
    {
        if (isScaling)
        {
            Vector3 pos = dots[dots.Count - 1].transform.position + 0.5f * (pointer.transform.position - dots[dots.Count - 1].transform.position);
            currentLine.transform.position = new Vector3(pos.x, 1f, pos.z);
            pos -= dots[dots.Count - 1].transform.position;
            if (Mathf.Abs(pos.x) > 0f)
            {
                currentLine.transform.localScale = new Vector3(Mathf.Abs(pos.x * 2f), 0.1f, 1f);
            }
            else if (Mathf.Abs(pos.z) > 0f)
            {
                currentLine.transform.localScale = new Vector3(1f, 0.1f, Mathf.Abs(pos.z * 2f));
            }
            else currentLine.transform.localScale = new Vector3(0f, 0.1f, 0f);

            
            //creating new dot and line
            float dist = pointer.transform.position.x - dots[dots.Count - 1].transform.position.x;
            while (Mathf.Abs(dist) >= 5f)
            {
                if (dist>0)
                {
                    if (lines.Count > 1 && lines[lines.Count - 2].transform.position.x > dots[dots.Count - 1].transform.position.x)
                    {
                        lineScaleDestroy();
                    }
                    else
                    {
                        currentLine.transform.localScale = new Vector3(5f, 0.1f, 1f);
                        dotsOnPole.Add(dotsOnPole[dotsOnPole.Count - 1].GetComponent<PoleDot>().right.GetComponent<PoleLine>().right);
                        dots.Add(Instantiate(PathDotPrefab, dotsOnPole[dotsOnPole.Count - 1].transform.position + new Vector3(0f, 1f, 0f), PathDotPrefab.transform.rotation));
                        linesOnPole.Add(dotsOnPole[dotsOnPole.Count - 2].GetComponent<PoleDot>().right);
                        currentLine.transform.position = linesOnPole[linesOnPole.Count - 1].transform.position + new Vector3(0f, 1f, 0f);
                        currentLine = Instantiate(PathLinePrefab, dots[dots.Count - 1].transform);
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
                        currentLine.transform.localScale = new Vector3(5f, 0.1f, 1f);
                        dotsOnPole.Add(dotsOnPole[dotsOnPole.Count - 1].GetComponent<PoleDot>().left.GetComponent<PoleLine>().left);
                        dots.Add(Instantiate(PathDotPrefab, dotsOnPole[dotsOnPole.Count - 1].transform.position + new Vector3(0f, 1f, 0f), PathDotPrefab.transform.rotation));
                        linesOnPole.Add(dotsOnPole[dotsOnPole.Count - 2].GetComponent<PoleDot>().left);
                        currentLine.transform.position = linesOnPole[linesOnPole.Count - 1].transform.position + new Vector3(0f, 1f, 0f);
                        currentLine = Instantiate(PathLinePrefab, dots[dots.Count - 1].transform);
                        lines.Add(currentLine);
                        
                    }
                }
                dist = pointer.transform.position.x - dots[dots.Count - 1].transform.position.x;
            }
            dist = pointer.transform.position.z - dots[dots.Count - 1].transform.position.z;
            while (Mathf.Abs(dist) >= 5f)
            {
                if (dist > 0)
                {
                    if (lines.Count > 1 && lines[lines.Count - 2].transform.position.z > dots[dots.Count - 1].transform.position.z)
                    {
                        lineScaleDestroy();
                    }
                    else
                    {
                        currentLine.transform.localScale = new Vector3(1f, 0.1f, 5f);
                        dotsOnPole.Add(dotsOnPole[dotsOnPole.Count - 1].GetComponent<PoleDot>().up.GetComponent<PoleLine>().up);
                        dots.Add(Instantiate(PathDotPrefab, dotsOnPole[dotsOnPole.Count - 1].transform.position + new Vector3(0f, 1f, 0f), PathDotPrefab.transform.rotation));
                        linesOnPole.Add(dotsOnPole[dotsOnPole.Count - 2].GetComponent<PoleDot>().up);
                        currentLine.transform.position = linesOnPole[linesOnPole.Count - 1].transform.position + new Vector3(0f, 1f, 0f);
                        currentLine = Instantiate(PathLinePrefab, dots[dots.Count - 1].transform);
                        lines.Add(currentLine);
                        
                    }
                }
                else
                {
                    if (lines.Count > 1 && lines[lines.Count - 2].transform.position.z < dots[dots.Count - 1].transform.position.z)
                    {
                        lineScaleDestroy();
                    }
                    else
                    {
                        currentLine.transform.localScale = new Vector3(1f, 0.1f, 5f);
                        dotsOnPole.Add(dotsOnPole[dotsOnPole.Count - 1].GetComponent<PoleDot>().down.GetComponent<PoleLine>().down);
                        dots.Add(Instantiate(PathDotPrefab, dotsOnPole[dotsOnPole.Count - 1].transform.position + new Vector3(0f, 1f, 0f), PathDotPrefab.transform.rotation));
                        linesOnPole.Add(dotsOnPole[dotsOnPole.Count - 2].GetComponent<PoleDot>().down);
                        currentLine.transform.position = linesOnPole[linesOnPole.Count - 1].transform.position + new Vector3(0f, 1f, 0f);
                        currentLine = Instantiate(PathLinePrefab, dots[dots.Count - 1].transform);
                        lines.Add(currentLine);
                        
                    }
                }
                dist = pointer.transform.position.x - dots[dots.Count - 1].transform.position.x;
            }
            
            if (Mathf.Abs(currentLine.transform.localScale.x) >= 5f || Mathf.Abs(currentLine.transform.localScale.z) >= 5f)
            {
                dotsOnPole.Add(pointer.GetComponent<follow>().nearestDot);
                dots.Add(Instantiate(PathDotPrefab, dotsOnPole[dotsOnPole.Count - 1].transform.position + new Vector3(0f, 1f, 0f), PathDotPrefab.transform.rotation));
                currentLine = Instantiate(PathLinePrefab,dots[dots.Count-1].transform);
                lines.Add(currentLine);
                linesOnPole.Add(pointer.GetComponent<follow>().currentLine);
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
        else
        {
            Vector3 pos = dots[dots.Count - 1].transform.position + 0.5f * (pointer.transform.position - dots[dots.Count - 1].transform.position);
            currentLine = Instantiate(PathLinePrefab, pos, PathLinePrefab.transform.rotation);
            lines.Add(currentLine);
            isScaling = true;
        }
        //foreach(GameObject g in dotsOnPole)
        //{
        //    g.GetComponent<Renderer>().material.color = new Color(1, 0, 0);
        //}
    }

    private void AddNewDot()
    {

    }
    private void DeleteLastDot()
    {
        if (dots.Count != 1)
        {

        }
    }
    private void ResizeLastLine()
    {

    }

	
	
}
