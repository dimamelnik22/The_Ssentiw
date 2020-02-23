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
    public int glob;

    [Header("Prefabs")]
    public GameObject PolePF;
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

    [HideInInspector]
    public bool pathIsShown = false;
    [HideInInspector]
    public GameObject activePath;
    [HideInInspector]
    public GameObject mirrorPath;

    private GameObject activePole;
    private List<GameObject> playerPathLinesOnScreen;
    private List<GameObject> playerPathDotsOnScreen;

    //need to change later
    public static class PolePreferences
    {
        
        public static int height = 5;
        public static int width = 6;
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
    public static List<string> LevelList = new List<string>();
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
            SceneManager.LoadScene("PoleLevel");
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
            Debug.Log(activePole.GetComponent<Pole>().eltsManager.points.Count); 
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
                str += Color2HEX((int)(ring.c.color.r * 255));
                str += Color2HEX((int)(ring.c.color.g * 255));
                str += Color2HEX((int)(ring.c.color.b * 255));
                str += Color2HEX((int)(ring.c.color.a * 255));
            }
            str += "*";
        }
        if (GameObject.FindGameObjectsWithTag("EltShape").Length != 0)
        {
            str += "t";
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
        bool CutFlag = false;
        foreach (GameObject s in activePole.GetComponent<Pole>().poleLines)
        {
            if(s.GetComponent<PoleLine>().cut)
            {
                CutFlag = true;
                break;
            }
        }
        if (CutFlag)
        {
            str += "l";
            foreach (GameObject l in activePole.GetComponent<Pole>().poleLines)
            {
                if (l.GetComponent<PoleLine>().cut)
                {
                    if (l.GetComponent<PoleLine>().isHorizontal)
                    {
                        str += l.GetComponent<PoleLine>().left.GetComponent<PoleDot>().posX;
                        str += l.GetComponent<PoleLine>().left.GetComponent<PoleDot>().posY;
                        str += 0;
                    }
                    else
                    {
                        str += l.GetComponent<PoleLine>().up.GetComponent<PoleDot>().posX;
                        str += l.GetComponent<PoleLine>().up.GetComponent<PoleDot>().posY;
                        str += 1;
                    }
                }
            }
            str += "*";
        }
        GUIUtility.systemCopyBuffer = str;
        Debug.Log(str);

        File.AppendAllText("Assets/Resources/LvlsShapesSum.txt", str + Environment.NewLine);

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
    public void ButtonOpenEditor()
    {
        Debug.Log("editor");
        SceneManager.LoadScene("LevelEditor");
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
            if (Core.LevelList.IndexOf(Core.PolePreferences.info) < Core.LevelList.Count - 1)
            {
                Core.PolePreferences.info = Core.LevelList[Core.LevelList.IndexOf(Core.PolePreferences.info) + 1];
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
            else
            {
                PlayerPrefs.SetInt("IntroSkip", 1);
                ButtonMenu();
            }
        }
        else if (SceneManager.GetActiveScene().name == "PoleLevel")
        {
            Core.PolePreferences.MyRandom.SetSeed(Core.PolePreferences.MyRandom.GetRandom());
            MenuManager.DebugMessage.SaveSeed(Core.PolePreferences.MyRandom.seed);
            SceneManager.LoadScene("PoleLevel");
        }
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
            //foreach (GameObject dot in activePole.GetComponent<Pole>().systemPath.dots)
            //{
            //    if (dot == activePole.GetComponent<Pole>().starts[0]) Instantiate(PathStartPF, dot.transform);
            //    else Instantiate(PathDotPF, dot.transform);
            //}
            //foreach (GameObject line in activePole.GetComponent<Pole>().systemPath.lines)
            //{
            //    if (line.GetComponent<PoleLine>().isHorizontal) Instantiate(PathHorizontalLinePF, line.transform);
            //    else Instantiate(PathVerticalLinePF, line.transform);
            //}
            foreach (GameObject dot in GameObject.FindGameObjectsWithTag("PoleDot"))
            {
                if (dot.GetComponent<PoleDot>().isUsedBySolution)
                {
                    foreach (var f in activePole.GetComponent<Pole>().finishes)
                    {
                        if (dot == f)
                        {
                            int i = 0;
                            if (f.GetComponent<PoleDot>().AllowedToDown()  && f.GetComponent<PoleDot>().down.GetComponent<PoleLine>().isUsedBySolution) ++i;
                            if (f.GetComponent<PoleDot>().AllowedToUp()    && f.GetComponent<PoleDot>().up.GetComponent<PoleLine>().isUsedBySolution) ++i;
                            if (f.GetComponent<PoleDot>().AllowedToLeft()  && f.GetComponent<PoleDot>().left.GetComponent<PoleLine>().isUsedBySolution) ++i;
                            if (f.GetComponent<PoleDot>().AllowedToRight() && f.GetComponent<PoleDot>().right.GetComponent<PoleLine>().isUsedBySolution) ++i;
                            if(i == 1)Instantiate(PathFinishPF, activePole.GetComponent<Pole>().finishes[0].transform);
                        }
                    }
                    if (dot == activePole.GetComponent<Pole>().starts[0])
                    {
                        Instantiate(PathStartPF, dot.transform);
                    }
                    else Instantiate(PathDotPF, dot.transform);
                }
            }
            foreach (GameObject line in activePole.GetComponent<Pole>().poleLines)
            {
                if (line.GetComponent<PoleLine>().isUsedBySolution)
                {
                    if (line.GetComponent<PoleLine>().isHorizontal) Instantiate(PathHorizontalLinePF, line.transform);
                    else Instantiate(PathVerticalLinePF, line.transform);
                }
            }
            //do something to find real finish
            //Instantiate(PathFinishPF, activePole.GetComponent<Pole>().finishes[0].transform);
        }
        pathIsShown = !pathIsShown;
    }
    

    //check update
    void Start() {
         
        //QualitySettings.vSyncCount = 0;
        //Application.targetFrameRate = 10;
        int height = PolePreferences.height;
        int width = PolePreferences.width;
        activePole = Instantiate(PolePF);
        switch (SceneManager.GetActiveScene().name)
        {
            case "PoleLevel":
                if (PolePreferences.mode == "custom")
                {
                    activePole.GetComponent<Pole>().Custom(Core.PolePreferences.info);
                    foreach (GameObject start in activePole.GetComponent<Pole>().starts)
                        activePole.GetComponent<Pole>().StartScaling(start);
                }
                else
                {
                    activePole.GetComponent<Pole>().Init(height, width);
                    // START and FINISH creating
                    var borderDots = new List<GameObject>();
                    foreach (GameObject curdot in GameObject.FindGameObjectsWithTag("PoleDot"))
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

                    //dot = borderDots[Core.PolePreferences.MyRandom.GetRandom() % borderDots.Count];
                    //activePole.GetComponent<Pole>().AddStart(dot);
                    //borderDots.Remove(dot);
                    //dot = borderDots[Core.PolePreferences.MyRandom.GetRandom() % borderDots.Count];
                    //activePole.GetComponent<Pole>().AddFinish(dot);
                    //borderDots.Remove(dot);
                    //for (int a = 0; a < 3; ++a)
                    //{
                    //    activePole.GetComponent<Pole>().poleLines[PolePreferences.MyRandom.GetRandom() % activePole.GetComponent<Pole>().poleLines.Count].GetComponent<PoleLine>().cut = true;
                    //}
                    activePole.GetComponent<Pole>().GenerateShapes(Core.PolePreferences.numOfShapes);
                    activePole.GetComponent<Pole>().SetClrRing(activePole.GetComponent<Pole>().quantityColor, activePole.GetComponent<Pole>().quantityRing);
                    activePole.GetComponent<Pole>().GeneratePoints(PolePreferences.numOfPoints);
                    foreach (GameObject start in activePole.GetComponent<Pole>().starts)
                        activePole.GetComponent<Pole>().StartScaling(start);
                }
                break;
            case "Introduction":
				if (PlayerPrefs.GetInt("IntroSkip") > 0)
                {
                    SceneManager.LoadScene("MainMenu");
                }
                if (Core.PolePreferences.info == "")
                {
                    MenuManager.ParseLevels("LvlsPoints");
                    Core.PolePreferences.info = Core.LevelList[0];
                    GameObject.FindGameObjectWithTag("Canvas").GetComponent<CanvasScript>().ShowText();
                }
                activePole.GetComponent<Pole>().Custom(Core.PolePreferences.info);
                break;
            case "Test":
                activePole.GetComponent<Pole>().Init(height, width);
                activePole.GetComponent<Pole>().AddStart(activePole.GetComponent<Pole>().poleDots[0][0]);
                activePole.GetComponent<Pole>().AddStart(activePole.GetComponent<Pole>().poleDots[height - 1][width - 1]);
                activePole.GetComponent<Pole>().AddFinish(activePole.GetComponent<Pole>().poleDots[0][width - 1]);
                activePole.GetComponent<Pole>().AddFinish(activePole.GetComponent<Pole>().poleDots[height - 1][0]);
                activePath = Instantiate(ActivePathPF);
                activePath.GetComponent<ActivePath>().InitWithClone(activePole, activePole.GetComponent<Pole>().starts, activePole.GetComponent<Pole>().finishes);
                
                break;
        }

        //55s0444*f0040*p032312*t022111312111*

        foreach (GameObject start in activePole.GetComponent<Pole>().starts)
            activePole.GetComponent<Pole>().StartScaling(start);
        pathIsShown = false;
        playerPathLinesOnScreen = new List<GameObject>();
        playerPathDotsOnScreen = new List<GameObject>();
        if (activePath == null)
        {
            activePath = Instantiate(ActivePathPF);
            //activePath.GetComponent<ActivePath>().Init(activePole, activePole.GetComponent<Pole>().starts, activePole.GetComponent<Pole>().finishes);
            activePath.GetComponent<ActivePath>().InitWithClone(activePole, activePole.GetComponent<Pole>().starts, activePole.GetComponent<Pole>().finishes);

        }

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
            activePath.GetComponent<ActivePath>().MarkPath();

            if (activePole.GetComponent<Pole>().eltsManager.CheckSolution(activePole.GetComponent<Pole>().poleDots[0][0].GetComponent<PoleDot>().right.GetComponent<PoleLine>().down))
            {
                foreach (GameObject path in GameObject.FindGameObjectsWithTag("Path"))
                {
                    //path.GetComponent<Renderer>().material.Lerp(path.GetComponent<Renderer>().material, PlayerGoodPathMaterial, 1f);
                    path.GetComponent<Renderer>().material = PlayerGoodPathMaterial;
                    if (SceneManager.GetActiveScene().name == "Introduction")
                        GameObject.FindGameObjectWithTag("Canvas").GetComponent<CanvasScript>().ShowNextButton();
                }
            }
            else
            {
                foreach (GameObject path in GameObject.FindGameObjectsWithTag("Path"))
                {
                    //path.GetComponent<Renderer>().material.Lerp(path.GetComponent<Renderer>().material, PlayerWrongPathMaterial, 1f);
                    path.GetComponent<Renderer>().material = PlayerWrongPathMaterial;
                    path.GetComponent<Path>().fade = true;
                }
                foreach (Elements point in activePole.GetComponent<Pole>().eltsManager.unsolvedElts)
                {
                    point.ShowUnsolvedColor();
                }
            }

            MenuManager.DebugMessage.SavePath(activePole.GetComponent<Pole>().PathToStr(activePath.GetComponent<ActivePath>().dotsOnPole[0]));
            if (SceneManager.GetActiveScene().name == "Introduction" && Core.LevelList.IndexOf(Core.PolePreferences.info) == 3 && MenuManager.DebugMessage.path == "SDRDRURDDLDRRF")
            {
                GameObject.FindGameObjectWithTag("Canvas").GetComponent<CanvasScript>().ShowEditorButton();
            }


            
            
            
        }


    }

}
