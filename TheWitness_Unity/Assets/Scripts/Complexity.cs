using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Complexity
{
    private static float gentime = 0f;
    private static GameObject start;
    private static GameObject finish;
    private static int[][] pole;
    private static GameObject[][] poleDots;
    private static void Swap(ref int lhs, ref int rhs)
    {
        int temp;
        temp = lhs;
        lhs = rhs;
        rhs = temp;
    }
    private class path
    {
        public GameObject dot1;
        public GameObject dot2;
        public path(GameObject d1, GameObject d2)
        {
            dot1 = d1;
            dot2 = d2;
        }
    }
    private static bool NextSet(int[] mass, int n)
    {
        int j = n - 2;
        while (j != -1 && mass[j] >= mass[j + 1])
        {
            j--;
        }
        if(j == -1)
        {
            return false;
        }
        int k = n - 1;
        while (mass[j] >= mass[k])
            k--;
        Swap(ref mass[j], ref mass[k]);
        int l = j + 1, r = n - 1;
        while (l < r)
            Swap(ref mass[l++], ref mass[r--]);
        return true;
    }

    private static bool checkLeft(GameObject localStart, GameObject localFinish)
    {
        if (localStart.GetComponent<PoleDot>().left != null)
        {
            localStart.GetComponent<PoleDot>().left.GetComponent<PoleLine>().isUsedByPlayer = true;
            localStart.GetComponent<PoleDot>().left.GetComponent<PoleLine>().left.GetComponent<PoleDot>().isUsedByPlayer = true;
            if (findLocalShortPath(localStart.GetComponent<PoleDot>().left.GetComponent<PoleLine>().left, localFinish))
            {
                return true;
            }
            localStart.GetComponent<PoleDot>().left.GetComponent<PoleLine>().isUsedByPlayer = false;
            localStart.GetComponent<PoleDot>().left.GetComponent<PoleLine>().left.GetComponent<PoleDot>().isUsedByPlayer = false;
        }
        return false;
    }
    private static bool checkRight(GameObject localStart, GameObject localFinish)
    {
        if (localStart.GetComponent<PoleDot>().right != null)
        {
            localStart.GetComponent<PoleDot>().right.GetComponent<PoleLine>().isUsedByPlayer = true;
            localStart.GetComponent<PoleDot>().right.GetComponent<PoleLine>().right.GetComponent<PoleDot>().isUsedByPlayer = true;
            if (findLocalShortPath(localStart.GetComponent<PoleDot>().right.GetComponent<PoleLine>().right, localFinish))
            {
                return true;
            }
            localStart.GetComponent<PoleDot>().right.GetComponent<PoleLine>().isUsedByPlayer = false;
            localStart.GetComponent<PoleDot>().right.GetComponent<PoleLine>().right.GetComponent<PoleDot>().isUsedByPlayer = false;
        }
        return false;
    }
    private static bool checkUp(GameObject localStart, GameObject localFinish)
    {
        if (localStart.GetComponent<PoleDot>().up != null)
        {
            localStart.GetComponent<PoleDot>().up.GetComponent<PoleLine>().isUsedByPlayer = true;
            localStart.GetComponent<PoleDot>().up.GetComponent<PoleLine>().up.GetComponent<PoleDot>().isUsedByPlayer = true;
            if (findLocalShortPath(localStart.GetComponent<PoleDot>().up.GetComponent<PoleLine>().up, localFinish))
            {
                return true;
            }
            localStart.GetComponent<PoleDot>().up.GetComponent<PoleLine>().isUsedByPlayer = false;
            localStart.GetComponent<PoleDot>().up.GetComponent<PoleLine>().up.GetComponent<PoleDot>().isUsedByPlayer = false;
        }
        return false;
    }
    private static bool checkDown(GameObject localStart, GameObject localFinish)
    {
        if (localStart.GetComponent<PoleDot>().down != null)
        {
            localStart.GetComponent<PoleDot>().down.GetComponent<PoleLine>().isUsedByPlayer = true;
            localStart.GetComponent<PoleDot>().down.GetComponent<PoleLine>().down.GetComponent<PoleDot>().isUsedByPlayer = true;
            if (findLocalShortPath(localStart.GetComponent<PoleDot>().down.GetComponent<PoleLine>().down, localFinish))
            {
                return true;
            }
            localStart.GetComponent<PoleDot>().down.GetComponent<PoleLine>().isUsedByPlayer = false;
            localStart.GetComponent<PoleDot>().down.GetComponent<PoleLine>().down.GetComponent<PoleDot>().isUsedByPlayer = false;
        }
        return false;
    }
    private static bool findLocalShortPath(GameObject localStart, GameObject localFinish)
    {
        Debug.Log(localStart);
        if (Mathf.Abs(localStart.GetComponent<PoleDot>().posX - localFinish.GetComponent<PoleDot>().posX) > Mathf.Abs(localStart.GetComponent<PoleDot>().posY - localFinish.GetComponent<PoleDot>().posY))
        {
            if(localStart.GetComponent<PoleDot>().posX > localFinish.GetComponent<PoleDot>().posX)
            {
                if (checkLeft(localStart, localFinish)) return true;
                if (checkUp(localStart, localFinish)) return true;
                if (checkDown(localStart, localFinish)) return true;
                if (checkRight(localStart, localFinish)) return true;
            }
            else
            {
                if (checkRight(localStart, localFinish)) return true;
                if (checkUp(localStart, localFinish)) return true;
                if (checkDown(localStart, localFinish)) return true;
                if (checkLeft(localStart, localFinish)) return true;
            }
        }
        else
        {
            if (localStart.GetComponent<PoleDot>().posY > localFinish.GetComponent<PoleDot>().posY)
            {
                if (checkUp(localStart, localFinish)) return true;
                if (checkRight(localStart, localFinish)) return true;
                if (checkLeft(localStart, localFinish)) return true;
                if (checkDown(localStart, localFinish)) return true;
            }
            else
            {
                if (checkDown(localStart, localFinish)) return true;
                if (checkRight(localStart, localFinish)) return true;
                if (checkLeft(localStart, localFinish)) return true;
                if (checkUp(localStart, localFinish)) return true;
            }
        }
        return false;
    }
    private static void setUsed(GameObject d1, GameObject d2, bool used)
    {
        d1.GetComponent<PoleDot>().isUsedByPlayer = used;
        d2.GetComponent<PoleDot>().isUsedByPlayer = used;
        if (d1.GetComponent<PoleDot>().posX  == d2.GetComponent<PoleDot>().posX)
        {
            if(d1.GetComponent<PoleDot>().posY - d2.GetComponent<PoleDot>().posY > 0)
            {
                d1.GetComponent<PoleDot>().up.GetComponent<PoleLine>().isUsedByPlayer = used;
            }
            else
            {
                d2.GetComponent<PoleDot>().up.GetComponent<PoleLine>().isUsedByPlayer = used;
            }
        }
        else
        {
            if (d1.GetComponent<PoleDot>().posX - d2.GetComponent<PoleDot>().posX > 0)
            {
                d1.GetComponent<PoleDot>().left.GetComponent<PoleLine>().isUsedByPlayer = used;
            }
            else
            {
                d2.GetComponent<PoleDot>().right.GetComponent<PoleLine>().isUsedByPlayer = used;
            }
        }
    }
    private static bool setPath(GameObject from,List<path> localMustVisit)
    {
        if(localMustVisit.Count == 0)
        {
            if (findLocalShortPath(from, finish))
                return true;
        }
        for (int i = 0; i < localMustVisit.Count; ++i)
        {
            List<path> nextMustVisit = new List<path>(localMustVisit);
            nextMustVisit.RemoveAt(i);

            if (localMustVisit[i].dot1 == localMustVisit[i].dot2)
            {
                if (findLocalShortPath(from, localMustVisit[i].dot1) == true)
                {
                    localMustVisit[i].dot1.GetComponent<PoleDot>().isUsedByPlayer = true;
                    if (setPath(localMustVisit[i].dot1, nextMustVisit))
                    {
                        return true;
                    }
                }
            }
            else
            {
                if (findLocalShortPath(from, localMustVisit[i].dot1) == true)
                {
                    setUsed(localMustVisit[i].dot2, localMustVisit[i].dot1, true);
                    if (setPath(localMustVisit[i].dot2, nextMustVisit))
                        return true;
                    else
                    {
                        setUsed(localMustVisit[i].dot2, localMustVisit[i].dot1, false);
                    }
                }
                if (findLocalShortPath(from, localMustVisit[i].dot2) == true)
                {
                    setUsed(localMustVisit[i].dot2, localMustVisit[i].dot1, false);
                    if (setPath(localMustVisit[i].dot1, nextMustVisit))
                        return true;
                    else
                    {
                        setUsed(localMustVisit[i].dot2, localMustVisit[i].dot1, false);
                    }
                }
            }
        }
        return false;
    }

    public static void findPath(GameObject[][] pDots, Pole myPole)
    {
        poleDots = pDots;
        List<Elements> points = myPole.eltsManager.points;
        List<Elements> clrRing = myPole.eltsManager.clrRing;
        List<Elements> shape = myPole.eltsManager.shape;
        List<GameObject> globalStarts = myPole.starts;
        List<GameObject> globalFinishes = myPole.finishes;
        List<path> mustVisit;
        mustVisit = new List<path>();
        for(int i = 0; i < points.Count;++i)
        {
            if (points[i].GetComponent<PoleEltPoint>().right || points[i].GetComponent<PoleEltPoint>().down)
            {
                mustVisit.Add(new path(poleDots[points[i].GetComponent<PoleEltPoint>().y][points[i].GetComponent<PoleEltPoint>().x], poleDots[points[i].GetComponent<PoleEltPoint>().y + (points[i].GetComponent<PoleEltPoint>().down? 1:0)][points[i].GetComponent<PoleEltPoint>().x + (points[i].GetComponent<PoleEltPoint>().right ? 1 : 0)]));
            }
            else
            {
                mustVisit.Add(new path(poleDots[points[i].GetComponent<PoleEltPoint>().y][points[i].GetComponent<PoleEltPoint>().x], poleDots[points[i].GetComponent<PoleEltPoint>().y][points[i].GetComponent<PoleEltPoint>().x]));
            }
            
        }
        int[] sequence = new int[mustVisit.Count]; 
        for (int i = 0; i < mustVisit.Count; ++i)
        {
            sequence[i] = i;
        }
        /*while(NextSet(sequence, sequence.Length))
        {
        }*/
        pole = new int[poleDots.Length][];
        for (int i = 0; i < poleDots.Length; ++i)
        {
            pole[i] = new int[poleDots[0].Length];
            for (int j = 0; j < poleDots[0].Length; ++j)
            {
                pole[i][j] = 0;
            }
        }
        for (int i = 0; i < mustVisit.Count; ++i)
        {
            pole[mustVisit[i].dot1.GetComponent<PoleDot>().posY][mustVisit[i].dot1.GetComponent<PoleDot>().posX] = 1;
            if (mustVisit[i].dot1.GetComponent<PoleDot>().posX != mustVisit[i].dot2.GetComponent<PoleDot>().posX || mustVisit[i].dot1.GetComponent<PoleDot>().posY != mustVisit[i].dot2.GetComponent<PoleDot>().posY)
                pole[mustVisit[i].dot2.GetComponent<PoleDot>().posY][mustVisit[i].dot2.GetComponent<PoleDot>().posX] = 1;
        }

        start = globalStarts[0];
        finish = globalFinishes[0];
        // fixed bug with long generation. and pole set is used by player
        //if(setPath(start, new List<path>(mustVisit))) Debug.Log("all good");
    }
    public static void countComplexity(GameObject[][] poleDots, Pole mypole)
    {
        gentime = Time.realtimeSinceStartup;

        findPath(poleDots, mypole);

        gentime = Time.realtimeSinceStartup - gentime;
        Debug.Log("complexity gentime" + gentime);
    }
}
