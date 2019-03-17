using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour {

    public GameObject PolePF;
    public GameObject ActivePathPF;

    public GameObject activePath;
    public GameObject menuPole;
    public List<GameObject> prevPoles = new List<GameObject>();
    public List<GameObject> prevPaths = new List<GameObject>();

    private Vector3 lastPos;
    private bool resumed = true;
	// Use this for initialization
	void Start () {
        //QualitySettings.vSyncCount = 0;
        //Application.targetFrameRate = 10;
        lastPos = Input.mousePosition;
        menuPole = Instantiate(PolePF, transform);
        menuPole.GetComponent<Pole>().InitSpec(4);
        activePath = Instantiate(ActivePathPF, transform);
        activePath.GetComponent<ActivePath>().Init(menuPole, menuPole.GetComponent<Pole>().start, menuPole.GetComponent<Pole>().finishes);
    }
	
	// Update is called once per frame
	void Update () {
        Vector3 dist = Input.mousePosition - lastPos;
        lastPos = Input.mousePosition;
        if (activePath.GetComponent<ActivePath>().isFinished && resumed)
        {
            prevPaths.Add(activePath);
            activePath.GetComponent<ActivePath>().pointer.GetComponent<follow>().notActive = true;
            prevPoles.Add(menuPole);
            menuPole = Instantiate(PolePF, activePath.GetComponent<ActivePath>().currentFinish.transform.position + new Vector3(0f, 0f, 0.5f), transform.rotation);
            menuPole.GetComponent<Pole>().InitSpec(4);
            activePath = Instantiate(ActivePathPF, activePath.GetComponent<ActivePath>().currentFinish.transform.position + new Vector3(0f, 0f, 0.5f), transform.rotation);
            activePath.GetComponent<ActivePath>().Init(menuPole, menuPole.GetComponent<Pole>().start, menuPole.GetComponent<Pole>().finishes);
        }
        else if (prevPaths.Count > 0 && resumed)
        {
            
            if (activePath.GetComponent<ActivePath>().pointer.transform.position.x <= menuPole.GetComponent<Pole>().start.transform.position.x && dist.x < 0)
            {
                Destroy(menuPole);
                Destroy(activePath);
                
                activePath = prevPaths[prevPaths.Count - 1];
                
                menuPole = prevPoles[prevPoles.Count - 1];
                prevPaths.RemoveAt(prevPaths.Count - 1);
                prevPoles.RemoveAt(prevPoles.Count - 1);
                activePath.GetComponent<ActivePath>().pointer.GetComponent<follow>().notActive = false;

                resumed = false;
            }
        }
        if (!activePath.GetComponent<ActivePath>().isFinished) resumed = true;
    }
}
