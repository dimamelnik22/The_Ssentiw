using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Complexity
{
    //bug 55s41*f10*p310220431100130020042*r20ff00ffff01ff00ffff12ff00ffff23ff00ffff03ff00ffff300000ffff310000ffff*t0011122230101111011133121111111021211131113232011101*
    private static Pole ActivePole;
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
        bool flag = false;
        if (localStart.GetComponent<PoleDot>().AllowedToLeft())
        {
            if (!localStart.GetComponent<PoleDot>().left.GetComponent<PoleLine>().left.GetComponent<PoleDot>().isUsedByPlayer)
            {
                if (localStart.GetComponent<PoleDot>().left.GetComponent<PoleLine>().left.GetComponent<PoleDot>().isUsedBySolution)
                {
                    if (localStart.GetComponent<PoleDot>().left.GetComponent<PoleLine>().left == localFinish)
                    {
                        flag = true;
                    }
                }
                else
                {
                    flag = true;
                }
            }
        }
        if(flag)
        {
            localStart.GetComponent<PoleDot>().left.GetComponent<PoleLine>().isUsedByPlayer = true;
            localStart.GetComponent<PoleDot>().left.GetComponent<PoleLine>().left.GetComponent<PoleDot>().isUsedByPlayer = true;
            if (findLocalShortPath(localStart.GetComponent<PoleDot>().left.GetComponent<PoleLine>().left, localFinish, localMustVisit, LinePath))
            {
                //Debug.Log("l");
                return true;
            }
            localStart.GetComponent<PoleDot>().left.GetComponent<PoleLine>().isUsedByPlayer = false;
            //if (localFinish != localStart.GetComponent<PoleDot>().left.GetComponent<PoleLine>().left) 
            localStart.GetComponent<PoleDot>().left.GetComponent<PoleLine>().left.GetComponent<PoleDot>().isUsedByPlayer = false;
        }
        return false;
    }
    private static bool checkRight(GameObject localStart, GameObject localFinish, List<path> localMustVisit, path LinePath)
    {
        bool flag = false;

        if (localStart.GetComponent<PoleDot>().AllowedToRight())
        {
            if (!localStart.GetComponent<PoleDot>().right.GetComponent<PoleLine>().right.GetComponent<PoleDot>().isUsedByPlayer)
            {
                if (localStart.GetComponent<PoleDot>().right.GetComponent<PoleLine>().right.GetComponent<PoleDot>().isUsedBySolution)
                {
                    if (localStart.GetComponent<PoleDot>().right.GetComponent<PoleLine>().right == localFinish)
                    {
                        flag = true;
                    }
                }
                else
                {
                    flag = true;
                }
            }
        }
        if (flag)
            {
            localStart.GetComponent<PoleDot>().right.GetComponent<PoleLine>().isUsedByPlayer = true;
            localStart.GetComponent<PoleDot>().right.GetComponent<PoleLine>().right.GetComponent<PoleDot>().isUsedByPlayer = true;
            if (findLocalShortPath(localStart.GetComponent<PoleDot>().right.GetComponent<PoleLine>().right, localFinish, localMustVisit, LinePath))
            {
                //Debug.Log("R");
                return true;
            }
            localStart.GetComponent<PoleDot>().right.GetComponent<PoleLine>().isUsedByPlayer = false;
            //if (localFinish != localStart.GetComponent<PoleDot>().right.GetComponent<PoleLine>().right) 
            localStart.GetComponent<PoleDot>().right.GetComponent<PoleLine>().right.GetComponent<PoleDot>().isUsedByPlayer = false;
        }
        return false;
    }
    private static bool checkUp(GameObject localStart, GameObject localFinish, List<path> localMustVisit, path LinePath)
    {
        bool flag = false;
        if (localStart.GetComponent<PoleDot>().AllowedToUp())
        {
            if (!localStart.GetComponent<PoleDot>().up.GetComponent<PoleLine>().up.GetComponent<PoleDot>().isUsedByPlayer)
            {
                if (localStart.GetComponent<PoleDot>().up.GetComponent<PoleLine>().up.GetComponent<PoleDot>().isUsedBySolution)
                {
                    if (localStart.GetComponent<PoleDot>().up.GetComponent<PoleLine>().up == localFinish)
                    {
                        flag = true;
                    }
                }
                else
                {
                    flag = true;
                }
            }

        }
        if (flag)
        {
            localStart.GetComponent<PoleDot>().up.GetComponent<PoleLine>().isUsedByPlayer = true;
            localStart.GetComponent<PoleDot>().up.GetComponent<PoleLine>().up.GetComponent<PoleDot>().isUsedByPlayer = true;
            if (findLocalShortPath(localStart.GetComponent<PoleDot>().up.GetComponent<PoleLine>().up, localFinish, localMustVisit, LinePath))
            {
                //Debug.Log("U");
                return true;
            }
            localStart.GetComponent<PoleDot>().up.GetComponent<PoleLine>().isUsedByPlayer = false;
            //if (localFinish != localStart.GetComponent<PoleDot>().up.GetComponent<PoleLine>().up) 
            localStart.GetComponent<PoleDot>().up.GetComponent<PoleLine>().up.GetComponent<PoleDot>().isUsedByPlayer = false;
            
        }
        return false;
    }
    private static bool checkDown(GameObject localStart, GameObject localFinish, List<path> localMustVisit, path LinePath)
    {
        bool flag = false;
        if (localStart.GetComponent<PoleDot>().AllowedToDown())
        {
            if (!localStart.GetComponent<PoleDot>().down.GetComponent<PoleLine>().down.GetComponent<PoleDot>().isUsedByPlayer)
            {
                if (localStart.GetComponent<PoleDot>().down.GetComponent<PoleLine>().down.GetComponent<PoleDot>().isUsedBySolution)
                {
                    if (localStart.GetComponent<PoleDot>().down.GetComponent<PoleLine>().down == localFinish)
                    {
                        flag = true;
                    }
                }
                else
                {
                    flag = true;
                }
            }

        }
        if (flag)
        {
            localStart.GetComponent<PoleDot>().down.GetComponent<PoleLine>().isUsedByPlayer = true;
            localStart.GetComponent<PoleDot>().down.GetComponent<PoleLine>().down.GetComponent<PoleDot>().isUsedByPlayer = true;
            if (findLocalShortPath(localStart.GetComponent<PoleDot>().down.GetComponent<PoleLine>().down, localFinish, localMustVisit, LinePath))
            {
                //Debug.Log("D");
                return true;
            }
            localStart.GetComponent<PoleDot>().down.GetComponent<PoleLine>().isUsedByPlayer = false;
            //if (localFinish != localStart.GetComponent<PoleDot>().down.GetComponent<PoleLine>().down)
                localStart.GetComponent<PoleDot>().down.GetComponent<PoleLine>().down.GetComponent<PoleDot>().isUsedByPlayer = false;
        }
        return false;
    }

    private static bool findLocalShortPath(GameObject localStart, GameObject localFinish, List<path> localMustVisit, path LinePath)
    {
        if (localFinish == localStart)
        {
            if(LinePath.dot1 != LinePath.dot2)
            {
                if (LinePath.dot1 == localFinish)
                {
                    if (LinePath.dot2.GetComponent<PoleDot>().isUsedByPlayer)
                    {
                        //Debug.Log(LinePath.dot2.GetComponent<PoleDot>().posX +" "+ LinePath.dot2.GetComponent<PoleDot>().posY);
                        return false;
                    }
                    LinePath.dot2.GetComponent<PoleDot>().isUsedByPlayer = true;
                    LinePath.dot1.GetComponent<PoleDot>().isUsedByPlayer = true;
                    if (setPath(LinePath.dot2, localMustVisit)) return true;
                    LinePath.dot2.GetComponent<PoleDot>().isUsedByPlayer = false;
                    LinePath.dot1.GetComponent<PoleDot>().isUsedByPlayer = false;
                    return false;
                }
                if (LinePath.dot2 == localFinish)
                {
                    if (LinePath.dot1.GetComponent<PoleDot>().isUsedByPlayer)
                    {
                        //Debug.Log(LinePath.dot1.GetComponent<PoleDot>().posX + " " + LinePath.dot1.GetComponent<PoleDot>().posY);
                        return false;
                    }
                    LinePath.dot1.GetComponent<PoleDot>().isUsedByPlayer = true;
                    LinePath.dot2.GetComponent<PoleDot>().isUsedByPlayer = true;
                    if (setPath(LinePath.dot1, localMustVisit)) return true;
                    LinePath.dot2.GetComponent<PoleDot>().isUsedByPlayer = false;
                    LinePath.dot1.GetComponent<PoleDot>().isUsedByPlayer = false;
                    return false;
                }
            }
            else
            {
                if (setPath(localFinish, localMustVisit)) return true;
                else return false;
            }
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
                return true;// dont check solution
                if (ActivePole.eltsManager.CheckSolution(poleDots[0][0].GetComponent<PoleDot>().right.GetComponent<PoleLine>().down))
                {
                    return true;
                }
                else
                {
                    return false;
                }
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
        ActivePole = myPole;
        //bug 
        //66s15*f35*p052352440110332100201*r14ef7d57ff34ef7d57ff30ef7d57ff21ef7d57ff23ef7d57ff11ef7d57ff325d275dff*t0011120111413201011144111*
        //66s51*f53*p520302342450130011142252150042*r14ef7d57ff12ef7d57ff20ef7d57ff22ef7d57ff1397da3fff3362232fff405d275dff*t03121144221101412111*
        //66s10*f02*p101210311330031042422152521531*r42facb3eff13facb3eff02facb3eff34facb3eff43facb3eff1062232fff2397da3fff*t0411120231110104111130111242111*
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
                    poleDots[points[i].GetComponent<PoleEltPoint>().GetY()][points[i].GetComponent<PoleEltPoint>().GetX() + 1].GetComponent<PoleDot>().left.GetComponent<PoleLine>().isUsedByPlayer = true;
                    poleDots[points[i].GetComponent<PoleEltPoint>().GetY()][points[i].GetComponent<PoleEltPoint>().GetX() + 1].GetComponent<PoleDot>().isUsedBySolution = true;
                    mustVisit.Add(new path(poleDots[points[i].GetComponent<PoleEltPoint>().GetY()][points[i].GetComponent<PoleEltPoint>().GetX()], poleDots[points[i].GetComponent<PoleEltPoint>().GetY()][points[i].GetComponent<PoleEltPoint>().GetX() + 1]));
                }
                else
                {
                    poleDots[points[i].GetComponent<PoleEltPoint>().GetY() + 1][points[i].GetComponent<PoleEltPoint>().GetX()].GetComponent<PoleDot>().up.GetComponent<PoleLine>().isUsedByPlayer = true;
                    poleDots[points[i].GetComponent<PoleEltPoint>().GetY() + 1][points[i].GetComponent<PoleEltPoint>().GetX()].GetComponent<PoleDot>().isUsedBySolution = true;
                    mustVisit.Add(new path(poleDots[points[i].GetComponent<PoleEltPoint>().GetY()][points[i].GetComponent<PoleEltPoint>().GetX()], poleDots[points[i].GetComponent<PoleEltPoint>().GetY() + 1][points[i].GetComponent<PoleEltPoint>().GetX()]));
                }
                
            }
            else
            {
                mustVisit.Add(new path(poleDots[points[i].GetComponent<PoleEltPoint>().GetY()][points[i].GetComponent<PoleEltPoint>().GetX()], poleDots[points[i].GetComponent<PoleEltPoint>().GetY()][points[i].GetComponent<PoleEltPoint>().GetX()]));
            }
            
        }
        for(int i = 0; i < mustVisit.Count;++i)
        {
            for (int j = i+1; j < mustVisit.Count; ++j)
            {
                if (mustVisit[i].dot1 == mustVisit[j].dot1)
                {
                    mustVisit[i].dot1 = mustVisit[j].dot2;
                    mustVisit.RemoveAt(j);
                    --j;
                }
                else if (mustVisit[i].dot1 == mustVisit[j].dot2)
                {
                    mustVisit[i].dot1 = mustVisit[j].dot1;
                    mustVisit.RemoveAt(j);
                    --j;
                }
                else if (mustVisit[i].dot2 == mustVisit[j].dot1)
                {
                    mustVisit[i].dot2 = mustVisit[j].dot2;
                    mustVisit.RemoveAt(j);
                    --j;
                }
                else if (mustVisit[i].dot2 == mustVisit[j].dot2)
                {
                    mustVisit[i].dot2 = mustVisit[j].dot1;
                    mustVisit.RemoveAt(j);
                    --j;
                }
            }
        }
        Debug.Log(mustVisit.Count);
        //55s00*f44*p220*
        
        start = globalStarts[0];
        start.GetComponent<PoleDot>().isUsedBySolution = true;
        start.GetComponent<PoleDot>().isUsedByPlayer = true;
        finish = globalFinishes[0];
        finish.GetComponent<PoleDot>().isUsedBySolution = true;
        // fixed bug with long generation. and pole set is used by player
        if (setPath(start, new List<path>(mustVisit)))
        {
            Debug.Log("all good");
        }
            foreach (GameObject line in myPole.poleLines)
            {
                //line.GetComponent<PoleLine>().isUsedBySolution = line.GetComponent<PoleLine>().isUsedByPlayer || line.GetComponent<PoleLine>().isUsedBySolution;
                line.GetComponent<PoleLine>().isUsedBySolution = line.GetComponent<PoleLine>().isUsedByPlayer;// bug
            }
            foreach (GameObject dot in GameObject.FindGameObjectsWithTag("PoleDot"))
            {
                //dot.GetComponent<PoleDot>().isUsedBySolution = dot.GetComponent<PoleDot>().isUsedByPlayer || dot.GetComponent<PoleDot>().isUsedBySolution;
                dot.GetComponent<PoleDot>().isUsedBySolution = dot.GetComponent<PoleDot>().isUsedByPlayer;// bug
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
