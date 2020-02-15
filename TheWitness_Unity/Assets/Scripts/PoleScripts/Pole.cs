using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Pole : MonoBehaviour
{
    [Header("Pole Dimensions")]
    public int height;
    public int width;

    [Header("Start Locations")]
    public List<GameObject> starts = new List<GameObject>();

    [Header("Finish Locations")]
    public List<GameObject> finishes = new List<GameObject>();

    [Header("Containers")]
    public GameObject DotsParent;
    public GameObject LinesParent;
    public GameObject SquaresParent;

    [Header("Prefabs")]
    public GameObject ShapePF;
	public GameObject ClrRingPF;
    public GameObject ClrStarPF;
    public GameObject SquerePF;
    public GameObject DotPF;
    public GameObject VerticalLinePF;
    public GameObject HorizontalLinePF;

    [HideInInspector]
    public PoleElts eltsManager;
    [HideInInspector]
    public GameObject[][] poleDots;
    [HideInInspector]
    public List<GameObject> poleLines;
    [HideInInspector]
    public PolePath systemPath;
    [HideInInspector]
    public PolePath playerPath;

    //no need?
    private PathDotStack dotData;

	private readonly List<List<GameObject>> zone = new List<List<GameObject>>();
    private readonly List<List<List<GameObject>>> shapes = new List<List<List<GameObject>>>();
    private readonly List<List<List<GameObject>>> activeShapes = new List<List<List<GameObject>>>();
    private readonly List<GameObject> scalingLines = new List<GameObject>();

    //?????????????????????????????????????????????????
    private int quantityZones;
    [HideInInspector]
    public int quantityColor = 3;
    [HideInInspector]
    public int quantityRing = 0;
    private readonly List<Color> colorStar = new List<Color>() { Color.cyan, Color.yellow, Color.green, Color.magenta, Color.blue };
    private readonly List<Color> color = new List<Color>() { Color.cyan, Color.yellow, Color.green, Color.magenta, Color.blue };
    //?????????????????????????????????????????????????

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
        public int height;
        public int width;
        public List<Elements> points;
        public List<Elements> shapes;
        public List<Elements> clrRing;
        public List<Elements> unsolvedElts;
        public int[][] checkZones;
        public List<List<GameObject>> zone = new List<List<GameObject>>();
        private void FindZone(GameObject square, int x, int y)
        {
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
                if (y < height - 1 && checkZones[y + 1][x] == 0)
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
                if (x < width - 1 && checkZones[y][x + 1] == 0)
                {
                    checkZones[y][x + 1] = checkZones[y][x];
                    FindZone(lineV.GetComponent<PoleLine>().right, x + 1, y);
                }
            }
        }
        public void SetZone(GameObject square)
        {
            zone = new List<List<GameObject>>();
            checkZones = new int[height-1][];
            for (int i = 0; i < height-1; ++i)
            {
                checkZones[i] = new int[width-1];
                for (int j = 0; j < width-1; ++j)
                {
                    checkZones[i][j] = 0;
                }
            }
            GameObject square1 = square;
            int quantityZones = 0;
            for (int x = 0; x < width-1; ++x)
            {
                GameObject square2 = square1;
                for (int y = 0; y < height-1; ++y)
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
            for (int x = 0; x < width-1; ++x)
            {
                GameObject square2 = square1;
                for (int y = 0; y < height-1; ++y)
                {
                    zone[checkZones[y][x] - 1].Add(square2);
                    square2 = square2.GetComponent<PoleSquare>().down.GetComponent<PoleLine>().down;
                }
                square1 = square1.GetComponent<PoleSquare>().right.GetComponent<PoleLine>().right;
            }
        }
        public PoleElts(int h, int w)
        {
            height = h;
            width = w;
            points = new List<Elements>();
            clrRing = new List<Elements>();
            shapes = new List<Elements>();
            unsolvedElts = new List<Elements>();

        }
        public bool CheckShapeSplit(List<GameObject> zone, List<Elements> shapes)
        {
            //List<GameObject> curzone = new List<GameObject>(zone);
            int size = 0;
            for (int i = 0; i < shapes.Count; i++)
            {
                size += shapes[i].GetComponent<PoleEltShape>().GetSize();
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
                List<Elements> localShapes = new List<Elements>();
                foreach (GameObject z in p)
                {

                    if (z.GetComponent<PoleSquare>().hasElem == true && z.GetComponent<PoleSquare>().element.GetComponent<PoleEltClrRing>() != null)
                    {
                        if (c == Color.red)
                        {
                            c = z.GetComponent<PoleSquare>().element.GetComponent<PoleEltClrRing>().c;
                        }
                        else
                        {
                            if (c != z.GetComponent<PoleSquare>().element.GetComponent<PoleEltClrRing>().c)
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
                        if (z.GetComponent<PoleSquare>().hasElem == true && z.GetComponent<PoleSquare>().element.GetComponent<PoleEltClrRing>() != null)
                        {
                            unsolvedElts.Add(z.GetComponent<PoleSquare>().element);
                        }
                    }
                }
                bool solveShape = CheckShapeSplit(p, localShapes);
                if (!solveShape)
                {
                    foreach (Elements sh in localShapes)
                        unsolvedElts.Add(sh);
                }
                localIsSolved = localIsSolved && solveShape;
                isSolved = localIsSolved && isSolved;
            }
            
            foreach (PoleEltPoint p in points)
            {
                if (!p.IsSolvedByPlayer()) unsolvedElts.Add(p);
                isSolved = isSolved && p.IsSolvedByPlayer();
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
    
    //public void OnDestroy()
    //{
        
    //    for (int i = transform.childCount - 1; i >= 0; --i)
    //    {
    //        Destroy(transform.GetChild(i));
    //    }
    //}
    
    //skip
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
                poleDots[y][x] = Instantiate(DotPF, transform.position + stepx * x + stepy * y, DotPF.transform.rotation);
                poleDots[y][x].transform.parent = DotsParent.transform;
                poleDots[y][x].GetComponent<PoleDot>().posX = x;
                poleDots[y][x].GetComponent<PoleDot>().posY = y;
            }
            GameObject line = Instantiate(HorizontalLinePF, poleDots[y][1].transform.position + stepx * 0.5f, HorizontalLinePF.transform.rotation);
            line.transform.parent = LinesParent.transform;
            poleLines.Add(line);
            poleDots[y][1].GetComponent<PoleDot>().AddLine(line, poleDots[y][2]);
            poleDots[y][2].GetComponent<PoleDot>().AddLine(line, poleDots[y][1]);
            line.GetComponent<PoleLine>().pole = this.gameObject;
            line.GetComponent<PoleLine>().left = poleDots[y][1];
            line.GetComponent<PoleLine>().right = poleDots[y][2];
            if (y>0)
            {
                line = Instantiate(VerticalLinePF, poleDots[y][1].transform.position - stepy * 0.5f, VerticalLinePF.transform.rotation);
                poleLines.Add(line);
                line.transform.parent = LinesParent.transform;
                poleDots[y][1].GetComponent<PoleDot>().AddLine(line, poleDots[y - 1][1]);
                poleDots[y - 1][1].GetComponent<PoleDot>().AddLine(line, poleDots[y][1]);
                line.GetComponent<PoleLine>().pole = this.gameObject;
                line.GetComponent<PoleLine>().up = poleDots[y - 1][1];
                line.GetComponent<PoleLine>().down = poleDots[y][1];
            }
        }
        for (int y = 0; y < numOfParams; y++)
        {
            AddFinish(poleDots[y][2]);
        }
        poleDots[0][0] = Instantiate(DotPF, transform.position, DotPF.transform.rotation);
        poleDots[0][0].transform.parent = DotsParent.transform;
        poleDots[0][0].GetComponent<PoleDot>().posX = 0;
        poleDots[0][0].GetComponent<PoleDot>().posY = 0;
        GameObject opline = Instantiate(HorizontalLinePF, poleDots[0][0].transform.position + stepx * 0.5f, HorizontalLinePF.transform.rotation);
        opline.transform.parent = LinesParent.transform;
        poleLines.Add(opline);
        poleDots[0][0].GetComponent<PoleDot>().AddLine(opline, poleDots[0][1]);
        poleDots[0][1].GetComponent<PoleDot>().AddLine(opline, poleDots[0][0]);
        opline.GetComponent<PoleLine>().pole = this.gameObject;
        opline.GetComponent<PoleLine>().left = poleDots[0][0];
        opline.GetComponent<PoleLine>().right = poleDots[0][1];
        AddStart(poleDots[0][0]);
        poleDots[0][0].GetComponent<PoleDot>().CreateDot();
        poleDots[0][0].GetComponent<PoleDot>().CreateObject();
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
            point.GetComponent<PoleEltClrRing>().ShowNormalizedColor();

        }
        foreach (GameObject point in GameObject.FindGameObjectsWithTag("EltShape"))
        {
            point.GetComponent<PoleEltShape>().ShowNormalizedColor();

        }
    }
    public void Init(int _height, int _width)
    {
        quantityRing = Core.PolePreferences.numOfCircles;
        playerPath = new PolePath();
        systemPath = new PolePath();
        height = _height;
        width = _width;
        eltsManager = new PoleElts(height,width);
        poleDots = new GameObject[height][];
        poleLines = new List<GameObject>();
        for (int y = 0; y < height; y++)
        {
            poleDots[y] = new GameObject[width];
            for (int x = 0; x < width; x++)
            {
                poleDots[y][x] = Instantiate(DotPF, transform.position + new Vector3(x * 5f, -y * 5f, 0f), DotPF.transform.rotation);
                poleDots[y][x].GetComponent<PoleDot>().posX = x;
                poleDots[y][x].GetComponent<PoleDot>().posY = y;
                poleDots[y][x].transform.parent = DotsParent.transform;
            }
        }

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                // horizontal line
                if (x < width - 1)
                {
                    GameObject lineH = Instantiate(HorizontalLinePF, poleDots[y][x].transform.position + new Vector3(2.5f, 0f, 0f), transform.rotation);
                    lineH.GetComponent<PoleLine>().pole = this.gameObject;
                    lineH.GetComponent<PoleLine>().left = poleDots[y][x];
                    lineH.GetComponent<PoleLine>().right = poleDots[y][x + 1];
                    poleDots[y][x].GetComponent<PoleDot>().AddLine(lineH, poleDots[y][x + 1]);
                    poleDots[y][x + 1].GetComponent<PoleDot>().AddLine(lineH, poleDots[y][x]);
                    poleLines.Add(lineH);
                    lineH.transform.parent = LinesParent.transform;
                    if (y < height - 1)
                    {
                        GameObject Squere = Instantiate(SquerePF, poleDots[y][x].transform.position + new Vector3(2.5f, -2.5f, 0f), SquerePF.transform.rotation);
                        Squere.GetComponent<PoleSquare>().indexI = y;
                        Squere.GetComponent<PoleSquare>().indexJ = x;
                        Squere.GetComponent<PoleSquare>().up = lineH;
                        lineH.GetComponent<PoleLine>().down = Squere;
                        Squere.transform.parent = SquaresParent.transform;
                    }
                }
                // vertical line
                if (y < height - 1)
                {
                    GameObject lineV = Instantiate(VerticalLinePF, poleDots[y][x].transform.position + new Vector3(0f, -2.5f, 0f), transform.rotation);
                    lineV.GetComponent<PoleLine>().pole = this.gameObject;
                    lineV.GetComponent<PoleLine>().up = poleDots[y][x];
                    lineV.GetComponent<PoleLine>().down = poleDots[y + 1][x];
                    poleDots[y][x].GetComponent<PoleDot>().AddLine(lineV, poleDots[y + 1][x]);
                    poleDots[y + 1][x].GetComponent<PoleDot>().AddLine(lineV, poleDots[y][x]);
                    poleLines.Add(lineV);
                    lineV.transform.parent = LinesParent.transform;
                }

            }

        }
        for (int y = 1; y < width; y++)
        {
            for (int x = 1; x < height; x++)
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
    //???? move up?
    delegate void funcDecode(string input);
    //????
    public void StartDecode(string input)
    {
        int x;
        int y;
        for (int i = 0; i < input.Length;i +=2)
        {
            x = input[i] - '0';
            y = input[i + 1] - '0';

            AddStart(poleDots[y][x]);
        }
    }
    //????
    public void FinishDecode(string input)
    {
        int x;
        int y;
        for (int i = 0; i < input.Length; i += 2)
        {
            x = input[i] - '0';
            y = input[i + 1] - '0';
            AddFinish(poleDots[y][x]);
        }
    }
    //????
    public void PointDecode(string input)
    {
        int x;
        int y;
        int f;
        for (int i = 0; i < input.Length; i += 3)
        {
            x = input[i] - '0';
            y = input[i + 1] - '0';
            f = input[i + 2] - '0';
            var dot = poleDots[y][x].GetComponent<PoleDot>();
            if (f == 0)
            {
                dot.hasPoint = true;
            }
            else
            {
                if(f == 1)
                {
                    dot.down.GetComponent<PoleLine>().hasPoint = true;
                }
                else
                {
                    dot.right.GetComponent<PoleLine>().hasPoint = true;
                }
            }
        }
    }
    //????
    public void RingDecode(string input)
    {
        int x;
        int y;
        int f;
        string hex = "";
        for (int i = 0; i < input.Length; i += 10)
        {
            x = input[i] - '0';
            y = input[i + 1] - '0';
            Color col = new Color((float)int.Parse(input.Substring(i + 2, 2), System.Globalization.NumberStyles.HexNumber) / 255,
                                (float)int.Parse(input.Substring(i + 4, 2), System.Globalization.NumberStyles.HexNumber) / 255,
                                (float)int.Parse(input.Substring(i + 6, 2), System.Globalization.NumberStyles.HexNumber) / 255,
                                (float)int.Parse(input.Substring(i + 8, 2), System.Globalization.NumberStyles.HexNumber) / 255);

            PoleSquare sq = poleDots[y][x].GetComponent<PoleDot>().down.GetComponent<PoleLine>().right.GetComponent<PoleSquare>();
            sq.hasElem = true;
            sq.element = Instantiate(ClrRingPF, sq.transform).GetComponent<Elements>();
            sq.element.GetComponent<Renderer>().material.color = col;
            sq.element.GetComponent<PoleEltClrRing>().c = col;
        }
    }
    public void ShapeDecode(string input)
    {

        for (int k = 0; k < input.Length;)
        {
            int x = input[k] - '0';
            int y = input[k + 1] - '0';
            int row = input[k + 2] - '0';
            int col = input[k + 3] - '0';
            k += 4;
            List < List < bool>> bitmap = new List<List<bool>>();
            for (int i = 0; i < row; ++i)
            {
                bitmap.Add(new List<bool>());
                for (int j = 0; j < col; ++j)
                {
                    if (input[k + i * col + j] - '0' == 1)
                    {
                        bitmap[i].Add(true);
                    }
                    else
                    {
                        bitmap[i].Add(false);
                    }

                }
            }
            PoleSquare sq = poleDots[y][x].GetComponent<PoleDot>().down.GetComponent<PoleLine>().right.GetComponent<PoleSquare>();
            sq.GetComponent<PoleSquare>().hasElem = true;
            sq.GetComponent<PoleSquare>().element = Instantiate(ShapePF, sq.transform).GetComponent<Elements>();
            sq.GetComponent<PoleSquare>().element.GetComponent<PoleEltShape>().boolList = bitmap;
            sq.GetComponent<PoleSquare>().element.GetComponent<PoleEltShape>().Create();
            eltsManager.shapes.Add(sq.GetComponent<PoleSquare>().element);
            k += row * col;
        }
    }
    public void CustomInit(int _height, int _width)
    {
        playerPath = new PolePath();
        height = _height;
        width = _width;
        eltsManager = new PoleElts(height, width);
        Core.PolePreferences.poleSize = height;
        poleDots = new GameObject[height][];
        poleLines = new List<GameObject>();
        for (int y = 0; y < height; y++)
        {
            poleDots[y] = new GameObject[width];
            for (int x = 0; x < width; x++)
            {
                poleDots[y][x] = Instantiate(DotPF, transform.position + stepx * x + stepy * y, DotPF.transform.rotation);
                poleDots[y][x].GetComponent<PoleDot>().posX = x;
                poleDots[y][x].GetComponent<PoleDot>().posY = y;
                poleDots[y][x].transform.parent = this.transform;
            }
        }
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width - 1; x++)
            {
                // horizontal line
                GameObject lineH = Instantiate(HorizontalLinePF, transform.position + stepx * (x + 0.5f) + stepy * y, transform.rotation);
                lineH.GetComponent<PoleLine>().pole = this.gameObject;
                lineH.GetComponent<PoleLine>().left = poleDots[y][x];
                lineH.GetComponent<PoleLine>().right = poleDots[y][x + 1];
                poleDots[y][x].GetComponent<PoleDot>().AddLine(lineH, poleDots[y][x + 1]);
                poleDots[y][x + 1].GetComponent<PoleDot>().AddLine(lineH, poleDots[y][x]);
                poleLines.Add(lineH);
                lineH.transform.parent = this.transform;
                // vertical line
                GameObject lineV = Instantiate(VerticalLinePF, transform.position + stepx * y + stepy * (x + 0.5f), transform.rotation);
                lineV.GetComponent<PoleLine>().pole = this.gameObject;
                lineV.GetComponent<PoleLine>().up = poleDots[x][y];
                lineV.GetComponent<PoleLine>().down = poleDots[x + 1][y];
                poleDots[x][y].GetComponent<PoleDot>().AddLine(lineV, poleDots[x + 1][y]);
                poleDots[x + 1][y].GetComponent<PoleDot>().AddLine(lineV, poleDots[x][y]);
                poleLines.Add(lineV);
                lineV.transform.parent = this.transform;
                if (y < height - 1)
                {
                    GameObject Squere = Instantiate(SquerePF, transform.position + stepx * (x + 0.5f) + stepy * (y + 0.5f), SquerePF.transform.rotation);
                    Squere.GetComponent<PoleSquare>().indexI = y;
                    Squere.GetComponent<PoleSquare>().indexJ = x;
                    Squere.GetComponent<PoleSquare>().up = lineH;
                    lineH.GetComponent<PoleLine>().down = Squere;
                    Squere.transform.parent = this.transform;
                }

            }

        }
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
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
    //????
    public void Custom(string info)
    {
        // test coode 

        funcDecode[] decode = new funcDecode[5];// decode funcs array
        decode[0] = StartDecode;
        decode[1] = FinishDecode;
        decode[2] = PointDecode;
        decode[3] = RingDecode;
        decode[4] = ShapeDecode;
        int decodeId = 0; //index of decode func

        int _height = info[0] - '0';
        int _width = info[1] - '0';

        Init(_height, _width);
        bool flag = true;
        string saveLine = ""; // save line one type of obj
        for (int i = 2; i < info.Length; ++i)
        {
            if (flag)
            {
                switch (info[i])
                {
                    case 's':
                        decodeId = 0;
                        break;
                    case 'f':
                        decodeId = 1;
                        break;
                    case 'p':
                        decodeId = 2;
                        break;
                    case 'r':
                        decodeId = 3;
                        break;
                    case 'T':
                        decodeId = 4;
                        break;
                }
                flag = false;
                saveLine = "";

            }
            else if (info[i] == '*')
            {
                decode[decodeId](saveLine);
                flag = true;
            }
            else
            {
                saveLine += info[i];
            }
        }

    }

    public void GetZoneFreeDots(GameObject begin, List<GameObject> dots)
    {
        var newdots = new List<GameObject>();
        if (begin.GetComponent<PoleDot>().up != null && !dots.Contains(begin.GetComponent<PoleDot>().up.GetComponent<PoleLine>().up)
            && !begin.GetComponent<PoleDot>().up.GetComponent<PoleLine>().up.GetComponent<PoleDot>().isUsedByPlayer && !begin.GetComponent<PoleDot>().up.GetComponent<PoleLine>().up.GetComponent<PoleDot>().isUsedBySolution)
        {
            dots.Add(begin.GetComponent<PoleDot>().up.GetComponent<PoleLine>().up);
            newdots.Add(begin.GetComponent<PoleDot>().up.GetComponent<PoleLine>().up);
        }
        if (begin.GetComponent<PoleDot>().right != null && !dots.Contains(begin.GetComponent<PoleDot>().right.GetComponent<PoleLine>().right)
            && !begin.GetComponent<PoleDot>().right.GetComponent<PoleLine>().right.GetComponent<PoleDot>().isUsedByPlayer && !begin.GetComponent<PoleDot>().right.GetComponent<PoleLine>().right.GetComponent<PoleDot>().isUsedBySolution)
        {
            dots.Add(begin.GetComponent<PoleDot>().right.GetComponent<PoleLine>().right);
            newdots.Add(begin.GetComponent<PoleDot>().right.GetComponent<PoleLine>().right);
        }
        if (begin.GetComponent<PoleDot>().down != null && !dots.Contains(begin.GetComponent<PoleDot>().down.GetComponent<PoleLine>().down)
            && !begin.GetComponent<PoleDot>().down.GetComponent<PoleLine>().down.GetComponent<PoleDot>().isUsedByPlayer && !begin.GetComponent<PoleDot>().down.GetComponent<PoleLine>().down.GetComponent<PoleDot>().isUsedBySolution)
        {
            dots.Add(begin.GetComponent<PoleDot>().down.GetComponent<PoleLine>().down);
            newdots.Add(begin.GetComponent<PoleDot>().down.GetComponent<PoleLine>().down);
        }
        if (begin.GetComponent<PoleDot>().left != null && !dots.Contains(begin.GetComponent<PoleDot>().left.GetComponent<PoleLine>().left)
            && !begin.GetComponent<PoleDot>().left.GetComponent<PoleLine>().left.GetComponent<PoleDot>().isUsedByPlayer && !begin.GetComponent<PoleDot>().left.GetComponent<PoleLine>().left.GetComponent<PoleDot>().isUsedBySolution)
        {
            dots.Add(begin.GetComponent<PoleDot>().left.GetComponent<PoleLine>().left);
            newdots.Add(begin.GetComponent<PoleDot>().left.GetComponent<PoleLine>().left);
        }
        foreach (GameObject dot in newdots)
            GetZoneFreeDots(dot, dots);
    }
    public bool FindFinish(GameObject begin)
    {
        var dots = new List<GameObject>
        {
            begin
        };
        GetZoneFreeDots(begin, dots);
        foreach (GameObject dot in dots)
            if (finishes.Contains(dot))
                return true;
        return false;
    }

    public bool FindUnsolvedPoint(GameObject begin)
    {
        var dots = new List<GameObject>
        {
            begin
        };
        GetZoneFreeDots(begin, dots);
        foreach (GameObject dot in dots)
            if (dot.GetComponent<PoleDot>().hasPoint)
                return false;
        return true;
    }

    //need more fixes
    public bool FindPath(GameObject begin, List<GameObject> ends, bool needToCheckLocal)
    {
        begin.GetComponent<PoleDot>().isUsedBySolution = true;

        var dotslist = systemPath.dots;
        dotslist.Add(begin);
        int i = dotslist.Count - 1;
        GameObject line = null;
        if (i > 0)
        {
            if (dotslist[i].GetComponent<PoleDot>().AllowedToDown() && dotslist[i].GetComponent<PoleDot>().down.GetComponent<PoleLine>().down == dotslist[i - 1])
            {
                line = dotslist[i].GetComponent<PoleDot>().down;
                line.GetComponent<PoleLine>().isUsedBySolution = true;
                systemPath.lines.Add(line);
            }
            if (dotslist[i].GetComponent<PoleDot>().AllowedToLeft() && dotslist[i].GetComponent<PoleDot>().left.GetComponent<PoleLine>().left == dotslist[i - 1])
            {
                line = dotslist[i].GetComponent<PoleDot>().left;
                line.GetComponent<PoleLine>().isUsedBySolution = true;
                systemPath.lines.Add(line);
            }
            if (dotslist[i].GetComponent<PoleDot>().AllowedToRight() && dotslist[i].GetComponent<PoleDot>().right.GetComponent<PoleLine>().right == dotslist[i - 1])
            {
                line = dotslist[i].GetComponent<PoleDot>().right;
                line.GetComponent<PoleLine>().isUsedBySolution = true;
                systemPath.lines.Add(line);
            }
            if (dotslist[i].GetComponent<PoleDot>().AllowedToUp() && dotslist[i].GetComponent<PoleDot>().up.GetComponent<PoleLine>().up == dotslist[i - 1])
            {
                line = dotslist[i].GetComponent<PoleDot>().up;
                line.GetComponent<PoleLine>().isUsedBySolution = true;
                systemPath.lines.Add(line);
            }
        }
        if (ends.Contains(begin))
        {
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
                if (FindFinish(dot))
                    tmp.Add(dot);
            dots = new List<GameObject>(tmp);
        }
        bool success = false;
        for (int k = 0; k < dots.Count; k++)
        {
            GameObject next = dots[Core.PolePreferences.MyRandom.GetRandom() % dots.Count];
            bool needCheck = false;
            if (beginDot.posX > 0 && beginDot.posX < width - 1 && beginDot.posY > 0 && beginDot.posY < height - 1
                && (next.GetComponent<PoleDot>().posX == 0 || next.GetComponent<PoleDot>().posY == 0 || next.GetComponent<PoleDot>().posX == width - 1 || next.GetComponent<PoleDot>().posY == height - 1))
            {
                needCheck = true;
            }

            if (FindPath(next, ends, needCheck))
            {
                success = true;
                break;
            }
            dots.Remove(next);
        }
        if (!success)
        {
            if (needToCheckLocal && systemPath.dots.Count >= width * height * Core.PolePreferences.complexity)
                return true;
            begin.GetComponent<PoleDot>().isUsedBySolution = false;
            systemPath.dots.Remove(begin);
            systemPath.lines.Remove(line);
            if (line != null)
                line.GetComponent<PoleLine>().isUsedBySolution = false;
        }

        return success;
    }

    //old version
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
                    if (begin.GetComponent<PoleDot>().posX < width - 1 && ways[begin.GetComponent<PoleDot>().posY][begin.GetComponent<PoleDot>().posX + 1] == 0 && tries[1])
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
                    if (begin.GetComponent<PoleDot>().posY < height - 1 && ways[begin.GetComponent<PoleDot>().posY + 1][begin.GetComponent<PoleDot>().posX] == 0 && tries[2])
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
        if (dotData.PathLength() > width * height * Core.PolePreferences.complexity &&
            (begin.GetComponent<PoleDot>().posX == 0 ||
             begin.GetComponent<PoleDot>().posX == width - 1 ||
             begin.GetComponent<PoleDot>().posY == 0 ||
             begin.GetComponent<PoleDot>().posY == height - 1))
        {
            finishes[0].GetComponent<PoleDot>().isUsedBySolution = false;
            finishes.Clear();
            Destroy(GameObject.FindGameObjectWithTag("PoleFinish"));
            AddFinish(poleDots[begin.GetComponent<PoleDot>().posY][begin.GetComponent<PoleDot>().posX]);
            return true;
        }
        //ways[begin.GetComponent<PoleDot>().posY][begin.GetComponent<PoleDot>().posX] = 0;
        begin.GetComponent<PoleDot>().isUsedBySolution = false;
        dotData.GetDot();
        
        return false;
    }
    private void FindZone(GameObject square, int[][] poleZones, int x, int y)
    {
        int sizeY = height - 1;
        int sizeX = width - 1;
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
            if (y < sizeY && poleZones[y + 1][x] == 0)
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
            if (x < sizeX && poleZones[y][x + 1] == 0)
            {
                poleZones[y][x + 1] = poleZones[y][x];
                FindZone(lineV.GetComponent<PoleLine>().right, poleZones, x + 1, y);
            }
        }
    }
    private void SetZone()
    {
        int sizeY = height - 1;
        int sizeX = width - 1;
        int[][] poleZones;
        poleZones = new int[sizeY][];
        for (int y = 0; y < sizeY; y++)
        {
            poleZones[y] = new int[sizeX];
            for (int x = 0; x < sizeX; x++)
            {
                poleZones[y][x] = 0;
            }
        }
        GameObject square = poleDots[0][0].GetComponent<PoleDot>().right.GetComponent<PoleLine>().down;
        quantityZones = 0;
        for (int x = 0; x < sizeX; ++x)
        {
            GameObject square1 = square;
            for (int y = 0; y < sizeY; ++y)
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
        for (int x = 0; x < sizeX; ++x)
        {
            GameObject square1 = square;
            for (int y = 0; y < sizeY; ++y)
            {
                zone[poleZones[y][x] - 1].Add(square1);
                square1 = square1.GetComponent<PoleSquare>().down.GetComponent<PoleLine>().down;
            }
            square = square.GetComponent<PoleSquare>().right.GetComponent<PoleLine>().right;
        }
        
    }
    public void AddStart(GameObject dot)
    {
        starts.Add(dot);
        dot.GetComponent<PoleDot>().AddStart();
    }
    public void AddFinish(GameObject dot)
    {
        finishes.Add(dot);
        dot.GetComponent<PoleDot>().AddFinish();
    }

    //need to rewrite
    public void GeneratePoints(int numberOfPoints)
    {
        eltsManager.points.Clear();
        numberOfPoints = System.Math.Min(numberOfPoints, (systemPath.dots.Count + systemPath.lines.Count) / 2 - 4);
        List<GameObject> pathList = new List<GameObject>
        {
            systemPath.dots[0]
        };
        for (int i = 0; i<systemPath.dots.Count; i++)
        {
            pathList.Add(systemPath.dots[i]);
        }
        for (int i = 0; i < systemPath.lines.Count; i++)
            pathList.Add(systemPath.lines[i]);
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

    //????
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
                int mm = 0;
                List<GameObject> m = coloredZones[0];
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
                coloredZones[j][i].GetComponent<PoleSquare>().element = Instantiate(ClrRingPF, coloredZones[j][i].transform).GetComponent<Elements>();
                coloredZones[j][i].GetComponent<PoleSquare>().element.location = coloredZones[j][i];
                coloredZones[j][i].GetComponent<PoleSquare>().element.GetComponent<PoleEltClrRing>().c = color[k];
                eltsManager.clrRing.Add(coloredZones[j][i].GetComponent<PoleSquare>().element);
                coloredZones[j][i].GetComponent<PoleSquare>().element.GetComponent<MeshRenderer>().material.color = color[k];
                quantityClrRingInZone[j]--;
                coloredZones[j].RemoveAt(i);
            }
            color.RemoveAt(k);
        }
    }

    //????
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
                            c = zone[i][g].GetComponent<PoleSquare>().element.GetComponent<PoleEltClrRing>().c;
                            break;
                        }
                    }
                    int j = Core.PolePreferences.MyRandom.GetRandom() % localZones[i].Count;
                    localZones[i][j].GetComponent<PoleSquare>().hasElem = true;
                    localZones[i][j].GetComponent<PoleSquare>().element = Instantiate(ClrStarPF, localZones[i][j].transform.position, ClrStarPF.transform.rotation).GetComponent<Elements>();
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
                            c = zone[i][j].GetComponent<PoleSquare>().element.GetComponent<PoleEltClrRing>().c;
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
                        localZones[i][j].GetComponent<PoleSquare>().element = Instantiate(ClrStarPF, localZones[i][j].transform.position, ClrStarPF.transform.rotation).GetComponent<Elements>();
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

    //skip
    public void CreateSolution()
    {
        int[][] ways = new int[height][];
        for (int i = 0; i < height; i++)
        {
            ways[i] = new int[width];
            for (int j = 0; j < width; j++)
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
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
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
        //FindPath(starts[0], finishes, false);
        SetZone();
        SetShapes();
        
    }
    public void GenerateShapes(int zoneSize)
    {
        for (int i = 0; i < zone.Count; i++)
            activeShapes.Add(new List<List<GameObject>>());
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
        
        DrawShapes();

    }
    private void DrawShapes()
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
                Elements shapeElt = Instantiate(ShapePF, sq.transform).GetComponent<Elements>();
                sq.GetComponent<PoleSquare>().hasElem = true;
                sq.GetComponent<PoleSquare>().element = shapeElt;
                shapeElt.location = sq;
                shapeElt.GetComponent<PoleEltShape>().boolList = ZoneToBoolList(activeShapes[i][j]);
                shapeElt.GetComponent<PoleEltShape>().Create();
                //eltsManager.shapes.Add(shapeElt);
                sqList.Remove(sq);
            }
        }
    }
    private void SetShapes()
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

    //to improve: add posibility to choose squares that are not neighburs but ar in one zone
    private List<List<GameObject>> SplitZone(List<GameObject> zone)
    {
        List<List<GameObject>> zoneShapes = new List<List<GameObject>>();
        List<GameObject> currentShape;
        List<GameObject> squeresToCheck = new List<GameObject>();
        while (zone.Count > 0)
        {
            currentShape = new List<GameObject>
            {
                zone[Core.PolePreferences.MyRandom.GetRandom() % zone.Count]
            };
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
        List<List<bool>> boolList = new List<List<bool>>();

        for (int i = 0; i < Core.PolePreferences.poleSize; i++)
        {
            boolList.Add(new List<bool>());
            for (int j = 0; j < Core.PolePreferences.poleSize; j++)
            {
                boolList[i].Add(false);
            }
        }
        foreach(GameObject squere in zone)
        {
            boolList[squere.GetComponent<PoleSquare>().indexI][squere.GetComponent<PoleSquare>().indexJ] = true;
        }

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

        return boolList;
        
    }
    public void ClearPole()
    {
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
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
        eltsManager.points.Clear();
        eltsManager.clrRing.Clear();
        eltsManager.shapes.Clear();
        foreach (GameObject dot in starts) Destroy(dot.GetComponent<PoleDot>().startFinish);
        foreach (GameObject dot in finishes) Destroy(dot.GetComponent<PoleDot>().startFinish);
        systemPath.Clear();
    }
    public void StartScaling(GameObject dot)
    {
        dot.GetComponent<PoleDot>().CreateDot();
        dot.GetComponent<PoleDot>().CreateObject();
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

    //need??
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

    //need??
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

}
