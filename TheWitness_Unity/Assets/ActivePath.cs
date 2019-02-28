using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivePath : MonoBehaviour {

    public GameObject pointerPF;

    public List<GameObject> dots = new List<GameObject>();
    public List<GameObject> lines = new List<GameObject>();
    public GameObject pointer;
    public GameObject currentLine;
    public bool isScaling = false;

    public GameObject PathDotPrefab;
    public GameObject PathLinePrefab;


    public void Init(GameObject start)
    {
        dots.Add(Instantiate(PathDotPrefab,start.transform.position + new Vector3(0f,1f,0f),PathDotPrefab.transform.rotation));
        pointer = Instantiate(pointerPF, start.transform.position + new Vector3(0f, 2f, 0f), pointerPF.transform.rotation);
        pointer.GetComponent<follow>().nearestDot = start;
        pointer.GetComponent<follow>().currentLine = start.GetComponent<PoleDot>().left;
    }

    void Update()
    {
        if (isScaling)
        {
            Vector3 pos = dots[dots.Count - 1].transform.position + 0.5f * (pointer.transform.position - dots[dots.Count - 1].transform.position);
            currentLine.transform.position = new Vector3(pos.x, 1f, pos.z);
            pos -= dots[dots.Count - 1].transform.position;
            if (Mathf.Abs(pos.x) > 0.1f)
            {
                currentLine.transform.localScale = new Vector3(pos.x * 2f, 0.1f, 1f);
            }
            else if (Mathf.Abs(pos.z) > 0.1f)
            {
                currentLine.transform.localScale = new Vector3(1f, 0.1f, pos.z * 2f);
            }
            
            if (Mathf.Abs(currentLine.transform.localScale.x) >= 5f || Mathf.Abs(currentLine.transform.localScale.z) >= 5f)
            {
                
                dots.Add(Instantiate(PathDotPrefab, pointer.GetComponent<follow>().nearestDot.transform.position + new Vector3(0f, 1f, 0f), PathDotPrefab.transform.rotation));
                Vector3 newpos = dots[dots.Count - 1].transform.position - 0.5f * (dots[dots.Count - 1].transform.position - pointer.GetComponent<follow>().nearestDot.transform.position);
                currentLine.transform.position = new Vector3(newpos.x, 1f, newpos.z);
                if (Mathf.Abs(currentLine.transform.localScale.x) >= 5f)
                    currentLine.transform.localScale = new Vector3(currentLine.transform.localScale.x / Mathf.Abs(currentLine.transform.localScale.x) * 5f, 0.1f, 1f);
                else currentLine.transform.localScale = new Vector3(1f, 0.1f, currentLine.transform.localScale.z / Mathf.Abs(currentLine.transform.localScale.z) * 5f);

                currentLine = Instantiate(PathLinePrefab, pointer.GetComponent<follow>().nearestDot.transform.position + new Vector3(0f,1f,0f), PathLinePrefab.transform.rotation);
                lines.Add(currentLine);
                
            }
            if (Vector3.Distance(lines[lines.Count - 2].transform.position, pointer.transform.position) < 2.5f)
            {
                Destroy(currentLine);
                lines.RemoveAt(lines.Count - 1);
                currentLine = lines[lines.Count - 1];
                Destroy(dots[dots.Count - 1]);
                dots.RemoveAt(dots.Count - 1);
            }

        }
        else
        {
            Vector3 pos = dots[dots.Count - 1].transform.position + 0.5f * (pointer.transform.position - dots[dots.Count - 1].transform.position);
            currentLine = Instantiate(PathLinePrefab, pos, PathLinePrefab.transform.rotation);
            lines.Add(currentLine);
            isScaling = true;
        }
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
