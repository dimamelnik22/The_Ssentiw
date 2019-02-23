using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Core : MonoBehaviour {

    public GameObject DotPrefab;
    public GameObject VerticalLinePrefab;
    public GameObject HorizontalLinePrefab;
    public GameObject StartPrefab;
    public GameObject FinishPrefab;
    public GameObject PointPrefab;

    public Transform PathDotPrefab;
    public GameObject PathVerticalLinePrefab;
    public GameObject PathHorizontalLinePrefab;
    public Transform PathStartPrefab;
    public Transform PathFinishPrefab;

    public GameObject PlayerPathDotPrefab;
    public GameObject PlayerPathVerticalLinePrefab;
    public GameObject PlayerPathHorizontalLinePrefab;
    public GameObject PlayerPathStartPrefab;
    public GameObject PlayerPathFinishPrefab;

    public Material PlayerWrongPathMaterial;
    public Material PlayerGoodPathMaterial;
    public Material EltPointMaterial;
    public Material EltWrongPointMaterial;

    private List<GameObject> playerPathLinesOnScreen;
    private List<GameObject> playerPathDotsOnScreen;

    private static Vector3 pathstepy = new Vector3(0, 0.5f, 0);
    private static Vector3 stepx = new Vector3(5, 0, 0);
    private static Vector3 stepz = new Vector3(0, 0, -5);
    public int seed = 95;
    public bool mode = true;
    public bool pathIsShown = false;
    public bool playerIsActive = false;
    private Pole myPole;

    public void ControllerUp()
    {
        if (playerIsActive)
        {
            GameObject current = myPole.playerPath.dots[myPole.playerPath.dots.Count - 1];
            if (current != myPole.start)
            {
                if (current.GetComponent<PoleDot>().AllowedToUp())
                {
                    GameObject next;
                    if (current.GetComponent<PoleDot>().up.GetComponent<PoleLine>().first == current)
                        next = current.GetComponent<PoleDot>().up.GetComponent<PoleLine>().second;
                    else next = current.GetComponent<PoleDot>().up.GetComponent<PoleLine>().first;
                    if (!next.GetComponent<PoleDot>().isUsedByPlayer)
                    {
                        playerPathLinesOnScreen.Add(Instantiate(PlayerPathVerticalLinePrefab, playerPathDotsOnScreen[playerPathDotsOnScreen.Count - 1].transform.position - stepz * 0.5f, PlayerPathVerticalLinePrefab.transform.rotation));
                        playerPathDotsOnScreen.Add(Instantiate(PlayerPathDotPrefab, playerPathDotsOnScreen[playerPathDotsOnScreen.Count - 1].transform.position - stepz, PlayerPathDotPrefab.transform.rotation));
                        current.GetComponent<PoleDot>().up.GetComponent<PoleLine>().isUsedByPlayer = true;
                        next.GetComponent<PoleDot>().isUsedByPlayer = true;
                        myPole.playerPath.dots.Add(next);
                        myPole.playerPath.lines.Add(current.GetComponent<PoleDot>().up);
                    }
                    else if (current.GetComponent<PoleDot>().up.GetComponent<PoleLine>().isUsedByPlayer)
                    {
                        Destroy(playerPathDotsOnScreen[playerPathDotsOnScreen.Count - 1]);
                        Destroy(playerPathLinesOnScreen[playerPathLinesOnScreen.Count - 1]);
                        playerPathDotsOnScreen.RemoveAt(playerPathDotsOnScreen.Count - 1);
                        playerPathLinesOnScreen.RemoveAt(playerPathLinesOnScreen.Count - 1);
                        current.GetComponent<PoleDot>().isUsedByPlayer = false;
                        current.GetComponent<PoleDot>().up.GetComponent<PoleLine>().isUsedByPlayer = false;
                        myPole.playerPath.dots.Remove(current);
                        myPole.playerPath.lines.Remove(current.GetComponent<PoleDot>().up);
                    }
                }
            }
            else if (current.GetComponent<PoleDot>().AllowedToUp())
            {
                GameObject next;
                if (current.GetComponent<PoleDot>().up.GetComponent<PoleLine>().first == current)
                    next = current.GetComponent<PoleDot>().up.GetComponent<PoleLine>().second;
                else next = current.GetComponent<PoleDot>().up.GetComponent<PoleLine>().first;
                playerPathLinesOnScreen.Add(Instantiate(PlayerPathVerticalLinePrefab, playerPathDotsOnScreen[playerPathDotsOnScreen.Count - 1].transform.position - stepz * 0.5f, PlayerPathVerticalLinePrefab.transform.rotation));
                playerPathDotsOnScreen.Add(Instantiate(PlayerPathDotPrefab, playerPathDotsOnScreen[playerPathDotsOnScreen.Count - 1].transform.position - stepz, PlayerPathDotPrefab.transform.rotation));
                current.GetComponent<PoleDot>().up.GetComponent<PoleLine>().isUsedByPlayer = true;
                next.GetComponent<PoleDot>().isUsedByPlayer = true;
                myPole.playerPath.dots.Add(next);
                myPole.playerPath.lines.Add(current.GetComponent<PoleDot>().up);
            }
        }
    }
    public void ControllerDown()
    {
        if (playerIsActive)
        {
            GameObject current = myPole.playerPath.dots[myPole.playerPath.dots.Count - 1];
            if (current != myPole.start)
            {
                if (current.GetComponent<PoleDot>().AllowedToDown())
                {
                    GameObject next;
                    if (current.GetComponent<PoleDot>().down.GetComponent<PoleLine>().first == current)
                        next = current.GetComponent<PoleDot>().down.GetComponent<PoleLine>().second;
                    else next = current.GetComponent<PoleDot>().down.GetComponent<PoleLine>().first;
                    if (!next.GetComponent<PoleDot>().isUsedByPlayer)
                    {
                        playerPathLinesOnScreen.Add(Instantiate(PlayerPathVerticalLinePrefab, playerPathDotsOnScreen[playerPathDotsOnScreen.Count - 1].transform.position + stepz * 0.5f, PlayerPathVerticalLinePrefab.transform.rotation));
                        playerPathDotsOnScreen.Add(Instantiate(PlayerPathDotPrefab, playerPathDotsOnScreen[playerPathDotsOnScreen.Count - 1].transform.position + stepz, PlayerPathDotPrefab.transform.rotation));
                        current.GetComponent<PoleDot>().down.GetComponent<PoleLine>().isUsedByPlayer = true;
                        next.GetComponent<PoleDot>().isUsedByPlayer = true;
                        myPole.playerPath.dots.Add(next);
                        myPole.playerPath.lines.Add(current.GetComponent<PoleDot>().down);
                    }
                    else if (current.GetComponent<PoleDot>().down.GetComponent<PoleLine>().isUsedByPlayer)
                    {
                        Destroy(playerPathDotsOnScreen[playerPathDotsOnScreen.Count - 1]);
                        Destroy(playerPathLinesOnScreen[playerPathLinesOnScreen.Count - 1]);
                        playerPathDotsOnScreen.RemoveAt(playerPathDotsOnScreen.Count - 1);
                        playerPathLinesOnScreen.RemoveAt(playerPathLinesOnScreen.Count - 1);
                        current.GetComponent<PoleDot>().isUsedByPlayer = false;
                        current.GetComponent<PoleDot>().down.GetComponent<PoleLine>().isUsedByPlayer = false;
                        myPole.playerPath.dots.Remove(current);
                        myPole.playerPath.lines.Remove(current.GetComponent<PoleDot>().down);
                    }
                }
            }
            else if (current.GetComponent<PoleDot>().AllowedToDown())
            {
                GameObject next;
                if (current.GetComponent<PoleDot>().down.GetComponent<PoleLine>().first == current)
                    next = current.GetComponent<PoleDot>().down.GetComponent<PoleLine>().second;
                else next = current.GetComponent<PoleDot>().down.GetComponent<PoleLine>().first;
                playerPathLinesOnScreen.Add(Instantiate(PlayerPathVerticalLinePrefab, playerPathDotsOnScreen[playerPathDotsOnScreen.Count - 1].transform.position + stepz * 0.5f, PlayerPathVerticalLinePrefab.transform.rotation));
                playerPathDotsOnScreen.Add(Instantiate(PlayerPathDotPrefab, playerPathDotsOnScreen[playerPathDotsOnScreen.Count - 1].transform.position + stepz, PlayerPathDotPrefab.transform.rotation));
                current.GetComponent<PoleDot>().down.GetComponent<PoleLine>().isUsedByPlayer = true;
                next.GetComponent<PoleDot>().isUsedByPlayer = true;
                myPole.playerPath.dots.Add(next);
                myPole.playerPath.lines.Add(current.GetComponent<PoleDot>().down);
            }
        }
    }
    public void ControllerLeft()
    {
        if (playerIsActive)
        {
            GameObject current = myPole.playerPath.dots[myPole.playerPath.dots.Count - 1];
            if (current != myPole.start)
            {
                if (current.GetComponent<PoleDot>().AllowedToLeft())
                {
                    GameObject next;
                    if (current.GetComponent<PoleDot>().left.GetComponent<PoleLine>().first == current)
                        next = current.GetComponent<PoleDot>().left.GetComponent<PoleLine>().second;
                    else next = current.GetComponent<PoleDot>().left.GetComponent<PoleLine>().first;
                    if (!next.GetComponent<PoleDot>().isUsedByPlayer)
                    {
                        playerPathLinesOnScreen.Add(Instantiate(PlayerPathHorizontalLinePrefab, playerPathDotsOnScreen[playerPathDotsOnScreen.Count - 1].transform.position - stepx * 0.5f, PlayerPathHorizontalLinePrefab.transform.rotation));
                        playerPathDotsOnScreen.Add(Instantiate(PlayerPathDotPrefab, playerPathDotsOnScreen[playerPathDotsOnScreen.Count - 1].transform.position - stepx, PlayerPathDotPrefab.transform.rotation));
                        current.GetComponent<PoleDot>().left.GetComponent<PoleLine>().isUsedByPlayer = true;
                        next.GetComponent<PoleDot>().isUsedByPlayer = true;
                        myPole.playerPath.dots.Add(next);
                        myPole.playerPath.lines.Add(current.GetComponent<PoleDot>().left);
                    }
                    else if (current.GetComponent<PoleDot>().left.GetComponent<PoleLine>().isUsedByPlayer)
                    {
                        Destroy(playerPathDotsOnScreen[playerPathDotsOnScreen.Count - 1]);
                        Destroy(playerPathLinesOnScreen[playerPathLinesOnScreen.Count - 1]);
                        playerPathDotsOnScreen.RemoveAt(playerPathDotsOnScreen.Count - 1);
                        playerPathLinesOnScreen.RemoveAt(playerPathLinesOnScreen.Count - 1);
                        current.GetComponent<PoleDot>().isUsedByPlayer = false;
                        current.GetComponent<PoleDot>().left.GetComponent<PoleLine>().isUsedByPlayer = false;
                        myPole.playerPath.dots.Remove(current);
                        myPole.playerPath.lines.Remove(current.GetComponent<PoleDot>().left);
                    }
                }
            }
            else if (current.GetComponent<PoleDot>().AllowedToLeft())
            {
                GameObject next;
                if (current.GetComponent<PoleDot>().left.GetComponent<PoleLine>().first == current)
                    next = current.GetComponent<PoleDot>().left.GetComponent<PoleLine>().second;
                else next = current.GetComponent<PoleDot>().left.GetComponent<PoleLine>().first;
                playerPathLinesOnScreen.Add(Instantiate(PlayerPathHorizontalLinePrefab, playerPathDotsOnScreen[playerPathDotsOnScreen.Count - 1].transform.position - stepx * 0.5f, PlayerPathHorizontalLinePrefab.transform.rotation));
                playerPathDotsOnScreen.Add(Instantiate(PlayerPathDotPrefab, playerPathDotsOnScreen[playerPathDotsOnScreen.Count - 1].transform.position - stepz, PlayerPathDotPrefab.transform.rotation));
                current.GetComponent<PoleDot>().left.GetComponent<PoleLine>().isUsedByPlayer = true;
                next.GetComponent<PoleDot>().isUsedByPlayer = true;
                myPole.playerPath.dots.Add(next);
                myPole.playerPath.lines.Add(current.GetComponent<PoleDot>().left);
            }
        }
    }
    public void ControllerRight()
    {
        if (playerIsActive)
        {
            GameObject current = myPole.playerPath.dots[myPole.playerPath.dots.Count - 1];
            if (current != myPole.start)
            {
                if (current.GetComponent<PoleDot>().AllowedToRight())
                {
                    GameObject next;
                    if (current.GetComponent<PoleDot>().right.GetComponent<PoleLine>().first == current)
                        next = current.GetComponent<PoleDot>().right.GetComponent<PoleLine>().second;
                    else next = current.GetComponent<PoleDot>().right.GetComponent<PoleLine>().first;
                    if (!next.GetComponent<PoleDot>().isUsedByPlayer)
                    {
                        playerPathLinesOnScreen.Add(Instantiate(PlayerPathHorizontalLinePrefab, playerPathDotsOnScreen[playerPathDotsOnScreen.Count - 1].transform.position + stepx * 0.5f, PlayerPathHorizontalLinePrefab.transform.rotation));
                        playerPathDotsOnScreen.Add(Instantiate(PlayerPathDotPrefab, playerPathDotsOnScreen[playerPathDotsOnScreen.Count - 1].transform.position + stepx, PlayerPathDotPrefab.transform.rotation));
                        current.GetComponent<PoleDot>().right.GetComponent<PoleLine>().isUsedByPlayer = true;
                        next.GetComponent<PoleDot>().isUsedByPlayer = true;
                        myPole.playerPath.dots.Add(next);
                        myPole.playerPath.lines.Add(current.GetComponent<PoleDot>().right);
                    }
                    else if (current.GetComponent<PoleDot>().right.GetComponent<PoleLine>().isUsedByPlayer)
                    {
                        Destroy(playerPathDotsOnScreen[playerPathDotsOnScreen.Count - 1]);
                        Destroy(playerPathLinesOnScreen[playerPathLinesOnScreen.Count - 1]);
                        playerPathDotsOnScreen.RemoveAt(playerPathDotsOnScreen.Count - 1);
                        playerPathLinesOnScreen.RemoveAt(playerPathLinesOnScreen.Count - 1);
                        current.GetComponent<PoleDot>().isUsedByPlayer = false;
                        current.GetComponent<PoleDot>().right.GetComponent<PoleLine>().isUsedByPlayer = false;
                        myPole.playerPath.dots.Remove(current);
                        myPole.playerPath.lines.Remove(current.GetComponent<PoleDot>().right);
                    }
                }
            }
            else if (current.GetComponent<PoleDot>().AllowedToRight())
            {
                GameObject next;
                if (current.GetComponent<PoleDot>().right.GetComponent<PoleLine>().first == current)
                    next = current.GetComponent<PoleDot>().right.GetComponent<PoleLine>().second;
                else next = current.GetComponent<PoleDot>().right.GetComponent<PoleLine>().first;
                playerPathLinesOnScreen.Add(Instantiate(PlayerPathHorizontalLinePrefab, playerPathDotsOnScreen[playerPathDotsOnScreen.Count - 1].transform.position + stepx * 0.5f, PlayerPathHorizontalLinePrefab.transform.rotation));
                playerPathDotsOnScreen.Add(Instantiate(PlayerPathDotPrefab, playerPathDotsOnScreen[playerPathDotsOnScreen.Count - 1].transform.position + stepx, PlayerPathDotPrefab.transform.rotation));
                current.GetComponent<PoleDot>().right.GetComponent<PoleLine>().isUsedByPlayer = true;
                next.GetComponent<PoleDot>().isUsedByPlayer = true;
                myPole.playerPath.dots.Add(next);
                myPole.playerPath.lines.Add(current.GetComponent<PoleDot>().right);
            }
        }
    }
    public void ControllerStartFinish()
    {
        if (!playerIsActive)
        {
            foreach (GameObject point in myPole.eltsManager.unsolvedPoints)
            {
                point.GetComponent<PoleEltPoint>().NormalizeColor();
            }
            foreach (GameObject gameObject in GameObject.FindGameObjectsWithTag("Path"))
                Destroy(gameObject);
            myPole.start.GetComponent<PoleDot>().isUsedByPlayer = true;
            playerPathDotsOnScreen.Add(Instantiate(PlayerPathStartPrefab, transform.position + stepx * myPole.start.GetComponent<PoleDot>().position_x + stepz * myPole.start.GetComponent<PoleDot>().position_y + pathstepy, PlayerPathStartPrefab.transform.rotation));
            myPole.playerPath.dots.Add(myPole.start);
        }
        else
        {
            if (myPole.playerPath.dots[myPole.playerPath.dots.Count - 1] == myPole.finish)
            {
                if (myPole.eltsManager.CheckSolution())
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
                    foreach(GameObject point in myPole.eltsManager.unsolvedPoints)
                    {
                        point.GetComponent<PoleEltPoint>().ShowUnsolved();
                    }
                }
            }
            else foreach(GameObject path in GameObject.FindGameObjectsWithTag("Path"))
                {
                    path.GetComponent<Renderer>().material.Lerp(path.GetComponent<Renderer>().material, PlayerWrongPathMaterial, 1f);
                }
            foreach (GameObject dot in myPole.playerPath.dots)
                dot.GetComponent<PoleDot>().isUsedByPlayer = false;
            foreach (GameObject line in myPole.playerPath.lines)
                line.GetComponent<PoleLine>().isUsedByPlayer = false;
            myPole.playerPath.dots.Clear();
            myPole.playerPath.lines.Clear();
            playerPathDotsOnScreen.Clear(); 
            playerPathLinesOnScreen.Clear();
        }
        playerIsActive = !playerIsActive;
    }
    public void ButtonCreate()
    {
        playerIsActive = false;
        foreach (GameObject dot in myPole.playerPath.dots)
            dot.GetComponent<PoleDot>().isUsedByPlayer = false;
        foreach (GameObject line in myPole.playerPath.lines)
            line.GetComponent<PoleLine>().isUsedByPlayer = false;
        myPole.playerPath.dots.Clear();
        myPole.playerPath.lines.Clear();
        playerPathDotsOnScreen.Clear();
        playerPathLinesOnScreen.Clear();
        foreach (GameObject gameObject in GameObject.FindGameObjectsWithTag("Path"))
            Destroy(gameObject);
        foreach (GameObject gameObject in GameObject.FindGameObjectsWithTag("EltPoint"))
            Destroy(gameObject);
        myPole.ClearPole();
        if (mode)
        {
            myPole.SetStart(myPole.myRandGen.GetRandom() % myPole.GetSize(), myPole.myRandGen.GetRandom() % myPole.GetSize());
            int x = myPole.myRandGen.GetRandom() % myPole.GetSize();
            int y = myPole.myRandGen.GetRandom() % myPole.GetSize();
            while (myPole.start.GetComponent<PoleDot>().position_x == x && myPole.start.GetComponent<PoleDot>().position_y ==y)
            {
                x = myPole.myRandGen.GetRandom() % myPole.GetSize();
                y = myPole.myRandGen.GetRandom() % myPole.GetSize();
            }
            myPole.SetFinish(x, y);
            myPole.CreateSolution();
            myPole.GeneratePoints(7);
            for (int i = 0; i < myPole.GetSize(); i++)
            {
                for (int j = 0; j < myPole.GetSize(); j++)
                {
                    if (myPole.poleDots[i][j].GetComponent<PoleDot>().hasPoint)
                    {
                        myPole.poleDots[i][j].GetComponent<PoleDot>().point = Instantiate(PointPrefab, transform.position + stepx * j + stepz * i + new Vector3(0, 1, 0), transform.rotation);
                        myPole.poleDots[i][j].GetComponent<PoleDot>().point.GetComponent<PoleEltPoint>().SetDot(myPole.poleDots[i][j]);
                        myPole.eltsManager.points.Add(myPole.poleDots[i][j].GetComponent<PoleDot>().point);
                    }
                }
            }
            for (int i = 0; i < myPole.GetSize(); i++)
            {
                for (int j = 0; j < myPole.GetSize(); j++)
                {
                    if (j < myPole.GetSize() - 1)
                        if (myPole.poleDots[i][j].GetComponent<PoleDot>().right != null)
                        {
                            if (myPole.poleDots[i][j].GetComponent<PoleDot>().right.GetComponent<PoleLine>().hasPoint)
                            {
                                myPole.poleDots[i][j].GetComponent<PoleDot>().right.GetComponent<PoleLine>().point = Instantiate(PointPrefab, transform.position + stepx * 0.5f + stepx * j + stepz * i + new Vector3(0, 1, 0), transform.rotation);
                                myPole.poleDots[i][j].GetComponent<PoleDot>().right.GetComponent<PoleLine>().point.GetComponent<PoleEltPoint>().SetLine(myPole.poleDots[i][j].GetComponent<PoleDot>().right);
                                myPole.eltsManager.points.Add(myPole.poleDots[i][j].GetComponent<PoleDot>().right.GetComponent<PoleLine>().point);
                            }
                        }
                }
                if (i < myPole.GetSize() - 1)
                    for (int j = 0; j < myPole.GetSize(); j++)
                    {
                        if (myPole.poleDots[i][j].GetComponent<PoleDot>().down != null)
                        {
                            if (myPole.poleDots[i][j].GetComponent<PoleDot>().down.GetComponent<PoleLine>().hasPoint)
                            {
                                myPole.poleDots[i][j].GetComponent<PoleDot>().down.GetComponent<PoleLine>().point = Instantiate(PointPrefab, transform.position + stepz * 0.5f + stepx * j + stepz * i + new Vector3(0, 1, 0), transform.rotation);
                                myPole.poleDots[i][j].GetComponent<PoleDot>().down.GetComponent<PoleLine>().point.GetComponent<PoleEltPoint>().SetLine(myPole.poleDots[i][j].GetComponent<PoleDot>().down);
                                myPole.eltsManager.points.Add(myPole.poleDots[i][j].GetComponent<PoleDot>().down.GetComponent<PoleLine>().point);
                            }
                        }
                    }
            }
            seed = myPole.myRandGen.seed;
        }
        mode = !mode;
        pathIsShown = false;
    }
    public void ButtonShowSolution()
    {
        playerIsActive = false;
        foreach (GameObject dot in myPole.playerPath.dots)
            dot.GetComponent<PoleDot>().isUsedByPlayer = false;
        foreach (GameObject line in myPole.playerPath.lines)
            line.GetComponent<PoleLine>().isUsedByPlayer = false;
        myPole.playerPath.dots.Clear();
        myPole.playerPath.lines.Clear();
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
            for (int i = 0; i < myPole.systemPath.dots.Count; i++)
            {
                if (i < myPole.systemPath.dots.Count - 1)
                {
                    if (i == 0) Instantiate(PathStartPrefab, transform.position + stepx * myPole.systemPath.dots[i].GetComponent<PoleDot>().position_x + stepz * myPole.systemPath.dots[i].GetComponent<PoleDot>().position_y + pathstepy, transform.rotation);
                    else Instantiate(PathDotPrefab, transform.position + stepx * myPole.systemPath.dots[i].GetComponent<PoleDot>().position_x + stepz * myPole.systemPath.dots[i].GetComponent<PoleDot>().position_y + pathstepy, transform.rotation);
                    if (myPole.systemPath.lines[i] == myPole.systemPath.dots[i].GetComponent<PoleDot>().up) Instantiate(PathVerticalLinePrefab, transform.position - stepz * 0.5f + stepx * myPole.systemPath.dots[i].GetComponent<PoleDot>().position_x + stepz * myPole.systemPath.dots[i].GetComponent<PoleDot>().position_y + pathstepy, VerticalLinePrefab.transform.rotation);
                    else if (myPole.systemPath.lines[i] == myPole.systemPath.dots[i].GetComponent<PoleDot>().down) Instantiate(PathVerticalLinePrefab, transform.position + stepz * 0.5f + stepx * myPole.systemPath.dots[i].GetComponent<PoleDot>().position_x + stepz * myPole.systemPath.dots[i].GetComponent<PoleDot>().position_y + pathstepy, VerticalLinePrefab.transform.rotation);
                    else if (myPole.systemPath.lines[i] == myPole.systemPath.dots[i].GetComponent<PoleDot>().left) Instantiate(PathHorizontalLinePrefab, transform.position - stepx * 0.5f + stepx * myPole.systemPath.dots[i].GetComponent<PoleDot>().position_x + stepz * myPole.systemPath.dots[i].GetComponent<PoleDot>().position_y + pathstepy, PathHorizontalLinePrefab.transform.rotation);
                    else if (myPole.systemPath.lines[i] == myPole.systemPath.dots[i].GetComponent<PoleDot>().right) Instantiate(PathHorizontalLinePrefab, transform.position + stepx * 0.5f + stepx * myPole.systemPath.dots[i].GetComponent<PoleDot>().position_x + stepz * myPole.systemPath.dots[i].GetComponent<PoleDot>().position_y + pathstepy, PathHorizontalLinePrefab.transform.rotation);
                }
            }
            Instantiate(PathFinishPrefab, transform.position + stepx * myPole.finish.GetComponent<PoleDot>().position_x + stepz * myPole.finish.GetComponent<PoleDot>().position_y + pathstepy, transform.rotation);
        }
        pathIsShown = !pathIsShown;
    }

    void Start() {
        int size = 5;
        myPole = new Pole(size, seed,DotPrefab,StartPrefab,FinishPrefab,HorizontalLinePrefab,VerticalLinePrefab);
        playerPathLinesOnScreen = new List<GameObject>();
        playerPathDotsOnScreen = new List<GameObject>();
    }

    void Update()
    {
        
    }

    class MyRandom
    {
        public int seed;
        public MyRandom(int k)
        {

            seed = k;
        }
        public int GetRandom()
        {
            seed = ( seed * 106 + 1283) % 6075;
            return seed;
        }
    }

    class PathDotStack
    {
        class DotNode
        {
            public GameObject dot;
            public DotNode next;
        };
        private int size;
        DotNode head;
        public void AddDot(GameObject newDot)
        {
            DotNode curNode = new DotNode
            {
                dot = newDot
            };
            if (head == null)
            {
                head = curNode;
                size = 1;
            }
            else
            {
                curNode.next = head;
                head = curNode;
                size++;
            }
        }
        public GameObject GetDot()
        {
            DotNode tmp = head;
            head = head.next;
            size--;
            return tmp.dot;
        }
        public bool IsEmpty()
        {
            return head == null;
        }
        public int PathLength()
        {
            return size;
        }
    };

    class PoleElts
    {
        public List<GameObject> points;
        public List<GameObject> unsolvedPoints;
        public PoleElts()
        {
            points = new List<GameObject>();
            unsolvedPoints = new List<GameObject>();
        }
        public bool CheckSolution()
        {
            bool isSolved = true;
            unsolvedPoints.Clear();
            foreach (GameObject p in points)
            {
                if (!p.GetComponent<PoleEltPoint>().IsSolvedByPlayer()) unsolvedPoints.Add(p);
                isSolved = isSolved && p.GetComponent<PoleEltPoint>().IsSolvedByPlayer();
            }

            return isSolved;
        }
    }

    class PolePath
    {
        public List<GameObject> dots;
        public List<GameObject> lines;
        public PolePath()
        {
            dots = new List<GameObject>();
            lines = new List<GameObject>();
        }
    }

    class Pole
    {
        public PoleElts eltsManager;
        public MyRandom myRandGen;
        public GameObject start;
        public GameObject finish;
        private readonly int poleSize;
        PathDotStack dotData;
        public GameObject[][] poleDots;
        public List<GameObject> poleLines;
        public PolePath systemPath;
        public PolePath playerPath;

        private GameObject DotPrefab;
        private GameObject VerticalLinePrefab;
        private GameObject HorizontalLinePrefab;
        private GameObject StartPrefab;
        private GameObject FinishPrefab;

        public bool FindPath(GameObject begin, GameObject end, int[][] ways)
        {
            begin.GetComponent<PoleDot>().isUsedBySolution = true;
            if (begin == end)
            {
                if (dotData.PathLength() < poleSize * 3)
                {
                    return false;
                }
                return true;
            }
            bool[] tries = { true, true, true, true };
            dotData.AddDot(begin);
            bool triesLeft = true;
            while (triesLeft)
            {
                int k = myRandGen.GetRandom() % 4;
                while (!tries[k])
                {
                    k = myRandGen.GetRandom() % 4;
                }
                switch (k)
                {
                    case 0:
                        if (begin.GetComponent<PoleDot>().position_y > 0 && ways[begin.GetComponent<PoleDot>().position_y - 1][begin.GetComponent<PoleDot>().position_x] == 0 && tries[0])
                        {
                            ways[begin.GetComponent<PoleDot>().position_y - 1][begin.GetComponent<PoleDot>().position_x] = 1;
                            begin.GetComponent<PoleDot>().up.GetComponent<PoleLine>().isUsedBySolution = true;
                            if (FindPath(poleDots[begin.GetComponent<PoleDot>().position_y - 1][begin.GetComponent<PoleDot>().position_x], end, ways)) return true;
                            else
                            {
                                ways[begin.GetComponent<PoleDot>().position_y - 1][begin.GetComponent<PoleDot>().position_x] = 0;
                                begin.GetComponent<PoleDot>().up.GetComponent<PoleLine>().isUsedBySolution = false;
                                tries[0] = false;
                            }
                        }
                        else tries[0] = false;
                        break;
                    case 1:
                        if (begin.GetComponent<PoleDot>().position_x < poleSize - 1 && ways[begin.GetComponent<PoleDot>().position_y][begin.GetComponent<PoleDot>().position_x + 1] == 0 && tries[1])
                        {
                            ways[begin.GetComponent<PoleDot>().position_y][begin.GetComponent<PoleDot>().position_x + 1] = 1;
                            begin.GetComponent<PoleDot>().right.GetComponent<PoleLine>().isUsedBySolution = true;
                            if (FindPath(poleDots[begin.GetComponent<PoleDot>().position_y][begin.GetComponent<PoleDot>().position_x + 1], end, ways)) return true;
                            else
                            {
                                ways[begin.GetComponent<PoleDot>().position_y][begin.GetComponent<PoleDot>().position_x + 1] = 0;
                                begin.GetComponent<PoleDot>().right.GetComponent<PoleLine>().isUsedBySolution = false;
                                tries[1] = false;
                            }
                        }
                        else tries[1] = false;
                        break;
                    case 2:
                        if (begin.GetComponent<PoleDot>().position_y < poleSize - 1 && ways[begin.GetComponent<PoleDot>().position_y + 1][begin.GetComponent<PoleDot>().position_x] == 0 && tries[2])
                        {
                            ways[begin.GetComponent<PoleDot>().position_y + 1][begin.GetComponent<PoleDot>().position_x] = 1;
                            begin.GetComponent<PoleDot>().down.GetComponent<PoleLine>().isUsedBySolution = true;
                            if (FindPath(poleDots[begin.GetComponent<PoleDot>().position_y + 1][begin.GetComponent<PoleDot>().position_x], end, ways)) return true;
                            else
                            {
                                ways[begin.GetComponent<PoleDot>().position_y + 1][begin.GetComponent<PoleDot>().position_x] = 0;
                                begin.GetComponent<PoleDot>().down.GetComponent<PoleLine>().isUsedBySolution = false;
                                tries[2] = false;
                            }
                        }
                        else tries[2] = false;
                        break;
                    case 3:
                        if (begin.GetComponent<PoleDot>().position_x > 0 && ways[begin.GetComponent<PoleDot>().position_y][begin.GetComponent<PoleDot>().position_x - 1] == 0 && tries[3])
                        {
                            ways[begin.GetComponent<PoleDot>().position_y][begin.GetComponent<PoleDot>().position_x - 1] = 1;
                            begin.GetComponent<PoleDot>().left.GetComponent<PoleLine>().isUsedBySolution = true;
                            if (FindPath(poleDots[begin.GetComponent<PoleDot>().position_y][begin.GetComponent<PoleDot>().position_x - 1], end, ways)) return true;
                            else
                            {
                                ways[begin.GetComponent<PoleDot>().position_y][begin.GetComponent<PoleDot>().position_x - 1] = 0;
                                begin.GetComponent<PoleDot>().left.GetComponent<PoleLine>().isUsedBySolution = false;
                                tries[3] = false;
                            }
                        }
                        else tries[3] = false;
                        break;
                }
                if (!tries[0] && !tries[1] && !tries[2] && !tries[3])
                {
                    triesLeft = false;
                }
            }
            ways[begin.GetComponent<PoleDot>().position_y][begin.GetComponent<PoleDot>().position_x] = 0;
            begin.GetComponent<PoleDot>().isUsedBySolution = false;
            dotData.GetDot();
            return false;
        }
        public int GetSize()
        {
            return poleSize;
        }
        public Pole(int size, int seed, GameObject dot, GameObject st, GameObject fin, GameObject hor, GameObject ver)
        {
            DotPrefab = dot;
            StartPrefab = st;
            FinishPrefab = fin;
            HorizontalLinePrefab = hor;
            VerticalLinePrefab = ver;
            playerPath = new PolePath();
            systemPath = new PolePath();
            myRandGen = new MyRandom(seed);
            poleSize = size;
            eltsManager = new PoleElts();
            poleDots = new GameObject[size][];
            poleLines = new List<GameObject>();
            for (int y = 0; y < size; y++)
            {
                poleDots[y] = new GameObject[size];
                for (int x = 0; x < size; x++)
                {
                    poleDots[y][x] = Instantiate(DotPrefab, stepx * x + stepz * y, DotPrefab.transform.rotation);
                    poleDots[y][x].GetComponent<PoleDot>().position_x = x;
                    poleDots[y][x].GetComponent<PoleDot>().position_y = y;
                }
            }
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size - 1; x++)
                {
                    GameObject line = Instantiate(HorizontalLinePrefab, stepx * 0.5f + stepx * x + stepz * y, HorizontalLinePrefab.transform.rotation);
                    line.GetComponent<PoleLine>().first = poleDots[y][x];
                    line.GetComponent<PoleLine>().second = poleDots[y][x + 1];
                    poleDots[y][x].GetComponent<PoleDot>().AddLine(line, poleDots[y][x + 1]);
                    poleDots[y][x + 1].GetComponent<PoleDot>().AddLine(line, poleDots[y][x]);
                    poleLines.Add(line);
                }
                if (y < size - 1)
                    for (int x = 0; x < size; x++)
                    {
                        GameObject line = Instantiate(VerticalLinePrefab, stepz * 0.5f + stepx * x + stepz * y, VerticalLinePrefab.transform.rotation); ;
                        line.GetComponent<PoleLine>().first = poleDots[y][x];
                        line.GetComponent<PoleLine>().second = poleDots[y + 1][x];
                        poleDots[y][x].GetComponent<PoleDot>().AddLine(line, poleDots[y + 1][x]);
                        poleDots[y + 1][x].GetComponent<PoleDot>().AddLine(line, poleDots[y][x]);
                        poleLines.Add(line);
                    }
            }
        }
        public void SetStart(int x, int y)
        {
            Instantiate(StartPrefab, stepx * x + stepz * y, StartPrefab.transform.rotation);
            start = poleDots[y][x];
        }
        public void SetFinish(int x, int y)
        {
            Instantiate(FinishPrefab, stepx * x + stepz * y, FinishPrefab.transform.rotation);
            finish = poleDots[y][x];
        }
        public void SetNewSeed(int k)
        {
            myRandGen = new MyRandom(k);
        }
        public void GeneratePoints(int numberOfPoints)
        {
            eltsManager.points.Clear();
            numberOfPoints = System.Math.Min(numberOfPoints, systemPath.dots.Count + systemPath.lines.Count - 1);
            for (int i = 0; i < numberOfPoints; i++)
            {
                int r = myRandGen.GetRandom();
                r %= systemPath.dots.Count + systemPath.lines.Count;
                if (r % 2 == 0)
                {
                    if (!systemPath.dots[(r / 2)].GetComponent<PoleDot>().hasPoint)
                    {
                        systemPath.dots[(r / 2)].GetComponent<PoleDot>().hasPoint = true;
                    }
                    else i--;
                }
                else
                {
                    if (!systemPath.lines[(r - 1) / 2].GetComponent<PoleLine>().hasPoint)
                    {
                        systemPath.lines[(r - 1) / 2].GetComponent<PoleLine>().hasPoint = true;
                    }
                    else i--;
                }
            }
        }
        public void CreateSolution()
        {
            int[][] ways = new int[poleSize][];
            for (int i = 0; i < poleSize; i++)
            {
                ways[i] = new int[poleSize];
                for (int j = 0; j < poleSize; j++)
                {
                    ways[i][j] = 0;
                }
            }
            ways[start.GetComponent<PoleDot>().position_y][start.GetComponent<PoleDot>().position_x] = 1;
            dotData = new PathDotStack();
            bool isFound = FindPath(start, finish, ways);
            if (isFound)
            {
                GameObject prevDot;
                GameObject curDot = finish;
                systemPath.dots.Add(curDot);
                while (!dotData.IsEmpty())
                {
                    prevDot = dotData.GetDot();
                    systemPath.dots.Add(prevDot);
                    if (curDot.GetComponent<PoleDot>().position_x < prevDot.GetComponent<PoleDot>().position_x) systemPath.lines.Add(curDot.GetComponent<PoleDot>().right);
                    else if (curDot.GetComponent<PoleDot>().position_x > prevDot.GetComponent<PoleDot>().position_x) systemPath.lines.Add(curDot.GetComponent<PoleDot>().left);
                    else if (curDot.GetComponent<PoleDot>().position_y < prevDot.GetComponent<PoleDot>().position_y) systemPath.lines.Add(curDot.GetComponent<PoleDot>().down);
                    else if (curDot.GetComponent<PoleDot>().position_y > prevDot.GetComponent<PoleDot>().position_y) systemPath.lines.Add(curDot.GetComponent<PoleDot>().up);
                    curDot = prevDot;
                }
                systemPath.dots.Reverse();
                systemPath.lines.Reverse();
            }
        }
        public void ClearPole()
        {
            for (int y = 0; y < poleSize; y++)
            {
                for (int x = 0; x < poleSize; x++)
                {
                    poleDots[y][x].GetComponent<PoleDot>().isUsedBySolution = false;
                    poleDots[y][x].GetComponent<PoleDot>().hasPoint = false;
                }
            }
            for (int n = 0; n < poleLines.Count; n++)
            {
                poleLines[n].GetComponent<PoleLine>().isUsedBySolution = false;
                poleLines[n].GetComponent<PoleLine>().hasPoint = false;
            }
            foreach (GameObject temp in GameObject.FindGameObjectsWithTag("PoleTemp")) Destroy(temp);
            systemPath.dots.Clear();
            systemPath.lines.Clear();
        }
    }
}
