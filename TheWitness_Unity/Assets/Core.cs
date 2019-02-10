using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Core : MonoBehaviour {

    public Transform DotPrefab;
    public GameObject VerticalLinePrefab;
    public GameObject HorizontalLinePrefab;
    public Transform StartPrefab;
    public Transform FinishPrefab;
    public Transform PointPrefab;

    public Transform PathDotPrefab;
    public GameObject PathVerticalLinePrefab;
    public GameObject PathHorizontalLinePrefab;
    public Transform PathStartPrefab;
    public Transform PathFinishPrefab;
    private static Vector3 pathstepy = new Vector3(0, 0.5f, 0);
    private static Vector3 stepx = new Vector3(5, 0, 0);
    private static Vector3 stepz = new Vector3(0, 0, -5);
    public int seed = 95;
    public bool mode = true;
    public bool pathIsShown = false;
    private Pole myPole;
    public void ButtonCreate()
    {
        foreach (GameObject gameObject in GameObject.FindGameObjectsWithTag("Path"))
            Destroy(gameObject);

        foreach (GameObject gameObject in GameObject.FindGameObjectsWithTag("EltPoint"))
            Destroy(gameObject);
        if (mode)
        {
            myPole.ClearPole();
            myPole.SetStart(0, 0);
            myPole.SetFinish(myPole.GetSize() - 1, myPole.GetSize() - 1);
            myPole.CreateSolution();
            myPole.GeneratePoints(7);
            for (int i = 0; i < myPole.GetSize(); i++)
            {
                for (int j = 0; j < myPole.GetSize(); j++)
                {
                    if (myPole.poleDots[i][j] == myPole.start) Instantiate(StartPrefab, transform.position + stepx * j + stepz * i, transform.rotation);
                    else if (myPole.poleDots[i][j] == myPole.finish) Instantiate(FinishPrefab, transform.position + stepx * j + stepz * i, transform.rotation);
                    else Instantiate(DotPrefab, transform.position + stepx * j + stepz * i, transform.rotation);
                    if (myPole.poleDots[i][j].point != null) Instantiate(PointPrefab, transform.position + stepx * j + stepz * i + new Vector3(0, 1, 0), transform.rotation);
                }
            }
            for (int i = 0; i < myPole.GetSize(); i++)
            {
                for (int j = 0; j < myPole.GetSize(); j++)
                {
                    Instantiate(DotPrefab, transform.position + stepx * j + stepz * i, transform.rotation);
                    if (j < myPole.GetSize() - 1)
                        if (myPole.poleDots[i][j].right != null)
                        {
                            Instantiate(HorizontalLinePrefab, transform.position + stepx * 0.5f + stepx * j + stepz * i, HorizontalLinePrefab.transform.rotation);
                            if (myPole.poleDots[i][j].right.point != null) Instantiate(PointPrefab, transform.position + stepx * 0.5f + stepx * j + stepz * i + new Vector3(0, 1, 0), transform.rotation);
                        }
                }

                if (i < myPole.GetSize() - 1)

                    for (int j = 0; j < myPole.GetSize(); j++)
                    {
                        if (myPole.poleDots[i][j].down != null)
                        {
                            Instantiate(VerticalLinePrefab, transform.position + stepz * 0.5f + stepx * j + stepz * i, VerticalLinePrefab.transform.rotation);
                            if (myPole.poleDots[i][j].down.point != null) Instantiate(PointPrefab, transform.position + stepz * 0.5f + stepx * j + stepz * i + new Vector3(0, 1, 0), transform.rotation);
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
        if (pathIsShown)
        {
            foreach (GameObject gameObject in GameObject.FindGameObjectsWithTag("Path"))
                Destroy(gameObject);
        }
        else
        {
            Debug.Log(myPole.path.dots.Count);
            for (int i = 0; i < myPole.path.dots.Count; i++)
            {
                Debug.Log(i);
                if (i < myPole.path.dots.Count - 1)
                {
                    if (i == 0) Instantiate(PathStartPrefab, transform.position + stepx * myPole.path.dots[i].position_x + stepz * myPole.path.dots[i].position_y + pathstepy, transform.rotation);
                    else Instantiate(PathDotPrefab, transform.position + stepx * myPole.path.dots[i].position_x + stepz * myPole.path.dots[i].position_y + pathstepy, transform.rotation);
                    if (myPole.path.lines[i] == myPole.path.dots[i].up) Instantiate(PathVerticalLinePrefab, transform.position - stepz * 0.5f + stepx * myPole.path.dots[i].position_x + stepz * myPole.path.dots[i].position_y + pathstepy, VerticalLinePrefab.transform.rotation);
                    else if (myPole.path.lines[i] == myPole.path.dots[i].down) Instantiate(PathVerticalLinePrefab, transform.position + stepz * 0.5f + stepx * myPole.path.dots[i].position_x + stepz * myPole.path.dots[i].position_y + pathstepy, VerticalLinePrefab.transform.rotation);
                    else if (myPole.path.lines[i] == myPole.path.dots[i].left) Instantiate(PathHorizontalLinePrefab, transform.position - stepx * 0.5f + stepx * myPole.path.dots[i].position_x + stepz * myPole.path.dots[i].position_y + pathstepy, PathHorizontalLinePrefab.transform.rotation);
                    else if (myPole.path.lines[i] == myPole.path.dots[i].right) Instantiate(PathHorizontalLinePrefab, transform.position + stepx * 0.5f + stepx * myPole.path.dots[i].position_x + stepz * myPole.path.dots[i].position_y + pathstepy, PathHorizontalLinePrefab.transform.rotation);
                }
            }
            Instantiate(PathFinishPrefab, transform.position + stepx * myPole.finish.position_x + stepz * myPole.finish.position_y + pathstepy, transform.rotation);
        }
        pathIsShown = !pathIsShown;
    }

    // Use this for initialization
    void Start() {
        int size = 5;
        myPole = new Pole(size, seed);

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

    class PoleEltPoint
    {
        PoleDot attachedDot;
        PoleLine attachedLine;
        public PoleEltPoint(PoleDot dot)
        {
            attachedDot = dot;
        }
        public PoleEltPoint(PoleLine line)
        {
            attachedLine = line;
        }
        public bool IsSolved()
        {
            if (attachedDot != null) return attachedDot.isUsed;
            else return attachedLine.isUsed;
        }
    }

    class PoleDot
    {
        public PoleLine up;
        public PoleLine down;
        public PoleLine left;
        public PoleLine right;
        public bool isUsed;
        public PoleEltPoint point;
        public int position_x;
        public int position_y;
        public PoleDot(int x, int y)
        {
            position_x = x;
            position_y = y;
            up = null;
            down = null;
            right = null;
            left = null;
            isUsed = false;
        }
        public void AddLine(PoleLine newLine, PoleDot anotherDot)
        {
            if (position_x < anotherDot.position_x)
            {
                right = newLine;
            }
            else if (position_x > anotherDot.position_x)
            {
                left = newLine;
            }
            else if (position_y < anotherDot.position_y)
            {
                down = newLine;
            }
            else if (position_y > anotherDot.position_y)
            {
                up = newLine;
            }
        }
        bool AllowedToUp()
        {
            return (up != null);
        }
        bool AllowedToDown()
        {
            return (down != null);
        }
        bool AllowedToLeft()
        {
            return (left != null);
        }
        bool AllowedToRight()
        {
            return (right != null);
        }
    };

    class PathDotStack
    {
        class DotNode
        {
            public PoleDot dot;
            public DotNode next;
        };
        private int size;
        DotNode head;
        public void AddDot(PoleDot newDot)
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
        public PoleDot GetDot()
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

    class PoleLine
    {
        public PoleDot first;
        public PoleDot second;
        public bool isUsed;
        public PoleEltPoint point;
        public PoleLine(PoleDot firstDot, PoleDot secondDot)
        {
            first = firstDot;
            second = secondDot;
            isUsed = false;
        }
    };

    class PoleSquere
    {

    };

    class PoleElts
    {
        public List<PoleEltPoint> points;
        public List<PoleEltPoint> unsolvedPoints;
        public PoleElts()
        {
            points = new List<PoleEltPoint>();
            unsolvedPoints = new List<PoleEltPoint>();
        }
        public bool CheckSolution()
        {
            bool isSolved = true;
            unsolvedPoints.Clear();
            foreach (PoleEltPoint p in points)
            {
                if (!p.IsSolved()) unsolvedPoints.Add(p);
                isSolved = isSolved && p.IsSolved();
            }

            return isSolved;
        }
    }

    class PolePath
    {
        public List<PoleDot> dots;
        public List<PoleLine> lines;
        public PolePath()
        {
            dots = new List<PoleDot>();
            lines = new List<PoleLine>();
        }
    }

    class Pole
    {
        PoleElts eltsManager;
        public MyRandom myRandGen;
        public PoleDot start;
        public PoleDot finish;
        private readonly int poleSize;
        PathDotStack dotData;
        public PoleDot[][] poleDots;
        public List<PoleLine> poleLines;
        public PolePath path;

        public bool FindPath(PoleDot begin, PoleDot end, int[][] ways)
        {

            begin.isUsed = true;
            if (begin == end)
            {
                if (dotData.PathLength() < poleSize * 3)
                {
                    return false;
                }
                //if (!eltsManager.CheckSolution())
                //{
                //    return false;
                //}
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

                        if (begin.position_y > 0 && ways[begin.position_y - 1][begin.position_x] == 0 && tries[0])
                        {
                            ways[begin.position_y - 1][begin.position_x] = 1;
                            begin.up.isUsed = true;
                            if (FindPath(poleDots[begin.position_y - 1][begin.position_x], end, ways)) return true;
                            else
                            {
                                ways[begin.position_y - 1][begin.position_x] = 0;
                                begin.up.isUsed = false;
                                tries[0] = false;
                            }
                        }
                        else tries[0] = false;
                        break;
                    case 1:

                        if (begin.position_x < poleSize - 1 && ways[begin.position_y][begin.position_x + 1] == 0 && tries[1])
                        {
                            ways[begin.position_y][begin.position_x + 1] = 1;
                            begin.right.isUsed = true;
                            if (FindPath(poleDots[begin.position_y][begin.position_x + 1], end, ways)) return true;
                            else
                            {
                                ways[begin.position_y][begin.position_x + 1] = 0;
                                begin.right.isUsed = false;
                                tries[1] = false;
                            }
                        }
                        else tries[1] = false;
                        break;
                    case 2:

                        if (begin.position_y < poleSize - 1 && ways[begin.position_y + 1][begin.position_x] == 0 && tries[2])
                        {
                            ways[begin.position_y + 1][begin.position_x] = 1;
                            begin.down.isUsed = true;
                            if (FindPath(poleDots[begin.position_y + 1][begin.position_x], end, ways)) return true;
                            else
                            {
                                ways[begin.position_y + 1][begin.position_x] = 0;
                                begin.down.isUsed = false;
                                tries[2] = false;
                            }
                        }
                        else tries[2] = false;
                        break;
                    case 3:

                        if (begin.position_x > 0 && ways[begin.position_y][begin.position_x - 1] == 0 && tries[3])
                        {
                            ways[begin.position_y][begin.position_x - 1] = 1;
                            begin.left.isUsed = true;
                            if (FindPath(poleDots[begin.position_y][begin.position_x - 1], end, ways)) return true;
                            else
                            {
                                ways[begin.position_y][begin.position_x - 1] = 0;
                                begin.left.isUsed = false;
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
            ways[begin.position_y][begin.position_x] = 0;
            begin.isUsed = false;
            dotData.GetDot();
            return false;
        }
        public int GetSize()
        {
            return poleSize;
        }
        public Pole(int size, int seed)
        {
            path = new PolePath();
            myRandGen = new MyRandom(seed);
            poleSize = size;
            eltsManager = new PoleElts();
            poleDots = new PoleDot[size][];
            poleLines = new List<PoleLine>();
            for (int y = 0; y < size; y++)
            {
                poleDots[y] = new PoleDot[size];
                for (int x = 0; x < size; x++)
                {
                    poleDots[y][x] = new PoleDot(x, y);
                }
            }
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size - 1; x++)
                {
                    PoleLine line = new PoleLine(poleDots[y][x], poleDots[y][x + 1]);
                    poleDots[y][x].AddLine(line, poleDots[y][x + 1]);
                    poleDots[y][x + 1].AddLine(line, poleDots[y][x]);
                    poleLines.Add(line);
                }
                if (y < size - 1)
                    for (int x = 0; x < size; x++)
                    {
                        PoleLine line = new PoleLine(poleDots[y][x], poleDots[y + 1][x]);
                        poleDots[y][x].AddLine(line, poleDots[y + 1][x]);
                        poleDots[y + 1][x].AddLine(line, poleDots[y][x]);
                        poleLines.Add(line);
                    }
            }
        }
        public void SetStart(int x, int y)
        {
            start = poleDots[y][x];
        }
        public void SetFinish(int x, int y)
        {
            finish = poleDots[y][x];
        }
        public void SetNewSeed(int k)
        {
            myRandGen = new MyRandom(k);
        }
        public void GeneratePoints(int numberOfPoints)
        {
            numberOfPoints = System.Math.Min(numberOfPoints, path.dots.Count + path.lines.Count - 1);
            for (int i = 0; i < numberOfPoints; i++)
            {
                int r = myRandGen.GetRandom();
                r %= path.dots.Count + path.lines.Count;
                if (r % 2 == 0)
                {

                    if (path.dots[(r / 2)].point == null)
                    {
                        PoleEltPoint point = new PoleEltPoint(path.dots[(r / 2)]);
                        path.dots[(r / 2)].point = point;
                        eltsManager.points.Add(point);
                    }
                    else i--;

                }
                else
                {
                    if (path.lines[(r - 1) / 2].point == null)
                    {
                        PoleEltPoint point = new PoleEltPoint(path.lines[(r - 1) / 2]);
                        path.lines[(r - 1) / 2].point = point;
                        eltsManager.points.Add(point);
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
            ways[start.position_y][start.position_x] = 1;
            dotData = new PathDotStack();

            bool isFound = FindPath(start, finish, ways);
            if (isFound)
            {
                PoleDot prevDot;
                PoleDot curDot = finish;
                path.dots.Add(curDot);
                while (!dotData.IsEmpty())
                {
                    prevDot = dotData.GetDot();
                    path.dots.Add(prevDot);
                    if (curDot.position_x < prevDot.position_x) path.lines.Add(curDot.right);
                    else if (curDot.position_x > prevDot.position_x) path.lines.Add(curDot.left);
                    else if (curDot.position_y < prevDot.position_y) path.lines.Add(curDot.down);
                    else if (curDot.position_y > prevDot.position_y) path.lines.Add(curDot.up);
                    curDot = prevDot;
                }
                path.dots.Reverse();
                path.lines.Reverse();
            }
        }
        public void ClearPole()
        {
            for (int y = 0; y < poleSize; y++)
            {
                for (int x = 0; x < poleSize; x++)
                {
                    poleDots[y][x].isUsed = false;
                    poleDots[y][x].point = null;
                }
            }
            for (int n = 0; n < poleLines.Count; n++)
            {
                poleLines[n].isUsed = false;
                poleLines[n].point = null;
            }

            path.dots.Clear();
            path.lines.Clear();

        }

    }

}
