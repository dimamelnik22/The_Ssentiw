using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Editor : MonoBehaviour
{
    public GameObject PolePrefab;
    public GameObject PointPrefab;
    public GameObject ShapePF;

    public Text SolveTimeText;
    public float solvetime = 0f;

    public GameObject PathDotPrefab;
    public GameObject PathVerticalLinePrefab;
    public GameObject PathHorizontalLinePrefab;
    public GameObject PathStartPrefab;
    public GameObject PathFinishPrefab;

    public Material PlayerWrongPathMaterial;
    public Material PlayerGoodPathMaterial;
    public Material EltPointMaterial;
    public Material EltWrongPointMaterial;

    private static Vector3 pathstepz = new Vector3(0f, 0f, -0.5f);

    private static Vector3 stepx = new Vector3(5f, 0f, 0f);
    private static Vector3 stepy = new Vector3(0f, -5f, 0f);
    //private List<GameObject> finishes = new List<GameObject>();
    public bool pathIsShown = false;
    public bool playerIsActive = false;
    public GameObject myPole;

    public InputField heightInput;
    public InputField widthInput;
    private List<List<bool>> boolList;
    private string element = "";
    private GameObject finish;
    public int count = 0;

    public void ButtonSavePazzl()
    {
        foreach (PoleEltPoint point in myPole.GetComponent<Pole>().eltsManager.points)
        {

            Debug.Log(point.x);
            Debug.Log(point.y);
            Debug.Log(point.down);
            Debug.Log(point.right);
        }
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

    public void ButtonMenu()
    {
        SceneManager.LoadScene("MainMenu");
        MenuManager.DebugMessage.clear();
    }

    public void ButtonHideSolution()
    {
        foreach (GameObject gameObject in GameObject.FindGameObjectsWithTag("Path"))
            Destroy(gameObject);
        foreach (GameObject dot in myPole.GetComponent<Pole>().systemPath.dots)
            dot.GetComponent<PoleDot>().isUsedByPlayer = false;
        foreach (GameObject line in myPole.GetComponent<Pole>().systemPath.lines)
            line.GetComponent<PoleLine>().isUsedByPlayer = false;
        myPole.GetComponent<Pole>().systemPath.Clear();
    }
    public void ButtonTrySolve()
    {
        count = 0;
        if (myPole.GetComponent<Pole>().starts.Count > 0 && myPole.GetComponent<Pole>().finishes.Count > 0)
        {
            solvetime = Time.realtimeSinceStartup;
            foreach (GameObject start in myPole.GetComponent<Pole>().starts)
                if (TrySolve(start, myPole.GetComponent<Pole>().finishes, false))
                {
                    Debug.Log("success");

                    foreach (GameObject dot in myPole.GetComponent<Pole>().systemPath.dots)
                    {
                        if (dot == start) Instantiate(PathStartPrefab, dot.transform.position + pathstepz, PathStartPrefab.transform.rotation);
                        else if (dot == finish) Instantiate(PathFinishPrefab, dot.transform.position + pathstepz, PathFinishPrefab.transform.rotation);
                        else Instantiate(PathDotPrefab, dot.transform.position + pathstepz, PathDotPrefab.transform.rotation);
                    }
                    foreach (GameObject line in myPole.GetComponent<Pole>().systemPath.lines)
                    {
                        if (line.GetComponent<PoleLine>().isHorizontal) Instantiate(PathHorizontalLinePrefab, line.transform.position + pathstepz, PathHorizontalLinePrefab.transform.rotation);
                        else Instantiate(PathVerticalLinePrefab, line.transform.position + pathstepz, PathVerticalLinePrefab.transform.rotation);
                    }
                }
            SolveTimeText.text = (Time.realtimeSinceStartup - solvetime).ToString();
            Debug.Log(count);
        }
    }

    public bool TrySolve(GameObject begin, List<GameObject> ends, bool needToCheckLocal)
    {
        begin.GetComponent<PoleDot>().isUsedByPlayer = true;
        
        var dotslist = myPole.GetComponent<Pole>().systemPath.dots;
        dotslist.Add(begin);
        int i = dotslist.Count - 1;
        GameObject line = null;
        if (i > 0)
        {
            if (dotslist[i].GetComponent<PoleDot>().AllowedToDown() && dotslist[i].GetComponent<PoleDot>().down.GetComponent<PoleLine>().down == dotslist[i - 1])
            {
                line = dotslist[i].GetComponent<PoleDot>().down;
                line.GetComponent<PoleLine>().isUsedByPlayer = true;
                myPole.GetComponent<Pole>().systemPath.lines.Add(line);
            }
            if (dotslist[i].GetComponent<PoleDot>().AllowedToLeft() && dotslist[i].GetComponent<PoleDot>().left.GetComponent<PoleLine>().left == dotslist[i - 1])
            {
                line = dotslist[i].GetComponent<PoleDot>().left;
                line.GetComponent<PoleLine>().isUsedByPlayer = true;
                myPole.GetComponent<Pole>().systemPath.lines.Add(line);
            }
            if (dotslist[i].GetComponent<PoleDot>().AllowedToRight() && dotslist[i].GetComponent<PoleDot>().right.GetComponent<PoleLine>().right == dotslist[i - 1])
            {
                line = dotslist[i].GetComponent<PoleDot>().right;
                line.GetComponent<PoleLine>().isUsedByPlayer = true;
                myPole.GetComponent<Pole>().systemPath.lines.Add(line);
            }
            if (dotslist[i].GetComponent<PoleDot>().AllowedToUp() && dotslist[i].GetComponent<PoleDot>().up.GetComponent<PoleLine>().up == dotslist[i - 1])
            {
                line = dotslist[i].GetComponent<PoleDot>().up;
                line.GetComponent<PoleLine>().isUsedByPlayer = true;
                myPole.GetComponent<Pole>().systemPath.lines.Add(line);
            }
        }
        if (ends.Contains(begin) && myPole.GetComponent<Pole>().eltsManager.CheckSolution(myPole.GetComponent<Pole>().poleDots[0][0].GetComponent<PoleDot>().right.GetComponent<PoleLine>().down))
        {
            finish = begin;
            return true;
        }
        var beginDot = begin.GetComponent<PoleDot>();
        List<GameObject> dots = new List<GameObject>();
        if (beginDot.up != null && !beginDot.up.GetComponent<PoleLine>().up.GetComponent<PoleDot>().isUsedByPlayer)
            dots.Add(beginDot.up.GetComponent<PoleLine>().up);
        if (beginDot.right != null && !beginDot.right.GetComponent<PoleLine>().right.GetComponent<PoleDot>().isUsedByPlayer)
            dots.Add(beginDot.right.GetComponent<PoleLine>().right);
        if (beginDot.down != null && !beginDot.down.GetComponent<PoleLine>().down.GetComponent<PoleDot>().isUsedByPlayer)
            dots.Add(beginDot.down.GetComponent<PoleLine>().down);
        if (beginDot.left != null && !beginDot.left.GetComponent<PoleLine>().left.GetComponent<PoleDot>().isUsedByPlayer)
            dots.Add(beginDot.left.GetComponent<PoleLine>().left);
        if (needToCheckLocal)
        {
            var tmp = new List<GameObject>();
            foreach (GameObject dot in dots)
                if (myPole.GetComponent<Pole>().FindFinish(dot))
                    tmp.Add(dot);
            dots = new List<GameObject>(tmp);
            if (beginDot.posY == 4 && beginDot.posX == 2)
            {
                //Debug.Log(dots.Count + " " + beginDot.right.GetComponent<PoleLine>().right.GetComponent<PoleDot>().isUsedByPlayer + " " + beginDot.left.GetComponent<PoleLine>().left.GetComponent<PoleDot>().isUsedByPlayer);
            }
        }
        if (dots.Count == 0) count++;
        bool success = false;
        foreach (GameObject next in dots)
        {
            bool needCheck = false;
            if (beginDot.posX > 0 && beginDot.posX < myPole.GetComponent<Pole>().width - 1 && beginDot.posY > 0 && beginDot.posY < myPole.GetComponent<Pole>().height - 1
                && (next.GetComponent<PoleDot>().posX == 0 || next.GetComponent<PoleDot>().posY == 0 || next.GetComponent<PoleDot>().posX == myPole.GetComponent<Pole>().width - 1 || next.GetComponent<PoleDot>().posY == myPole.GetComponent<Pole>().height - 1))
            {
                needCheck = true;
                
            }
                
            if (TrySolve(next, ends, needCheck))
            {
                success = true;
            }
        }
        if (!success)
        {
            begin.GetComponent<PoleDot>().isUsedByPlayer = false;
            myPole.GetComponent<Pole>().systemPath.dots.Remove(begin);
            myPole.GetComponent<Pole>().systemPath.lines.Remove(line);
            if (line != null)
                line.GetComponent<PoleLine>().isUsedByPlayer = false;
        }
        
        return success;
    }

    public void ButtonResize()
    {
        ButtonHideSolution();
        HideEditButtons();
        myPole.GetComponent<Pole>().ClearPole();
        foreach (GameObject point in GameObject.FindGameObjectsWithTag("EltPoint"))
            Destroy(point);
        foreach (GameObject shape in GameObject.FindGameObjectsWithTag("EltShape"))
            Destroy(shape);
        foreach (GameObject clrRing in GameObject.FindGameObjectsWithTag("EltClrRing"))
            Destroy(clrRing);
        Destroy(myPole);
        myPole = Instantiate(PolePrefab);
        myPole.GetComponent<Pole>().Init(System.Int32.Parse(heightInput.text), System.Int32.Parse(widthInput.text));
        UpdateBoolList();
        myPole.GetComponent<Pole>().poleDots[0][0].GetComponent<PoleDot>().CreateDot();
        myPole.GetComponent<Pole>().StartScaling(myPole.GetComponent<Pole>().poleDots[0][0]);
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

    public void EditDot(int x, int y)
    {
        var dot = myPole.GetComponent<Pole>().poleDots[y][x];
        if (element == "start")
        {
            myPole.GetComponent<Pole>().AddStart(x, y);
        }
        else if (element == "finish")
        {
            myPole.GetComponent<Pole>().AddFinish(x, y);
        }
        else if (element == "point")
        {
            dot.GetComponent<PoleDot>().hasPoint = true;
            dot.GetComponent<PoleDot>().CreatePoint();
            myPole.GetComponent<Pole>().eltsManager.points.Add(dot.GetComponent<PoleDot>().point);
        }
        else if (element == "delete")
        {
            if (dot.GetComponent<PoleDot>().hasPoint)
            {
                myPole.GetComponent<Pole>().eltsManager.points.Remove(dot.GetComponent<PoleDot>().point);
                Destroy(dot.GetComponent<PoleDot>().point.gameObject);
                dot.GetComponent<PoleDot>().hasPoint = false;
            }
            else
            {
                myPole.GetComponent<Pole>().startDots.Remove(dot.GetComponent<PoleDot>().startFinish);
                myPole.GetComponent<Pole>().starts.Remove(dot);
                myPole.GetComponent<Pole>().finishDots.Remove(dot.GetComponent<PoleDot>().startFinish);
                myPole.GetComponent<Pole>().finishes.Remove(dot);
                Destroy(dot.GetComponent<PoleDot>().startFinish);
            }
        }
        HideEditButtons();
    }

    public void EditLine(GameObject line)
    {
        if (element == "point")
        {
            line.GetComponent<PoleLine>().hasPoint = true;
            line.GetComponent<PoleLine>().CreatePoint();
            myPole.GetComponent<Pole>().eltsManager.points.Add(line.GetComponent<PoleLine>().point);
        }
        else if (element == "delete")
        {
            line.GetComponent<PoleLine>().hasPoint = false;
            myPole.GetComponent<Pole>().eltsManager.points.Remove(line.GetComponent<PoleLine>().point);
            Destroy(line.GetComponent<PoleLine>().point.gameObject);
        }
        HideEditButtons();
    }

    public void EditSquare(GameObject square)
    {
        var sq = square.GetComponent<PoleSquare>();
        if (element == "shapeplace")
        {
            Elements shapeElt = Instantiate(ShapePF, square.transform).GetComponent<Elements>();
            sq.GetComponent<PoleSquare>().hasElem = true;
            sq.GetComponent<PoleSquare>().element = shapeElt;
            CutBoolList();
            shapeElt.GetComponent<PoleEltShape>().boolList = boolList;
            shapeElt.GetComponent<PoleEltShape>().Create();
            HideEditButtons();
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
            HideEditButtons();
        }
    }

    public void ShowEditButtonsDots()
    {
        foreach(var dot in GameObject.FindGameObjectsWithTag("PoleDot"))
        {
            if (!dot.GetComponent<PoleDot>().hasPoint && dot.GetComponent<PoleDot>().startFinish == null)
                dot.GetComponent<PoleDot>().ShowEditButton();
        }
    }

    public void ShowEditButtonsLines()
    {
        foreach (var line in GameObject.FindGameObjectsWithTag("PoleLine"))
        {
            if (!line.GetComponent<PoleLine>().hasPoint)
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

    public void HideEditButtons()
    {
        foreach (var dot in GameObject.FindGameObjectsWithTag("PoleDot"))
        {
            dot.GetComponent<PoleDot>().HideEditButton();
        }
        foreach (var line in GameObject.FindGameObjectsWithTag("PoleLine"))
        {
            line.GetComponent<PoleLine>().HideEditButton();
        }
        foreach (var squere in GameObject.FindGameObjectsWithTag("PoleSquare"))
        {
            squere.GetComponent<PoleSquare>().HideEditButton();
        }
        foreach (var sh in GameObject.FindGameObjectsWithTag("ShapeBool"))
            Destroy(sh);
    }
    void Start()
    {
        
        myPole = Instantiate(PolePrefab);
        myPole.GetComponent<Pole>().Init(5,5);
        UpdateBoolList();
        myPole.GetComponent<Pole>().poleDots[0][0].GetComponent<PoleDot>().CreateDot();
        myPole.GetComponent<Pole>().StartScaling(myPole.GetComponent<Pole>().poleDots[0][0]);
        
        //myPole.GetComponent<Pole>().GenerateShapes(Core.PolePreferences.numOfShapes);
        ////myPole.GetComponent<Pole>().GenerateShapes(100);
        //myPole.GetComponent<Pole>().SetClrRing(myPole.GetComponent<Pole>().quantityColor, myPole.GetComponent<Pole>().quantityRing);
        

    }

    void Update()
    {

    }

    private void UpdateBoolList()
    {
        boolList = new List<List<bool>>();
        for (int i = 0; i < myPole.GetComponent<Pole>().height; ++i)
        {
            boolList.Add(new List<bool>());
            for (int j = 0; j < myPole.GetComponent<Pole>().width; ++j)
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
