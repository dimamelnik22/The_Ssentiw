using System;
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
    public bool mode = true;
    public bool pathIsShown = false;
    public bool playerIsActive = false;
    private GameObject myPole;
    public GameObject activePath;

    public static class PolePreferences
    {
        
        public static int poleSize = 9;
        public static int complexity = 40;
        public static int numOfPoints = 7;
        public static int numOfCircles = 10;
        public static int numOfStars = 5;
        public static int numOfShapes = 20;
        public static bool isFrozen = false;
        public static System.Random r = new System.Random();
        public static class MyRandom
        {
            public static int seed = 4323;
            public static void SetSeed(int s = 0)
            {
                seed = s;
                r = new System.Random(seed);
            }
            public static int GetRandom()
            {
                //seed = (seed * 430 + 2531) % 11979;
                //return seed;

                return r.Next();
            }
        }
        public static string info = "";
        public static string mode = "normal";
    }
    public void ButtonReport()
    {
        MenuManager.DebugMessage.push2Buffer();
    }
    public void ButtonMenu()
    {
        SceneManager.LoadScene("MainMenu");
        MenuManager.DebugMessage.clear();
    }
    
    public void NextButton()
    {
        Core.PolePreferences.MyRandom.SetSeed(Core.PolePreferences.MyRandom.GetRandom());
        MenuManager.DebugMessage.saveSeed(Core.PolePreferences.MyRandom.seed);
        SceneManager.LoadScene("PoleLevel");
    }

    public void ButtonShowSolution()
    {
        PolePreferences.isFrozen = true;
        playerIsActive = false;
        foreach (GameObject point in myPole.GetComponent<Pole>().eltsManager.unsolvedElts)
        {
            if (point.GetComponent<PoleEltPoint>() != null)
            {
                point.GetComponent<PoleEltPoint>().ShowNormalizedColor();
            }
            if (point.GetComponent<EltClrRing>() != null)
            {
                point.GetComponent<EltClrRing>().ShowNormalizedColor();
            }
            if (point.GetComponent<PoleEltShape>() != null)
            {
                point.GetComponent<PoleEltShape>().ShowNormalizedColor();
            }
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
            foreach (GameObject dot in myPole.GetComponent<Pole>().systemPath.dots)
            {
                if (dot == myPole.GetComponent<Pole>().start) Instantiate(PathStartPrefab, dot.transform.position + pathstepz, PathStartPrefab.transform.rotation);
                else Instantiate(PathDotPrefab, dot.transform.position + pathstepz, PathDotPrefab.transform.rotation);
            }
            foreach (GameObject line in myPole.GetComponent<Pole>().systemPath.lines)
            {
                if (line.GetComponent<PoleLine>().isHorizontal) Instantiate(PathHorizontalLinePrefab, line.transform.position + pathstepz, PathHorizontalLinePrefab.transform.rotation);
                else Instantiate(PathVerticalLinePrefab, line.transform.position + pathstepz, PathVerticalLinePrefab.transform.rotation);
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
        switch (Core.PolePreferences.mode)
        {
            case "normal":
                myPole.GetComponent<Pole>().Init(PolePreferences.poleSize);
                // START and FINISH creating
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
                myPole.GetComponent<Pole>().GenerateShapes(Core.PolePreferences.numOfShapes);
                myPole.GetComponent<Pole>().SetClrRing(myPole.GetComponent<Pole>().quantityColor, myPole.GetComponent<Pole>().quantityRing);
                myPole.GetComponent<Pole>().GeneratePoints(PolePreferences.numOfPoints);
                gentimewin.text = (Time.realtimeSinceStartup - gentime).ToString();
                gentime = Time.realtimeSinceStartup;
                break;
            case "info":
                myPole.GetComponent<Pole>().InitStr("S5sST0Y0XFH4Y4XPT3p1Y1XS2Y2XS3Y3XURG2r3I0J11I3J2SR0sSP3s0I0J2H3W1001111I1J2H3W1001112I3J2H3W100111");
                gentimewin.text = "S5sST0Y0XFH4Y4XPT3p1Y1XS2Y2XS3Y3XURG2r3I0J11I3J2SR0sSP3s0I0J2H3W1001111I1J2H3W1001112I3J2H3W100111";
                break;
        }

        

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
        playerPathLinesOnScreen = new List<GameObject>();
        playerPathDotsOnScreen = new List<GameObject>();

        playerIsActive = false;

       
    }

    void Update()
    {
#if UNITY_EDITOR
        if (!playerIsActive && PolePreferences.isFrozen == false)
        {

            foreach (GameObject point in myPole.GetComponent<Pole>().eltsManager.unsolvedElts)
            {
                if (point.GetComponent<PoleEltPoint>() != null)
                {
                    point.GetComponent<PoleEltPoint>().ShowNormalizedColor();
                }
                if (point.GetComponent<EltClrRing>() != null)
                {
                    point.GetComponent<EltClrRing>().ShowNormalizedColor();
                }
                if (point.GetComponent<PoleEltShape>() != null)
                {
                    point.GetComponent<PoleEltShape>().ShowNormalizedColor();
                }
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
                if (myPole.GetComponent<Pole>().eltsManager.CheckSolution(myPole.GetComponent<Pole>().poleDots[0][0].GetComponent<PoleDot>().right.GetComponent<PoleLine>().down))
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
                        if (point.GetComponent<PoleEltPoint>() != null)
                        {
                            point.GetComponent<PoleEltPoint>().ShowUnsolvedColor();
                        }
                        if (point.GetComponent<EltClrRing>() != null)
                        {
                            point.GetComponent<EltClrRing>().ShowUnsolvedColor();
                        }
                        if (point.GetComponent<PoleEltShape>() != null)
                        {
                            point.GetComponent<PoleEltShape>().ShowUnsolvedColor();
                        }
                    }
                }
            }
            else foreach (GameObject path in GameObject.FindGameObjectsWithTag("Path"))
                {
                    path.GetComponent<Renderer>().material.Lerp(path.GetComponent<Renderer>().material, PlayerWrongPathMaterial, 1f);
                }
            gentimewin.text = myPole.GetComponent<Pole>().PathToStr();
            MenuManager.DebugMessage.savePath(gentimewin.text);
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
                if (point.GetComponent<PoleEltPoint>() != null)
                {
                    point.GetComponent<PoleEltPoint>().ShowNormalizedColor();
                }
                if (point.GetComponent<EltClrRing>() != null)
                {
                    point.GetComponent<EltClrRing>().ShowNormalizedColor();
                }
                if (point.GetComponent<PoleEltShape>() != null)
                {
                    point.GetComponent<PoleEltShape>().ShowNormalizedColor();
                }
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
                if (myPole.GetComponent<Pole>().eltsManager.CheckSolution(myPole.GetComponent<Pole>().poleDots[0][0].GetComponent<PoleDot>().right.GetComponent<PoleLine>().down))
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
                        if (point.GetComponent<PoleEltPoint>() != null)
                        {
                            point.GetComponent<PoleEltPoint>().ShowUnsolvedColor();
                        }
                        if (point.GetComponent<EltClrRing>() != null)
                        {
                            point.GetComponent<EltClrRing>().ShowUnsolvedColor();
                        }
                        if (point.GetComponent<PoleEltShape>() != null)
                        {
                            point.GetComponent<PoleEltShape>().ShowUnsolvedColor();
                        }
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
