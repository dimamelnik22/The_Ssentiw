using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class Core : MonoBehaviour {

    [Header("Input Field")]
    public InputField CustomPuzzle;

    [Header("Prefabs")]
    public GameObject PolePF;
    public GameObject PointPF;
    public GameObject PathDotPF;
    public GameObject PathVerticalLinePF;
    public GameObject PathHorizontalLinePF;
    public GameObject PathStartPF;
    public GameObject PathFinishPF;
    public GameObject ActivePathPF;

    [Header("Materials")]
    public Material PlayerWrongPathMaterial;
    public Material PlayerGoodPathMaterial;
    public Material EltPointMaterial;
    public Material EltWrongPointMaterial;


    //private static Vector3 pathstepz = new Vector3(0f, 0f, -0.5f);

    //private static Vector3 stepx = new Vector3(5f, 0f, 0f);
    //private static Vector3 stepy = new Vector3(0f, -5f, 0f);

    [HideInInspector]
    public bool pathIsShown = false;
    [HideInInspector]
    public GameObject activePath;

    private GameObject activePole;
    private List<GameObject> playerPathLinesOnScreen;
    private List<GameObject> playerPathDotsOnScreen;

    //need to change later
    public static class PolePreferences
    {
        
        public static int poleSize = 6;
        public static int complexity = 20;
        public static int numOfPoints = 7;
        public static int numOfCircles = 10;
        public static int numOfStars = 5;
        public static int numOfShapes = 20;
        public static System.Random r = new System.Random();
        public static class MyRandom
        {
            public static int seed = 4323;
            public static void SetSeed(int s = 0)
            {
                seed = s;
                r = s== 0 ? new System.Random():new System.Random(seed);
            }
            public static int GetRandom()
            {
                //return seed;

                return r.Next();
            }
        }
        public static string info = "";
        public static string mode = "normal";
    }
    public void ButtonComplexity()
    {
        Complexity.countComplexity(activePole.GetComponent<Pole>().poleDots, activePole.GetComponent<Pole>());
    }
    //????
    public void ButtonLoadPuzzle()
    {
        if (CustomPuzzle.text == "")
            Debug.Log("pusto");
        else
        {
            Core.PolePreferences.mode = "custom";
            Core.PolePreferences.info = CustomPuzzle.text;
            Debug.Log(Core.PolePreferences.info);
            activePole.GetComponent<Pole>().ClearPole();
            foreach (GameObject point in GameObject.FindGameObjectsWithTag("EltPoint"))
                Destroy(point);
            foreach (GameObject shape in GameObject.FindGameObjectsWithTag("EltShape"))
                Destroy(shape);
            foreach (GameObject clrRing in GameObject.FindGameObjectsWithTag("EltClrRing"))
                Destroy(clrRing);
            Destroy(activePole);
            activePole = Instantiate(PolePF);
            activePole.GetComponent<Pole>().Custom(Core.PolePreferences.info);
            activePole.GetComponent<Pole>().poleDots[0][0].GetComponent<PoleDot>().CreateDot();
            activePole.GetComponent<Pole>().StartScaling(activePole.GetComponent<Pole>().poleDots[0][0]);
        }
    }
    //????
    private string Color2HEX(int col)
    {
        string s = Convert.ToString((int)col, 16);
        if (s.Length == 1)
        {
            s = "0" + s;
        }
        return s;
    }
    //need update
    public void SavePuzzle(GameObject pole)
    {
        activePole = pole;
        ButtonSavePazzl();
    }
    public void ButtonSavePazzl()
    {
        string str = "";
        str += activePole.GetComponent<Pole>().eltsManager.height;
        str += activePole.GetComponent<Pole>().eltsManager.width;
        str += "s";
        foreach (var start in activePole.GetComponent<Pole>().starts)
        {
            str += start.GetComponent<PoleDot>().posX;
            str += start.GetComponent<PoleDot>().posY;
        }
        str += "*";
        str += "f";
        foreach (var finish in activePole.GetComponent<Pole>().finishes)
        {
            str += finish.GetComponent<PoleDot>().posX;
            str += finish.GetComponent<PoleDot>().posY;
        }
        str += "*";
        if (activePole.GetComponent<Pole>().eltsManager.points.Count != 0)
        {
            str += "p";
            foreach (PoleEltPoint point in activePole.GetComponent<Pole>().eltsManager.points)
            {
                str += point.GetX();
                str += point.GetY();
                if (point.location.GetComponent<PoleDot>())
                {
                    str += 0;
                }
                else
                {
                    if (point.location.GetComponent<PoleLine>().isHorizontal)
                    {
                        str += 2;
                    }
                    else
                    {
                        str += 1;
                    }
                }

                /*Debug.Log(point.x); 
                Debug.Log(point.y); 
                Debug.Log(point.down); 
                Debug.Log(point.right);*/
            }
            str += "*";
        }
        if (activePole.GetComponent<Pole>().eltsManager.clrRing.Count != 0)
        {
            str += "r";
            foreach (var ring in activePole.GetComponent<Pole>().eltsManager.clrRing)
            {
                str += ring.location.GetComponent<PoleSquare>().indexJ;// rework points not set 
                str += ring.location.GetComponent<PoleSquare>().indexI;
                str += Color2HEX((int)(ring.c.r * 255));
                str += Color2HEX((int)(ring.c.g * 255));
                str += Color2HEX((int)(ring.c.b * 255));
                str += Color2HEX((int)(ring.c.a * 255));
            }
            str += "*";
        }
        if (GameObject.FindGameObjectsWithTag("EltShape").Length != 0)
        {
            str += "T";
            foreach (GameObject s in GameObject.FindGameObjectsWithTag("EltShape"))
            {
                PoleEltShape shape = s.GetComponent<PoleEltShape>();
                str += shape.location.GetComponent<PoleSquare>().indexJ;
                str += shape.location.GetComponent<PoleSquare>().indexI;
                str += shape.boolList.Count;
                str += shape.boolList[0].Count;
                /*int k = 16; 
                int len = 1; 
                while (Math.Pow(2, shape.boolList[0].Count) > k) 
                { 
                    k *= 16; 
                }*/
                for (int i = 0; i < shape.boolList.Count; ++i)
                {
                    for (int j = 0; j < shape.boolList[0].Count; ++j)
                    {
                        str += shape.boolList[i][j] ? 1 : 0;
                        //bit += shape.boolList[i][j] ? 1 : 0; 
                    }
                    /*long intValue = long.Parse(bit, System.Globalization.NumberStyles.HexNumber); 
                    string t = Convert.ToString(intValue, 16); 
                    while (t.Length < len) ; 
                    { 
                        t = "0" + t; 
                    } 
                    str += t;*/
                }

            }
            str += "*";
        }
        GUIUtility.systemCopyBuffer = str;
        Debug.Log(str);
        /*using System; 
class Demo { 
static void Main() { 
  int val = 255*255; 
  val *= 255*255; 
  string hex = Convert.ToString(val, 16); 
  long intValue = long.Parse(hex, System.Globalization.NumberStyles.HexNumber); 
  Console.WriteLine("Integer: "+val); 
  Console.WriteLine("Hex String: "+hex); 
  Console.WriteLine("Integer: "+intValue); 
  hex = "00000000"; 
  intValue = long.Parse(hex, System.Globalization.NumberStyles.HexNumber); 
  Console.WriteLine("Hex String: "+hex); 
  Console.WriteLine("Integer: "+intValue); 
 
} 
}*/
        /*Debug.Log(ring.x); 
        Debug.Log(ring.y); 
        Debug.Log(ring.c.a * 255); 
        Debug.Log(ring.c.r * 255); 
        Debug.Log(ring.c.g * 255); 
        Debug.Log(ring.c.b * 255);*/

        /*foreach (PoleEltPoint ring in activePole.GetComponent<Pole>().eltsManager.clrRing) 
        { 
            Debug.Log(ring.x); 
            Debug.Log(ring.y); 
        }*/
        /*foreach (GameObject sq in GameObject.FindGameObjectsWithTag("PoleSquare")) 
        { 
            var now = sq.GetComponent<PoleSquare>(); 
            string cod = ""; 
            if (now.hasElem) 
            { 
                byte[] bytes; 
                //bytes = BitConverter.GetBytes(n.x); 
                //cod += BitConverter.ToString(bytes); 
                //bytes = BitConverter.GetBytes(n.y); 
                //cod += BitConverter.ToString(bytes); 
                //bytes = BitConverter.GetBytes(n.c.r); 
                //cod += BitConverter.ToString(bytes); 
                //bytes = BitConverter.GetBytes(n.c.g); 
                //cod += BitConverter.ToString(bytes); 
                //bytes = BitConverter.GetBytes(n.c.b); 
                //cod += BitConverter.ToString(bytes); 
                //bytes = BitConverter.GetBytes(n.Type); 
                //cod += BitConverter.ToString(bytes); 
                //bytes = BitConverter.GetBytes(n.rotate); 
                //cod += BitConverter.ToString(bytes); 
                Debug.Log(cod); 
                //if(now.element.) ;//!!! сделать все элементы наследованными от род класса 
 
            } 
        }*/
    }
    public void ButtonReport()
    {
        MenuManager.DebugMessage.Push2Buffer();
    }
    public void ButtonMenu()
    {
        SceneManager.LoadScene("MainMenu");
        MenuManager.DebugMessage.Clear();
    }
    
    public void ButtonNext()
    {
        if (SceneManager.GetActiveScene().name == "Introduction")
        {
            if (MenuManager.MainSettings.levels.IndexOf(Core.PolePreferences.info) < MenuManager.MainSettings.levels.Count - 1)
                Core.PolePreferences.info = MenuManager.MainSettings.levels[MenuManager.MainSettings.levels.IndexOf(Core.PolePreferences.info) + 1];
            else ButtonMenu();
        }

        Core.PolePreferences.MyRandom.SetSeed(Core.PolePreferences.MyRandom.GetRandom());
        MenuManager.DebugMessage.SaveSeed(Core.PolePreferences.MyRandom.seed);
        SceneManager.LoadScene("PoleLevel");
    }

    //need fix
    public void ButtonShowSolution()
    {
        if (Core.PolePreferences.mode == "info") return;
        //foreach (var point in activePole.GetComponent<Pole>().eltsManager.unsolvedElts)
        //{
        //    if (point.GetComponent<PoleEltPoint>() != null)
        //    {
        //        point.GetComponent<PoleEltPoint>().ShowNormalizedColor();
        //    }
        //    if (point.GetComponent<PoleEltClrRing>() != null)
        //    {
        //        point.GetComponent<PoleEltClrRing>().ShowNormalizedColor();
        //    }
        //    if (point.GetComponent<PoleEltShape>() != null)
        //    {
        //        point.GetComponent<PoleEltShape>().ShowNormalizedColor();
        //    }
        //}
        activePole.GetComponent<Pole>().NormalizeColors();
        foreach (GameObject dot in activePole.GetComponent<Pole>().playerPath.dots)
            dot.GetComponent<PoleDot>().isUsedByPlayer = false;
        foreach (GameObject line in activePole.GetComponent<Pole>().playerPath.lines)
            line.GetComponent<PoleLine>().isUsedByPlayer = false;
        //Destroy(GameObject.FindGameObjectWithTag("Player"));
        Destroy(activePath.GetComponent<ActivePath>().pointer);
        activePath.GetComponent<ActivePath>().isStarted = false;
        activePath.GetComponent<ActivePath>().isFinished = false;
        activePath.SetActive(!activePath.activeSelf);
        activePole.GetComponent<Pole>().playerPath.Clear();
        playerPathDotsOnScreen.Clear();
        playerPathLinesOnScreen.Clear();
        foreach (GameObject gameObject in GameObject.FindGameObjectsWithTag("Path"))
            Destroy(gameObject);
        if (!pathIsShown)
        {
            foreach (GameObject dot in activePole.GetComponent<Pole>().systemPath.dots)
            {
                if (dot == activePole.GetComponent<Pole>().starts[0]) Instantiate(PathStartPF, dot.transform);
                else Instantiate(PathDotPF, dot.transform);
            }
            foreach (GameObject line in activePole.GetComponent<Pole>().systemPath.lines)
            {
                if (line.GetComponent<PoleLine>().isHorizontal) Instantiate(PathHorizontalLinePF, line.transform);
                else Instantiate(PathVerticalLinePF, line.transform);
            }
            //do something to find real finish
            Instantiate(PathFinishPF, activePole.GetComponent<Pole>().finishes[0].transform);
        }
        pathIsShown = !pathIsShown;
    }

    //check update
    void Start() {
        Debug.Log("11");
        //QualitySettings.vSyncCount = 0;
        //Application.targetFrameRate = 10;
        int height = PolePreferences.poleSize;
        int width = PolePreferences.poleSize;
        activePole = Instantiate(PolePF);
        switch (SceneManager.GetActiveScene().name)
        {
            case "PoleLevel":
                Debug.Log("PoleLevel d");
                activePole.GetComponent<Pole>().Init(height, width);
                // START and FINISH creating
                var borderDots = new List<GameObject>();
                foreach(GameObject curdot in GameObject.FindGameObjectsWithTag("PoleDot"))
                {
                    var poledot = curdot.GetComponent<PoleDot>();
                    if (poledot.posX == 0 || poledot.posX == width - 1 || poledot.posY == 0 || poledot.posY == height - 1)
                        borderDots.Add(curdot);
                }
                GameObject dot = borderDots[Core.PolePreferences.MyRandom.GetRandom() % borderDots.Count];
                activePole.GetComponent<Pole>().AddStart(dot);
                borderDots.Remove(dot);
                dot = borderDots[Core.PolePreferences.MyRandom.GetRandom() % borderDots.Count];
                activePole.GetComponent<Pole>().AddFinish(dot);
                borderDots.Remove(dot);

                activePole.GetComponent<Pole>().CreateSolution();

                dot = borderDots[Core.PolePreferences.MyRandom.GetRandom() % borderDots.Count];
                activePole.GetComponent<Pole>().AddStart(dot);
                borderDots.Remove(dot);
                dot = borderDots[Core.PolePreferences.MyRandom.GetRandom() % borderDots.Count];
                activePole.GetComponent<Pole>().AddFinish(dot);
                borderDots.Remove(dot);
                //for (int a = 0; a < 3; ++a)
                //{
                //    activePole.GetComponent<Pole>().poleLines[PolePreferences.MyRandom.GetRandom() % activePole.GetComponent<Pole>().poleLines.Count].GetComponent<PoleLine>().cut = true;
                //}
                foreach (GameObject start in activePole.GetComponent<Pole>().starts)
                    activePole.GetComponent<Pole>().StartScaling(start);
                activePole.GetComponent<Pole>().GenerateShapes(Core.PolePreferences.numOfShapes);
                activePole.GetComponent<Pole>().SetClrRing(activePole.GetComponent<Pole>().quantityColor, activePole.GetComponent<Pole>().quantityRing);
                activePole.GetComponent<Pole>().GeneratePoints(PolePreferences.numOfPoints);
                break;
            case "Introduction":
                if (Core.PolePreferences.info == "")
                {
                    StreamReader sr = new StreamReader("Assets/Resources/introductionLevels.txt");
                    MenuManager.MainSettings.levels = new List<string>();
                    while (sr.Peek() >= 0)
                    {
                        MenuManager.MainSettings.levels.Add(sr.ReadLine());
                    }
                    Core.PolePreferences.info = MenuManager.MainSettings.levels[0];
                }
                activePole.GetComponent<Pole>().Custom(Core.PolePreferences.info);
                break;
        }

        
        //rework
        // update! moved to scaling in PoleDot and PoleLine (delete here if agreed)
        //
        //for (int i = 0; i < activePole.GetComponent<Pole>().height; i++)
        //{
        //    for (int j = 0; j < activePole.GetComponent<Pole>().width; j++)
        //    {
        //        if (activePole.GetComponent<Pole>().poleDots[i][j].GetComponent<PoleDot>().hasPoint)
        //        {
        //            activePole.GetComponent<Pole>().poleDots[i][j].GetComponent<PoleDot>().point = Instantiate(PointPF, transform.position + stepx * j + stepy * i + pathstepz, PointPF.transform.rotation).GetComponent<Elements>();
        //            activePole.GetComponent<Pole>().poleDots[i][j].GetComponent<PoleDot>().point.GetComponent<PoleEltPoint>().SetDot(activePole.GetComponent<Pole>().poleDots[i][j]);
        //            activePole.GetComponent<Pole>().eltsManager.points.Add(activePole.GetComponent<Pole>().poleDots[i][j].GetComponent<PoleDot>().point);

        //            activePole.GetComponent<Pole>().poleDots[i][j].GetComponent<PoleDot>().point.c = new Color(45 / 255, 104 / 255, 1);
        //            activePole.GetComponent<Pole>().poleDots[i][j].GetComponent<PoleDot>().point.GetComponent<Renderer>().material.color = new Color(45 / 255, 104 / 255, 1);
        //        }
        //        if (j < activePole.GetComponent<Pole>().width - 1)
        //        {
        //            if (activePole.GetComponent<Pole>().poleDots[i][j].GetComponent<PoleDot>().right != null)
        //            {
        //                if (activePole.GetComponent<Pole>().poleDots[i][j].GetComponent<PoleDot>().right.GetComponent<PoleLine>().hasPoint)
        //                {
        //                    activePole.GetComponent<Pole>().poleDots[i][j].GetComponent<PoleDot>().right.GetComponent<PoleLine>().point = Instantiate(PointPF, transform.position + stepx * 0.5f + stepx * j + stepy * i + pathstepz, PointPF.transform.rotation).GetComponent<Elements>();
        //                    activePole.GetComponent<Pole>().poleDots[i][j].GetComponent<PoleDot>().right.GetComponent<PoleLine>().point.GetComponent<PoleEltPoint>().SetLine(activePole.GetComponent<Pole>().poleDots[i][j].GetComponent<PoleDot>().right);
        //                    activePole.GetComponent<Pole>().eltsManager.points.Add(activePole.GetComponent<Pole>().poleDots[i][j].GetComponent<PoleDot>().right.GetComponent<PoleLine>().point);
        //                    activePole.GetComponent<Pole>().poleDots[i][j].GetComponent<PoleDot>().right.GetComponent<PoleLine>().point.c = new Color(45 / 255, 104 / 255, 1);
        //                    activePole.GetComponent<Pole>().poleDots[i][j].GetComponent<PoleDot>().right.GetComponent<PoleLine>().point.GetComponent<Renderer>().material.color = new Color(45 / 255, 104 / 255, 1);
        //                }
        //            }
        //        }
        //        if (i < activePole.GetComponent<Pole>().height - 1)
        //        {
        //            if (activePole.GetComponent<Pole>().poleDots[i][j].GetComponent<PoleDot>().down != null)
        //            {
        //                if (activePole.GetComponent<Pole>().poleDots[i][j].GetComponent<PoleDot>().down.GetComponent<PoleLine>().hasPoint)
        //                {
        //                    activePole.GetComponent<Pole>().poleDots[i][j].GetComponent<PoleDot>().down.GetComponent<PoleLine>().point = Instantiate(PointPF, transform.position + stepy * 0.5f + stepx * j + stepy * i + pathstepz, PointPF.transform.rotation).GetComponent<Elements>();
        //                    activePole.GetComponent<Pole>().poleDots[i][j].GetComponent<PoleDot>().down.GetComponent<PoleLine>().point.GetComponent<PoleEltPoint>().SetLine(activePole.GetComponent<Pole>().poleDots[i][j].GetComponent<PoleDot>().down);
        //                    activePole.GetComponent<Pole>().eltsManager.points.Add(activePole.GetComponent<Pole>().poleDots[i][j].GetComponent<PoleDot>().down.GetComponent<PoleLine>().point);
        //                    activePole.GetComponent<Pole>().poleDots[i][j].GetComponent<PoleDot>().down.GetComponent<PoleLine>().point.c = new Color(45 / 255, 104 / 255, 1);
        //                    activePole.GetComponent<Pole>().poleDots[i][j].GetComponent<PoleDot>().down.GetComponent<PoleLine>().point.GetComponent<Renderer>().material.color = new Color(45 / 255, 104 / 255, 1);

        //                }
        //            }
        //        }
        //    }
        //}

        pathIsShown = false;
        playerPathLinesOnScreen = new List<GameObject>();
        playerPathDotsOnScreen = new List<GameObject>();
        activePath = Instantiate(ActivePathPF);
        activePath.GetComponent<ActivePath>().Init(activePole, activePole.GetComponent<Pole>().starts, activePole.GetComponent<Pole>().finishes);

    }

    void Update()
    {
        
        //if (activePath == null)
        //{
        //    activePath = Instantiate(ActivePathPF);
        //    activePath.GetComponent<ActivePath>().Init(activePole, activePole.GetComponent<Pole>().starts, finishes);
        //}
#if UNITY_EDITOR
        if (activePath.GetComponent<ActivePath>().isFinished && !Input.GetMouseButton(0) && activePath.GetComponent<ActivePath>().pointer.activeSelf)
#else
        if (activePath.GetComponent<ActivePath>().isFinished && Input.touchCount == 0 && activePath.GetComponent<ActivePath>().pointer.activeSelf)
#endif
        {
            activePole.GetComponent<Pole>().playerPath.Clear();
            activePath.GetComponent<ActivePath>().EndSolution();
            foreach (GameObject dot in activePath.GetComponent<ActivePath>().dotsOnPole)
            {
                dot.GetComponent<PoleDot>().isUsedByPlayer = true;
                activePole.GetComponent<Pole>().playerPath.dots.Add(dot);
            }
            foreach (GameObject line in activePath.GetComponent<ActivePath>().linesOnPole)
            {
                line.GetComponent<PoleLine>().isUsedByPlayer = true;
                activePole.GetComponent<Pole>().playerPath.lines.Add(line);
            }

            if (activePole.GetComponent<Pole>().eltsManager.CheckSolution(activePole.GetComponent<Pole>().poleDots[0][0].GetComponent<PoleDot>().right.GetComponent<PoleLine>().down))
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
                foreach (Elements point in activePole.GetComponent<Pole>().eltsManager.unsolvedElts)
                {
                    point.ShowUnsolvedColor();
                }
            }

            MenuManager.DebugMessage.SavePath(activePole.GetComponent<Pole>().PathToStr(activePath.GetComponent<ActivePath>().dotsOnPole[0]));
            foreach (GameObject dot in activePole.GetComponent<Pole>().playerPath.dots)
                dot.GetComponent<PoleDot>().isUsedByPlayer = false;
            foreach (GameObject line in activePole.GetComponent<Pole>().playerPath.lines)
                line.GetComponent<PoleLine>().isUsedByPlayer = false;
            activePath.GetComponent<ActivePath>().pointer.SetActive(false);
            
        }


    }

}
