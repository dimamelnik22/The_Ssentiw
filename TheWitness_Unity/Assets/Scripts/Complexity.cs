using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Complexity
{
    //55s41*f42*p321231431042242301121*r1000ffffff0000ffffff3100ffffff0200ffffff2000ffffff3300ffffff1300ff00ff*T0341111122231100112322101112311113011132111*
    private static int glob = 0;
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
        if (localStart.GetComponent<PoleDot>().AllowedToLeft() && (!localStart.GetComponent<PoleDot>().left.GetComponent<PoleLine>().left.GetComponent<PoleDot>().isUsedByPlayer || localStart.GetComponent<PoleDot>().left.GetComponent<PoleLine>().left  == localFinish))
        {
            localStart.GetComponent<PoleDot>().left.GetComponent<PoleLine>().isUsedByPlayer = true;
            localStart.GetComponent<PoleDot>().left.GetComponent<PoleLine>().left.GetComponent<PoleDot>().isUsedByPlayer = true;
            return true;
        }
        return false;
    }
    private static bool checkRight(GameObject localStart, GameObject localFinish)
    {
        if (localStart.GetComponent<PoleDot>().AllowedToRight() && (!localStart.GetComponent<PoleDot>().right.GetComponent<PoleLine>().right.GetComponent<PoleDot>().isUsedByPlayer || localStart.GetComponent<PoleDot>().right.GetComponent<PoleLine>().right  == localFinish))
        {
            localStart.GetComponent<PoleDot>().right.GetComponent<PoleLine>().isUsedByPlayer = true;
            localStart.GetComponent<PoleDot>().right.GetComponent<PoleLine>().right.GetComponent<PoleDot>().isUsedByPlayer = true;
            return true;
        }
        return false;
    }
    private static bool checkUp(GameObject localStart, GameObject localFinish)
    {
        if (localStart.GetComponent<PoleDot>().AllowedToUp() && (!localStart.GetComponent<PoleDot>().up.GetComponent<PoleLine>().up.GetComponent<PoleDot>().isUsedByPlayer || localStart.GetComponent<PoleDot>().up.GetComponent<PoleLine>().up == localFinish))
        {
            localStart.GetComponent<PoleDot>().up.GetComponent<PoleLine>().isUsedByPlayer = true;
            localStart.GetComponent<PoleDot>().up.GetComponent<PoleLine>().up.GetComponent<PoleDot>().isUsedByPlayer = true;
            
            return true;
        }
        return false;
    }
    private static bool checkDown(GameObject localStart, GameObject localFinish)
    {
        if (localStart.GetComponent<PoleDot>().AllowedToDown() && (!localStart.GetComponent<PoleDot>().down.GetComponent<PoleLine>().down.GetComponent<PoleDot>().isUsedByPlayer || localStart.GetComponent<PoleDot>().down.GetComponent<PoleLine>().down == localFinish))
        {
            localStart.GetComponent<PoleDot>().down.GetComponent<PoleLine>().isUsedByPlayer = true;
            localStart.GetComponent<PoleDot>().down.GetComponent<PoleLine>().down.GetComponent<PoleDot>().isUsedByPlayer = true;
            return true;
        }
        return false;
    }

    private static bool findLocalShortPath(GameObject localStart, GameObject localFinish, List<path> localMustVisit, int debug)
    {
        
        if (localFinish == localStart)
        {
            if(setPath(localFinish, localMustVisit, debug + 1))
                return true;
            return false;
        }
        if (Mathf.Abs(localStart.GetComponent<PoleDot>().posX - localFinish.GetComponent<PoleDot>().posX) > Mathf.Abs(localStart.GetComponent<PoleDot>().posY - localFinish.GetComponent<PoleDot>().posY))
        {
            if(localStart.GetComponent<PoleDot>().posX > localFinish.GetComponent<PoleDot>().posX)
            {
                if (checkLeft(localStart, localFinish))
                {
                    if (findLocalShortPath(localStart.GetComponent<PoleDot>().left.GetComponent<PoleLine>().left, localFinish, localMustVisit, debug + 1))
                    {
                        return true;
                    }
                    localStart.GetComponent<PoleDot>().left.GetComponent<PoleLine>().isUsedByPlayer = false;
                    localStart.GetComponent<PoleDot>().left.GetComponent<PoleLine>().left.GetComponent<PoleDot>().isUsedByPlayer = false;
                }
                if (checkUp(localStart, localFinish))
                {
                    if (findLocalShortPath(localStart.GetComponent<PoleDot>().up.GetComponent<PoleLine>().up, localFinish, localMustVisit, debug + 1))
                    {
                        return true;
                    }
                    localStart.GetComponent<PoleDot>().up.GetComponent<PoleLine>().isUsedByPlayer = false;
                    localStart.GetComponent<PoleDot>().up.GetComponent<PoleLine>().up.GetComponent<PoleDot>().isUsedByPlayer = false;
                }
                if (checkDown(localStart, localFinish))
                {
                    if (findLocalShortPath(localStart.GetComponent<PoleDot>().down.GetComponent<PoleLine>().down, localFinish, localMustVisit, debug + 1))
                    {
                        return true;
                    }
                    localStart.GetComponent<PoleDot>().down.GetComponent<PoleLine>().isUsedByPlayer = false;
                    localStart.GetComponent<PoleDot>().down.GetComponent<PoleLine>().down.GetComponent<PoleDot>().isUsedByPlayer = false;
                }
                if (checkRight(localStart, localFinish))
                {
                    if (findLocalShortPath(localStart.GetComponent<PoleDot>().right.GetComponent<PoleLine>().right, localFinish, localMustVisit, debug + 1))
                    {
                        return true;
                    }
                    localStart.GetComponent<PoleDot>().right.GetComponent<PoleLine>().isUsedByPlayer = false;
                    localStart.GetComponent<PoleDot>().right.GetComponent<PoleLine>().right.GetComponent<PoleDot>().isUsedByPlayer = false;
                }
            }
            else
            {
                if (checkRight(localStart, localFinish))
                {
                    if (findLocalShortPath(localStart.GetComponent<PoleDot>().right.GetComponent<PoleLine>().right, localFinish, localMustVisit, debug + 1))
                    {
                        return true;
                    }
                    localStart.GetComponent<PoleDot>().right.GetComponent<PoleLine>().isUsedByPlayer = false;
                    localStart.GetComponent<PoleDot>().right.GetComponent<PoleLine>().right.GetComponent<PoleDot>().isUsedByPlayer = false;
                }
                if (checkUp(localStart, localFinish))
                {
                    if (findLocalShortPath(localStart.GetComponent<PoleDot>().up.GetComponent<PoleLine>().up, localFinish, localMustVisit, debug + 1))
                    {
                        return true;
                    }
                    localStart.GetComponent<PoleDot>().up.GetComponent<PoleLine>().isUsedByPlayer = false;
                    localStart.GetComponent<PoleDot>().up.GetComponent<PoleLine>().up.GetComponent<PoleDot>().isUsedByPlayer = false;
                }
                if (checkDown(localStart, localFinish))
                {
                    if (findLocalShortPath(localStart.GetComponent<PoleDot>().down.GetComponent<PoleLine>().down, localFinish, localMustVisit, debug + 1))
                    {
                        return true;
                    }
                    localStart.GetComponent<PoleDot>().down.GetComponent<PoleLine>().isUsedByPlayer = false;
                    localStart.GetComponent<PoleDot>().down.GetComponent<PoleLine>().down.GetComponent<PoleDot>().isUsedByPlayer = false;
                }
                if (checkLeft(localStart, localFinish))
                {
                    if (findLocalShortPath(localStart.GetComponent<PoleDot>().left.GetComponent<PoleLine>().left, localFinish,localMustVisit, debug + 1))
                    {
                        return true;
                    }
                    localStart.GetComponent<PoleDot>().left.GetComponent<PoleLine>().isUsedByPlayer = false;
                    localStart.GetComponent<PoleDot>().left.GetComponent<PoleLine>().left.GetComponent<PoleDot>().isUsedByPlayer = false;
                }
            }
        }
        else
        {
            if (localStart.GetComponent<PoleDot>().posY > localFinish.GetComponent<PoleDot>().posY)
            {
                if (checkUp(localStart, localFinish))
                {
                    if (findLocalShortPath(localStart.GetComponent<PoleDot>().up.GetComponent<PoleLine>().up, localFinish, localMustVisit, debug + 1))
                    {
                        return true;
                    }
                    localStart.GetComponent<PoleDot>().up.GetComponent<PoleLine>().isUsedByPlayer = false;
                    localStart.GetComponent<PoleDot>().up.GetComponent<PoleLine>().up.GetComponent<PoleDot>().isUsedByPlayer = false;
                }
                if (checkRight(localStart, localFinish))
                {
                    if (findLocalShortPath(localStart.GetComponent<PoleDot>().right.GetComponent<PoleLine>().right, localFinish, localMustVisit, debug + 1))
                    {
                        return true;
                    }
                    localStart.GetComponent<PoleDot>().right.GetComponent<PoleLine>().isUsedByPlayer = false;
                    localStart.GetComponent<PoleDot>().right.GetComponent<PoleLine>().right.GetComponent<PoleDot>().isUsedByPlayer = false;
                }
                if (checkLeft(localStart, localFinish))
                {
                    if (findLocalShortPath(localStart.GetComponent<PoleDot>().left.GetComponent<PoleLine>().left, localFinish, localMustVisit, debug + 1))
                    {
                        return true;
                    }
                    localStart.GetComponent<PoleDot>().left.GetComponent<PoleLine>().isUsedByPlayer = false;
                    localStart.GetComponent<PoleDot>().left.GetComponent<PoleLine>().left.GetComponent<PoleDot>().isUsedByPlayer = false;
                }
                if (checkDown(localStart, localFinish))
                {
                    if (findLocalShortPath(localStart.GetComponent<PoleDot>().down.GetComponent<PoleLine>().down, localFinish, localMustVisit, debug + 1))
                    {
                        return true;
                    }
                    localStart.GetComponent<PoleDot>().down.GetComponent<PoleLine>().isUsedByPlayer = false;
                    localStart.GetComponent<PoleDot>().down.GetComponent<PoleLine>().down.GetComponent<PoleDot>().isUsedByPlayer = false;
                }
            }
            else
            {

                if (checkDown(localStart, localFinish))
                {
                    if (findLocalShortPath(localStart.GetComponent<PoleDot>().down.GetComponent<PoleLine>().down, localFinish, localMustVisit, debug + 1))
                    {
                        return true;
                    }
                    localStart.GetComponent<PoleDot>().down.GetComponent<PoleLine>().isUsedByPlayer = false;
                    localStart.GetComponent<PoleDot>().down.GetComponent<PoleLine>().down.GetComponent<PoleDot>().isUsedByPlayer = false;
                }
                if (checkRight(localStart, localFinish))
                {
                    if (findLocalShortPath(localStart.GetComponent<PoleDot>().right.GetComponent<PoleLine>().right, localFinish, localMustVisit, debug + 1))
                    {
                        return true;
                    }
                    localStart.GetComponent<PoleDot>().right.GetComponent<PoleLine>().isUsedByPlayer = false;
                    localStart.GetComponent<PoleDot>().right.GetComponent<PoleLine>().right.GetComponent<PoleDot>().isUsedByPlayer = false;
                }
                if (checkLeft(localStart, localFinish))
                {
                    if (findLocalShortPath(localStart.GetComponent<PoleDot>().left.GetComponent<PoleLine>().left, localFinish, localMustVisit, debug + 1))
                    {
                        return true;
                    }
                    localStart.GetComponent<PoleDot>().left.GetComponent<PoleLine>().isUsedByPlayer = false;
                    localStart.GetComponent<PoleDot>().left.GetComponent<PoleLine>().left.GetComponent<PoleDot>().isUsedByPlayer = false;
                }
                if (checkUp(localStart, localFinish))
                {
                    if (findLocalShortPath(localStart.GetComponent<PoleDot>().up.GetComponent<PoleLine>().up, localFinish, localMustVisit, debug + 1))
                    {
                        return true;
                    }
                    localStart.GetComponent<PoleDot>().up.GetComponent<PoleLine>().isUsedByPlayer = false;
                    localStart.GetComponent<PoleDot>().up.GetComponent<PoleLine>().up.GetComponent<PoleDot>().isUsedByPlayer = false;
                }
            }
        }
        return false;
    }

    private static bool setPath(GameObject from,List<path> localMustVisit, int debug)
    {
       
        if (debug > glob)
        {
            glob = debug;
            GameObject.FindGameObjectWithTag("Core").GetComponent<Core>().glob = localMustVisit.Count;
        }
        if (localMustVisit.Count == 0)
        {
            if (from == finish)
            {
                return true;
            }
            if (findLocalShortPath(from, finish, localMustVisit,debug+1))
            {

                return true;
            }
        }

        for (int i = 0; i < localMustVisit.Count; ++i)
        {
            List<path> nextMustVisit = new List<path>(localMustVisit);
            nextMustVisit.RemoveAt(i);
            if (localMustVisit[i].dot1 == localMustVisit[i].dot2)
            {
                if (findLocalShortPath(from, localMustVisit[i].dot1, nextMustVisit, debug + 1))
                {
                    return true;
                }
            }
            else
            {
                if (findLocalShortPath(from, localMustVisit[i].dot1, nextMustVisit, debug + 1))
                {
                    return true;
                }
                if (findLocalShortPath(from, localMustVisit[i].dot2, nextMustVisit, debug + 1))
                {
                    return true;
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
        List<Elements> shape = myPole.eltsManager.shapes;
        List<GameObject> globalStarts = myPole.starts;
        List<GameObject> globalFinishes = myPole.finishes;
        List<path> mustVisit;
        mustVisit = new List<path>();
        for(int i = 0; i < points.Count;++i)
        {
            poleDots[points[i].GetComponent<PoleEltPoint>().GetY()][points[i].GetComponent<PoleEltPoint>().GetX()].GetComponent<PoleDot>().isUsedByPlayer = true;
            if (points[i].GetComponent<PoleEltPoint>().location.GetComponent<PoleLine>() != null)
            {
                points[i].GetComponent<PoleEltPoint>().location.GetComponent<PoleLine>().isUsedByPlayer = true;
                if (points[i].GetComponent<PoleEltPoint>().location.GetComponent<PoleLine>().isHorizontal)
                {
                    poleDots[points[i].GetComponent<PoleEltPoint>().GetY()][points[i].GetComponent<PoleEltPoint>().GetX() + 1].GetComponent<PoleDot>().isUsedByPlayer = true;
                    mustVisit.Add(new path(poleDots[points[i].GetComponent<PoleEltPoint>().GetY()][points[i].GetComponent<PoleEltPoint>().GetX()], poleDots[points[i].GetComponent<PoleEltPoint>().GetY()][points[i].GetComponent<PoleEltPoint>().GetX() + 1]));
                }
                else
                {
                    poleDots[points[i].GetComponent<PoleEltPoint>().GetY() + 1][points[i].GetComponent<PoleEltPoint>().GetX()].GetComponent<PoleDot>().isUsedByPlayer = true;
                    mustVisit.Add(new path(poleDots[points[i].GetComponent<PoleEltPoint>().GetY()][points[i].GetComponent<PoleEltPoint>().GetX()], poleDots[points[i].GetComponent<PoleEltPoint>().GetY() + 1][points[i].GetComponent<PoleEltPoint>().GetX()]));
                }
                
            }
            else
            {
                mustVisit.Add(new path(poleDots[points[i].GetComponent<PoleEltPoint>().GetY()][points[i].GetComponent<PoleEltPoint>().GetX()], poleDots[points[i].GetComponent<PoleEltPoint>().GetY()][points[i].GetComponent<PoleEltPoint>().GetX()]));
            }
            
        }
        mustVisit.RemoveAt(0);//55s00*f44*p220*
        
        start = globalStarts[0];
        start.GetComponent<PoleDot>().isUsedByPlayer = true;
        finish = globalFinishes[0];
        finish.GetComponent<PoleDot>().isUsedByPlayer = true;
        // fixed bug with long generation. and pole set is used by player
        if (setPath(start, new List<path>(mustVisit), 0))
        {
            Debug.Log("all good");
            foreach (GameObject line in myPole.poleLines)
            {
                line.GetComponent<PoleLine>().isUsedBySolution = line.GetComponent<PoleLine>().isUsedByPlayer;
            }
            foreach (GameObject dot in GameObject.FindGameObjectsWithTag("PoleDot"))
            {
                dot.GetComponent<PoleDot>().isUsedBySolution = dot.GetComponent<PoleDot>().isUsedByPlayer;
            }
        }
    }
    public static void countComplexity(GameObject[][] poleDots, Pole mypole)
    {
        gentime = Time.realtimeSinceStartup;

        findPath(poleDots, mypole);

        gentime = Time.realtimeSinceStartup - gentime;
        Debug.Log("complexity gentime" + gentime);
    }
}
