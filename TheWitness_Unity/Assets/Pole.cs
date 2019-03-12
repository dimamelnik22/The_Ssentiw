using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pole : MonoBehaviour
{

    private static Vector3 stepx = new Vector3(5, 0, 0);
    private static Vector3 stepz = new Vector3(0, 0, -5);
    public class MyRandom
    {
        public int seed;
        public MyRandom(int k)
        {

            seed = k;
        }
        public int GetRandom()
        {
            seed = (seed * 106 + 1283) % 6075;
            return seed;
        }
    }




    public class PathDotStack
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

    public class PoleElts
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

    public class PolePath
    {
        public List<GameObject> dots;
        public List<GameObject> lines;
        public void Clear()
        {
            dots.Clear();
            lines.Clear();
        }
        public PolePath()
        {
            dots = new List<GameObject>();
            lines = new List<GameObject>();
        }


    }
    
    public PoleElts eltsManager;
    public MyRandom myRandGen;
    public GameObject start;
    public GameObject finish;
    private int poleSize;
    PathDotStack dotData;
    public GameObject[][] poleDots;
    public List<GameObject> poleLines;
    public PolePath systemPath;
    public PolePath playerPath;

    public GameObject SquerePrefab;
    public GameObject DotPrefab;
    public GameObject VerticalLinePrefab;
    public GameObject HorizontalLinePrefab;
    public GameObject StartPrefab;
    public GameObject FinishPrefab;

    public void Init(int size, int seed)
    {
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
                poleDots[y][x].GetComponent<PoleDot>().posX = x;
                poleDots[y][x].GetComponent<PoleDot>().posY = y;
            }
        }

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size - 1; x++)
            {
                // horizontal line
                GameObject lineH = Instantiate(HorizontalLinePrefab, stepx * (x + 0.5f) + stepz * y, HorizontalLinePrefab.transform.rotation);
                lineH.GetComponent<PoleLine>().left = poleDots[y][x];
                lineH.GetComponent<PoleLine>().right = poleDots[y][x + 1];
                poleDots[y][x].GetComponent<PoleDot>().AddLine(lineH, poleDots[y][x + 1]);
                poleDots[y][x + 1].GetComponent<PoleDot>().AddLine(lineH, poleDots[y][x]);
                poleLines.Add(lineH);
                // vertical line
                GameObject lineV = Instantiate(HorizontalLinePrefab, stepx * y + stepz * (x + 0.5f), VerticalLinePrefab.transform.rotation);
                lineV.GetComponent<PoleLine>().up = poleDots[x][y];
                lineV.GetComponent<PoleLine>().down = poleDots[x + 1][y];
                poleDots[x][y].GetComponent<PoleDot>().AddLine(lineV, poleDots[x + 1][y]);
                poleDots[x + 1][y].GetComponent<PoleDot>().AddLine(lineV, poleDots[x][y]);
                poleLines.Add(lineV);
                if (y < size - 1)
                {
                    GameObject Squere = Instantiate(SquerePrefab, stepx * (x + 0.5f) + stepz * (y + 0.5f), SquerePrefab.transform.rotation);
                    Squere.GetComponent<PoleSquare>().up = lineH;
                    lineH.GetComponent<PoleLine>().down = Squere;
                }

            }

        }
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                if (x > 0 && y > 0)
                {
                    GameObject Squere = poleDots[x - 1][y - 1].GetComponent<PoleDot>().right.GetComponent<PoleLine>().down;
                    GameObject lineH = poleDots[x][y].GetComponent<PoleDot>().left;
                    Squere.GetComponent<PoleSquare>().down = lineH;
                    lineH.GetComponent<PoleLine>().up = Squere;
                    GameObject lineVR = poleDots[x][y].GetComponent<PoleDot>().up;
                    Squere.GetComponent<PoleSquare>().right = lineVR;
                    lineVR.GetComponent<PoleLine>().left = Squere;
                    GameObject lineVL = poleDots[x - 1][y - 1].GetComponent<PoleDot>().down;
                    Squere.GetComponent<PoleSquare>().left = lineVL;
                    lineVL.GetComponent<PoleLine>().right = Squere;
                }
            }
        }

    }
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
                    if (begin.GetComponent<PoleDot>().posY > 0 && ways[begin.GetComponent<PoleDot>().posY - 1][begin.GetComponent<PoleDot>().posX] == 0 && tries[0])
                    {
                        ways[begin.GetComponent<PoleDot>().posY - 1][begin.GetComponent<PoleDot>().posX] = 1;
                        begin.GetComponent<PoleDot>().up.GetComponent<PoleLine>().isUsedBySolution = true;
                        if (FindPath(poleDots[begin.GetComponent<PoleDot>().posY - 1][begin.GetComponent<PoleDot>().posX], end, ways)) return true;
                        else
                        {
                            ways[begin.GetComponent<PoleDot>().posY - 1][begin.GetComponent<PoleDot>().posX] = 0;
                            begin.GetComponent<PoleDot>().up.GetComponent<PoleLine>().isUsedBySolution = false;
                            tries[0] = false;
                        }
                    }
                    else tries[0] = false;
                    break;
                case 1:
                    if (begin.GetComponent<PoleDot>().posX < poleSize - 1 && ways[begin.GetComponent<PoleDot>().posY][begin.GetComponent<PoleDot>().posX + 1] == 0 && tries[1])
                    {
                        ways[begin.GetComponent<PoleDot>().posY][begin.GetComponent<PoleDot>().posX + 1] = 1;
                        begin.GetComponent<PoleDot>().right.GetComponent<PoleLine>().isUsedBySolution = true;
                        if (FindPath(poleDots[begin.GetComponent<PoleDot>().posY][begin.GetComponent<PoleDot>().posX + 1], end, ways)) return true;
                        else
                        {
                            ways[begin.GetComponent<PoleDot>().posY][begin.GetComponent<PoleDot>().posX + 1] = 0;
                            begin.GetComponent<PoleDot>().right.GetComponent<PoleLine>().isUsedBySolution = false;
                            tries[1] = false;
                        }
                    }
                    else tries[1] = false;
                    break;
                case 2:
                    if (begin.GetComponent<PoleDot>().posY < poleSize - 1 && ways[begin.GetComponent<PoleDot>().posY + 1][begin.GetComponent<PoleDot>().posX] == 0 && tries[2])
                    {
                        ways[begin.GetComponent<PoleDot>().posY + 1][begin.GetComponent<PoleDot>().posX] = 1;
                        begin.GetComponent<PoleDot>().down.GetComponent<PoleLine>().isUsedBySolution = true;
                        if (FindPath(poleDots[begin.GetComponent<PoleDot>().posY + 1][begin.GetComponent<PoleDot>().posX], end, ways)) return true;
                        else
                        {
                            ways[begin.GetComponent<PoleDot>().posY + 1][begin.GetComponent<PoleDot>().posX] = 0;
                            begin.GetComponent<PoleDot>().down.GetComponent<PoleLine>().isUsedBySolution = false;
                            tries[2] = false;
                        }
                    }
                    else tries[2] = false;
                    break;
                case 3:
                    if (begin.GetComponent<PoleDot>().posX > 0 && ways[begin.GetComponent<PoleDot>().posY][begin.GetComponent<PoleDot>().posX - 1] == 0 && tries[3])
                    {
                        ways[begin.GetComponent<PoleDot>().posY][begin.GetComponent<PoleDot>().posX - 1] = 1;
                        begin.GetComponent<PoleDot>().left.GetComponent<PoleLine>().isUsedBySolution = true;
                        if (FindPath(poleDots[begin.GetComponent<PoleDot>().posY][begin.GetComponent<PoleDot>().posX - 1], end, ways)) return true;
                        else
                        {
                            ways[begin.GetComponent<PoleDot>().posY][begin.GetComponent<PoleDot>().posX - 1] = 0;
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
        ways[begin.GetComponent<PoleDot>().posY][begin.GetComponent<PoleDot>().posX] = 0;
        begin.GetComponent<PoleDot>().isUsedBySolution = false;
        dotData.GetDot();
        return false;
    }
    public int GetSize()
    {
        return poleSize;
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
        ways[start.GetComponent<PoleDot>().posY][start.GetComponent<PoleDot>().posX] = 1;
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
                if (curDot.GetComponent<PoleDot>().posX < prevDot.GetComponent<PoleDot>().posX) systemPath.lines.Add(curDot.GetComponent<PoleDot>().right);
                else if (curDot.GetComponent<PoleDot>().posX > prevDot.GetComponent<PoleDot>().posX) systemPath.lines.Add(curDot.GetComponent<PoleDot>().left);
                else if (curDot.GetComponent<PoleDot>().posY < prevDot.GetComponent<PoleDot>().posY) systemPath.lines.Add(curDot.GetComponent<PoleDot>().down);
                else if (curDot.GetComponent<PoleDot>().posY > prevDot.GetComponent<PoleDot>().posY) systemPath.lines.Add(curDot.GetComponent<PoleDot>().up);
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
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
