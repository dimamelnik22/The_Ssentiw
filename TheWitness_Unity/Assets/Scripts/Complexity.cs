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
                Debug.Log("l");
                return true;
            }
            localStart.GetComponent<PoleDot>().left.GetComponent<PoleLine>().isUsedByPlayer = false;
            //if (localFinish != localStart.GetComponent<PoleDot>().left.GetComponent<PoleLine>().left) 
            localStart.GetComponent<PoleDot>().left.GetComponent<PoleLine>().left.GetComponent<PoleDot>().isUsedByPlayer = false;
            if (localStart.GetComponent<PoleDot>().posX == 3 && localStart.GetComponent<PoleDot>().posY == 1)
                Debug.Log("IDI");
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
                Debug.Log("R");
                return true;
            }
            localStart.GetComponent<PoleDot>().right.GetComponent<PoleLine>().isUsedByPlayer = false;
            //if (localFinish != localStart.GetComponent<PoleDot>().right.GetComponent<PoleLine>().right) 
            if (localStart.GetComponent<PoleDot>().posX == 2 && localStart.GetComponent<PoleDot>().posY == 1)
                Debug.Log("IDI");
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
                Debug.Log("U");
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
                Debug.Log("D");
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
        //bug
        //56s20*f24*p301012120322430340440*r210120220330134401301*t3223111010422311001110111201110323100111231113122111143111*
        //56s53*f3001*p332231310220121101002*r320210230033204335425*t303210111031321110104031111111111322011110121143220111*
        //56s44*f34*p240322321221032011000*r114214030130411421201*t003210101133111013210111012220111103201110130141111*
        for (int y = 0; y < ActivePole.height-2; ++y)
        {
            int x = 0;
            PoleSquare sq;
            PoleSquare sqDown;

            for (; x < ActivePole.width-2; ++x)
            {
                sq = poleDots[y][x].GetComponent<PoleDot>().right.GetComponent<PoleLine>().down.GetComponent<PoleSquare>();
                sqDown = sq.down.GetComponent<PoleLine>().down.GetComponent<PoleSquare>();
                PoleSquare sqRight = sq.right.GetComponent<PoleLine>().right.GetComponent<PoleSquare>();
                PoleSquare sqDownRight = sq.right.GetComponent<PoleLine>().right.GetComponent<PoleSquare>().down.GetComponent<PoleLine>().down.GetComponent<PoleSquare>();
                if (sq.hasElem && sq.element.GetComponent<PoleEltClrRing>() != null)
                {
                    if (sqDownRight.hasElem && sqDownRight.element.GetComponent<PoleEltClrRing>() != null && sqDownRight.element.c != sq.element.c)
                    {
                        if (poleDots[y + 1][x + 1].GetComponent<PoleDot>().isUsedBySolution != true)
                        {
                            poleDots[y + 1][x + 1].GetComponent<PoleDot>().isUsedBySolution = true;
                            mustVisit.Add(new path(poleDots[y + 1][x + 1], poleDots[y + 1][x + 1]));
                        }
                    }
                    if (sqRight.hasElem && sqRight.element.GetComponent<PoleEltClrRing>() != null && sqRight.element.c != sq.element.c)
                    {
                        if (poleDots[y][x + 1].GetComponent<PoleDot>().down.GetComponent<PoleLine>().isUsedByPlayer != true)
                        {
                            poleDots[y + 1][x + 1].GetComponent<PoleDot>().isUsedBySolution = true;
                            poleDots[y][x + 1].GetComponent<PoleDot>().isUsedBySolution = true;
                            poleDots[y][x + 1].GetComponent<PoleDot>().down.GetComponent<PoleLine>().isUsedByPlayer = true;
                            mustVisit.Add(new path(poleDots[y + 1][x + 1], poleDots[y][x + 1]));
                        }
                    }
                    if (sqDown.hasElem && sqDown.element.GetComponent<PoleEltClrRing>() != null && sqDown.element.c != sq.element.c)
                    {
                        if (poleDots[y + 1][x].GetComponent<PoleDot>().right.GetComponent<PoleLine>().isUsedByPlayer != true)
                        {
                            poleDots[y + 1][x + 1].GetComponent<PoleDot>().isUsedBySolution = true;
                            poleDots[y + 1][x].GetComponent<PoleDot>().isUsedBySolution = true;
                            poleDots[y + 1][x].GetComponent<PoleDot>().right.GetComponent<PoleLine>().isUsedByPlayer = true;
                            mustVisit.Add(new path(poleDots[y + 1][x + 1], poleDots[y + 1][x]));
                        }
                    }
                }
                if (sqDown.hasElem && sqDown.element.GetComponent<PoleEltClrRing>() != null && sqRight.hasElem && sqRight.element.GetComponent<PoleEltClrRing>() != null && sqDown.element.c != sqRight.element.c)
                {
                    poleDots[y + 1][x + 1].GetComponent<PoleDot>().isUsedBySolution = true;
                    mustVisit.Add(new path(poleDots[y + 1][x + 1], poleDots[y + 1][x + 1]));
                }
            }
            
        }

        for (int i = 0; i < mustVisit.Count;++i)
        {
            for (int j = i+1; j < mustVisit.Count; ++j)
            {
                if (mustVisit[j].dot1 != mustVisit[j].dot2 && mustVisit[i].dot1 != mustVisit[i].dot2)
                {
                    if (mustVisit[i].dot1 == mustVisit[j].dot1)
                    {
                        mustVisit[i].dot1.GetComponent<PoleDot>().isUsedByPlayer = true;
                        mustVisit[i].dot1 = mustVisit[j].dot2;
                        mustVisit.RemoveAt(j);
                        --j;
                    }
                    else if (mustVisit[i].dot1 == mustVisit[j].dot2)
                    {

                        mustVisit[i].dot1.GetComponent<PoleDot>().isUsedByPlayer = true;
                        mustVisit[i].dot1 = mustVisit[j].dot1;
                        mustVisit.RemoveAt(j);
                        --j;
                    }
                    else if (mustVisit[i].dot2 == mustVisit[j].dot1)
                    {

                        mustVisit[i].dot2.GetComponent<PoleDot>().isUsedByPlayer = true;
                        mustVisit[i].dot2 = mustVisit[j].dot2;
                        mustVisit.RemoveAt(j);
                        --j;
                    }
                    else if (mustVisit[i].dot2 == mustVisit[j].dot2)
                    {
                        mustVisit[i].dot2.GetComponent<PoleDot>().isUsedByPlayer = true;
                        mustVisit[i].dot2 = mustVisit[j].dot1;
                        mustVisit.RemoveAt(j);
                        --j;
                    }
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
        //if (setPath(start, new List<path>(mustVisit)))
        //{
        //    Debug.Log("all good");
        //}
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
