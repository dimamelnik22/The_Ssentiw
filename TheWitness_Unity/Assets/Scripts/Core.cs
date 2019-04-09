using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class Core : MonoBehaviour {

    public GameObject PolePrefab;
    public GameObject PointPrefab;

    public Text gentimewin;
    public float gentime = 0f;

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
    private List<GameObject> finishes = new List<GameObject>();
    public int seed = 95;
    public bool mode = true;
    public bool pathIsShown = false;
    public bool playerIsActive = false;
    private GameObject myPole;
    public GameObject activePath;

    public static class PolePreferences
    {
        
        public static int poleSize = 5;
        public static int numOfPoints = 5;
        public static int numOfCircles = 5;
        public static int numOfStars = 5;
        public static int numOfShapes = 0;
        public static float complexity =0.3f;
        public static bool isFrozen = false;
        public static class MyRandom
        {
            public static int seed = 4323;

            public static int GetRandom()
            {
                seed = (seed * 430 + 2531) % 11979;
                return seed;
            }
        }
        
    }
    
    public void ButtonMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
    
    public void NextButton()
    {
        SceneManager.LoadScene("PoleLevel");
    }

    public void ButtonShowSolution()
    {
        PolePreferences.isFrozen = true;
        playerIsActive = false;
        foreach (GameObject point in myPole.GetComponent<Pole>().eltsManager.unsolvedElts)
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
            for (int i = 0; i < myPole.GetComponent<Pole>().systemPath.dots.Count-2; i++)
            {
                if (i == 0) Instantiate(PathStartPrefab, transform.position + stepx * myPole.GetComponent<Pole>().systemPath.dots[i].GetComponent<PoleDot>().posX + stepy * myPole.GetComponent<Pole>().systemPath.dots[i].GetComponent<PoleDot>().posY + pathstepz, PathDotPrefab.transform.rotation);
                else Instantiate(PathDotPrefab, transform.position + stepx * myPole.GetComponent<Pole>().systemPath.dots[i].GetComponent<PoleDot>().posX + stepy * myPole.GetComponent<Pole>().systemPath.dots[i].GetComponent<PoleDot>().posY + pathstepz, PathDotPrefab.transform.rotation);
                if (myPole.GetComponent<Pole>().systemPath.lines[i] == myPole.GetComponent<Pole>().systemPath.dots[i].GetComponent<PoleDot>().up) Instantiate(PathVerticalLinePrefab, transform.position - stepy * 0.5f + stepx * myPole.GetComponent<Pole>().systemPath.dots[i].GetComponent<PoleDot>().posX + stepy * myPole.GetComponent<Pole>().systemPath.dots[i].GetComponent<PoleDot>().posY + pathstepz, PathVerticalLinePrefab.transform.rotation);
                else if (myPole.GetComponent<Pole>().systemPath.lines[i] == myPole.GetComponent<Pole>().systemPath.dots[i].GetComponent<PoleDot>().down) Instantiate(PathVerticalLinePrefab, transform.position + stepy * 0.5f + stepx * myPole.GetComponent<Pole>().systemPath.dots[i].GetComponent<PoleDot>().posX + stepy * myPole.GetComponent<Pole>().systemPath.dots[i].GetComponent<PoleDot>().posY + pathstepz, PathVerticalLinePrefab.transform.rotation);
                else if (myPole.GetComponent<Pole>().systemPath.lines[i] == myPole.GetComponent<Pole>().systemPath.dots[i].GetComponent<PoleDot>().left) Instantiate(PathHorizontalLinePrefab, transform.position - stepx * 0.5f + stepx * myPole.GetComponent<Pole>().systemPath.dots[i].GetComponent<PoleDot>().posX + stepy * myPole.GetComponent<Pole>().systemPath.dots[i].GetComponent<PoleDot>().posY + pathstepz, PathHorizontalLinePrefab.transform.rotation);
                else if (myPole.GetComponent<Pole>().systemPath.lines[i] == myPole.GetComponent<Pole>().systemPath.dots[i].GetComponent<PoleDot>().right) Instantiate(PathHorizontalLinePrefab, transform.position + stepx * 0.5f + stepx * myPole.GetComponent<Pole>().systemPath.dots[i].GetComponent<PoleDot>().posX + stepy * myPole.GetComponent<Pole>().systemPath.dots[i].GetComponent<PoleDot>().posY + pathstepz, PathHorizontalLinePrefab.transform.rotation);
            }
            Instantiate(PathFinishPrefab, transform.position + stepx * myPole.GetComponent<Pole>().finish.GetComponent<PoleDot>().posX + stepy * myPole.GetComponent<Pole>().finish.GetComponent<PoleDot>().posY + pathstepz, PathFinishPrefab.transform.rotation);
        }
        pathIsShown = !pathIsShown;
    }

    void Start() {
        //QualitySettings.vSyncCount = 0;
        //Application.targetFrameRate = 10;

        gentime = Time.realtimeSinceStartup;
        myPole = Instantiate(PolePrefab);
        myPole.GetComponent<Pole>().Init(PolePreferences.poleSize);
        //GameObject.FindGameObjectWithTag("Player").GetComponent<follow>().dot = myPole.poleDots[0][0];
        playerPathLinesOnScreen = new List<GameObject>();
        playerPathDotsOnScreen = new List<GameObject>();

        playerIsActive = false;

        //int index = PolePreferences.MyRandom.GetRandom() % (PolePreferences.poleSize * PolePreferences.poleSize);
        //int x = index % PolePreferences.poleSize;
        //int y = index / PolePreferences.poleSize;
        //myPole.GetComponent<Pole>().SetStart(x, y);
        //index = PolePreferences.MyRandom.GetRandom() % (PolePreferences.poleSize * PolePreferences.poleSize);
        //x = index % PolePreferences.poleSize;
        //y = index / PolePreferences.poleSize;
        //while (myPole.GetComponent<Pole>().start.GetComponent<PoleDot>().posX == x && myPole.GetComponent<Pole>().start.GetComponent<PoleDot>().posY == y)
        //{
        //    index = PolePreferences.MyRandom.GetRandom() % (PolePreferences.poleSize * PolePreferences.poleSize);
        //    x = index % PolePreferences.poleSize;
        //    y = index / PolePreferences.poleSize;
        //}
        //myPole.GetComponent<Pole>().SetFinish(x, y);
        int x = 0;
        int y = 0;
        switch (PolePreferences.MyRandom.GetRandom() % 4)
        {
            case 0:
                x = PolePreferences.MyRandom.GetRandom() % PolePreferences.poleSize;
                y = 0;
                break;
            case 1:
                y = PolePreferences.MyRandom.GetRandom() % PolePreferences.poleSize;
                x = PolePreferences.poleSize - 1;
                break;
            case 2:
                x = PolePreferences.MyRandom.GetRandom() % PolePreferences.poleSize;
                y = PolePreferences.poleSize - 1;
                break;
            case 3:
                y = PolePreferences.MyRandom.GetRandom() % PolePreferences.poleSize;
                x = 0;
                break;
        }
        myPole.GetComponent<Pole>().SetStart(x, y);
        do
        {
            switch (PolePreferences.MyRandom.GetRandom() % 4)
            {
                case 0:
                    x = PolePreferences.MyRandom.GetRandom() % PolePreferences.poleSize;
                    y = 0;
                    break;
                case 1:
                    y = PolePreferences.MyRandom.GetRandom() % PolePreferences.poleSize;
                    x = PolePreferences.poleSize - 1;
                    break;
                case 2:
                    x = PolePreferences.MyRandom.GetRandom() % PolePreferences.poleSize;
                    y = PolePreferences.poleSize - 1;
                    break;
                case 3:
                    y = PolePreferences.MyRandom.GetRandom() % PolePreferences.poleSize;
                    x = 0;
                    break;
            }
        } while (myPole.GetComponent<Pole>().poleDots[y][x] == myPole.GetComponent<Pole>().start);
        myPole.GetComponent<Pole>().SetFinish(x, y);

        myPole.GetComponent<Pole>().CreateSolution();
        myPole.GetComponent<Pole>().GeneratePoints(PolePreferences.numOfPoints); 
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
        finishes.Add(myPole.GetComponent<Pole>().finish);
        mode = !mode;
        pathIsShown = false;
        gentimewin.text = (Time.realtimeSinceStartup - gentime).ToString();
        gentime = Time.realtimeSinceStartup;
    }

    void Update()
    {
#if UNITY_EDITOR
        if (!playerIsActive && PolePreferences.isFrozen == false)
        {

            foreach (GameObject point in myPole.GetComponent<Pole>().eltsManager.unsolvedElts)
            {
                point.GetComponent<PoleEltPoint>().NormalizeColor();
            }
            if (activePath == null)
            {
                activePath = Instantiate(ActivePathPF);
                
                
                activePath.GetComponent<ActivePath>().Init(myPole, myPole.GetComponent<Pole>().start, finishes);
            }
            else
            {
                activePath.GetComponent<ActivePath>().Restart(myPole.GetComponent<Pole>().start, finishes);
            }
            playerIsActive = !playerIsActive;
        }
        else if (!PolePreferences.isFrozen && activePath.GetComponent<ActivePath>().isFinished && !Input.GetMouseButton(0))
        {
            PolePreferences.isFrozen = true;
            myPole.GetComponent<Pole>().playerPath.Clear();
            activePath.GetComponent<ActivePath>().EndSolution();
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
            if (myPole.GetComponent<Pole>().playerPath.dots[myPole.GetComponent<Pole>().playerPath.dots.Count - 1] == myPole.GetComponent<Pole>().finish && activePath.GetComponent<ActivePath>().isFinished)
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
                    foreach (GameObject point in myPole.GetComponent<Pole>().eltsManager.unsolvedElts)
                    {
                        point.GetComponent<PoleEltPoint>().ShowUnsolved();
                    }
                }
            }
            else foreach (GameObject path in GameObject.FindGameObjectsWithTag("Path"))
                {
                    path.GetComponent<Renderer>().material.Lerp(path.GetComponent<Renderer>().material, PlayerWrongPathMaterial, 1f);
                }
            
            playerIsActive = !playerIsActive;
        }
        if (Input.GetMouseButton(0) && PolePreferences.isFrozen && !pathIsShown)
        {
            foreach (GameObject dot in myPole.GetComponent<Pole>().playerPath.dots)
                dot.GetComponent<PoleDot>().isUsedByPlayer = false;
            foreach (GameObject line in myPole.GetComponent<Pole>().playerPath.lines)
                line.GetComponent<PoleLine>().isUsedByPlayer = false;
            //playerIsActive = !playerIsActive;
            PolePreferences.isFrozen = false;
            //activePath.GetComponent<ActivePath>().Restart(myPole.GetComponent<Pole>().start, finishes);
        }
#else

        if (!playerIsActive && PolePreferences.isFrozen == false)
        {

            foreach (GameObject point in myPole.GetComponent<Pole>().eltsManager.unsolvedElts)
            {
                point.GetComponent<PoleEltPoint>().NormalizeColor();
            }
            if (activePath == null)
            {
                activePath = Instantiate(ActivePathPF);


                activePath.GetComponent<ActivePath>().Init(myPole, myPole.GetComponent<Pole>().start, finishes);
                
            }
            else
            {


                activePath.GetComponent<ActivePath>().Restart(myPole.GetComponent<Pole>().start, finishes);
            }
            playerIsActive = !playerIsActive;
        }
        else if (!PolePreferences.isFrozen && activePath.GetComponent<ActivePath>().isFinished && Input.touchCount == 0)
        {
            PolePreferences.isFrozen = true;
            myPole.GetComponent<Pole>().playerPath.Clear();
            activePath.GetComponent<ActivePath>().EndSolution();
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
            if (myPole.GetComponent<Pole>().playerPath.dots[myPole.GetComponent<Pole>().playerPath.dots.Count - 1] == myPole.GetComponent<Pole>().finish && activePath.GetComponent<ActivePath>().isFinished)
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
                    foreach (GameObject point in myPole.GetComponent<Pole>().eltsManager.unsolvedElts)
                    {
                        point.GetComponent<PoleEltPoint>().ShowUnsolved();
                    }
                }
            }
            else foreach (GameObject path in GameObject.FindGameObjectsWithTag("Path"))
                {
                    path.GetComponent<Renderer>().material.Lerp(path.GetComponent<Renderer>().material, PlayerWrongPathMaterial, 1f);
                }
            
            playerIsActive = !playerIsActive;
        }
        if (Input.touchCount > 0 && PolePreferences.isFrozen && !pathIsShown)
        {
            foreach (GameObject dot in myPole.GetComponent<Pole>().playerPath.dots)
                dot.GetComponent<PoleDot>().isUsedByPlayer = false;
            foreach (GameObject line in myPole.GetComponent<Pole>().playerPath.lines)
                line.GetComponent<PoleLine>().isUsedByPlayer = false;
            PolePreferences.isFrozen = false;
            
        }
#endif
    }

}
