using System;
using System.Collections.Generic;


namespace TheWitness_CStest
{
    class Program
    {
        static void Main(string[] args)
        {
            
            int size = 8;
            Pole myPole = new Pole(size);
            
            int seed = 100;
            while (true)
            {
                
                Console.Clear();
                myPole.SetNewSeed(seed);
                myPole.SetStart(0, 0);
                myPole.SetFinish(size - 1, size - 1);
                myPole.CreateSolution();
                for (int i = 0; i < size; i++)
                {
                    for (int j = 0; j < size; j++)
                    {
                        Console.Write("0 ");
                        if (j < size - 1)
                            if (myPole.poleDots[i][j].right != null)
                                Console.Write("0 0 0 ");
                            else Console.Write("      ");
                    }
                    Console.WriteLine();
                    if (i < size - 1)
                        for (int k = 0; k < 3; k++)
                        {
                            for (int j = 0; j < size; j++)
                            {
                                if (myPole.poleDots[i][j].down != null)
                                    Console.Write("0       ");
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
