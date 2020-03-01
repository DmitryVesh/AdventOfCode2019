using System;
using System.Collections.Generic;
using System.IO;

namespace Day3
{
    class Day3Code
    {
        public static List<int[]> TraverseWire(string[] splitLine, string[,] board, string character)
        {
            /*
            char[] direction = { 'U', 'R', 'D', 'L' };
            int[] totalDirValues = new int[4];
            foreach (string element in splitLine)
            {
                for (int count = 0; count < direction.Length; count++)
                {
                    if (element[0] == direction[count]) 
                    {
                        totalDirValues[count] += int.Parse(element.Substring(1));
                        break;
                    }
                }
            }
            for (int count = 0; count < direction.Length; count++)
            {
                Console.WriteLine("The value for direction {0} : {1}",direction[count], totalDirValues[count]);
            }
            */

            int currentIndexX = 16000;
            int currentIndexY = 10000;
            int total = 0;
            List<int[]> smallestStepsToReachIntercept = new List<int[]>();

            foreach (string element in splitLine)
            {

                if (element[0] == 'U')
                {
                    for (int count = 1; count < int.Parse(element.Substring(1)) + 1; count++)
                    {
                        if (board[currentIndexX, currentIndexY + count] != character && board[currentIndexX, currentIndexY + count] != null)
                        {
                            //Console.WriteLine("Coordinates ({0}, {1})", currentIndexX - 16000, (currentIndexY + count) - 10000);
                            int[] holder = { currentIndexX - 16000, (currentIndexY + count) - 10000, total + count };
                            smallestStepsToReachIntercept.Add(holder);
                        }
                        board[currentIndexX, currentIndexY + count] = character;
                    }
                    currentIndexY += int.Parse(element.Substring(1));
                    total += int.Parse(element.Substring(1));
                }
                else if (element[0] == 'D')
                {
                    for (int count = 1; count < int.Parse(element.Substring(1)) + 1; count++)
                    {
                        if (board[currentIndexX, currentIndexY - count] != character && board[currentIndexX, currentIndexY - count] != null)
                        {
                            //Console.WriteLine("Coordinates ({0}, {1})", currentIndexX - 16000, (currentIndexY - count) - 10000); 
                            int[] holder = { currentIndexX - 16000, (currentIndexY - count) - 10000, total + count };
                            smallestStepsToReachIntercept.Add(holder);
                        }
                        board[currentIndexX, currentIndexY - count] = character;
                    }
                    currentIndexY -= int.Parse(element.Substring(1));
                    total += int.Parse(element.Substring(1));
                }
                else if (element[0] == 'R')
                {
                    for (int count = 1; count < int.Parse(element.Substring(1)) + 1; count++)
                    {
                        if (board[currentIndexX + count, currentIndexY] != character && board[currentIndexX + count, currentIndexY] != null)
                        {
                            //Console.WriteLine("Coordinates ({0}, {1})", (currentIndexX + count) - 16000, currentIndexY - 10000);
                            int[] holder = { (currentIndexX + count) - 16000, currentIndexY - 10000, total + count };
                            smallestStepsToReachIntercept.Add(holder);
                        }
                        board[currentIndexX + count, currentIndexY] = character;
                    }
                    currentIndexX += int.Parse(element.Substring(1));
                    total += int.Parse(element.Substring(1));
                }
                else if (element[0] == 'L')
                {
                    for (int count = 1; count < int.Parse(element.Substring(1)) + 1; count++)
                    {
                        if (board[currentIndexX - count, currentIndexY] != character && board[currentIndexX - count, currentIndexY] != null)
                        {
                            //Console.WriteLine("Coordinates ({0}, {1})", (currentIndexX - count) - 16000, currentIndexY - 10000);
                            int[] holder = { (currentIndexX - count) - 16000, currentIndexY - 10000, total + count };
                            smallestStepsToReachIntercept.Add(holder);
                        }
                        board[currentIndexX - count, currentIndexY] = character;
                    }
                    currentIndexX -= int.Parse(element.Substring(1));
                    total += int.Parse(element.Substring(1));
                }

            }
            return smallestStepsToReachIntercept;

        }

        public void main()
        {
            string dir = "D:\\C# projects\\AdventOfCode2019\\inputFiles\\day3.txt";
            StreamReader sr = new StreamReader(dir);
            string line;
            line = sr.ReadLine();
            string[] splitLine1 = line.Split(',');
            line = sr.ReadLine();
            string[] splitLine2 = line.Split(',');

            List<int[]> holder1 = new List<int[]>();
            List<int[]> holder2 = new List<int[]>();


            string[,] board = new string[23000, 23000];

            Console.WriteLine("First");
            TraverseWire(splitLine1, board, "b");

            Console.WriteLine();
            Console.WriteLine("Second");
            holder1 = TraverseWire(splitLine2, board, "a");
            Console.WriteLine("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
            holder2 = TraverseWire(splitLine1, board, "b");

            int smallestSteps = 9999999;

            foreach (int[] intArray1 in holder1)
            {
                foreach (int[] intArray2 in holder2)
                {
                    if (intArray1[0] == intArray2[0] && intArray1[1] == intArray2[1])
                    {
                        int whatever = intArray1[2] + intArray2[2];
                        if (whatever < smallestSteps) { smallestSteps = whatever; }
                    }
                }
            }
            Console.WriteLine("finally {0}", smallestSteps);


            sr.Close();
        }
    }
}