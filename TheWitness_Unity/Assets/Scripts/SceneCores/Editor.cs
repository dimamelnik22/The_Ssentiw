using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Editor : MonoBehaviour
{
    [Header("Info Field")]
    public Text SolveTimeTextField;
    private float solvetime = 0f;

    [Header("Input Fields")]
    public InputField heightInput;
    public InputField widthInput;

    [Header("Prefabs")]
    public GameObject PolePF;
    public GameObject ShapePF;
    public GameObject PathDotPF;
    public GameObject PathVerticalLinePF;
    public GameObject PathHorizontalLinePF;
    public GameObject PathStartPF;
    public GameObject PathFinishPF;

    [HideInInspector]
    private readonly List<Color> color = new List<Color>() { Color.cyan, Color.yellow, Color.green, Color.magenta, Color.blue };
    [HideInInspector]
    private int k = 0;
    private GameObject activePole;
    private List<List<bool>> boolList;
    private string element = "";
    private GameObject finish;

    //????
    public void ButtonSavePuzzle()
    {
        Core c = new Core();
        c.SavePuzzle(activePole);// fucking костыль
    }
    public void ButtonMenu()
    {
        SceneManager.LoadScene("MainMenu");
        MenuManager.DebugMessage.Clear();
    }
    public void ButtonHideSolution()
    {
        foreach (GameObject gameObject in GameObject.FindGameObjectsWithTag("Path"))
            Destroy(gameObject);
        foreach (GameObject dot in activePole.GetComponent<Pole>().systemPath.dots)
            dot.GetComponent<PoleDot>().isUsedByPlayer = false;
        foreach (GameObject line in activePole.GetComponent<Pole>().systemPath.lines)
            line.GetComponent<PoleLine>().isUsedByPlayer = false;
        activePole.GetComponent<Pole>().systemPath.Clear();
    }
    public void ButtonTrySolve()
    {
        if (activePole.GetComponent<Pole>().starts.Count > 0 && activePole.GetComponent<Pole>().finishes.Count > 0)
        {
            solvetime = Time.realtimeSinceStartup;
            foreach (GameObject start in activePole.GetComponent<Pole>().starts)
                if (TrySolve(start, activePole.GetComponent<Pole>().finishes, null))
                {
                    Debug.Log("success");

                    foreach (GameObject dot in activePole.GetComponent<Pole>().systemPath.dots)
                    {
                        if (dot == start) Instantiate(PathStartPF, dot.transform);
                        else if (dot == finish) Instantiate(PathFinishPF, dot.transform);
                        else Instantiate(PathDotPF, dot.transform);
                    }
                    foreach (GameObject line in activePole.GetComponent<Pole>().systemPath.lines)
                    {
                        if (line.GetComponent<PoleLine>().isHorizontal) Instantiate(PathHorizontalLinePF, line.transform);
                        else Instantiate(PathVerticalLinePF, line.transform);
                    }
                }
            SolveTimeTextField.text = (Time.realtimeSinceStartup - solvetime).ToString();
        }
    }
    public void ButtonResize()
    {
        ButtonHideSolution();
        HideEditButtons();
        activePole.GetComponent<Pole>().ClearPole();
        foreach (GameObject point in GameObject.FindGameObjectsWithTag("EltPoint"))
            Destroy(point);
        foreach (GameObject shape in GameObject.FindGameObjectsWithTag("EltShape"))
            Destroy(shape);
        foreach (GameObject clrRing in GameObject.FindGameObjectsWithTag("EltClrRing"))
            Destroy(clrRing);
        Destroy(activePole);
        activePole = Instantiate(PolePF);
        activePole.GetComponent<Pole>().Init(System.Int32.Parse(heightInput.text), System.Int32.Parse(widthInput.text));
        UpdateBoolList();
        activePole.GetComponent<Pole>().poleDots[0][0].GetComponent<PoleDot>().CreateDot();
        activePole.GetComponent<Pole>().StartScaling(activePole.GetComponent<Pole>().poleDots[0][0]);
    }
    public void ButtonAddStart()
    {
        HideEditButtons();
        ShowEditButtonsDots();
        element = "start";
    }
    public void ButtonAddFinish()
    {
        HideEditButtons();
        ShowEditButtonsDots();
        element = "finish";
    }
    public void ButtonAddPoint()
    {
        HideEditButtons();
        ShowEditButtonsDots();
        ShowEditButtonsLines();
        element = "point";
    }
    public void ButtonDelete()
    {
        HideEditButtons();
        foreach (GameObject dot in GameObject.FindGameObjectsWithTag("PoleDot"))
            if (dot.GetComponent<PoleDot>().hasPoint || dot.GetComponent<PoleDot>().startFinish != null)
                dot.GetComponent<PoleDot>().ShowEditButton();
        foreach (GameObject line in GameObject.FindGameObjectsWithTag("PoleLine"))
            if (line.GetComponent<PoleLine>().hasPoint)
                line.GetComponent<PoleLine>().ShowEditButton();
        foreach (GameObject square in GameObject.FindGameObjectsWithTag("PoleSquare"))
            if (square.GetComponent<PoleSquare>().hasElem)
                square.GetComponent<PoleSquare>().ShowEditButton();
        element = "delete";
    }
    public void ButtonShapeBuild()
    {
        HideEditButtons();
        UpdateBoolList();
        foreach (var square in GameObject.FindGameObjectsWithTag("PoleSquare"))
        {
            square.GetComponent<PoleSquare>().ShowEditButton();
        }
        element = "shapebuild";
    }
    public void ButtonShapePlace()
    {
        HideEditButtons();
        ShowEditButtonsSqueres();
        element = "shapeplace";
    }
    public void ButtonAddClrRing()
    {

        HideEditButtons();
        ShowEditButtonsSqueres();
        element = "clrRing";
    }
    public void ButtonCutRestore()
    {
        HideEditButtons();
        foreach (var line in GameObject.FindGameObjectsWithTag("PoleLine"))
        {
            line.GetComponent<PoleLine>().ShowEditButton();
        }
        element = "cut";
    }
    public void EditDot(GameObject dot)
    {
        if (element == "start")
        {
            activePole.GetComponent<Pole>().AddStart(dot);
            dot.GetComponent<PoleDot>().startFinish.GetComponent<StartDot>().Create();
            ButtonAddStart();
        }
        else if (element == "finish")
        {
            activePole.GetComponent<Pole>().AddFinish(dot);
            dot.GetComponent<PoleDot>().startFinish.GetComponent<FinishDot>().Create();
            ButtonAddFinish();
        }
        else if (element == "point")
        {
            dot.GetComponent<PoleDot>().hasPoint = true;
            dot.GetComponent<PoleDot>().CreateObject();
            ButtonAddPoint();
        }
        else if (element == "delete")
        {
            if (dot.GetComponent<PoleDot>().hasPoint)
            {
                activePole.GetComponent<Pole>().eltsManager.points.Remove(dot.GetComponent<PoleDot>().point);
                Destroy(dot.GetComponent<PoleDot>().point.gameObject);
                dot.GetComponent<PoleDot>().hasPoint = false;
            }
            else
            {
                activePole.GetComponent<Pole>().starts.Remove(dot);
                activePole.GetComponent<Pole>().finishes.Remove(dot);
                //sf
                Destroy(dot.GetComponent<PoleDot>().startFinish);
				dot.GetComponent<PoleDot>().startFinish = null;
            }
            ButtonDelete();
        }
        //HideEditButtons();
    }
    public void EditLine(GameObject line)
    {
        if (element == "point")
        {
            line.GetComponent<PoleLine>().hasPoint = true;
            line.GetComponent<PoleLine>().CreatePoint();
            activePole.GetComponent<Pole>().eltsManager.points.Add(line.GetComponent<PoleLine>().point);
            ButtonAddPoint();
        }
        else if (element == "delete")
        {
            line.GetComponent<PoleLine>().hasPoint = false;
            activePole.GetComponent<Pole>().eltsManager.points.Remove(line.GetComponent<PoleLine>().point);
            Destroy(line.GetComponent<PoleLine>().point.gameObject);
            ButtonDelete();
        }
        else if (element == "cut")
        {
            line.GetComponent<PoleLine>().CutOrCreateLine();
            ButtonCutRestore();
        }
        //HideEditButtons();
    }
    public void EditSquare(GameObject square)
    {
        var sq = square.GetComponent<PoleSquare>();
        if (element == "shapeplace")
        {
            Elements shapeElt = Instantiate(ShapePF, square.transform).GetComponent<Elements>();
            sq.GetComponent<PoleSquare>().hasElem = true;
            sq.GetComponent<PoleSquare>().element = shapeElt;
            shapeElt.location = sq.gameObject;
            CutBoolList();
            shapeElt.GetComponent<PoleEltShape>().boolList = boolList;
            shapeElt.GetComponent<PoleEltShape>().Create();
            ButtonShapePlace();
            //HideEditButtons();
        }
        else if (element == "shapebuild")
        {
            boolList[sq.indexI][sq.indexJ] = !boolList[sq.indexI][sq.indexJ];
            sq.UpdateShapeBool(boolList[sq.indexI][sq.indexJ]);
        }
        else if (element == "delete")
        {
            sq.hasElem = false;
            Destroy(sq.element);
            ButtonDelete();
            //HideEditButtons();
        }
    }
    public void ShowEditButtonsDots()
    {
        foreach(var dot in GameObject.FindGameObjectsWithTag("PoleDot"))
        {
            if (dot.GetComponent<PoleDot>().dot !=null && !dot.GetComponent<PoleDot>().hasPoint && dot.GetComponent<PoleDot>().startFinish == null)
                dot.GetComponent<PoleDot>().ShowEditButton();
        }
    }
    public void ShowEditButtonsLines()
    {
        foreach (var line in GameObject.FindGameObjectsWithTag("PoleLine"))
        {
            if (!line.GetComponent<PoleLine>().cut && !line.GetComponent<PoleLine>().hasPoint)
                line.GetComponent<PoleLine>().ShowEditButton();
        }
    }
    public void ShowEditButtonsSqueres()
    {
        foreach (var square in GameObject.FindGameObjectsWithTag("PoleSquare"))
        {
            if (!square.GetComponent<PoleSquare>().hasElem)
                square.GetComponent<PoleSquare>().ShowEditButton();
        }
    }

    public void ButtonCancel()
    {
        HideEditButtons();
        element = "";
    }
    public void HideEditButtons()
    {
        foreach (var but in GameObject.FindGameObjectsWithTag("EditButton"))
        {
            Destroy(but);
        }
        
        foreach (var sh in GameObject.FindGameObjectsWithTag("ShapeBool"))
            Destroy(sh);
    }
    void Start()
    {
        activePole = Instantiate(PolePF);
        activePole.GetComponent<Pole>().Init(5,5);
        UpdateBoolList();
        activePole.GetComponent<Pole>().poleDots[0][0].GetComponent<PoleDot>().CreateDot();
        activePole.GetComponent<Pole>().StartScaling(activePole.GetComponent<Pole>().poleDots[0][0]);
        //myPole.GetComponent<Pole>().SetClrRing(myPole.GetComponent<Pole>().quantityColor, myPole.GetComponent<Pole>().quantityRing);
    }


    private bool TrySolve(GameObject begin, List<GameObject> ends, GameObject prevDot)
    {
        begin.GetComponent<PoleDot>().isUsedByPlayer = true;
        
        var dotslist = activePole.GetComponent<Pole>().systemPath.dots;
        dotslist.Add(begin);
        int i = dotslist.Count - 1;
        GameObject line = null;
        var beginDot = begin.GetComponent<PoleDot>();
        if (i > 0)
        {
            if (beginDot.AllowedToDown() && beginDot.down.GetComponent<PoleLine>().down == prevDot)
            {
                line = beginDot.down;
                line.GetComponent<PoleLine>().isUsedByPlayer = true;
                activePole.GetComponent<Pole>().systemPath.lines.Add(line);
            }
            if (beginDot.AllowedToLeft() && beginDot.left.GetComponent<PoleLine>().left == prevDot)
            {
                line = beginDot.left;
                line.GetComponent<PoleLine>().isUsedByPlayer = true;
                activePole.GetComponent<Pole>().systemPath.lines.Add(line);
            }
            if (beginDot.AllowedToRight() && beginDot.right.GetComponent<PoleLine>().right == prevDot)
            {
                line = beginDot.right;
                line.GetComponent<PoleLine>().isUsedByPlayer = true;
                activePole.GetComponent<Pole>().systemPath.lines.Add(line);
            }
            if (beginDot.AllowedToUp() && beginDot.up.GetComponent<PoleLine>().up == prevDot)
            {
                line = beginDot.up;
                line.GetComponent<PoleLine>().isUsedByPlayer = true;
                activePole.GetComponent<Pole>().systemPath.lines.Add(line);
            }
        }
        if (ends.Contains(begin) && activePole.GetComponent<Pole>().eltsManager.CheckSolution(activePole.GetComponent<Pole>().poleDots[0][0].GetComponent<PoleDot>().right.GetComponent<PoleLine>().down))
        {
            finish = begin;
            return true;
        }
        bool border = false;
        if (prevDot != null)
            if (prevDot.GetComponent<PoleDot>().posX > 0 && prevDot.GetComponent<PoleDot>().posX < activePole.GetComponent<Pole>().width - 1 && prevDot.GetComponent<PoleDot>().posY > 0 && prevDot.GetComponent<PoleDot>().posY < activePole.GetComponent<Pole>().height - 1
                    && (beginDot.posX == 0 || beginDot.posY == 0 || beginDot.posX == activePole.GetComponent<Pole>().width - 1 || beginDot.posY == activePole.GetComponent<Pole>().height - 1))
            {
                border = true;
                activePole.GetComponent<Pole>().eltsManager.SetZone(activePole.GetComponent<Pole>().poleDots[0][0].GetComponent<PoleDot>().right.GetComponent<PoleLine>().down);
                
            }
        string deb = "";
        List<GameObject> dots = new List<GameObject>();
        if (beginDot.AllowedToUp() && !beginDot.up.GetComponent<PoleLine>().up.GetComponent<PoleDot>().isUsedByPlayer)
        {
            bool localIsSolved = true;
            GameObject squere = null;
            if (border)
            {
                if (activePole.GetComponent<Pole>().FindUnsolvedPoint(beginDot.down.GetComponent<PoleLine>().down))
                {
                    List<GameObject> zoneToCheck = new List<GameObject>();
                    

                    Color c = Color.red;
                    if (beginDot.AllowedToLeft())
                    {
                        squere = beginDot.left.GetComponent<PoleLine>().down;
                    }
                    else
                    {
                        squere = beginDot.right.GetComponent<PoleLine>().down;
                    }
                    foreach (var zone in activePole.GetComponent<Pole>().eltsManager.zone)
                    {
                        if (zone.Contains(squere))
                        {
                            zoneToCheck = zone;
                            break;
                        }
                    }
                    List<Elements> shapeList = new List<Elements>();
                    foreach (GameObject sq in zoneToCheck)
                    {
                        if (sq.GetComponent<PoleSquare>().hasElem)
                        {
                            if (sq.GetComponent<PoleSquare>().element.GetComponent<PoleEltClrRing>() != null)
                            {
                                if (c == Color.red)
                                {
                                    c = sq.GetComponent<PoleSquare>().element.GetComponent<PoleEltClrRing>().c.color;
                                }
                                else
                                {
                                    if (c != sq.GetComponent<PoleSquare>().element.GetComponent<PoleEltClrRing>().c.color)
                                    {
                                        localIsSolved = false;
                                    }
                                }
                            }
                            else if (sq.GetComponent<PoleSquare>().element.GetComponent<PoleEltShape>() != null)
                            {
                                shapeList.Add(sq.GetComponent<PoleSquare>().element);
                            }
                        }
                        if (sq.GetComponent<PoleSquare>().up.GetComponent<PoleLine>().hasPoint && !sq.GetComponent<PoleSquare>().up.GetComponent<PoleLine>().isUsedByPlayer)
                            localIsSolved = false;
                        else if (sq.GetComponent<PoleSquare>().right.GetComponent<PoleLine>().hasPoint && !sq.GetComponent<PoleSquare>().right.GetComponent<PoleLine>().isUsedByPlayer)
                            localIsSolved = false;
                        else if (sq.GetComponent<PoleSquare>().down.GetComponent<PoleLine>().hasPoint && !sq.GetComponent<PoleSquare>().down.GetComponent<PoleLine>().isUsedByPlayer)
                            localIsSolved = false;
                        else if (sq.GetComponent<PoleSquare>().left.GetComponent<PoleLine>().hasPoint && !sq.GetComponent<PoleSquare>().left.GetComponent<PoleLine>().isUsedByPlayer)
                            localIsSolved = false;
                    }
                    if (localIsSolved && shapeList.Count > 0)
                    {
                        localIsSolved = activePole.GetComponent<Pole>().eltsManager.CheckShapeSplit(zoneToCheck, shapeList);
                    }
                }
                else localIsSolved = false;
            }
            if (localIsSolved)
                dots.Add(beginDot.up.GetComponent<PoleLine>().up);
        }
        if (beginDot.AllowedToRight() && !beginDot.right.GetComponent<PoleLine>().right.GetComponent<PoleDot>().isUsedByPlayer)
        {
            bool localIsSolved = true;
            GameObject squere = null;
            if (border)
            {
                if (activePole.GetComponent<Pole>().FindUnsolvedPoint(beginDot.left.GetComponent<PoleLine>().left))
                {
                    List<GameObject> zoneToCheck = new List<GameObject>();

                    Color c = Color.red;
                    if (beginDot.AllowedToDown())
                    {
                        squere = beginDot.down.GetComponent<PoleLine>().left;
                    }
                    else
                    {
                        squere = beginDot.up.GetComponent<PoleLine>().left;
                    }
                    foreach (var zone in activePole.GetComponent<Pole>().eltsManager.zone)
                    {
                        if (zone.Contains(squere))
                        {
                            zoneToCheck = zone;
                            break;
                        }
                    }
                    deb += zoneToCheck.Count;
                    List<Elements> shapeList = new List<Elements>();
                    foreach (GameObject sq in zoneToCheck)
                    {
                        if (sq.GetComponent<PoleSquare>().hasElem)
                        {
                            if (sq.GetComponent<PoleSquare>().element.GetComponent<PoleEltClrRing>() != null)
                            {
                                if (c == Color.red)
                                {
                                    c = sq.GetComponent<PoleSquare>().element.GetComponent<PoleEltClrRing>().c.color;
                                }
                                else
                                {
                                    if (c != sq.GetComponent<PoleSquare>().element.GetComponent<PoleEltClrRing>().c.color)
                                    {
                                        localIsSolved = false;
                                    }
                                }
                            }
                            else if (sq.GetComponent<PoleSquare>().element.GetComponent<PoleEltShape>() != null)
                            {
                                shapeList.Add(sq.GetComponent<PoleSquare>().element);
                            }
                        }
                        if (sq.GetComponent<PoleSquare>().up.GetComponent<PoleLine>().hasPoint && !sq.GetComponent<PoleSquare>().up.GetComponent<PoleLine>().isUsedByPlayer)
                            localIsSolved = false;
                        else if (sq.GetComponent<PoleSquare>().right.GetComponent<PoleLine>().hasPoint && !sq.GetComponent<PoleSquare>().right.GetComponent<PoleLine>().isUsedByPlayer)
                            localIsSolved = false;
                        else if (sq.GetComponent<PoleSquare>().down.GetComponent<PoleLine>().hasPoint && !sq.GetComponent<PoleSquare>().down.GetComponent<PoleLine>().isUsedByPlayer)
                            localIsSolved = false;
                        else if (sq.GetComponent<PoleSquare>().left.GetComponent<PoleLine>().hasPoint && !sq.GetComponent<PoleSquare>().left.GetComponent<PoleLine>().isUsedByPlayer)
                            localIsSolved = false;
                    }
                    deb += " " + shapeList.Count;
                    if (localIsSolved && shapeList.Count > 0)
                    {
                        localIsSolved = activePole.GetComponent<Pole>().eltsManager.CheckShapeSplit(zoneToCheck, shapeList);
                    }
                }
                else localIsSolved = false;
            }
            if (localIsSolved)
                dots.Add(beginDot.right.GetComponent<PoleLine>().right);
        }
        if (beginDot.AllowedToDown() && !beginDot.down.GetComponent<PoleLine>().down.GetComponent<PoleDot>().isUsedByPlayer)
        {
            bool localIsSolved = true;
            GameObject squere = null;
            if (border)
            {
                if (activePole.GetComponent<Pole>().FindUnsolvedPoint(beginDot.up.GetComponent<PoleLine>().up))
                {
                    List<GameObject> zoneToCheck = new List<GameObject>();

                    Color c = Color.red;
                    if (beginDot.AllowedToLeft())
                    {
                        squere = beginDot.left.GetComponent<PoleLine>().up;
                    }
                    else
                    {
                        squere = beginDot.right.GetComponent<PoleLine>().up;
                    }
                    foreach (var zone in activePole.GetComponent<Pole>().eltsManager.zone)
                    {
                        if (zone.Contains(squere))
                        {
                            zoneToCheck = zone;
                            break;
                        }
                    }
                    List<Elements> shapeList = new List<Elements>();
                    foreach (GameObject sq in zoneToCheck)
                    {
                        if (sq.GetComponent<PoleSquare>().hasElem)
                        {
                            if (sq.GetComponent<PoleSquare>().element.GetComponent<PoleEltClrRing>() != null)
                            {
                                if (c == Color.red)
                                {
                                    c = sq.GetComponent<PoleSquare>().element.GetComponent<PoleEltClrRing>().c.color;
                                }
                                else
                                {
                                    if (c != sq.GetComponent<PoleSquare>().element.GetComponent<PoleEltClrRing>().c.color)
                                    {
                                        localIsSolved = false;
                                    }
                                }
                            }
                            else if (sq.GetComponent<PoleSquare>().element.GetComponent<PoleEltShape>() != null)
                            {
                                shapeList.Add(sq.GetComponent<PoleSquare>().element);
                            }
                        }
                        if (sq.GetComponent<PoleSquare>().up.GetComponent<PoleLine>().hasPoint && !sq.GetComponent<PoleSquare>().up.GetComponent<PoleLine>().isUsedByPlayer)
                            localIsSolved = false;
                        else if (sq.GetComponent<PoleSquare>().right.GetComponent<PoleLine>().hasPoint && !sq.GetComponent<PoleSquare>().right.GetComponent<PoleLine>().isUsedByPlayer)
                            localIsSolved = false;
                        else if (sq.GetComponent<PoleSquare>().down.GetComponent<PoleLine>().hasPoint && !sq.GetComponent<PoleSquare>().down.GetComponent<PoleLine>().isUsedByPlayer)
                            localIsSolved = false;
                        else if (sq.GetComponent<PoleSquare>().left.GetComponent<PoleLine>().hasPoint && !sq.GetComponent<PoleSquare>().left.GetComponent<PoleLine>().isUsedByPlayer)
                            localIsSolved = false;
                    }
                    if (localIsSolved && shapeList.Count > 0)
                    {
                        localIsSolved = activePole.GetComponent<Pole>().eltsManager.CheckShapeSplit(zoneToCheck, shapeList);
                    }
                }
                else localIsSolved = false;
            }
            if (localIsSolved)
                dots.Add(beginDot.down.GetComponent<PoleLine>().down);
        }
        if (beginDot.AllowedToLeft() && !beginDot.left.GetComponent<PoleLine>().left.GetComponent<PoleDot>().isUsedByPlayer)
        {
            bool localIsSolved = true;
            GameObject squere = null;
            if (border)
            {
                if (activePole.GetComponent<Pole>().FindUnsolvedPoint(beginDot.right.GetComponent<PoleLine>().right))
                {
                    List<GameObject> zoneToCheck = new List<GameObject>();

                    Color c = Color.red;
                    if (beginDot.AllowedToDown())
                    {
                        squere = beginDot.down.GetComponent<PoleLine>().right;
                    }
                    else
                    {
                        squere = beginDot.up.GetComponent<PoleLine>().right;
                    }
                    foreach (var zone in activePole.GetComponent<Pole>().eltsManager.zone)
                    {
                        if (zone.Contains(squere))
                        {
                            zoneToCheck = zone;
                            break;
                        }
                    }
                    List<Elements> shapeList = new List<Elements>();
                    foreach (GameObject sq in zoneToCheck)
                    {
                        if (sq.GetComponent<PoleSquare>().hasElem)
                        {
                            if (sq.GetComponent<PoleSquare>().element.GetComponent<PoleEltClrRing>() != null)
                            {
                                if (c == Color.red)
                                {
                                    c = sq.GetComponent<PoleSquare>().element.GetComponent<PoleEltClrRing>().c.color;
                                }
                                else
                                {
                                    if (c != sq.GetComponent<PoleSquare>().element.GetComponent<PoleEltClrRing>().c.color)
                                    {
                                        localIsSolved = false;
                                    }
                                }
                            }
                            else if (sq.GetComponent<PoleSquare>().element.GetComponent<PoleEltShape>() != null)
                            {
                                shapeList.Add(sq.GetComponent<PoleSquare>().element);
                            }
                        }
                        if (sq.GetComponent<PoleSquare>().up.GetComponent<PoleLine>().hasPoint && !sq.GetComponent<PoleSquare>().up.GetComponent<PoleLine>().isUsedByPlayer)
                            localIsSolved = false;
                        else if (sq.GetComponent<PoleSquare>().right.GetComponent<PoleLine>().hasPoint && !sq.GetComponent<PoleSquare>().right.GetComponent<PoleLine>().isUsedByPlayer)
                            localIsSolved = false;
                        else if (sq.GetComponent<PoleSquare>().down.GetComponent<PoleLine>().hasPoint && !sq.GetComponent<PoleSquare>().down.GetComponent<PoleLine>().isUsedByPlayer)
                            localIsSolved = false;
                        else if (sq.GetComponent<PoleSquare>().left.GetComponent<PoleLine>().hasPoint && !sq.GetComponent<PoleSquare>().left.GetComponent<PoleLine>().isUsedByPlayer)
                            localIsSolved = false;
                    }
                    if (localIsSolved && shapeList.Count > 0)
                    {
                        localIsSolved = activePole.GetComponent<Pole>().eltsManager.CheckShapeSplit(zoneToCheck, shapeList);
                    }
                }
                else localIsSolved = false;
            }
            if (localIsSolved)
                dots.Add(beginDot.left.GetComponent<PoleLine>().left);
        }

        if (border)
        {
            string zz = "";
            for (int k = 0; k < activePole.GetComponent<Pole>().eltsManager.checkZones.Length; ++k)
            {
                for (int j = 0; j < activePole.GetComponent<Pole>().eltsManager.checkZones[k].Length; ++j)
                {
                    zz += " " + (int)activePole.GetComponent<Pole>().eltsManager.checkZones[k][j];
                }
                zz += "\n";
            }
            zz += dots.Count + " " + deb;
            //Debug.Log(zz);
        }
        var tmp = new List<GameObject>();
        foreach (GameObject dot in dots)
            if (activePole.GetComponent<Pole>().FindFinish(dot))
                tmp.Add(dot);
        dots = new List<GameObject>(tmp);

        bool success = false;
        foreach (GameObject next in dots)
        {
            if (TrySolve(next, ends, begin))
            {
                success = true;
                break;
            }
        }
        if (!success)
        {
            begin.GetComponent<PoleDot>().isUsedByPlayer = false;
            activePole.GetComponent<Pole>().systemPath.dots.Remove(begin);
            activePole.GetComponent<Pole>().systemPath.lines.Remove(line);
            if (line != null)
                line.GetComponent<PoleLine>().isUsedByPlayer = false;
        }
        return success;
    }
    private void UpdateBoolList()
    {
        boolList = new List<List<bool>>();
        for (int i = 0; i < activePole.GetComponent<Pole>().height; ++i)
        {
            boolList.Add(new List<bool>());
            for (int j = 0; j < activePole.GetComponent<Pole>().width; ++j)
                boolList[i].Add(false);
        }
    }
    private void CutBoolList()
    {
        bool empty = true;
        while (empty)
        {
            for (int i = 0; i < boolList[0].Count; ++i)
                if (boolList[0][i]) empty = false;
            if (empty)
                boolList.RemoveAt(0);
        }
        empty = true;
        while (empty)
        {
            for (int i = 0; i < boolList[0].Count; ++i)
                if (boolList[boolList.Count - 1][i]) empty = false;
            if (empty)
                boolList.RemoveAt(boolList.Count - 1);
        }
        empty = true;
        while (empty)
        {
            for (int i = 0; i < boolList.Count; ++i)
                if (boolList[i][0]) empty = false;
            if (empty)
                for (int i = 0; i < boolList.Count; ++i)
                    boolList[i].RemoveAt(0);
        }
        empty = true;
        while (empty)
        {
            for (int i = 0; i < boolList.Count; ++i)
                if (boolList[i][boolList[0].Count - 1]) empty = false;
            if (empty)
                for (int i = 0; i < boolList.Count; ++i)
                    boolList[i].RemoveAt(boolList[i].Count - 1);
        }
    }
}
