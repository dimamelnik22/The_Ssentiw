using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pole : MonoBehaviour
{

    private static Vector3 stepx = new Vector3(5f,0f,0f);
    private static Vector3 stepy = new Vector3(0f,-5f,0f);




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
        public List<GameObject> clrRing;
        public List<GameObject> unsolvedElts;
        public PoleElts()
        {
            points = new List<GameObject>();
            clrRing = new List<GameObject>();
            unsolvedElts = new List<GameObject>();
        }
        public bool CheckSolution()
        {
            bool isSolved = true;
            unsolvedElts.Clear();
            foreach (GameObject p in points)
            {

                if (!p.GetComponent<PoleEltPoint>().IsSolvedByPlayer()) unsolvedElts.Add(p);
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
    public GameObject start;
    public GameObject tempStart;
    public GameObject finish;
    public List<GameObject> tempFins = new List<GameObject>();
    public List<GameObject> finishes = new List<GameObject>();
    private int poleSize;
    PathDotStack dotData;
	List<List<GameObject>> zone = new List<List<GameObject>>();
    public GameObject[][] poleDots;
    public List<GameObject> poleLines;
    public PolePath systemPath;
    public PolePath playerPath;

	
	public GameObject ClrRingPrefab;
    public GameObject SquerePrefab;
    public GameObject DotPrefab;
    public GameObject VerticalLinePrefab;
    public GameObject HorizontalLinePrefab;
    public GameObject StartPrefab;
    public GameObject FinishPrefab;
    public int[][] poleZones;
	public int quantityZones;
    int quantityColor = 3;
    int quantityRing = 0;
    List<Color> color = new List<Color>() { Color.cyan, Color.yellow, Color.green, Color.magenta, Color.blue };
    public void OnDestroy()
    {
        
        for (int i = transform.childCount - 1; i >= 0; --i)
        {
            Destroy(transform.GetChild(i));
        }
    }

    public void InitMenuSlider(int size)
    {
        playerPath = new PolePath();
        poleDots = new GameObject[2][];
        poleLines = new List<GameObject>();

        for (int y = 0; y < 2; y++)
        {
            poleDots[y] = new GameObject[size];
            for (int x = 1; x < size; x++)
            {
                poleDots[y][x] = Instantiate(DotPrefab, transform.position + stepx * x + stepy * y, DotPrefab.transform.rotation);
                poleDots[y][x].transform.parent = this.transform;
                poleDots[y][x].GetComponent<PoleDot>().posX = x;
                poleDots[y][x].GetComponent<PoleDot>().posY = y;
                if (y > 0 && x < size - 1)
                {
                    GameObject line;
                    line = Instantiate(VerticalLinePrefab, poleDots[1][x].transform.position - stepy * 0.5f, VerticalLinePrefab.transform.rotation);
                    poleLines.Add(line);
                    line.transform.parent = this.transform;
                    poleDots[1][x].GetComponent<PoleDot>().AddLine(line, poleDots[0][x]);
                    poleDots[0][x].GetComponent<PoleDot>().AddLine(line, poleDots[1][x]);
                    line.GetComponent<PoleLine>().up = poleDots[0][x];
                    line.GetComponent<PoleLine>().down = poleDots[1][x];
                }
            }
            
            
        }
        for (int x = 1; x < size; x++)
        {
            finishes.Add(poleDots[0][x]);
            tempFins.Add(Instantiate(FinishPrefab, poleDots[0][x].transform.position, FinishPrefab.transform.rotation));
            tempFins[tempFins.Count - 1].transform.parent = this.transform;
        }
        poleDots[1][0] = Instantiate(DotPrefab, transform.position, DotPrefab.transform.rotation);
        poleDots[1][0].transform.parent = this.transform;
        poleDots[1][0].GetComponent<PoleDot>().posX = 0;
        poleDots[1][0].GetComponent<PoleDot>().posY = 1;
        GameObject opline = Instantiate(HorizontalLinePrefab, poleDots[1][0].transform.position + stepx * 0.5f, HorizontalLinePrefab.transform.rotation);
        opline.transform.parent = this.transform;
        poleLines.Add(opline);
        poleDots[1][0].GetComponent<PoleDot>().AddLine(opline, poleDots[1][1]);
        poleDots[1][1].GetComponent<PoleDot>().AddLine(opline, poleDots[1][0]);
        opline.GetComponent<PoleLine>().left = poleDots[1][0];
        opline.GetComponent<PoleLine>().right = poleDots[1][1];
        start = poleDots[1][0];
        tempStart = Instantiate(StartPrefab, poleDots[1][0].transform.position, StartPrefab.transform.rotation);
        tempStart.transform.parent = this.transform;
    }

    public void InitMenuItem(int numOfParams)
    {
        playerPath = new PolePath();
        poleDots = new GameObject[numOfParams][];
        poleLines = new List<GameObject>();
        
        for (int y = 0; y < numOfParams; y++)
        {
            poleDots[y] = new GameObject[3];
            for (int x = 1; x < 3; x++)
            {
                poleDots[y][x] = Instantiate(DotPrefab, transform.position + stepx * x + stepy * y, DotPrefab.transform.rotation);
                poleDots[y][x].transform.parent = this.transform;
                poleDots[y][x].GetComponent<PoleDot>().posX = x;
                poleDots[y][x].GetComponent<PoleDot>().posY = y;
            }
            GameObject line = Instantiate(HorizontalLinePrefab, poleDots[y][1].transform.position + stepx * 0.5f, HorizontalLinePrefab.transform.rotation);
            line.transform.parent = this.transform;
            poleLines.Add(line);
            poleDots[y][1].GetComponent<PoleDot>().AddLine(line, poleDots[y][2]);
            poleDots[y][2].GetComponent<PoleDot>().AddLine(line, poleDots[y][1]);
            line.GetComponent<PoleLine>().left = poleDots[y][1];
            line.GetComponent<PoleLine>().right = poleDots[y][2];
            if (y>0)
            {
                line = Instantiate(VerticalLinePrefab, poleDots[y][1].transform.position - stepy * 0.5f, VerticalLinePrefab.transform.rotation);
                poleLines.Add(line);
                line.transform.parent = this.transform;
                poleDots[y][1].GetComponent<PoleDot>().AddLine(line, poleDots[y - 1][1]);
                poleDots[y - 1][1].GetComponent<PoleDot>().AddLine(line, poleDots[y][1]);
                line.GetComponent<PoleLine>().up = poleDots[y - 1][1];
                line.GetComponent<PoleLine>().down = poleDots[y][1];
            }
        }
        for (int y = 0; y < numOfParams; y++)
        {
            finishes.Add(poleDots[y][2]);
            tempFins.Add( Instantiate(FinishPrefab, poleDots[y][2].transform.position, FinishPrefab.transform.rotation));
            tempFins[tempFins.Count - 1].transform.parent = this.transform;
        }
        poleDots[0][0] = Instantiate(DotPrefab, transform.position, DotPrefab.transform.rotation);
        poleDots[0][0].transform.parent = this.transform;
        poleDots[0][0].GetComponent<PoleDot>().posX = 0;
        poleDots[0][0].GetComponent<PoleDot>().posY = 0;
        GameObject opline = Instantiate(HorizontalLinePrefab, poleDots[0][0].transform.position + stepx * 0.5f, HorizontalLinePrefab.transform.rotation);
        opline.transform.parent = this.transform;
        poleLines.Add(opline);
        poleDots[0][0].GetComponent<PoleDot>().AddLine(opline, poleDots[0][1]);
        poleDots[0][1].GetComponent<PoleDot>().AddLine(opline, poleDots[0][0]);
        opline.GetComponent<PoleLine>().left = poleDots[0][0];
        opline.GetComponent<PoleLine>().right = poleDots[0][1];
        start = poleDots[0][0];
        tempStart = Instantiate(StartPrefab, poleDots[0][0].transform.position, StartPrefab.transform.rotation);
        tempStart.transform.parent = this.transform;
    }

    public void Init(int size)
    {
        quantityRing = Core.PolePreferences.numOfCircles;
        playerPath = new PolePath();
        systemPath = new PolePath();
        poleSize = size;
        eltsManager = new PoleElts();
        poleDots = new GameObject[size][];
        poleLines = new List<GameObject>();
        poleZones = new int[size - 1][];
        for (int y = 0; y < size - 1; y++)
        {
            poleZones[y] = new int[size - 1];
            for (int x = 0; x < size - 1; x++)
            {
                poleZones[y][x] = 0;
            }
        }
        for (int y = 0; y < size; y++)
        {
            poleDots[y] = new GameObject[size];
            for (int x = 0; x < size; x++)
            {
                poleDots[y][x] = Instantiate(DotPrefab, transform.position + stepx * x + stepy * y, DotPrefab.transform.rotation);
                poleDots[y][x].GetComponent<PoleDot>().posX = x;
                poleDots[y][x].GetComponent<PoleDot>().posY = y;
                poleDots[y][x].transform.parent = this.transform;
            }
        }

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size - 1; x++)
            {
                // horizontal line
                GameObject lineH = Instantiate(HorizontalLinePrefab, transform.position + stepx * (x + 0.5f) + stepy * y, transform.rotation);
                lineH.GetComponent<PoleLine>().left = poleDots[y][x];
                lineH.GetComponent<PoleLine>().right = poleDots[y][x + 1];
                poleDots[y][x].GetComponent<PoleDot>().AddLine(lineH, poleDots[y][x + 1]);
                poleDots[y][x + 1].GetComponent<PoleDot>().AddLine(lineH, poleDots[y][x]);
                poleLines.Add(lineH);
                lineH.transform.parent = this.transform;
                // vertical line
                GameObject lineV = Instantiate(VerticalLinePrefab, transform.position + stepx * y + stepy * (x + 0.5f), transform.rotation);
                lineV.GetComponent<PoleLine>().up = poleDots[x][y];
                lineV.GetComponent<PoleLine>().down = poleDots[x + 1][y];
                poleDots[x][y].GetComponent<PoleDot>().AddLine(lineV, poleDots[x + 1][y]);
                poleDots[x + 1][y].GetComponent<PoleDot>().AddLine(lineV, poleDots[x][y]);
                poleLines.Add(lineV);
                lineV.transform.parent = this.transform;
                if (y < size - 1)
                {
                    GameObject Squere = Instantiate(SquerePrefab, transform.position + stepx * (x + 0.5f) + stepy * (y + 0.5f), SquerePrefab.transform.rotation);
                    Squere.GetComponent<PoleSquare>().up = lineH;
                    lineH.GetComponent<PoleLine>().down = Squere;
                    Squere.transform.parent = this.transform;
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
            if (dotData.PathLength() < poleSize * Core.PolePreferences.complexity)
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
            int k = Core.PolePreferences.MyRandom.GetRandom() % 4;
            while (!tries[k])
            {
                k = Core.PolePreferences.MyRandom.GetRandom() % 4;
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

    public bool FindPathQuick(GameObject begin, GameObject end, int[][] ways)
    {
        begin.GetComponent<PoleDot>().isUsedBySolution = true;
        if (begin == end)
        {
            if (dotData.PathLength() < poleSize * 4)
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
            int k = Core.PolePreferences.MyRandom.GetRandom() % 4;
            while (!tries[k])
            {
                k = Core.PolePreferences.MyRandom.GetRandom() % 4;
            }
            switch (k)
            {
                case 0:
                    if (begin.GetComponent<PoleDot>().posY > 0 && ways[begin.GetComponent<PoleDot>().posY - 1][begin.GetComponent<PoleDot>().posX] == 0 && tries[0])
                    {
                        ways[begin.GetComponent<PoleDot>().posY - 1][begin.GetComponent<PoleDot>().posX] = 1;
                        begin.GetComponent<PoleDot>().up.GetComponent<PoleLine>().isUsedBySolution = true;
                        if (FindPathQuick(poleDots[begin.GetComponent<PoleDot>().posY - 1][begin.GetComponent<PoleDot>().posX], end, ways)) return true;
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
                        if (FindPathQuick(poleDots[begin.GetComponent<PoleDot>().posY][begin.GetComponent<PoleDot>().posX + 1], end, ways)) return true;
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
                        if (FindPathQuick(poleDots[begin.GetComponent<PoleDot>().posY + 1][begin.GetComponent<PoleDot>().posX], end, ways)) return true;
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
                        if (FindPathQuick(poleDots[begin.GetComponent<PoleDot>().posY][begin.GetComponent<PoleDot>().posX - 1], end, ways)) return true;
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
        //ways[begin.GetComponent<PoleDot>().posY][begin.GetComponent<PoleDot>().posX] = 0;
        begin.GetComponent<PoleDot>().isUsedBySolution = false;
        dotData.GetDot();
        return false;
    }

    private void FindZone(GameObject square, int x, int y)
    {
        GameObject lineH = square.GetComponent<PoleSquare>().up;
        if (!lineH.GetComponent<PoleLine>().isUsedBySolution && lineH.GetComponent<PoleLine>().up != null)
        {
            if (poleZones[y - 1][x] == 0)
            {
                poleZones[y - 1][x] = poleZones[y][x];
                FindZone(lineH.GetComponent<PoleLine>().up, x, y - 1);
            }
        }
        lineH = square.GetComponent<PoleSquare>().down;
        if (!lineH.GetComponent<PoleLine>().isUsedBySolution && lineH.GetComponent<PoleLine>().down != null)
        {
            if (poleZones[y + 1][x] == 0)
            {
                poleZones[y + 1][x] = poleZones[y][x];
                FindZone(lineH.GetComponent<PoleLine>().down, x, y + 1);
            }
        }

        GameObject lineV = square.GetComponent<PoleSquare>().left;
        if (!lineV.GetComponent<PoleLine>().isUsedBySolution && lineV.GetComponent<PoleLine>().left != null)
        {
            if (poleZones[y][x - 1] == 0)
            {
                poleZones[y][x - 1] = poleZones[y][x];
                FindZone(lineV.GetComponent<PoleLine>().left, x - 1, y);
            }
        }
        lineV = square.GetComponent<PoleSquare>().right;
        if (!lineV.GetComponent<PoleLine>().isUsedBySolution && lineV.GetComponent<PoleLine>().right != null)
        {
            if (poleZones[y][x + 1] == 0)
            {
                poleZones[y][x + 1] = poleZones[y][x];
                FindZone(lineV.GetComponent<PoleLine>().right, x + 1, y);
            }
        }
    }
    public void SetZone()
    {
        GameObject square = poleDots[0][0].GetComponent<PoleDot>().right.GetComponent<PoleLine>().down;
        quantityZones = 0;
        int size = poleSize - 1;
        for (int x = 0; x < size; ++x)
        {
            GameObject square1 = square;
            for (int y = 0; y < size; ++y)
            {
                if (poleZones[y][x] == 0)
                {
                    ++quantityZones;
                    poleZones[y][x] = quantityZones;
                    FindZone(square1, x, y);
                }
                square1 = square1.GetComponent<PoleSquare>().down.GetComponent<PoleLine>().down;
            }
            square = square.GetComponent<PoleSquare>().right.GetComponent<PoleLine>().right;
        }
        square = poleDots[0][0].GetComponent<PoleDot>().right.GetComponent<PoleLine>().down;
        for (int i = 0; i < quantityZones; ++i)
        {
            zone.Add(new List<GameObject>());
        }
        for (int x = 0; x < size; ++x)
        {
            GameObject square1 = square;
            for (int y = 0; y < size; ++y)
            {
                zone[poleZones[y][x] - 1].Add(square1);
                square1 = square1.GetComponent<PoleSquare>().down.GetComponent<PoleLine>().down;
            }
            square = square.GetComponent<PoleSquare>().right.GetComponent<PoleLine>().right;
        }
    }
    public int GetSize()
    {
        return poleSize;
    }

    public void SetStart(int x, int y)
    {
        Instantiate(StartPrefab, stepx * x + stepy * y, StartPrefab.transform.rotation);
        start = poleDots[y][x];
    }
    public void SetFinish(int x, int y)
    {
        Instantiate(FinishPrefab, stepx * x + stepy * y, FinishPrefab.transform.rotation);
        finish = poleDots[y][x];
    }
    public void GeneratePoints(int numberOfPoints)
    {
        eltsManager.points.Clear();
        numberOfPoints = System.Math.Min(numberOfPoints, (systemPath.dots.Count + systemPath.lines.Count) / 2);
        for (int i = 0; i < numberOfPoints; i++)
        {
            int r = Core.PolePreferences.MyRandom.GetRandom();
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
	public void SetClrRing(int zoneQuantity, int ringQuantity)
    {
        List<List<GameObject>> coloredZones = new List<List<GameObject>>(zoneQuantity);
        List<int> quantityClrRingInZone = new List<int>();
        ringQuantity -= zoneQuantity;
        int quantityNotUsedSquare = 0;
        for (int i = 0; i < zone.Count;++i)
        {
            quantityNotUsedSquare += zone[i].Count;
            if (i < zoneQuantity)
            {
                quantityClrRingInZone.Add(1);
                coloredZones.Add(new List<GameObject>());
                coloredZones[i].AddRange(zone[i]);
            }
            else
            {
                List<GameObject> m = new List<GameObject>();
                int mm = 0;
                m = coloredZones[0];
                for (int j = 1; j < coloredZones.Count; ++j)
                {
                    if(m.Count > coloredZones[j].Count)
                    {
                        m = coloredZones[j];
                        mm = j;
                    }
                }
                coloredZones[mm].AddRange(zone[i]);
            }
        }
        
        while(ringQuantity > 0)
        {
            int i = Core.PolePreferences.MyRandom.GetRandom() % (quantityNotUsedSquare-1);
            int k = 0;
            Debug.Log(i + " " + k);
            while (i - (coloredZones[k].Count - quantityClrRingInZone[k]) > 0)
            {
                i -= (coloredZones[k].Count - quantityClrRingInZone[k]);
                k++;
            }
            quantityNotUsedSquare--;
            quantityClrRingInZone[k]++;
            ringQuantity--;
        }
        for(int j = 0;j < zoneQuantity;++j)
        {
            int k = Core.PolePreferences.MyRandom.GetRandom() % color.Count;
            while(quantityClrRingInZone[j] > 0)
            {
                int i = Core.PolePreferences.MyRandom.GetRandom() % coloredZones[j].Count;
                coloredZones[j][i].GetComponent<PoleSquare>().hasElem = true;
                coloredZones[j][i].GetComponent<PoleSquare>().element = Instantiate(ClrRingPrefab, coloredZones[j][i].transform.position, ClrRingPrefab.transform.rotation);
                eltsManager.clrRing.Add(coloredZones[j][i].GetComponent<PoleSquare>().element);
                coloredZones[j][i].GetComponent<PoleSquare>().element.GetComponent<MeshRenderer>().material.color = color[k];
                quantityClrRingInZone[j]--;
                coloredZones[j].RemoveAt(i);
            }
            color.RemoveAt(k);
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
        //bool isFound = FindPath(start, finish, ways);
        bool isFound = FindPathQuick(start, finish, ways);
        while(!isFound)
        {
            for (int i = 0; i < poleSize; i++)
            {
                for (int j = 0; j < poleSize; j++)
                {
                    ways[i][j] = 0;
                }
            }
            isFound = FindPathQuick(start, finish, ways);
        }
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
        SetZone();
		if (quantityZones >= quantityColor)
        {
            
            SetClrRing(quantityColor, quantityRing);
        }
        else
        {
            Debug.Log("Call find path again");
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
