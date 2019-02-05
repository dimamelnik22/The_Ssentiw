using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Core : MonoBehaviour {

    public Transform DotPrefab;
    public GameObject VerticalLinePrefab;
    public GameObject HorizontalLinePrefab;
    public Transform StartPrefab;
    public Transform FinishPrefab;
    private static Vector3 stepx = new Vector3(5, 0, 0);
    private static Vector3 stepz = new Vector3(0, 0, -5);
    public int seed = 95;
    public bool mode = true;
    public void ButtonClick()
    {
        int size = 5;
        Pole myPole = new Pole(size,seed);
        foreach (GameObject gameObject in GameObject.FindGameObjectsWithTag("PolePart"))
            Destroy(gameObject);
        if (mode)
        {
            
            myPole.SetStart(0, 0);
            myPole.SetFinish(size - 1, size - 1);
            myPole.CreateSolution();
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    if (myPole.poleDots[i][j] == myPole.start) Instantiate(StartPrefab, transform.position + stepx * j + stepz * i, transform.rotation);
                    else if (myPole.poleDots[i][j] == myPole.finish) Instantiate(FinishPrefab, transform.position + stepx * j + stepz * i, transform.rotation);
                    else Instantiate(DotPrefab, transform.position + stepx * j + stepz * i, transform.rotation);

                }
            }
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    Instantiate(DotPrefab, transform.position + stepx * j + stepz * i, transform.rotation);
                    if (j < size - 1)
                        if (myPole.poleDots[i][j].right != null)
                            Instantiate(HorizontalLinePrefab, transform.position + stepx * 0.5f + stepx * j + stepz * i, HorizontalLinePrefab.transform.rotation);
                }

                if (i < size - 1)

                    for (int j = 0; j < size; j++)
                    {
                        if (myPole.poleDots[i][j].down != null)
                            Instantiate(VerticalLinePrefab, transform.position + stepz * 0.5f + stepx * j + stepz * i, VerticalLinePrefab.transform.rotation);
                    }
            }
            
            myPole.ClearPole();
        }
        seed += 10;
        mode = !mode;
    }
    

    // Use this for initialization
    void Start() {
       

    }

    void Update()
    {
        
    }


    class MyRandom
    {
        private int seed;
        public MyRandom(int k)
        {
            seed = k;
        }
        public int GetRandom()
        {
            seed = (seed * 13 + (seed >> 3)) % 1000000 + 22;
            return seed;
        }
    }

    class PoleDot
    {
        public PoleLine up;
        public PoleLine down;
        public PoleLine left;
        public PoleLine right;
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
        }
        public void AddLine(PoleLine newLine, PoleDot anotherDot)
        {
            if (position_x < anotherDot.position_x /*&& position_y == anotherDot.position_y*/)
            {
                right = newLine;
            }
            else if (position_x > anotherDot.position_x /*&& position_y == anotherDot.position_y*/)
            {
                left = newLine;
            }
            else if (position_y < anotherDot.position_y /*&& position_x == anotherDot.position_x*/)
            {
                down = newLine;
            }
            else if (position_y > anotherDot.position_y /*&& position_x == anotherDot.position_x*/)
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
        public PoleLine(PoleDot firstDot, PoleDot secondDot)
        {
            first = firstDot;
            second = secondDot;
        }
    };

    class PoleSquere
    {

    };

    class Pole
    {
        MyRandom myRandGen;
        public PoleDot start;
        public PoleDot finish;
        private readonly int poleSize;
        PathDotStack dotData;

        public bool FindPath(PoleDot begin, PoleDot end, int[][] ways)
        {
            //for (int i = 0; i < poleSize; i++)
            //{
            //    for (int j = 0; j < poleSize; j++)
            //    {
            //        Console.Write(ways[i][j] + " ");
            //    }
            //    Console.WriteLine();
            //}
            //Console.ReadKey();

            if (begin == end)
            {
                if (dotData.PathLength() < poleSize * 2 + poleSize / 2)
                {
                    return false;
                }
                return true;
            }
            bool[] tries = { true, true, true, true };
            //for (int i = 0; i < 4; i++)
            //{
            //    tries[i] = true;
            //}
            dotData.AddDot(begin);

            bool triesLeft = true;

            while (triesLeft)
            {

                // var randGen = new System.Random();

                //int k = randGen.Next(0, 4);
                //while (!tries[k])
                //{
                //    k = randGen.Next(0, 4);
                //}

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
                            if (FindPath(poleDots[begin.position_y - 1][begin.position_x], end, ways)) return true;
                            else
                            {
                                ways[begin.position_y - 1][begin.position_x] = 0;
                                tries[0] = false;
                            }
                        }
                        else tries[0] = false;
                        break;
                    case 1:

                        if (begin.position_x < poleSize - 1 && ways[begin.position_y][begin.position_x + 1] == 0 && tries[1])
                        {
                            ways[begin.position_y][begin.position_x + 1] = 1;
                            if (FindPath(poleDots[begin.position_y][begin.position_x + 1], end, ways)) return true;
                            else
                            {
                                ways[begin.position_y][begin.position_x + 1] = 0;
                                tries[1] = false;
                            }
                        }
                        else tries[1] = false;
                        break;
                    case 2:

                        if (begin.position_y < poleSize - 1 && ways[begin.position_y + 1][begin.position_x] == 0 && tries[2])
                        {
                            ways[begin.position_y + 1][begin.position_x] = 1;
                            if (FindPath(poleDots[begin.position_y + 1][begin.position_x], end, ways)) return true;
                            else
                            {
                                ways[begin.position_y + 1][begin.position_x] = 0;
                                tries[2] = false;
                            }
                        }
                        else tries[2] = false;
                        break;
                    case 3:

                        if (begin.position_x > 0 && ways[begin.position_y][begin.position_x - 1] == 0 && tries[3])
                        {
                            ways[begin.position_y][begin.position_x - 1] = 1;
                            if (FindPath(poleDots[begin.position_y][begin.position_x - 1], end, ways)) return true;
                            else
                            {
                                ways[begin.position_y][begin.position_x - 1] = 0;
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

            dotData.GetDot();
            return false;
        }
        public PoleDot[][] poleDots;
        public Pole(int size,int seed)
        {
            myRandGen = new MyRandom(seed);
            poleSize = size;
            poleDots = new PoleDot[size][];
            for (int y = 0; y < size; y++)
            {
                poleDots[y] = new PoleDot[size];
                for (int x = 0; x < size; x++)
                {
                    poleDots[y][x] = new PoleDot(x, y);
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
            dotData.AddDot(start);
            if (start == finish)
            {
                Debug.Log("wtf");
            }
            bool isFound = FindPath(start, finish, ways);
            if (isFound)
            {
                int rate = 0;
                PoleDot prevDot;
                PoleDot curDot = finish;
                while (!dotData.IsEmpty())
                {
                    prevDot = dotData.GetDot();
                    PoleLine newLine = new PoleLine(curDot, prevDot);
                    prevDot.AddLine(newLine, curDot);
                    curDot.AddLine(newLine, prevDot);
                    curDot = prevDot;
                    rate++;
                }
            }
        }
        public void ClearPole()
        {
            poleDots = new PoleDot[poleSize][];
            for (int y = 0; y < poleSize; y++)
            {
                poleDots[y] = new PoleDot[poleSize];
                for (int x = 0; x < poleSize; x++)
                {
                    poleDots[y][x] = new PoleDot(x, y);
                }
            }
        }
        
    };
}
