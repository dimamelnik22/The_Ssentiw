using System;
using System.Collections.Generic;


namespace TheWitness_CStest
{
    class Program
    {
        static void Main(string[] args)
        {

            //for (int i = 0; i < 100; i++)
            //{
            //    MyRandom r = new MyRandom(i);
            //    for (int k = 0; k < 10; k++)
            //    {
            //        Console.Write(r.GetRandom() % 4 + " ");
            //    }
            //    Console.WriteLine();
            //}

            int size = 7;
            int seed = 433;
            Pole myPole = new Pole(size, seed);


            while (true)
            {

                Console.Clear();
                myPole.SetNewSeed(seed);
                int x = 0;
                int y = 0;
                switch (myPole.myRandGen.GetRandom() % 4)
                {
                    case 0:
                        x = myPole.myRandGen.GetRandom() % size;
                        y = 0;
                        break;
                    case 1:
                        y = myPole.myRandGen.GetRandom() % size;
                        x = size - 1;
                        break;
                    case 2:
                        x = myPole.myRandGen.GetRandom() % size;
                        y = size - 1;
                        break;
                    case 3:
                        y = myPole.myRandGen.GetRandom() % size;
                        x = 0;
                        break;
                }
                myPole.SetStart(x, y);
                do
                {
                    switch (myPole.myRandGen.GetRandom() % 4)
                    {
                        case 0:
                            x = myPole.myRandGen.GetRandom() % size;
                            y = 0;
                            break;
                        case 1:
                            y = myPole.myRandGen.GetRandom() % size;
                            x = size - 1;
                            break;
                        case 2:
                            x = myPole.myRandGen.GetRandom() % size;
                            y = size - 1;
                            break;
                        case 3:
                            y = myPole.myRandGen.GetRandom() % size;
                            x = 0;
                            break;
                    }
                } while (myPole.poleDots[y][x] == myPole.start);
                myPole.SetFinish(x, y);


                myPole.CreateSolution();
                myPole.GeneratePoints(13);
                for (int i = 0; i < size; i++)
                {
                    for (int j = 0; j < size; j++)
                    {
                        if (myPole.poleDots[i][j].isUsed)
                        {
                            if (myPole.poleDots[i][j].point != null) Console.Write("0 ");
                            else Console.Write("1 ");
                        }
                        else Console.Write("- ");
                        if (j < size - 1)
                            if (myPole.poleDots[i][j].right != null)
                                if (myPole.poleDots[i][j].right.isUsed)
                                {
                                    if (myPole.poleDots[i][j].right.point != null) Console.Write("1 0 1 ");
                                    else Console.Write("1 1 1 ");
                                }
                                else Console.Write("- - - ");
                            else Console.Write("      ");
                    }
                    Console.WriteLine();
                    if (i < size - 1)
                        for (int k = 0; k < 3; k++)
                        {
                            for (int j = 0; j < size; j++)
                            {
                                if (myPole.poleDots[i][j].down != null)
                                    if (myPole.poleDots[i][j].down.isUsed)
                                    {
                                        if ((k == 1) && myPole.poleDots[i][j].down.point != null) Console.Write("0       ");
                                        else Console.Write("1       ");
                                    }
                                    else Console.Write("-       ");
                                else Console.Write("        ");
                            }
                            Console.WriteLine();
                        }
                }
                myPole.ClearPole();
                string str = Console.ReadLine();
                seed = 0;
                for (int s = 0; s < str.Length; s++)
                {
                    seed = seed * 10 + str[s] - 48;
                }
            }
        }
    }
}
