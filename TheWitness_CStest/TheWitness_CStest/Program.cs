using System;
using System.Collections.Generic;


namespace TheWitness_CStest
{
    class Program
    {
        static void Main(string[] args)
        {
            
            int size = 5;
            int seed = 100;
            Pole myPole = new Pole(size, seed);
            
            
            while (true)
            {
                
                Console.Clear();
                myPole.SetNewSeed(seed);
                myPole.SetStart(0, 0);
                myPole.SetFinish(size - 1, size - 1);
                
                myPole.CreateSolution();
                myPole.GeneratePoints(25);
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
                                        if ((k == 1) && myPole.poleDots[i][j].down.point !=null) Console.Write("0       ");
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
