using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Core : MonoBehaviour {

    public GameObject PolePrefab;
    public GameObject PointPrefab;

    public GameObject PathDotPrefab;
    public GameObject PathVerticalLinePrefab;
    public GameObject PathHorizontalLinePrefab;
    public GameObject PathStartPrefab;
    public GameObject PathFinishPrefab;

    public Material PlayerWrongPathMaterial;
    public Material PlayerGoodPathMaterial;
    public Material EltPointMaterial;
    public Material EltWrongPointMaterial;

    public GameObject ActivePathPF;

    private List<GameObject> playerPathLinesOnScreen;
    private List<GameObject> playerPathDotsOnScreen;

    private static Vector3 pathstepz = new Vector3(0f, 0f, -0.5f);

    private static Vector3 stepx = new Vector3(5f, 0f, 0f);
    private static Vector3 stepy = new Vector3(0f, -5f, 0f);

    public int seed = 95;
    public bool mode = true;
    public bool pathIsShown = false;
    public bool playerIsActive = false;
    private GameObject myPole;
    public GameObject activePath;

    public static class PolePreferences
    {
        public static int poleSize = 10;
        public static int numOfPoints = 0;
    }
    
    public void ControllerStartFinish()
    {
        if (!playerIsActive)
        {
            foreach (GameObject point in myPole.GetComponent<Pole>().eltsManager.unsolvedPoints)
            {
                point.GetComponent<PoleEltPoint>().NormalizeColor();
            }
            if (activePath == null)
            {
                activePath = Instantiate(ActivePathPF);
                activePath.GetComponent<ActivePath>().Init(myPole.GetComponent<Pole>().start);
            }
            else
            {
                activePath.GetComponent<ActivePath>().Restart(myPole.GetComponent<Pole>().start);
            }
            playerIsActive = !playerIsActive;
        }
        else
        {
            myPole.GetComponent<Pole>().playerPath.Clear();
            foreach (GameObject dot in activePath.GetComponent<ActivePath>().dotsOnPole)
            {
                dot.GetComponent<PoleDot>().isUsedByPlayer = true;
                myPole.GetComponent<Pole>().playerPath.dots.Add(dot);
            }
            foreach (GameObject line in activePath.GetComponent<ActivePath>().linesOnPole)
            {
                line.GetComponent<PoleLine>().isUsedByPlayer = true;
                myPole.GetComponent<Pole>().playerPath.lines.Add(line);
            }
            if (myPole.GetComponent<Pole>().playerPath.dots[myPole.GetComponent<Pole>().playerPath.dots.Count - 1] == myPole.GetComponent<Pole>().finish)
            {
                if (myPole.GetComponent<Pole>().eltsManager.CheckSolution())
                {
                    foreach (GameObject path in GameObject.FindGameObjectsWithTag("Path"))
                    {
                        path.GetComponent<Renderer>().material.Lerp(path.GetComponent<Renderer>().material, PlayerGoodPathMaterial, 1f);
                    }
                }
                else
                {
                    foreach (GameObject path in GameObject.FindGameObjectsWithTag("Path"))
                    {
                        path.GetComponent<Renderer>().material.Lerp(path.GetComponent<Renderer>().material, PlayerWrongPathMaterial, 1f);
                    }
                    foreach (GameObject point in myPole.GetComponent<Pole>().eltsManager.unsolvedPoints)
                    {
                        point.GetComponent<PoleEltPoint>().ShowUnsolved();
                    }
                }
            }
            else foreach (GameObject path in GameObject.FindGameObjectsWithTag("Path"))
                {
                    path.GetComponent<Renderer>().material.Lerp(path.GetComponent<Renderer>().material, PlayerWrongPathMaterial, 1f);
                }
            foreach (GameObject dot in myPole.GetComponent<Pole>().playerPath.dots)
                dot.GetComponent<PoleDot>().isUsedByPlayer = false;
            foreach (GameObject line in myPole.GetComponent<Pole>().playerPath.lines)
                line.GetComponent<PoleLine>().isUsedByPlayer = false;
            playerIsActive = !playerIsActive;
        }
    }
    public void ButtonCreate()
    {
        playerIsActive = false;
        foreach (GameObject dot in myPole.GetComponent<Pole>().playerPath.dots)
            dot.GetComponent<PoleDot>().isUsedByPlayer = false;
        foreach (GameObject line in myPole.GetComponent<Pole>().playerPath.lines)
            line.GetComponent<PoleLine>().isUsedByPlayer = false;
        myPole.GetComponent<Pole>().playerPath.dots.Clear();
        myPole.GetComponent<Pole>().playerPath.lines.Clear();
        playerPathDotsOnScreen.Clear();
        playerPathLinesOnScreen.Clear();
        foreach (GameObject gameObject in GameObject.FindGameObjectsWithTag("Path"))
            Destroy(gameObject);
        foreach (GameObject gameObject in GameObject.FindGameObjectsWithTag("EltPoint"))
            Destroy(gameObject);
        Destroy(GameObject.FindGameObjectWithTag("Player"));
        Destroy(activePath);
        myPole.GetComponent<Pole>().ClearPole();
        if (mode)
        {
            myPole.GetComponent<Pole>().SetStart(myPole.GetComponent<Pole>().myRandGen.GetRandom() % myPole.GetComponent<Pole>().GetSize(), myPole.GetComponent<Pole>().myRandGen.GetRandom() % myPole.GetComponent<Pole>().GetSize());
            int x = myPole.GetComponent<Pole>().myRandGen.GetRandom() % myPole.GetComponent<Pole>().GetSize();
            int y = myPole.GetComponent<Pole>().myRandGen.GetRandom() % myPole.GetComponent<Pole>().GetSize();
            while (myPole.GetComponent<Pole>().start.GetComponent<PoleDot>().posX == x && myPole.GetComponent<Pole>().start.GetComponent<PoleDot>().posY == y)
            {
                x = myPole.GetComponent<Pole>().myRandGen.GetRandom() % myPole.GetComponent<Pole>().GetSize();
                y = myPole.GetComponent<Pole>().myRandGen.GetRandom() % myPole.GetComponent<Pole>().GetSize();
            }
            myPole.GetComponent<Pole>().SetFinish(x, y);
            myPole.GetComponent<Pole>().CreateSolution();
            myPole.GetComponent<Pole>().GeneratePoints(7);
            for (int i = 0; i < myPole.GetComponent<Pole>().GetSize(); i++)
            {
                for (int j = 0; j < myPole.GetComponent<Pole>().GetSize(); j++)
                {
                    if (myPole.GetComponent<Pole>().poleDots[i][j].GetComponent<PoleDot>().hasPoint)
                    {
                        myPole.GetComponent<Pole>().poleDots[i][j].GetComponent<PoleDot>().point = Instantiate(PointPrefab, transform.position + stepx * j + stepy * i + pathstepz, PointPrefab.transform.rotation);
                        myPole.GetComponent<Pole>().poleDots[i][j].GetComponent<PoleDot>().point.GetComponent<PoleEltPoint>().SetDot(myPole.GetComponent<Pole>().poleDots[i][j]);
                        myPole.GetComponent<Pole>().eltsManager.points.Add(myPole.GetComponent<Pole>().poleDots[i][j].GetComponent<PoleDot>().point);
                    }
                }
            }
            for (int i = 0; i < myPole.GetComponent<Pole>().GetSize(); i++)
            {
                for (int j = 0; j < myPole.GetComponent<Pole>().GetSize(); j++)
                {
                    if (j < myPole.GetComponent<Pole>().GetSize() - 1)
                        if (myPole.GetComponent<Pole>().poleDots[i][j].GetComponent<PoleDot>().right != null)
                        {
                            if (myPole.GetComponent<Pole>().poleDots[i][j].GetComponent<PoleDot>().right.GetComponent<PoleLine>().hasPoint)
                            {
                                myPole.GetComponent<Pole>().poleDots[i][j].GetComponent<PoleDot>().right.GetComponent<PoleLine>().point = Instantiate(PointPrefab, transform.position + stepx * 0.5f + stepx * j + stepy * i + pathstepz, PointPrefab.transform.rotation);
                                myPole.GetComponent<Pole>().poleDots[i][j].GetComponent<PoleDot>().right.GetComponent<PoleLine>().point.GetComponent<PoleEltPoint>().SetLine(myPole.GetComponent<Pole>().poleDots[i][j].GetComponent<PoleDot>().right);
                                myPole.GetComponent<Pole>().eltsManager.points.Add(myPole.GetComponent<Pole>().poleDots[i][j].GetComponent<PoleDot>().right.GetComponent<PoleLine>().point);
                            }
                        }
                }
                if (i < myPole.GetComponent<Pole>().GetSize() - 1)
                    for (int j = 0; j < myPole.GetComponent<Pole>().GetSize(); j++)
                    {
                        if (myPole.GetComponent<Pole>().poleDots[i][j].GetComponent<PoleDot>().down != null)
                        {
                            if (myPole.GetComponent<Pole>().poleDots[i][j].GetComponent<PoleDot>().down.GetComponent<PoleLine>().hasPoint)
                            {
                                myPole.GetComponent<Pole>().poleDots[i][j].GetComponent<PoleDot>().down.GetComponent<PoleLine>().point = Instantiate(PointPrefab, transform.position + stepy * 0.5f + stepx * j + stepy * i + pathstepz, PointPrefab.transform.rotation);
                                myPole.GetComponent<Pole>().poleDots[i][j].GetComponent<PoleDot>().down.GetComponent<PoleLine>().point.GetComponent<PoleEltPoint>().SetLine(myPole.GetComponent<Pole>().poleDots[i][j].GetComponent<PoleDot>().down);
                                myPole.GetComponent<Pole>().eltsManager.points.Add(myPole.GetComponent<Pole>().poleDots[i][j].GetComponent<PoleDot>().down.GetComponent<PoleLine>().point);
                            }
                        }
                    }
            }
        }
        seed = myPole.GetComponent<Pole>().myRandGen.seed;

        mode = !mode;
        pathIsShown = false;
    }
    
    public void ButtonShowSolution()
    {
        playerIsActive = false;
        foreach (GameObject point in myPole.GetComponent<Pole>().eltsManager.unsolvedPoints)
        {
            point.GetComponent<PoleEltPoint>().NormalizeColor();
        }
        foreach (GameObject dot in myPole.GetComponent<Pole>().playerPath.dots)
            dot.GetComponent<PoleDot>().isUsedByPlayer = false;
        foreach (GameObject line in myPole.GetComponent<Pole>().playerPath.lines)
            line.GetComponent<PoleLine>().isUsedByPlayer = false;
        Destroy(GameObject.FindGameObjectWithTag("Player"));
        Destroy(activePath);
        myPole.GetComponent<Pole>().playerPath.dots.Clear();
        myPole.GetComponent<Pole>().playerPath.lines.Clear();
        playerPathDotsOnScreen.Clear();
        playerPathLinesOnScreen.Clear();
        if (pathIsShown)
        {
            foreach (GameObject gameObject in GameObject.FindGameObjectsWithTag("Path"))
                Destroy(gameObject);
        }
        else
        {
            foreach (GameObject gameObject in GameObject.FindGameObjectsWithTag("Path"))
                Destroy(gameObject);
            for (int i = 0; i < myPole.GetComponent<Pole>().systemPath.dots.Count; i++)
            {
                if (i < myPole.GetComponent<Pole>().systemPath.dots.Count - 1)
                {
                    if (i == 0) Instantiate(PathStartPrefab, transform.position + stepx * myPole.GetComponent<Pole>().systemPath.dots[i].GetComponent<PoleDot>().posX + stepy * myPole.GetComponent<Pole>().systemPath.dots[i].GetComponent<PoleDot>().posY + pathstepz, PathDotPrefab.transform.rotation);
                    else Instantiate(PathDotPrefab, transform.position + stepx * myPole.GetComponent<Pole>().systemPath.dots[i].GetComponent<PoleDot>().posX + stepy * myPole.GetComponent<Pole>().systemPath.dots[i].GetComponent<PoleDot>().posY + pathstepz, PathDotPrefab.transform.rotation);
                    if (myPole.GetComponent<Pole>().systemPath.lines[i] == myPole.GetComponent<Pole>().systemPath.dots[i].GetComponent<PoleDot>().up) Instantiate(PathVerticalLinePrefab, transform.position - stepy * 0.5f + stepx * myPole.GetComponent<Pole>().systemPath.dots[i].GetComponent<PoleDot>().posX + stepy * myPole.GetComponent<Pole>().systemPath.dots[i].GetComponent<PoleDot>().posY + pathstepz, PathVerticalLinePrefab.transform.rotation);
                    else if (myPole.GetComponent<Pole>().systemPath.lines[i] == myPole.GetComponent<Pole>().systemPath.dots[i].GetComponent<PoleDot>().down) Instantiate(PathVerticalLinePrefab, transform.position + stepy * 0.5f + stepx * myPole.GetComponent<Pole>().systemPath.dots[i].GetComponent<PoleDot>().posX + stepy * myPole.GetComponent<Pole>().systemPath.dots[i].GetComponent<PoleDot>().posY + pathstepz, PathVerticalLinePrefab.transform.rotation);
                    else if (myPole.GetComponent<Pole>().systemPath.lines[i] == myPole.GetComponent<Pole>().systemPath.dots[i].GetComponent<PoleDot>().left) Instantiate(PathHorizontalLinePrefab, transform.position - stepx * 0.5f + stepx * myPole.GetComponent<Pole>().systemPath.dots[i].GetComponent<PoleDot>().posX + stepy * myPole.GetComponent<Pole>().systemPath.dots[i].GetComponent<PoleDot>().posY + pathstepz, PathHorizontalLinePrefab.transform.rotation);
                    else if (myPole.GetComponent<Pole>().systemPath.lines[i] == myPole.GetComponent<Pole>().systemPath.dots[i].GetComponent<PoleDot>().right) Instantiate(PathHorizontalLinePrefab, transform.position + stepx * 0.5f + stepx * myPole.GetComponent<Pole>().systemPath.dots[i].GetComponent<PoleDot>().posX + stepy * myPole.GetComponent<Pole>().systemPath.dots[i].GetComponent<PoleDot>().posY + pathstepz, PathHorizontalLinePrefab.transform.rotation);
                }
            }
            Instantiate(PathFinishPrefab, transform.position + stepx * myPole.GetComponent<Pole>().finish.GetComponent<PoleDot>().posX + stepy * myPole.GetComponent<Pole>().finish.GetComponent<PoleDot>().posY + pathstepz, transform.rotation);
        }
        pathIsShown = !pathIsShown;
    }

    void Start() {
        //QualitySettings.vSyncCount = 0;
        //Application.targetFrameRate = 10;

        
        myPole = Instantiate(PolePrefab);
        myPole.GetComponent<Pole>().Init(PolePreferences.poleSize, seed);
        //GameObject.FindGameObjectWithTag("Player").GetComponent<follow>().dot = myPole.poleDots[0][0];
        playerPathLinesOnScreen = new List<GameObject>();
        playerPathDotsOnScreen = new List<GameObject>();
    }

    void Update()
    {
        
    }

}
