using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class Core : MonoBehaviour {
    public void move()
    {
        activePath.GetComponent<ActivePath>().move();
    }

    public InputField CustomPuzzle;


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
    public bool pathIsShown = false;
    public bool playerIsActive = false;
    public GameObject myPole; 
    public GameObject activePath;

    public static class PolePreferences
    {
        
        public static int poleSize = 9;
        public static int complexity = 40;
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
    public void ButtonLoadPazzl()
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
    private string color2HEX(int col)
    {
        string s = Convert.ToString((int)col, 16);
        if (s.Length == 1)
        {
            s = "0" + s;
        }
        return s;
    }
    public void ButtonSavePazzl()
    {
        string str = "";
        str += myPole.GetComponent<Pole>().eltsManager.height;
        str += myPole.GetComponent<Pole>().eltsManager.width;
        str += "s";
        foreach (var start in myPole.GetComponent<Pole>().starts)
        {
            str += start.GetComponent<PoleDot>().posX;
            str += start.GetComponent<PoleDot>().posY;
        }
        str += "*";
        str += "f";
        foreach (var finish in myPole.GetComponent<Pole>().finishes)
        {
            str += finish.GetComponent<PoleDot>().posX;
            str += finish.GetComponent<PoleDot>().posY;
        }
        str += "*";
        str += "p";
        foreach(PoleEltPoint point in myPole.GetComponent<Pole>().eltsManager.points)
        {
            str += point.x;
            str += point.y;
            switch ((point.down ? 1 : 0) + 2 * (point.right ? 1 : 0))
            {
                case 0:
                    str += 0;
                    break;
                case 1:
                    str += 1;
                    break;
                case 2:
                    str += 2;
                    break;
            }

            /*Debug.Log(point.x);
            Debug.Log(point.y);
            Debug.Log(point.down);
            Debug.Log(point.right);*/
        }
        str += "*";
        str += "r";
        foreach (var ring in myPole.GetComponent<Pole>().eltsManager.clrRing)
        {
            str += ring.x;// rework points not set
            str += ring.y;
            str += color2HEX((int)ring.c.a * 255);
            str += color2HEX((int)ring.c.r * 255);
            str += color2HEX((int)ring.c.g * 255);
            str += color2HEX((int)ring.c.b * 255);
        }
        str += "*";
        str += "T";
        foreach (GameObject s in GameObject.FindGameObjectsWithTag("EltShape"))
        {
            PoleEltShape shape = s.GetComponent<PoleEltShape>(); 
            str += shape.x;
            str += shape.y;
            str += shape.boolList.Count;
            str += shape.boolList[0].Count;
            int k = 16;
            int len = 1;
            while(Math.Pow(2, shape.boolList[0].Count) > k)
            {
                k *= 16;
            }
            for (int i = 0; i < shape.boolList.Count; ++i)
            {
                string bit = "";
                for (int j = 0; j < shape.boolList[0].Count; ++j)
                {
                    bit += shape.boolList[i][j] ? 1 : 0;
                }
                long intValue = long.Parse(bit, System.Globalization.NumberStyles.HexNumber);
                string t = Convert.ToString(intValue, 16);
                while (t.Length < len);
                {
                    t = "0" + t;
                }
                str += t;
            }
            
        }
        str += "*";
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

        /*foreach (PoleEltPoint ring in myPole.GetComponent<Pole>().eltsManager.clrRing)
        {
            Debug.Log(ring.x);
            Debug.Log(ring.y);
        }*/
        /*foreach (GameObject sq in GameObject.FindGameObjectsWithTag("PoleSquere"))
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
        MenuManager.DebugMessage.push2Buffer();
    }
    public void ButtonMenu()
    {
        SceneManager.LoadScene("MainMenu");
        MenuManager.DebugMessage.clear();
    }
    
    public void NextButton()
    {
        if (Core.PolePreferences.mode =="info")
        {
            if (MenuManager.MainSettings.levels.IndexOf(Core.PolePreferences.info) < MenuManager.MainSettings.levels.Count - 1)
                Core.PolePreferences.info = MenuManager.MainSettings.levels[MenuManager.MainSettings.levels.IndexOf(Core.PolePreferences.info) + 1];
            else Core.PolePreferences.info = MenuManager.MainSettings.levels[0];
        }

        Core.PolePreferences.MyRandom.SetSeed(Core.PolePreferences.MyRandom.GetRandom());
        MenuManager.DebugMessage.saveSeed(Core.PolePreferences.MyRandom.seed);
        SceneManager.LoadScene("PoleLevel");
    }

    public void ButtonShowSolution()
    {
        if (Core.PolePreferences.mode == "info") return;
        foreach (var point in myPole.GetComponent<Pole>().eltsManager.unsolvedElts)
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
        foreach (GameObject gameObject in GameObject.FindGameObjectsWithTag("Path"))
            Destroy(gameObject);
        if (!pathIsShown)
        {
            foreach (GameObject dot in myPole.GetComponent<Pole>().systemPath.dots)
            {
                if (dot == myPole.GetComponent<Pole>().starts[0]) Instantiate(PathStartPrefab, dot.transform.position + pathstepz, PathStartPrefab.transform.rotation);
                else Instantiate(PathDotPrefab, dot.transform.position + pathstepz, PathDotPrefab.transform.rotation);
            }
            foreach (GameObject line in myPole.GetComponent<Pole>().systemPath.lines)
            {
                if (line.GetComponent<PoleLine>().isHorizontal) Instantiate(PathHorizontalLinePrefab, line.transform.position + pathstepz, PathHorizontalLinePrefab.transform.rotation);
                else Instantiate(PathVerticalLinePrefab, line.transform.position + pathstepz, PathVerticalLinePrefab.transform.rotation);
            }
            
            Instantiate(PathFinishPrefab, transform.position + stepx * myPole.GetComponent<Pole>().finishes[0].GetComponent<PoleDot>().posX + stepy * myPole.GetComponent<Pole>().finishes[0].GetComponent<PoleDot>().posY + pathstepz, PathFinishPrefab.transform.rotation);
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
                myPole.GetComponent<Pole>().Init(PolePreferences.poleSize, PolePreferences.poleSize);
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
                myPole.GetComponent<Pole>().AddStart(x, y);
                
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
                } while (myPole.GetComponent<Pole>().starts.Contains(myPole.GetComponent<Pole>().poleDots[y][x]) || myPole.GetComponent<Pole>().finishes.Contains(myPole.GetComponent<Pole>().poleDots[y][x]));
                myPole.GetComponent<Pole>().AddFinish(x, y);
                finishes = myPole.GetComponent<Pole>().finishes;
                
                myPole.GetComponent<Pole>().CreateSolution();
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
                } while (myPole.GetComponent<Pole>().starts.Contains(myPole.GetComponent<Pole>().poleDots[y][x]) || myPole.GetComponent<Pole>().finishes.Contains(myPole.GetComponent<Pole>().poleDots[y][x]));
                myPole.GetComponent<Pole>().AddStart(x, y);
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
                } while (myPole.GetComponent<Pole>().starts.Contains(myPole.GetComponent<Pole>().poleDots[y][x]) || myPole.GetComponent<Pole>().finishes.Contains(myPole.GetComponent<Pole>().poleDots[y][x]));
                myPole.GetComponent<Pole>().AddFinish(x, y);
                foreach (GameObject start in myPole.GetComponent<Pole>().starts)
                    myPole.GetComponent<Pole>().StartScaling(start);
                myPole.GetComponent<Pole>().GenerateShapes(Core.PolePreferences.numOfShapes);
                //myPole.GetComponent<Pole>().GenerateShapes(100);
                myPole.GetComponent<Pole>().SetClrRing(myPole.GetComponent<Pole>().quantityColor, myPole.GetComponent<Pole>().quantityRing);
                myPole.GetComponent<Pole>().GeneratePoints(PolePreferences.numOfPoints);
                //gentimewin.text = (Time.realtimeSinceStartup - gentime).ToString();
                gentime = Time.realtimeSinceStartup;
                break;
            case "info":
                myPole.GetComponent<Pole>().InitStr(Core.PolePreferences.info);
                //gentimewin.text = Core.PolePreferences.info;
                break;
            case "custom":
                myPole.GetComponent<Pole>().custom(Core.PolePreferences.info);
                //gentimewin.text = Core.PolePreferences.info;
                break;
        }

        
        //rework
        for (int i = 0; i < myPole.GetComponent<Pole>().height; i++)
        {
            for (int j = 0; j < myPole.GetComponent<Pole>().width; j++)
            {
                if (myPole.GetComponent<Pole>().poleDots[i][j].GetComponent<PoleDot>().hasPoint)
                {
                    myPole.GetComponent<Pole>().poleDots[i][j].GetComponent<PoleDot>().point = Instantiate(PointPrefab, transform.position + stepx * j + stepy * i + pathstepz, PointPrefab.transform.rotation).GetComponent<Elements>();
                    myPole.GetComponent<Pole>().poleDots[i][j].GetComponent<PoleDot>().point.GetComponent<PoleEltPoint>().SetDot(myPole.GetComponent<Pole>().poleDots[i][j]);
                    myPole.GetComponent<Pole>().eltsManager.points.Add(myPole.GetComponent<Pole>().poleDots[i][j].GetComponent<PoleDot>().point);

                    myPole.GetComponent<Pole>().poleDots[i][j].GetComponent<PoleDot>().point.c = new Color(45 / 255, 104 / 255, 1);
                    myPole.GetComponent<Pole>().poleDots[i][j].GetComponent<PoleDot>().point.GetComponent<Renderer>().material.color = new Color(45 / 255, 104 / 255, 1);
                }
                if (j < myPole.GetComponent<Pole>().width - 1)
                {
                    if (myPole.GetComponent<Pole>().poleDots[i][j].GetComponent<PoleDot>().right != null)
                    {
                        if (myPole.GetComponent<Pole>().poleDots[i][j].GetComponent<PoleDot>().right.GetComponent<PoleLine>().hasPoint)
                        {
                            myPole.GetComponent<Pole>().poleDots[i][j].GetComponent<PoleDot>().right.GetComponent<PoleLine>().point = Instantiate(PointPrefab, transform.position + stepx * 0.5f + stepx * j + stepy * i + pathstepz, PointPrefab.transform.rotation).GetComponent<Elements>();
                            myPole.GetComponent<Pole>().poleDots[i][j].GetComponent<PoleDot>().right.GetComponent<PoleLine>().point.GetComponent<PoleEltPoint>().SetLine(myPole.GetComponent<Pole>().poleDots[i][j].GetComponent<PoleDot>().right);
                            myPole.GetComponent<Pole>().eltsManager.points.Add(myPole.GetComponent<Pole>().poleDots[i][j].GetComponent<PoleDot>().right.GetComponent<PoleLine>().point);
                            myPole.GetComponent<Pole>().poleDots[i][j].GetComponent<PoleDot>().right.GetComponent<PoleLine>().point.c = new Color(45 / 255, 104 / 255, 1);
                            myPole.GetComponent<Pole>().poleDots[i][j].GetComponent<PoleDot>().right.GetComponent<PoleLine>().point.GetComponent<Renderer>().material.color = new Color(45 / 255, 104 / 255, 1);
                        }
                    }
                }
                if (i < myPole.GetComponent<Pole>().height - 1)
                {
                    if (myPole.GetComponent<Pole>().poleDots[i][j].GetComponent<PoleDot>().down != null)
                    {
                        if (myPole.GetComponent<Pole>().poleDots[i][j].GetComponent<PoleDot>().down.GetComponent<PoleLine>().hasPoint)
                        {
                            myPole.GetComponent<Pole>().poleDots[i][j].GetComponent<PoleDot>().down.GetComponent<PoleLine>().point = Instantiate(PointPrefab, transform.position + stepy * 0.5f + stepx * j + stepy * i + pathstepz, PointPrefab.transform.rotation).GetComponent<Elements>();
                            myPole.GetComponent<Pole>().poleDots[i][j].GetComponent<PoleDot>().down.GetComponent<PoleLine>().point.GetComponent<PoleEltPoint>().SetLine(myPole.GetComponent<Pole>().poleDots[i][j].GetComponent<PoleDot>().down);
                            myPole.GetComponent<Pole>().eltsManager.points.Add(myPole.GetComponent<Pole>().poleDots[i][j].GetComponent<PoleDot>().down.GetComponent<PoleLine>().point);
                            myPole.GetComponent<Pole>().poleDots[i][j].GetComponent<PoleDot>().down.GetComponent<PoleLine>().point.c = new Color(45 / 255, 104 / 255, 1);
                            myPole.GetComponent<Pole>().poleDots[i][j].GetComponent<PoleDot>().down.GetComponent<PoleLine>().point.GetComponent<Renderer>().material.color = new Color(45 / 255, 104 / 255, 1);

                        }
                    }
                }
            }
        }
        //finishes.Add(myPole.GetComponent<Pole>().finish);
        pathIsShown = false;
        playerPathLinesOnScreen = new List<GameObject>();
        playerPathDotsOnScreen = new List<GameObject>();

        playerIsActive = false;
        activePath = Instantiate(ActivePathPF);


        activePath.GetComponent<ActivePath>().Init(myPole, myPole.GetComponent<Pole>().starts, finishes);

    }

    void Update()
    {
        
        if (activePath == null)
        {
            activePath = Instantiate(ActivePathPF);
            activePath.GetComponent<ActivePath>().Init(myPole, myPole.GetComponent<Pole>().starts, finishes);
        }
#if UNITY_EDITOR
        if (activePath.GetComponent<ActivePath>().isFinished && !Input.GetMouseButton(0) && activePath.GetComponent<ActivePath>().pointer.activeSelf)
#else
        if (activePath.GetComponent<ActivePath>().isFinished && Input.touchCount == 0 && activePath.GetComponent<ActivePath>().pointer.activeSelf)
#endif
        {
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
                foreach (Elements point in myPole.GetComponent<Pole>().eltsManager.unsolvedElts)
                {
                    point.ShowUnsolvedColor();
                }
            }

            MenuManager.DebugMessage.savePath(myPole.GetComponent<Pole>().PathToStr(activePath.GetComponent<ActivePath>().dotsOnPole[0]));
            foreach (GameObject dot in myPole.GetComponent<Pole>().playerPath.dots)
                dot.GetComponent<PoleDot>().isUsedByPlayer = false;
            foreach (GameObject line in myPole.GetComponent<Pole>().playerPath.lines)
                line.GetComponent<PoleLine>().isUsedByPlayer = false;
            activePath.GetComponent<ActivePath>().pointer.SetActive(false);
            
        }


    }

}
