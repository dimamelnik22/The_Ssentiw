using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
        private class Shape
        {
            public List<List<bool>> s;
            public int count;
        }
        public List<GameObject> points;
        public List<GameObject> clrRing;
        public List<GameObject> unsolvedElts;
        int[][] checkZones = new int[Core.PolePreferences.poleSize - 1][];
        public List<List<GameObject>> zone = new List<List<GameObject>>();
        private void FindZone(GameObject square, int x, int y)
        {
            int size = Core.PolePreferences.poleSize - 1;
            GameObject lineH = square.GetComponent<PoleSquare>().up;
            if (!lineH.GetComponent<PoleLine>().isUsedByPlayer && lineH.GetComponent<PoleLine>().up != null)
            {
                if (y > 0 && checkZones[y - 1][x] == 0)
                {
                    checkZones[y - 1][x] = checkZones[y][x];
                    FindZone(lineH.GetComponent<PoleLine>().up, x, y - 1);
                }
            }
            lineH = square.GetComponent<PoleSquare>().down;
            if (!lineH.GetComponent<PoleLine>().isUsedByPlayer && lineH.GetComponent<PoleLine>().down != null)
            {
                if (y < size && checkZones[y + 1][x] == 0)
                {
                    checkZones[y + 1][x] = checkZones[y][x];
                    FindZone(lineH.GetComponent<PoleLine>().down, x, y + 1);
                }
            }

            GameObject lineV = square.GetComponent<PoleSquare>().left;
            if (!lineV.GetComponent<PoleLine>().isUsedByPlayer && lineV.GetComponent<PoleLine>().left != null)
            {
                if (x > 0 && checkZones[y][x - 1] == 0)
                {
                    checkZones[y][x - 1] = checkZones[y][x];
                    FindZone(lineV.GetComponent<PoleLine>().left, x - 1, y);
                }
            }
            lineV = square.GetComponent<PoleSquare>().right;
            if (!lineV.GetComponent<PoleLine>().isUsedByPlayer && lineV.GetComponent<PoleLine>().right != null)
            {
                if (x < size && checkZones[y][x + 1] == 0)
                {
                    checkZones[y][x + 1] = checkZones[y][x];
                    FindZone(lineV.GetComponent<PoleLine>().right, x + 1, y);
                }
            }
        }
        public void SetZone(GameObject square)
        {
            checkZones = new int[Core.PolePreferences.poleSize - 1][];
            int size = Core.PolePreferences.poleSize - 1;
            for (int i = 0; i < size; ++i)
            {
                checkZones[i] = new int[size];
                for (int j = 0; j < size; ++j)
                {
                    checkZones[i][j] = 0;
                }
            }
            GameObject square1 = square;
            int quantityZones = 0;
            for (int x = 0; x < size; ++x)
            {
                GameObject square2 = square1;
                for (int y = 0; y < size; ++y)
                {
                    if (checkZones[y][x] == 0)
                    {
                        ++quantityZones;
                        checkZones[y][x] = quantityZones;
                        FindZone(square2, x, y);
                    }
                    square2 = square2.GetComponent<PoleSquare>().down.GetComponent<PoleLine>().down;
                }
                square1 = square1.GetComponent<PoleSquare>().right.GetComponent<PoleLine>().right;
            }
            
            for (int i = 0; i < quantityZones; ++i)
            {
                zone.Add(new List<GameObject>());
            }
            square1 = square;
            for (int x = 0; x < size; ++x)
            {
                GameObject square2 = square1;
                for (int y = 0; y < size; ++y)
                {
                    zone[checkZones[y][x] - 1].Add(square2);
                    square2 = square2.GetComponent<PoleSquare>().down.GetComponent<PoleLine>().down;
                }
                square1 = square1.GetComponent<PoleSquare>().right.GetComponent<PoleLine>().right;
            }
        }
        public PoleElts()
        {
            points = new List<GameObject>();
            clrRing = new List<GameObject>();
            unsolvedElts = new List<GameObject>();

        }
        public bool CheckShapeSplit(List<GameObject> zone, List<GameObject> shapes)
        {
            //List<GameObject> curzone = new List<GameObject>(zone);
            int size = 0;
            for (int i = 0; i < shapes.Count; i++)
            {
                size += shapes[i].GetComponent<PoleEltShape>().size;
            }
            if (size > 0 && size != zone.Count)
            {
                return false;
            }
            
            List<List<bool>> zoneBoolList = ZoneToBoolList(zone);
            List<Shape> shapesList = new List<Shape>();
            for (int i = 0; i < shapes.Count; i++)
            {
                bool flag = true;
                for(int j = 0; j < shapesList.Count;++j)
                {
                    if(shapesList[j].s == shapes[i].GetComponent<PoleEltShape>().boolList)
                    {
                        shapesList[j].count += 1;
                        flag = false;
                        break;
                    }
                }
                if(flag)
                {
                    shapesList.Add(new Shape());
                    shapesList[shapesList.Count - 1].count = 1;
                    shapesList[shapesList.Count - 1].s = shapes[i].GetComponent<PoleEltShape>().boolList;
                }
            }
            return FillShape(zoneBoolList, shapesList, 0, 0,0);
        }
        private bool FillShape(List<List<bool>> zoneBoolList, List<Shape> shapesList,int i,int j,int q)
        {
            
            int shapesCount = 0;
            for(int t= 0; t < shapesList.Count;++t)
            {
                shapesCount += shapesList[t].count;
            }
            if (shapesCount == 0)
            {
                if (q == 0) return true;
                for (int y = 0; y < zoneBoolList.Count;++y)
                {
                    for (int x = 0; x < zoneBoolList[y].Count; ++x)
                    {
                        if (zoneBoolList[y][x])
                        {
                            return false;
                        }
                    }
                }
                return true;
            }
            for (int ii = i, jj = j; ii < zoneBoolList.Count;ii++)
            {
                for (; jj < zoneBoolList[0].Count; jj++)
                {
                    for(int k = 0;k < shapesList.Count;++k)
                    {
                        
                        if (shapesList[k].count == 0 ||
                            ii + shapesList[k].s.Count > zoneBoolList.Count ||
                            jj + shapesList[k].s[0].Count > zoneBoolList[0].Count) continue;
                        bool filt = true;
                        for (int y = 0; y < shapesList[k].s.Count;++y)
                        {
                            for (int x = 0; x < shapesList[k].s[0].Count;++x)
                            {
                                if(shapesList[k].s[y][x] && !zoneBoolList[ii+y][jj+x])
                                {
                                    filt = false;
                                    break;
                                }
                            }
                            if (!filt) break;
                        }
                        if (filt)
                        {
                            for (int y = 0; y < shapesList[k].s.Count; ++y)
                            {
                                for (int x = 0; x < shapesList[k].s[y].Count; ++x)
                                {
                                    if (shapesList[k].s[y][x])
                                    {
                                        zoneBoolList[ii + y][jj + x] = false;
                                    }
                                }
                            }
                            shapesList[k].count--;
                            bool flag = FillShape(zoneBoolList, shapesList, ii, jj, 1);
                            if (!flag)
                            {
                                for (int y = 0; y < shapesList[k].s.Count; ++y)
                                {
                                    for (int x = 0; x < shapesList[k].s[y].Count; ++x)
                                    {
                                        if (shapesList[k].s[y][x])
                                        {
                                            zoneBoolList[ii + y][jj + x] = true;
                                        }
                                    }
                                }
                                shapesList[k].count++;
                            }
                            else return true;
                        
                        }
                        
                    }
                    if (zoneBoolList[ii][jj])
                    {
                        return false;
                    } 
                }
                jj = 0;
            }
            return false;
        }
        /*public bool FillShape(List<List<bool>> zoneBoolList, List<List<List<bool>>> shapesList, int i)
        {
            if (i == shapesList.Count)
            {
                if (i == 0) return true;
                for (int y = 0; y < zoneBoolList.Count; y++)
                    for (int x = 0; x < zoneBoolList[0].Count; x++)
                        if (zoneBoolList[y][x])
                        {
                            return false;
                        }
                return true;
            }
            
            bool fits = true;
            for (int k = 0; k < zoneBoolList.Count - shapesList[i].Count + 1; k++)
            {
                for (int j = 0; j < zoneBoolList[0].Count - shapesList[i][0].Count + 1; j++)
                {
                    fits = true;
                    for (int y = 0; y < shapesList[i].Count; y++)
                    {
                        for (int x = 0; x < shapesList[i][0].Count; x++)
                            if (shapesList[i][y][x] && !zoneBoolList[k+y][j+x])

                            {
                                fits = false;
                                break;
                            }
                        if (!fits) break;
                    }
                    if (fits)
                    {
                        for (int y = 0; y < shapesList[i].Count; y++)
                        {
                            for (int x = 0; x < shapesList[i][0].Count; x++)
                                if (shapesList[i][y][x])
                                {
                                    zoneBoolList[k + y][j + x] = false;
                                }
                        }
                        fits = FillShape(zoneBoolList, shapesList, i + 1);
                        if (!fits)
                        {
                            for (int y = 0; y < shapesList[i].Count; y++)
                            {
                                for (int x = 0; x < shapesList[i][0].Count; x++)
                                    if (shapesList[i][y][x])
                                    {
                                        zoneBoolList[k + y][j + x] = true;  
                                    }
                            }
                        }
                        else
                        {
                            return true;
                        }
                    }
                    
                }
            }
            return false;
        }*/
        public bool CheckSolution(GameObject square)
        {
            bool isSolved = true;
            unsolvedElts.Clear();
            zone.Clear();
            SetZone(square);

            foreach (List<GameObject> p in zone)
            {
                bool localIsSolved = true;
                Color c = Color.red;
                List<GameObject> localShapes = new List<GameObject>();
                foreach (GameObject z in p)
                {

                    if (z.GetComponent<PoleSquare>().hasElem == true && z.GetComponent<PoleSquare>().element.GetComponent<EltClrRing>() != null)
                    {
                        if (c == Color.red)
                        {
                            c = z.GetComponent<PoleSquare>().element.GetComponent<EltClrRing>().c;
                        }
                        else
                        {
                            if (c != z.GetComponent<PoleSquare>().element.GetComponent<EltClrRing>().c)
                            {
                                localIsSolved = false;
                            }
                        }
                    }
                    else if (z.GetComponent<PoleSquare>().hasElem == true && z.GetComponent<PoleSquare>().element.GetComponent<PoleEltShape>() != null)
                    {
                       
                        localShapes.Add(z.GetComponent<PoleSquare>().element);
                    }
                }
                
                if(localIsSolved == false)
                {
                    foreach (GameObject z in p)
                    {
                        if (z.GetComponent<PoleSquare>().hasElem == true && z.GetComponent<PoleSquare>().element.GetComponent<EltClrRing>() != null)
                        {
                            unsolvedElts.Add(z.GetComponent<PoleSquare>().element);
                        }
                    }
                }
                bool solveShape = CheckShapeSplit(p, localShapes);
                if (!solveShape)
                {
                    foreach (GameObject sh in localShapes)
                        unsolvedElts.Add(sh);
                }
                localIsSolved = localIsSolved && solveShape;
                isSolved = localIsSolved && isSolved;
            }
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
    public List<GameObject> startDots = new List<GameObject>();
    public List<GameObject> starts = new List<GameObject>();
    public List<GameObject> finishDots = new List<GameObject>();
    public List<GameObject> finishes = new List<GameObject>();
    private int poleSize;
    PathDotStack dotData;
	public List<List<GameObject>> zone = new List<List<GameObject>>();
    public GameObject[][] poleDots;
    public List<GameObject> poleLines;
    public PolePath systemPath;
    public PolePath playerPath;
    List<List<List<GameObject>>> shapes = new List<List<List<GameObject>>>();
    List<List<List<GameObject>>> activeShapes = new List<List<List<GameObject>>>();

    public GameObject ShapePF;
	public GameObject ClrRingPrefab;
    public GameObject ClrStarPrefab;
    public GameObject SquerePrefab;
    public GameObject DotPrefab;
    public GameObject VerticalLinePrefab;
    public GameObject HorizontalLinePrefab;
    public GameObject StartPrefab;
    public GameObject FinishPrefab;
    public int quantityZones;
    public int quantityColor = 3;
    public int quantityRing = 0;
    List<Color> colorStar = new List<Color>() { Color.cyan, Color.yellow, Color.green, Color.magenta, Color.blue };
    List<Color> color = new List<Color>() { Color.cyan, Color.yellow, Color.green, Color.magenta, Color.black };
    public void OnDestroy()
    {
        
        for (int i = transform.childCount - 1; i >= 0; --i)
        {
            Destroy(transform.GetChild(i));
        }
    }
    public int GetSize() { return poleSize; }
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
            finishDots.Add(Instantiate(FinishPrefab, poleDots[0][x].transform.position, FinishPrefab.transform.rotation));
            finishDots[finishDots.Count - 1].transform.parent = this.transform;
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
        starts[0] = poleDots[1][0];
        startDots[0] = Instantiate(StartPrefab, poleDots[1][0].transform.position, StartPrefab.transform.rotation);
        startDots[0].GetComponent<StartDot>().LinkDot(starts[0]);
        startDots[0].transform.parent = this.transform;
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
            finishDots.Add( Instantiate(FinishPrefab, poleDots[y][2].transform.position, FinishPrefab.transform.rotation));
            finishDots[finishDots.Count - 1].transform.parent = this.transform;
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
        AddStart(0,0);
        
        StartScaling(starts[0]);
    }

    public void NormalizeColors()
    {
        foreach (GameObject point in GameObject.FindGameObjectsWithTag("EltPoint"))
        {
            point.GetComponent<PoleEltPoint>().ShowNormalizedColor();
            
        }
        foreach (GameObject point in GameObject.FindGameObjectsWithTag("EltClrRing"))
        {
            point.GetComponent<EltClrRing>().ShowNormalizedColor();

        }
        foreach (GameObject point in GameObject.FindGameObjectsWithTag("EltShape"))
        {
            point.GetComponent<PoleEltShape>().ShowNormalizedColor();

        }
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
                    Squere.GetComponent<PoleSquare>().indexI = y;
                    Squere.GetComponent<PoleSquare>().indexJ = x;
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

    public void InitStr(string info)
    {
        playerPath = new PolePath();
        
        eltsManager = new PoleElts();
        
        int iter = 0;
        if (info[iter].ToString() == "S")
        {
            iter++;
            string tmp = "";
            while(info[iter].ToString() != "s")
            {
                tmp += info[iter];
                iter++;
            }
            poleSize = int.Parse(tmp);
        }
        Core.PolePreferences.poleSize = poleSize;
        poleDots = new GameObject[poleSize][];
        poleLines = new List<GameObject>();
        for (int y = 0; y < poleSize; y++)
        {
            poleDots[y] = new GameObject[poleSize];
            for (int x = 0; x < poleSize; x++)
            {
                poleDots[y][x] = Instantiate(DotPrefab, transform.position + stepx * x + stepy * y, DotPrefab.transform.rotation);
                poleDots[y][x].GetComponent<PoleDot>().posX = x;
                poleDots[y][x].GetComponent<PoleDot>().posY = y;
                poleDots[y][x].transform.parent = this.transform;
            }
        }
        for (int y = 0; y < poleSize; y++)
        {
            for (int x = 0; x < poleSize - 1; x++)
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
                if (y < poleSize - 1)
                {
                    GameObject Squere = Instantiate(SquerePrefab, transform.position + stepx * (x + 0.5f) + stepy * (y + 0.5f), SquerePrefab.transform.rotation);
                    Squere.GetComponent<PoleSquare>().indexI = y;
                    Squere.GetComponent<PoleSquare>().indexJ = x;
                    Squere.GetComponent<PoleSquare>().up = lineH;
                    lineH.GetComponent<PoleLine>().down = Squere;
                    Squere.transform.parent = this.transform;
                }

            }

        }
        for (int y = 0; y < poleSize; y++)
        {
            for (int x = 0; x < poleSize; x++)
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
        if (info[++iter].ToString() == "S")
        {
            if (info[++iter].ToString() == "T")
            {
                iter++;
                string s = "";
                while (info[iter].ToString() != "Y")
                {
                    s += info[iter];
                    iter++;
                }
                int y = int.Parse(s);
                iter++;
                s = "";
                while (info[iter].ToString() != "X")
                {
                    s += info[iter];
                    iter++;
                }
                int x = int.Parse(s);
                AddStart(x, y);
            }
        }
        if (info[++iter].ToString() == "F")
        {
            if (info[++iter].ToString() == "H")
            {
                iter++;
                string s = "";
                while (info[iter].ToString() != "Y")
                {
                    s += info[iter];
                    iter++;
                }
                int y = int.Parse(s);
                iter++;
                s = "";
                while (info[iter].ToString() != "X")
                {
                    s += info[iter];
                    iter++;
                }
                int x = int.Parse(s);
                AddFinish(x, y);
            }
        }
        if (info[++iter].ToString() == "P")
        {
            if (info[++iter].ToString() == "T")
            {
                iter++;
                string tmp = "";
                while (info[iter].ToString() != "p")
                {
                    tmp += info[iter];
                    iter++;
                }
                int points = int.Parse(tmp);
                for (int i = 0; i < points; i++)
                {
                    iter++;
                    string s = "";
                    while (info[iter].ToString() != "Y")
                    {
                        s += info[iter];
                        iter++;
                    }
                    int y = int.Parse(s);
                    iter++;
                    s = "";
                    while (info[iter].ToString() != "X")
                    {
                        s += info[iter];
                        iter++;
                    }
                    int x = int.Parse(s);

                    switch (info[++iter].ToString())
                    {
                        case "U":
                            poleDots[y][x].GetComponent<PoleDot>().up.GetComponent<PoleLine>().hasPoint = true;
                            break;
                        case "D":
                            poleDots[y][x].GetComponent<PoleDot>().down.GetComponent<PoleLine>().hasPoint = true;
                            break;
                        case "L":
                            poleDots[y][x].GetComponent<PoleDot>().left.GetComponent<PoleLine>().hasPoint = true;
                            break;
                        case "R":
                            poleDots[y][x].GetComponent<PoleDot>().right.GetComponent<PoleLine>().hasPoint = true;
                            break;
                        case "S":
                            poleDots[y][x].GetComponent<PoleDot>().hasPoint = true;
                            break;
                    }

                }
            }
                    
        }
        if (info[++iter].ToString() == "R")
        {
            if (info[++iter].ToString() == "G")
            {
                iter++;
                string tmp = "";
                while (info[iter].ToString() != "r")
                {
                    tmp += info[iter];
                    iter++;
                }
                int rings = int.Parse(tmp);
                iter++;
                for (int k = 0; k < rings; k++)
                {
                    string s = "";
                    while (info[iter].ToString() != "I")
                    {
                        s += info[iter];
                        iter++;
                    }
                    int i = int.Parse(s);
                    iter++;
                    s = "";
                    while (info[iter].ToString() != "J")
                    {
                        s += info[iter];
                        iter++;
                    }
                    int j = int.Parse(s);
                    int color = int.Parse(info[++iter].ToString());
                    iter++;
                    foreach (GameObject sq in GameObject.FindGameObjectsWithTag("PoleSquere"))
                    {
                        if (sq.GetComponent<PoleSquare>().indexI == i && sq.GetComponent<PoleSquare>().indexJ == j)
                        {
                            sq.GetComponent<PoleSquare>().hasElem = true;
                            sq.GetComponent<PoleSquare>().element = Instantiate(ClrRingPrefab, sq.transform);
                            sq.GetComponent<PoleSquare>().element.GetComponent<Renderer>().material.color = this.color[color];
                            sq.GetComponent<PoleSquare>().element.GetComponent<EltClrRing>().c = this.color[color];
                        }
                    }
                }
            }
        }
        if (info[iter].ToString() == "S")
        {
            if (info[++iter].ToString() == "R")
            {
                iter++;
                string tmp = "";
                while (info[iter].ToString() != "s")
                {
                    tmp += info[iter];
                    iter++;
                }
                int rings = int.Parse(tmp);
                iter++;
                for (int k = 0; k < rings; k++)
                {
                    string s = "";
                    while (info[iter].ToString() != "I")
                    {
                        s += info[iter];
                        iter++;
                    }
                    int i = int.Parse(s);
                    iter++;
                    s = "";
                    while (info[iter].ToString() != "J")
                    {
                        s += info[iter];
                        iter++;
                    }
                    int j = int.Parse(s);
                    int color = int.Parse(info[++iter].ToString());
                    iter++;
                    foreach (GameObject sq in GameObject.FindGameObjectsWithTag("PoleSquere"))
                    {
                        if (sq.GetComponent<PoleSquare>().indexI == i && sq.GetComponent<PoleSquare>().indexJ == j)
                        {
                            sq.GetComponent<PoleSquare>().hasElem = true;
                            sq.GetComponent<PoleSquare>().element = Instantiate(ClrStarPrefab, sq.transform);
                            sq.GetComponent<PoleSquare>().element.GetComponent<Renderer>().material.color = this.color[color];
                            sq.GetComponent<PoleSquare>().element.GetComponent<PoleEltStar>().c = this.color[color];
                        }
                    }
                }
            }
        }
        if (info[iter].ToString() == "S")
        {
            if (info[++iter].ToString() == "P")
            {
                iter++;
                string tmp = "";
                while (info[iter].ToString() != "s")
                {
                    tmp += info[iter];
                    iter++;
                }
                int rings = int.Parse(tmp);
                iter++;
                for (int k = 0; k < rings; k++)
                {
                    string s = "";
                    while (info[iter].ToString() != "I")
                    {
                        s += info[iter];
                        iter++;
                    }
                    int i = int.Parse(s);
                    iter++;
                    s = "";
                    while (info[iter].ToString() != "J")
                    {
                        s += info[iter];
                        iter++;
                    }
                    int j = int.Parse(s);
                    iter++;
                    s = "";
                    while (info[iter].ToString() != "H")
                    {
                        s += info[iter];
                        iter++;
                    }
                    int height = int.Parse(s);
                    iter++;
                    s = "";
                    while (info[iter].ToString() != "W")
                    {
                        s += info[iter];
                        iter++;
                    }
                    int width = int.Parse(s);
                    iter++;
                    List<List<bool>> bitmap = new List<List<bool>>();
                    for (int y = 0; y < height; y++)
                    {
                        bitmap.Add(new List<bool>());
                        for (int x = 0; x < width; x++)
                        {
                            if (int.Parse(info[iter].ToString()) == 1) bitmap[y].Add(true); else bitmap[y].Add(false);
                            iter++;
                        }
                        
                    }
                    foreach (GameObject sq in GameObject.FindGameObjectsWithTag("PoleSquere"))
                    {
                        if (sq.GetComponent<PoleSquare>().indexI == i && sq.GetComponent<PoleSquare>().indexJ == j)
                        {
                            sq.GetComponent<PoleSquare>().hasElem = true;
                            sq.GetComponent<PoleSquare>().element = Instantiate(ShapePF, sq.transform);
                            sq.GetComponent<PoleSquare>().element.GetComponent<PoleEltShape>().boolList = bitmap;
                            sq.GetComponent<PoleSquare>().element.GetComponent<PoleEltShape>().Create();
                        }
                    }
                    //Debug.Log(rings + " " + k + " " + height + " " + width);
                }
            }
        }
        foreach (GameObject start in starts) StartScaling(start);
    }
    // "S(size)sSTposyYposxXFHposyYposxXPT(num)p{posyYposxXdir}RG(num)r{indexIIindexJJcolor}SR(num)s{indexIIindexJJcolor}SP(num)s{indexIIindexJJheightHwidthWbitmap}
    public bool FindPath(GameObject begin, GameObject end, int[][] ways)
    {
        begin.GetComponent<PoleDot>().isUsedBySolution = true;
        if (begin == end)
        {
            if (dotData.PathLength() <= Core.PolePreferences.complexity)
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
            if (dotData.PathLength() < Core.PolePreferences.complexity)
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
                            //ways[begin.GetComponent<PoleDot>().posY - 1][begin.GetComponent<PoleDot>().posX] = 0;
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
                            //ways[begin.GetComponent<PoleDot>().posY][begin.GetComponent<PoleDot>().posX + 1] = 0;
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
                            //ways[begin.GetComponent<PoleDot>().posY + 1][begin.GetComponent<PoleDot>().posX] = 0;
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
                            //ways[begin.GetComponent<PoleDot>().posY][begin.GetComponent<PoleDot>().posX - 1] = 0;
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
        if (dotData.PathLength() > poleSize * poleSize * Core.PolePreferences.complexity &&
            (begin.GetComponent<PoleDot>().posX == 0 ||
             begin.GetComponent<PoleDot>().posX == poleSize - 1 ||
             begin.GetComponent<PoleDot>().posY == 0 ||
             begin.GetComponent<PoleDot>().posY == poleSize - 1))
        {
            finishes[0].GetComponent<PoleDot>().isUsedBySolution = false;
            finishes.Clear();
            Destroy(GameObject.FindGameObjectWithTag("PoleFinish"));
            AddFinish(begin.GetComponent<PoleDot>().posX, begin.GetComponent<PoleDot>().posY);
            return true;
        }
        //ways[begin.GetComponent<PoleDot>().posY][begin.GetComponent<PoleDot>().posX] = 0;
        begin.GetComponent<PoleDot>().isUsedBySolution = false;
        dotData.GetDot();
        
        return false;
    }

    private void FindZone(GameObject square,int[][] poleZones, int x, int y)
    {
        int size = poleSize - 1;
        GameObject lineH = square.GetComponent<PoleSquare>().up;
        if (!lineH.GetComponent<PoleLine>().isUsedBySolution && lineH.GetComponent<PoleLine>().up != null)
        {
            if (y > 0 && poleZones[y - 1][x] == 0)
            {
                poleZones[y - 1][x] = poleZones[y][x];
                FindZone(lineH.GetComponent<PoleLine>().up, poleZones, x, y - 1);
            }
        }
        lineH = square.GetComponent<PoleSquare>().down;
        if (!lineH.GetComponent<PoleLine>().isUsedBySolution && lineH.GetComponent<PoleLine>().down != null)
        {
            if (y < size && poleZones[y + 1][x] == 0)
            {
                poleZones[y + 1][x] = poleZones[y][x];
                FindZone(lineH.GetComponent<PoleLine>().down, poleZones, x, y + 1);
            }
        }

        GameObject lineV = square.GetComponent<PoleSquare>().left;
        if (!lineV.GetComponent<PoleLine>().isUsedBySolution && lineV.GetComponent<PoleLine>().left != null)
        {
            if (x > 0 && poleZones[y][x - 1] == 0)
            {
                poleZones[y][x - 1] = poleZones[y][x];
                FindZone(lineV.GetComponent<PoleLine>().left, poleZones, x - 1, y);
            }
        }
        lineV = square.GetComponent<PoleSquare>().right;
        if (!lineV.GetComponent<PoleLine>().isUsedBySolution && lineV.GetComponent<PoleLine>().right != null)
        {
            if (x < size &&poleZones[y][x + 1] == 0)
            {
                poleZones[y][x + 1] = poleZones[y][x];
                FindZone(lineV.GetComponent<PoleLine>().right, poleZones, x + 1, y);
            }
        }
    }
    public void SetZone()
    {
        int size = poleSize - 1;
        int[][] poleZones;
        poleZones = new int[size][];
        for (int y = 0; y < size; y++)
        {
            poleZones[y] = new int[size];
            for (int x = 0; x < size; x++)
            {
                poleZones[y][x] = 0;
            }
        }
        GameObject square = poleDots[0][0].GetComponent<PoleDot>().right.GetComponent<PoleLine>().down;
        quantityZones = 0;
        for (int x = 0; x < size; ++x)
        {
            GameObject square1 = square;
            for (int y = 0; y < size; ++y)
            {
                if (poleZones[y][x] == 0)
                {
                    ++quantityZones;
                    poleZones[y][x] = quantityZones;
                    FindZone(square1, poleZones, x, y);
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
    

    public void AddStart(int x, int y)
    {
        startDots.Add(Instantiate(StartPrefab, stepx * x + stepy * y, StartPrefab.transform.rotation));

        starts.Add(poleDots[y][x]);
        startDots[startDots.Count - 1].GetComponent<StartDot>().LinkDot(starts[starts.Count - 1]);
        startDots[startDots.Count - 1].transform.parent = this.transform;
    }
    public void AddFinish(int x, int y)
    {
        finishDots.Add(Instantiate(FinishPrefab, stepx * x + stepy * y, FinishPrefab.transform.rotation));
        finishes.Add(poleDots[y][x]);
    }
    public void GeneratePoints(int numberOfPoints)
    {
        eltsManager.points.Clear();
        numberOfPoints = System.Math.Min(numberOfPoints, (systemPath.dots.Count + systemPath.lines.Count) / 2 - 4);
        List<GameObject> pathList = new List<GameObject>();
        pathList.Add(systemPath.dots[0]);
        for (int i = 0; i<systemPath.dots.Count-2; i++)
        {
            pathList.Add(systemPath.lines[i]);
            pathList.Add(systemPath.dots[i + 1]);
        }
        foreach(GameObject start in starts)
            pathList.Remove(start);
        foreach (GameObject finish in finishes)
            pathList.Remove(finish);
        for (int i = 0; i < numberOfPoints; i++)
        {
            if (pathList.Count == 0) break;
            int r = Core.PolePreferences.MyRandom.GetRandom() % pathList.Count;
            if (pathList[r].GetComponent<PoleDot>() != null)
            {
                pathList[r].GetComponent<PoleDot>().hasPoint = true;
                var obj = pathList[r];
                pathList.Remove(obj);
                if (pathList.Contains(obj.GetComponent<PoleDot>().left)) pathList.Remove(obj.GetComponent<PoleDot>().left);
                if (pathList.Contains(obj.GetComponent<PoleDot>().right)) pathList.Remove(obj.GetComponent<PoleDot>().right);
                if (pathList.Contains(obj.GetComponent<PoleDot>().up)) pathList.Remove(obj.GetComponent<PoleDot>().up);
                if (pathList.Contains(obj.GetComponent<PoleDot>().down)) pathList.Remove(obj.GetComponent<PoleDot>().down);
            }
            else
            {
                pathList[r].GetComponent<PoleLine>().hasPoint = true;
                var obj = pathList[r];
                pathList.Remove(obj);
                if (pathList.Contains(obj.GetComponent<PoleLine>().left)) pathList.Remove(obj.GetComponent<PoleLine>().left);
                if (pathList.Contains(obj.GetComponent<PoleLine>().right)) pathList.Remove(obj.GetComponent<PoleLine>().right);
                if (pathList.Contains(obj.GetComponent<PoleLine>().up)) pathList.Remove(obj.GetComponent<PoleLine>().up);
                if (pathList.Contains(obj.GetComponent<PoleLine>().down)) pathList.Remove(obj.GetComponent<PoleLine>().down);
            }
            if (pathList.Count == 0) break;

            //r %= systemPath.dots.Count + systemPath.lines.Count;
            //if (r % 2 == 0)
            //{
            //    if (!systemPath.dots[(r / 2)].GetComponent<PoleDot>().hasPoint)
            //    {
            //        systemPath.dots[(r / 2)].GetComponent<PoleDot>().hasPoint = true;
            //    }
            //    else i--;
            //}
            //else
            //{
            //    if (!systemPath.lines[(r - 1) / 2].GetComponent<PoleLine>().hasPoint)
            //    {
            //        systemPath.lines[(r - 1) / 2].GetComponent<PoleLine>().hasPoint = true;
            //    }
            //    else i--;
            //}

        }
    }
	public void SetClrRing(int zoneQuantity, int ringQuantity)
    {
        List<List<GameObject>> coloredZones = new List<List<GameObject>>(zoneQuantity);
        List<int> quantityClrRingInZone = new List<int>();
        List<List<GameObject>> localZone = zone;
        for (int i = 0; i < localZone.Count; ++i)
        {
            for(int j = 0; j < localZone[i].Count; ++j)
            {
                if(localZone[i][j].GetComponent<PoleSquare>().hasElem == true)
                {
                    localZone[i].RemoveAt(j);
                    --j;
                }
            }
            if(localZone[i].Count == 0)
            {
                localZone.RemoveAt(i);
                --i;
            }
        }
        zoneQuantity = (Core.PolePreferences.MyRandom.GetRandom()) % (color.Count - zoneQuantity) + zoneQuantity;
        if (zoneQuantity > localZone.Count)
        {
            Debug.Log( "free zones less than need"+ "need:"+ zoneQuantity + " have:" + localZone.Count);
            zoneQuantity = localZone.Count;
        }
        ringQuantity = Mathf.RoundToInt(ringQuantity * 0.7f);
        int quantityNotUsedSquare = -zoneQuantity;
        ringQuantity -= zoneQuantity;
        for (int i = 0; i < localZone.Count;++i)
        {
            quantityNotUsedSquare += localZone[i].Count;
            if (i < zoneQuantity)
            {
                quantityClrRingInZone.Add(1);
                coloredZones.Add(new List<GameObject>());
                coloredZones[i].AddRange(localZone[i]);
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
                coloredZones[mm].AddRange(localZone[i]);
            }
        }
        if (ringQuantity > quantityNotUsedSquare)
        {
            Debug.Log("square in zones less than need");
            ringQuantity = quantityNotUsedSquare;
        }
        while (ringQuantity > 0)
        {
            int i = Core.PolePreferences.MyRandom.GetRandom() % (quantityNotUsedSquare)+1;
            int k = 0;
            while (i - (coloredZones[k].Count - quantityClrRingInZone[k]) > 0)
            {
                i -= (coloredZones[k].Count - quantityClrRingInZone[k]);
                k++;
            }
            --quantityNotUsedSquare;
            ++quantityClrRingInZone[k];
            --ringQuantity;
        }
        for(int j = 0;j < zoneQuantity;++j)
        {
            int k = Core.PolePreferences.MyRandom.GetRandom() % color.Count;
            while(quantityClrRingInZone[j] > 0)
            {
                int i = Core.PolePreferences.MyRandom.GetRandom() % coloredZones[j].Count;
                coloredZones[j][i].GetComponent<PoleSquare>().hasElem = true;
                coloredZones[j][i].GetComponent<PoleSquare>().element = Instantiate(ClrRingPrefab, coloredZones[j][i].transform.position, ClrRingPrefab.transform.rotation);
                coloredZones[j][i].GetComponent<PoleSquare>().element.GetComponent<EltClrRing>().c = color[k];
                eltsManager.clrRing.Add(coloredZones[j][i].GetComponent<PoleSquare>().element);
                coloredZones[j][i].GetComponent<PoleSquare>().element.GetComponent<MeshRenderer>().material.color = color[k];
                quantityClrRingInZone[j]--;
                coloredZones[j].RemoveAt(i);
            }
            color.RemoveAt(k);
        }
    }
    public void SetClrStar(int starQuantity)
    {
        int k = 0;
        List<List<GameObject>> localZones = new List<List<GameObject>>();
        for (int i = 0; i < zone.Count; ++i)
        {
            localZones.Add(new List<GameObject>());
            for (int j = 0; j < zone[i].Count; ++j)
            {
                if (zone[i][j].GetComponent<PoleSquare>().hasElem == false)
                {
                    localZones[i].Add(zone[i][j]);
                }
            }
        }
        for (int i = 0; i < zone.Count; ++i)
        {
            if (localZones[i].Count > 1)
            {
                k += 2;
            }


        }
        if (starQuantity <= k)
        {
            while(starQuantity > 0)
            {
                int i;
                do
                {
                    i = Core.PolePreferences.MyRandom.GetRandom() % localZones.Count;
                } while (localZones[i].Count < 2);
                if(localZones[i].Count == zone[i].Count - 1 && Core.PolePreferences.MyRandom.GetRandom() % 2 == 0)
                {
                    Color c = new Color();
                    for (int g = 0; g < zone[i].Count; ++g)
                    {
                        if (zone[i][g].GetComponent<PoleSquare>().hasElem == true)
                        {
                            c = zone[i][g].GetComponent<PoleSquare>().element.GetComponent<EltClrRing>().c;
                            break;
                        }
                    }
                    int j = Core.PolePreferences.MyRandom.GetRandom() % localZones[i].Count;
                    localZones[i][j].GetComponent<PoleSquare>().hasElem = true;
                    localZones[i][j].GetComponent<PoleSquare>().element = Instantiate(ClrStarPrefab, localZones[i][j].transform.position, ClrStarPrefab.transform.rotation);
                    localZones[i][j].GetComponent<PoleSquare>().element.GetComponent<PoleEltStar>().c = c;
                    eltsManager.clrRing.Add(localZones[i][j].GetComponent<PoleSquare>().element);
                    localZones[i][j].GetComponent<PoleSquare>().element.GetComponent<MeshRenderer>().material.color = c;
                    localZones[i].RemoveAt(j);
                }
                else
                {
                    Color c = new Color();
                    for(int j = 0;j < zone[i].Count;++j)
                    {
                        if(zone[i][j].GetComponent<PoleSquare>().hasElem == true)
                        {
                            c = zone[i][j].GetComponent<PoleSquare>().element.GetComponent<EltClrRing>().c;
                            break;
                        }
                    }
                    int t;
                    do
                    {
                        t = Core.PolePreferences.MyRandom.GetRandom() % colorStar.Count;
                    } while (c == colorStar[t]);
                    for (int g = 0; g < 2; ++g)
                    { 
                        int j = Core.PolePreferences.MyRandom.GetRandom() % localZones[i].Count;
                        localZones[i][j].GetComponent<PoleSquare>().hasElem = true;
                        localZones[i][j].GetComponent<PoleSquare>().element = Instantiate(ClrStarPrefab, localZones[i][j].transform.position, ClrStarPrefab.transform.rotation);
                        localZones[i][j].GetComponent<PoleSquare>().element.GetComponent<PoleEltStar>().c = colorStar[t];
                        eltsManager.clrRing.Add(localZones[i][j].GetComponent<PoleSquare>().element);
                        localZones[i][j].GetComponent<PoleSquare>().element.GetComponent<MeshRenderer>().material.color = colorStar[t];
                        localZones[i].RemoveAt(j);
                    }
                }
                starQuantity -= 2;
                localZones.RemoveAt(i);
                zone.RemoveAt(i);
            }
        }
        else
        {
            Debug.Log("free square less than need to star");
            return;
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
        ways[starts[0].GetComponent<PoleDot>().posY][starts[0].GetComponent<PoleDot>().posX] = 1;
        dotData = new PathDotStack();
        //bool isFound = FindPath(start, finish, ways);
        bool isFound = FindPathQuick(starts[0], finishes[0], ways);
        while (!isFound)
        {
            for (int i = 0; i < poleSize; i++)
            {
                for (int j = 0; j < poleSize; j++)
                {
                    ways[i][j] = 0;
                }
            }
            ways[starts[0].GetComponent<PoleDot>().posY][starts[0].GetComponent<PoleDot>().posX] = 1;
            dotData = new PathDotStack();
            isFound = FindPathQuick(starts[0], finishes[0], ways);
        }
        if (isFound)
        {
            foreach (GameObject dot in GameObject.FindGameObjectsWithTag("PoleDot"))
                if (dot.GetComponent<PoleDot>().isUsedBySolution) systemPath.dots.Add(dot);
            
            foreach (GameObject obj in GameObject.FindGameObjectsWithTag("PoleLine"))
                if (obj.GetComponent<PoleLine>().isUsedBySolution) systemPath.lines.Add(obj);
        }
        SetZone();
        SetShapes();
        
    }
    public void GenerateShapes(int zoneSize)
    {
        for (int i = 0; i < zone.Count; i++) activeShapes.Add(new List<List<GameObject>>());
        //foreach(List<List<GameObject>> shlist in shapes)
        //{
        //    if (zone[shapes.IndexOf(shlist)].Count <= zoneSize)
        //    {
        //        foreach (List<GameObject> shape in shlist)
        //        {
        //            activeShapes[shapes.IndexOf(shlist)].Add(shape);
        //        }
        //        zoneSize -= zone[shapes.IndexOf(shlist)].Count;
        //    }
        //}
        var list = new List<List<List<GameObject>>>(shapes);
        
        for (int i = 0; i < shapes.Count; i++)
        {
            int k = Core.PolePreferences.MyRandom.GetRandom() % list.Count;
            if (zone[shapes.IndexOf(list[k])].Count <= zoneSize)
            {
                
                foreach (List<GameObject> shape in list[k])
                {
                    activeShapes[shapes.IndexOf(list[k])].Add(shape);
                }
                zoneSize -= zone[shapes.IndexOf(list[k])].Count;
            }
            list.Remove(list[k]);
        }
        //for (int i = 0; i < shapes.Count; i++)
        //{
        //    int k = i;
        //    if (zone[shapes.IndexOf(shapes[k])].Count <= zoneSize)
        //    {

        //        foreach (List<GameObject> shape in shapes[k])
        //        {
        //            activeShapes[shapes.IndexOf(shapes[k])].Add(shape);
        //        }
        //        zoneSize -= zone[shapes.IndexOf(shapes[k])].Count;
        //    }
        //    Debug.Log(zoneSize);
        //    Debug.Log(zone[shapes.IndexOf(shapes[k])].Count);
        //    shapes.Remove(shapes[k]);
        //}

        //if (difficulty == 0)
        //{
        //    foreach (List<List<GameObject>> shlist in shapes)
        //    {
        //        if (shlist.Count < numOfShapes)
        //        {
        //            foreach (List<GameObject> shape in shlist)
        //            {
        //                activeShapes[shapes.IndexOf(shlist)].Add(shape);
        //            }
        //            numOfShapes -= shlist.Count;
        //        }
        //    }
        //}
        //else
        //{
        //    foreach (List<List<GameObject>> shlist in shapes)
        //    {
        //        if (shlist.Count < difficulty)
        //        {
        //            foreach (List<GameObject> shape in shlist)
        //            {
        //                activeShapes[shapes.IndexOf(shlist)].Add(shape);
        //            }
        //            numOfShapes -= shlist.Count;
        //        }
        //    }
        //}
        DrawShapes();
        //foreach (List<List<GameObject>> shlist in shapes)
        //{
            
        //    foreach (List<GameObject> shape in shlist)
        //    {
        //        GameObject shapeElt = Instantiate(ShapePF, shape[0].transform);
        //        shape[0].GetComponent<PoleSquare>().hasElem = true;
        //        shape[0].GetComponent<PoleSquare>().element = shapeElt;
        //        shapeElt.GetComponent<PoleEltShape>().boolList = ZoneToBoolList(shape);
        //        shapeElt.GetComponent<PoleEltShape>().Create();
        //    }
        //}
    }

    public void DrawShapes()
    {
        for (int i = 0; i < zone.Count; i++)
        {
            List<GameObject> sqList = new List<GameObject>(zone[i]);
            var tmplist = new List<GameObject>(sqList);
            foreach (GameObject sq in sqList)
            {
                if (sq.GetComponent<PoleSquare>().hasElem) tmplist.Remove(sq);
            }
            sqList = tmplist;
            for (int j = 0; j < activeShapes[i].Count; j++)
            {
                if (sqList.Count == 0)
                {
                    //Debug.Log("ILYA ZAEBAL");
                    //SceneManager.LoadScene("PoleLevel");
                    continue;
                }
                GameObject sq = sqList[Core.PolePreferences.MyRandom.GetRandom() % sqList.Count];
                GameObject shapeElt = Instantiate(ShapePF, sq.transform);
                sq.GetComponent<PoleSquare>().hasElem = true;
                sq.GetComponent<PoleSquare>().element = shapeElt;
                shapeElt.GetComponent<PoleEltShape>().boolList = ZoneToBoolList(activeShapes[i][j]);
                shapeElt.GetComponent<PoleEltShape>().Create();
                sqList.Remove(sq);
            }
        }
    }

    public void SetShapes()
    {
        List<List<GameObject>> set = new List<List<GameObject>>(zone);
        
        
        for (int i = 0; i < set.Count; i++)
        {
            set[i] = new List<GameObject>(zone[i]);
        }
        foreach (List<GameObject> z in set)
        {
            shapes.Add(SplitZone(z));
        }
        
    }

    public List<List<GameObject>> SplitZone(List<GameObject> zone)
    {
        List<List<GameObject>> zoneShapes = new List<List<GameObject>>();
        List<GameObject> currentShape = new List<GameObject>();
        List<GameObject> squeresToCheck = new List<GameObject>();
        while (zone.Count > 0)
        {
            currentShape = new List<GameObject>();
            currentShape.Add(zone[Core.PolePreferences.MyRandom.GetRandom() % zone.Count]);
            while (currentShape.Count < 4)
            {
                foreach (GameObject currentSquere in currentShape)
                {
                    if (currentSquere.GetComponent<PoleSquare>().up.GetComponent<PoleLine>().up != null && zone.Contains(currentSquere.GetComponent<PoleSquare>().up.GetComponent<PoleLine>().up) && !currentShape.Contains(currentSquere.GetComponent<PoleSquare>().up.GetComponent<PoleLine>().up))
                    {
                        squeresToCheck.Add(currentSquere.GetComponent<PoleSquare>().up.GetComponent<PoleLine>().up);
                    }
                    if (currentSquere.GetComponent<PoleSquare>().right.GetComponent<PoleLine>().right != null && zone.Contains(currentSquere.GetComponent<PoleSquare>().right.GetComponent<PoleLine>().right) && !currentShape.Contains(currentSquere.GetComponent<PoleSquare>().right.GetComponent<PoleLine>().right))
                    {
                        squeresToCheck.Add(currentSquere.GetComponent<PoleSquare>().right.GetComponent<PoleLine>().right);
                    }
                    if (currentSquere.GetComponent<PoleSquare>().down.GetComponent<PoleLine>().down != null && zone.Contains(currentSquere.GetComponent<PoleSquare>().down.GetComponent<PoleLine>().down) && !currentShape.Contains(currentSquere.GetComponent<PoleSquare>().down.GetComponent<PoleLine>().down))
                    {
                        squeresToCheck.Add(currentSquere.GetComponent<PoleSquare>().down.GetComponent<PoleLine>().down);
                    }
                    if (currentSquere.GetComponent<PoleSquare>().left.GetComponent<PoleLine>().left != null && zone.Contains(currentSquere.GetComponent<PoleSquare>().left.GetComponent<PoleLine>().left) && !currentShape.Contains(currentSquere.GetComponent<PoleSquare>().left.GetComponent<PoleLine>().left))
                    {
                        squeresToCheck.Add(currentSquere.GetComponent<PoleSquare>().left.GetComponent<PoleLine>().left);
                    }
                }
                if (squeresToCheck.Count == 0) break;
                currentShape.Add(squeresToCheck[Core.PolePreferences.MyRandom.GetRandom() % squeresToCheck.Count]);
                squeresToCheck.Clear();
            }
            foreach (GameObject squere in currentShape)
            {
                zone.Remove(squere);
            }
            zoneShapes.Add(currentShape);
        }
        return zoneShapes;
    }

    public static List<List<bool>> ZoneToBoolList(List<GameObject> zone)
    {
        List<List<bool>> zoneBool = new List<List<bool>>();

        for (int i = 0; i < Core.PolePreferences.poleSize; i++)
        {
            zoneBool.Add(new List<bool>());
            for (int j = 0; j < Core.PolePreferences.poleSize; j++)
            {
                zoneBool[i].Add(false);
            }
        }
        foreach(GameObject squere in zone)
        {
            zoneBool[squere.GetComponent<PoleSquare>().indexI][squere.GetComponent<PoleSquare>().indexJ] = true;
        }

        bool[] rowsToDel = new bool[zoneBool.Count];
        for (int n = 0; n < zoneBool.Count; n++)
        {
            bool check = false;
            for (int i = 0; i < zoneBool[n].Count; i++) check = check || zoneBool[n][i];
            rowsToDel[n] = check;
        }
        for (int i = rowsToDel.Length-1; i >=0; i--)
        {
            if (!rowsToDel[i])
            {
                zoneBool.RemoveAt(i);
            }
        }
        bool[] columnsToDel = new bool[zoneBool[0].Count];
        
        for (int n = 0; n < zoneBool[0].Count; n++)
        {
            bool check = false;
            for (int i = 0; i < zoneBool.Count; i++) check = check || zoneBool[i][n];
            columnsToDel[n] = check;
        }
        for (int i = columnsToDel.Length - 1; i >=0; i--)
        {
            if (!columnsToDel[i])
            {
                for (int j = 0; j < zoneBool.Count; j++)
                {
                    zoneBool[j].RemoveAt(i);
                }
            }
        }
        
        return zoneBool;
        
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
        foreach (GameObject temp in GameObject.FindGameObjectsWithTag("PoleStart")) Destroy(temp);
        foreach (GameObject temp in GameObject.FindGameObjectsWithTag("PoleFinish")) Destroy(temp);
        systemPath.dots.Clear();
        systemPath.lines.Clear();
    }

    public List<GameObject> scalingLines = new List<GameObject>();
    public List<GameObject> scalingDots = new List<GameObject>();

    public void StartScaling(GameObject dot)
    {
        if (dot.GetComponent<PoleDot>().AllowedToDown())
        {
            if (!dot.GetComponent<PoleDot>().down.GetComponent<PoleLine>().scalingIsFinished)
                scalingLines.Add(dot.GetComponent<PoleDot>().down);
            else scalingLines.Remove(dot.GetComponent<PoleDot>().down);
        }
        if (dot.GetComponent<PoleDot>().AllowedToUp())
        {
            if (!dot.GetComponent<PoleDot>().up.GetComponent<PoleLine>().scalingIsFinished)
                scalingLines.Add(dot.GetComponent<PoleDot>().up);
            else scalingLines.Remove(dot.GetComponent<PoleDot>().up);
        }
        if (dot.GetComponent<PoleDot>().AllowedToLeft())
        {
            if (!dot.GetComponent<PoleDot>().left.GetComponent<PoleLine>().scalingIsFinished)
                scalingLines.Add(dot.GetComponent<PoleDot>().left);
            else scalingLines.Remove(dot.GetComponent<PoleDot>().left);
        }
        if (dot.GetComponent<PoleDot>().AllowedToRight())
        {
            if (!dot.GetComponent<PoleDot>().right.GetComponent<PoleLine>().scalingIsFinished)
                scalingLines.Add(dot.GetComponent<PoleDot>().right);
            else scalingLines.Remove(dot.GetComponent<PoleDot>().right);
        }
        foreach (GameObject line in scalingLines)
        {
            if (line != null)
                line.GetComponent<PoleLine>().StartScaling(dot);

        }
    }

    public string PathToStr(GameObject start)
    {
        string path = "S";
        GameObject cur = start;
        while (cur != playerPath.dots[playerPath.dots.Count - 1])
        {
            //Debug.Log(path.Length + " " + path);
            if (cur.GetComponent<PoleDot>().up != null && cur.GetComponent<PoleDot>().up.GetComponent<PoleLine>().isUsedByPlayer && path[path.Length-1].ToString() != "D")
            {
                path += "U";
                cur = cur.GetComponent<PoleDot>().up.GetComponent<PoleLine>().up;
            }
            else if (cur.GetComponent<PoleDot>().down != null && cur.GetComponent<PoleDot>().down.GetComponent<PoleLine>().isUsedByPlayer && path[path.Length - 1].ToString() != "U")
            {
                path += "D";
                cur = cur.GetComponent<PoleDot>().down.GetComponent<PoleLine>().down;
            }
            else if (cur.GetComponent<PoleDot>().left != null && cur.GetComponent<PoleDot>().left.GetComponent<PoleLine>().isUsedByPlayer && path[path.Length - 1].ToString() != "R")
            {
                path += "L";
                cur = cur.GetComponent<PoleDot>().left.GetComponent<PoleLine>().left;
            }
            else if (cur.GetComponent<PoleDot>().right != null && cur.GetComponent<PoleDot>().right.GetComponent<PoleLine>().isUsedByPlayer && path[path.Length - 1].ToString() != "L")
            {
                path += "R";
                cur = cur.GetComponent<PoleDot>().right.GetComponent<PoleLine>().right;
            }
        }
        path += "F";
        Debug.Log(path);
        return path;
    }

    public string SysPathToStr(GameObject start)
    {
        string path = "S";
        GameObject cur = start;
        while (cur != systemPath.dots[systemPath.dots.Count - 1])
        {
            //Debug.Log(path.Length + " " + path);
            if (cur.GetComponent<PoleDot>().up != null && cur.GetComponent<PoleDot>().up.GetComponent<PoleLine>().isUsedBySolution && path[path.Length - 1].ToString() != "D")
            {
                path += "U";
                cur = cur.GetComponent<PoleDot>().up.GetComponent<PoleLine>().up;
            }
            else if (cur.GetComponent<PoleDot>().down != null && cur.GetComponent<PoleDot>().down.GetComponent<PoleLine>().isUsedBySolution && path[path.Length - 1].ToString() != "U")
            {
                path += "D";
                cur = cur.GetComponent<PoleDot>().down.GetComponent<PoleLine>().down;
            }
            else if (cur.GetComponent<PoleDot>().left != null && cur.GetComponent<PoleDot>().left.GetComponent<PoleLine>().isUsedBySolution && path[path.Length - 1].ToString() != "R")
            {
                path += "L";
                cur = cur.GetComponent<PoleDot>().left.GetComponent<PoleLine>().left;
            }
            else if (cur.GetComponent<PoleDot>().right != null && cur.GetComponent<PoleDot>().right.GetComponent<PoleLine>().isUsedBySolution && path[path.Length - 1].ToString() != "L")
            {
                path += "R";
                cur = cur.GetComponent<PoleDot>().right.GetComponent<PoleLine>().right;
            }
        }
        path += "F";
        Debug.Log(path);
        return path;
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
