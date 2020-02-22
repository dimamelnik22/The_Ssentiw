using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Complexity
{
    //bug 55s41*f10*p310220431100130020042*r20ff00ffff01ff00ffff12ff00ffff23ff00ffff03ff00ffff300000ffff310000ffff*t0011122230101111011133121111111021211131113232011101*
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

    private static bool checkLeft(GameObject localStart, GameObject localFinish, List<path> localMustVisit, path LinePath)
    {
        if (localStart.GetComponent<PoleDot>().AllowedToLeft())
        {
            if (localStart.GetComponent<PoleDot>().left.GetComponent<PoleLine>().left.GetComponent<PoleDot>().isUsedBySolution && localStart.GetComponent<PoleDot>().left.GetComponent<PoleLine>().left == localFinish)
            {
                localStart.GetComponent<PoleDot>().left.GetComponent<PoleLine>().isUsedByPlayer = true;
                localStart.GetComponent<PoleDot>().left.GetComponent<PoleLine>().left.GetComponent<PoleDot>().isUsedByPlayer = true;
                if (findLocalShortPath(localStart.GetComponent<PoleDot>().left.GetComponent<PoleLine>().left, localFinish, localMustVisit, LinePath)) return true;
                localStart.GetComponent<PoleDot>().left.GetComponent<PoleLine>().isUsedByPlayer = false;
                if (localFinish != localStart.GetComponent<PoleDot>().left.GetComponent<PoleLine>().left) localStart.GetComponent<PoleDot>().left.GetComponent<PoleLine>().left.GetComponent<PoleDot>().isUsedByPlayer = false;
            }
            if (!localStart.GetComponent<PoleDot>().left.GetComponent<PoleLine>().left.GetComponent<PoleDot>().isUsedByPlayer)
            {
                localStart.GetComponent<PoleDot>().left.GetComponent<PoleLine>().isUsedByPlayer = true;
                localStart.GetComponent<PoleDot>().left.GetComponent<PoleLine>().left.GetComponent<PoleDot>().isUsedByPlayer = true;
                if (findLocalShortPath(localStart.GetComponent<PoleDot>().left.GetComponent<PoleLine>().left, localFinish, localMustVisit, LinePath)) return true;
                localStart.GetComponent<PoleDot>().left.GetComponent<PoleLine>().isUsedByPlayer = false;
                if (localFinish != localStart.GetComponent<PoleDot>().left.GetComponent<PoleLine>().left) localStart.GetComponent<PoleDot>().left.GetComponent<PoleLine>().left.GetComponent<PoleDot>().isUsedByPlayer = false;
            }
        }
        return false;
    }
    private static bool checkRight(GameObject localStart, GameObject localFinish, List<path> localMustVisit, path LinePath)
    {
        if (localStart.GetComponent<PoleDot>().AllowedToRight() && (!localStart.GetComponent<PoleDot>().right.GetComponent<PoleLine>().right.GetComponent<PoleDot>().isUsedByPlayer))
        {
            localStart.GetComponent<PoleDot>().right.GetComponent<PoleLine>().isUsedByPlayer = true;
            localStart.GetComponent<PoleDot>().right.GetComponent<PoleLine>().right.GetComponent<PoleDot>().isUsedByPlayer = true;
            if (findLocalShortPath(localStart.GetComponent<PoleDot>().right.GetComponent<PoleLine>().right, localFinish, localMustVisit, LinePath)) return true;
            localStart.GetComponent<PoleDot>().right.GetComponent<PoleLine>().isUsedByPlayer = false;
            if (localFinish != localStart.GetComponent<PoleDot>().right.GetComponent<PoleLine>().right) localStart.GetComponent<PoleDot>().right.GetComponent<PoleLine>().right.GetComponent<PoleDot>().isUsedByPlayer = false;
        }
        return false;
    }
    private static bool checkUp(GameObject localStart, GameObject localFinish, List<path> localMustVisit, path LinePath)
    {
        if (localStart.GetComponent<PoleDot>().AllowedToUp() && (!localStart.GetComponent<PoleDot>().up.GetComponent<PoleLine>().up.GetComponent<PoleDot>().isUsedByPlayer))
        {
            localStart.GetComponent<PoleDot>().up.GetComponent<PoleLine>().isUsedByPlayer = true;
            localStart.GetComponent<PoleDot>().up.GetComponent<PoleLine>().up.GetComponent<PoleDot>().isUsedByPlayer = true;
            if (findLocalShortPath(localStart.GetComponent<PoleDot>().up.GetComponent<PoleLine>().up, localFinish, localMustVisit, LinePath)) return true;
            localStart.GetComponent<PoleDot>().up.GetComponent<PoleLine>().isUsedByPlayer = false;
            if (localFinish != localStart.GetComponent<PoleDot>().up.GetComponent<PoleLine>().up) localStart.GetComponent<PoleDot>().up.GetComponent<PoleLine>().up.GetComponent<PoleDot>().isUsedByPlayer = false;
            
        }
        return false;
    }
    private static bool checkDown(GameObject localStart, GameObject localFinish, List<path> localMustVisit, path LinePath)
    {
        if (localStart.GetComponent<PoleDot>().AllowedToDown() && (!localStart.GetComponent<PoleDot>().down.GetComponent<PoleLine>().down.GetComponent<PoleDot>().isUsedByPlayer))
        {
            localStart.GetComponent<PoleDot>().down.GetComponent<PoleLine>().isUsedByPlayer = true;
            localStart.GetComponent<PoleDot>().down.GetComponent<PoleLine>().down.GetComponent<PoleDot>().isUsedByPlayer = true;
            if (findLocalShortPath(localStart.GetComponent<PoleDot>().down.GetComponent<PoleLine>().down, localFinish, localMustVisit, LinePath)) return true;
            localStart.GetComponent<PoleDot>().down.GetComponent<PoleLine>().isUsedByPlayer = false;
            if (localFinish != localStart.GetComponent<PoleDot>().down.GetComponent<PoleLine>().down) localStart.GetComponent<PoleDot>().down.GetComponent<PoleLine>().down.GetComponent<PoleDot>().isUsedByPlayer = false;
        }
        return false;
    }

    private static bool findLocalShortPath(GameObject localStart, GameObject localFinish, List<path> localMustVisit, path LinePath)
    {
        if (localFinish == localStart)
        {
            GameObject next = localFinish;
            if(LinePath.dot1 != LinePath.dot2)
            {
                if (LinePath.dot1 == localFinish)
                    next = LinePath.dot2;
                if (LinePath.dot2 == localFinish)
                    next = LinePath.dot1;
            }
            if (setPath(next, localMustVisit)) return true;
            else return false;
        }
        if(Mathf.Abs(localStart.GetComponent<PoleDot>().posX - localFinish.GetComponent<PoleDot>().posX) < Mathf.Abs(localStart.GetComponent<PoleDot>().posY - localFinish.GetComponent<PoleDot>().posY))
        {
            if (localFinish.GetComponent<PoleDot>().posY - localStart.GetComponent<PoleDot>().posY > 0)
            {
                if (checkDown(localStart, localFinish, localMustVisit, LinePath)) return true;
                if (localFinish.GetComponent<PoleDot>().posX - localStart.GetComponent<PoleDot>().posX > 0)
                {
                    if (checkRight(localStart, localFinish, localMustVisit, LinePath)) return true;
                    if (checkLeft(localStart, localFinish, localMustVisit, LinePath)) return true;
                }
                else
                {
                    if (checkLeft(localStart, localFinish, localMustVisit, LinePath)) return true;
                    if (checkRight(localStart, localFinish, localMustVisit, LinePath)) return true;
                }
                if (checkUp(localStart, localFinish, localMustVisit, LinePath)) return true;
            }
            else
            {
                if (checkUp(localStart, localFinish, localMustVisit, LinePath)) return true;
                if (localFinish.GetComponent<PoleDot>().posX - localStart.GetComponent<PoleDot>().posX > 0)
                {
                    if (checkRight(localStart, localFinish, localMustVisit, LinePath)) return true;
                    if (checkLeft(localStart, localFinish, localMustVisit, LinePath)) return true;
                }
                else
                {
                    if (checkLeft(localStart, localFinish, localMustVisit, LinePath)) return true;
                    if (checkRight(localStart, localFinish, localMustVisit, LinePath)) return true;
                }
                if (checkDown(localStart, localFinish, localMustVisit, LinePath)) return true;
            }
        }
        else
        {
            if (localFinish.GetComponent<PoleDot>().posX - localStart.GetComponent<PoleDot>().posX > 0)
            {
                if (checkRight(localStart, localFinish, localMustVisit, LinePath)) return true;
                if (localFinish.GetComponent<PoleDot>().posY - localStart.GetComponent<PoleDot>().posY > 0)
                {
                    if (checkDown(localStart, localFinish, localMustVisit, LinePath)) return true;
                    if (checkUp(localStart, localFinish, localMustVisit, LinePath)) return true;
                }
                else
                {
                    if (checkUp(localStart, localFinish, localMustVisit, LinePath)) return true;
                    if (checkDown(localStart, localFinish, localMustVisit, LinePath)) return true;
                }
                if (checkLeft(localStart, localFinish, localMustVisit, LinePath)) return true;
            }
            else
            {
                if (checkLeft(localStart, localFinish, localMustVisit, LinePath)) return true;
                if (localFinish.GetComponent<PoleDot>().posY - localStart.GetComponent<PoleDot>().posY > 0)
                {
                    if (checkDown(localStart, localFinish, localMustVisit, LinePath)) return true;
                    if (checkUp(localStart, localFinish, localMustVisit, LinePath)) return true;
                }
                else
                {
                    if (checkUp(localStart, localFinish, localMustVisit, LinePath)) return true;
                    if (checkDown(localStart, localFinish, localMustVisit, LinePath)) return true;
                }
                if (checkRight(localStart, localFinish, localMustVisit, LinePath)) return true;
            }
        }
        return false;
    }

    private static bool setPath(GameObject from,List<path> localMustVisit)
    {
        //return false;
        if (localMustVisit.Count == 0)
        {
            if (from == finish)
            {
                return true;
            }
            //if (finish.GetComponent<PoleDot>().isUsedBySolution)
            //    return false;
            if (findLocalShortPath(from, finish, localMustVisit,new path(finish,finish)))
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
                if (findLocalShortPath(from, localMustVisit[i].dot1, nextMustVisit, localMustVisit[i]))
                {
                    return true;
                }
            }
            else
            {
                if (findLocalShortPath(from, localMustVisit[i].dot1, nextMustVisit, localMustVisit[i]))
                {
                    return true;
                }
                if (findLocalShortPath(from, localMustVisit[i].dot2, nextMustVisit, localMustVisit[i]))
                {
                    return true;
                }
            }
        }
        return false;
    }

    public static void findPath(GameObject[][] pDots, Pole myPole)
    {
        foreach (GameObject line in myPole.poleLines)
        {
            line.GetComponent<PoleLine>().isUsedBySolution = false;
        }
        foreach (GameObject dot in GameObject.FindGameObjectsWithTag("PoleDot"))
        {
            dot.GetComponent<PoleDot>().isUsedBySolution = false;
        }
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
            poleDots[points[i].GetComponent<PoleEltPoint>().GetY()][points[i].GetComponent<PoleEltPoint>().GetX()].GetComponent<PoleDot>().isUsedBySolution = true;
            if (points[i].GetComponent<PoleEltPoint>().location.GetComponent<PoleLine>() != null)
            {
                points[i].GetComponent<PoleEltPoint>().location.GetComponent<PoleLine>().isUsedBySolution = true;
                if (points[i].GetComponent<PoleEltPoint>().location.GetComponent<PoleLine>().isHorizontal)
                {
                    poleDots[points[i].GetComponent<PoleEltPoint>().GetY()][points[i].GetComponent<PoleEltPoint>().GetX() + 1].GetComponent<PoleDot>().isUsedBySolution = true;
                    mustVisit.Add(new path(poleDots[points[i].GetComponent<PoleEltPoint>().GetY()][points[i].GetComponent<PoleEltPoint>().GetX()], poleDots[points[i].GetComponent<PoleEltPoint>().GetY()][points[i].GetComponent<PoleEltPoint>().GetX() + 1]));
                }
                else
                {
                    poleDots[points[i].GetComponent<PoleEltPoint>().GetY() + 1][points[i].GetComponent<PoleEltPoint>().GetX()].GetComponent<PoleDot>().isUsedBySolution = true;
                    mustVisit.Add(new path(poleDots[points[i].GetComponent<PoleEltPoint>().GetY()][points[i].GetComponent<PoleEltPoint>().GetX()], poleDots[points[i].GetComponent<PoleEltPoint>().GetY() + 1][points[i].GetComponent<PoleEltPoint>().GetX()]));
                }
                
            }
            else
            {
                mustVisit.Add(new path(poleDots[points[i].GetComponent<PoleEltPoint>().GetY()][points[i].GetComponent<PoleEltPoint>().GetX()], poleDots[points[i].GetComponent<PoleEltPoint>().GetY()][points[i].GetComponent<PoleEltPoint>().GetX()]));
            }
            
        }
        
        //55s00*f44*p220*
        
        start = globalStarts[0];
        start.GetComponent<PoleDot>().isUsedBySolution = true;
        finish = globalFinishes[0];
        finish.GetComponent<PoleDot>().isUsedBySolution = true;
        // fixed bug with long generation. and pole set is used by player
        if (setPath(start, new List<path>(mustVisit)))
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
