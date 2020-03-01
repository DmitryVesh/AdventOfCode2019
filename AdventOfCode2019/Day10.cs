using System;
using System.Collections.Generic;
using System.Linq;

namespace Day10
{
    class Day10Code
    {
        static string puzzleInput = "##.###.#.......#.#....#....#..........#.,....#..#..#.....#.##.............#......,...#.#..###..#..#.....#........#......#.,#......#.....#.##.#.##.##...#...#......#,.............#....#.....#.#......#.#....,..##.....#..#..#.#.#....##.......#.....#,.#........#...#...#.#.....#.....#.#..#.#,...#...........#....#..#.#..#...##.#.#..,#.##.#.#...#..#...........#..........#..,........#.#..#..##.#.##......##.........,................#.##.#....##.......#....,#............#.........###...#...#.....#,#....#..#....##.#....#...#.....#......#.,.........#...#.#....#.#.....#...#...#...,.............###.....#.#...##...........,...#...#.......#....#.#...#....#...#....,.....#..#...#.#.........##....#...#.....,....##.........#......#...#...#....#..#.,#...#..#..#.#...##.#..#.............#.##,.....#...##..#....#.#.##..##.....#....#.,..#....#..#........#.#.......#.##..###..,...#....#..#.#.#........##..#..#..##....,.......#.##.....#.#.....#...#...........,........#.......#.#...........#..###..##,...#.....#..#.#.......##.###.###...#....,...............#..#....#.#....#....#.#..,#......#...#.....#.#........##.##.#.....,###.......#............#....#..#.#......,..###.#.#....##..#.......#.............#,##.#.#...#.#..........##.#..#...##......,..#......#..........#.#..#....##........,......##.##.#....#....#..........#...#..,#.#..#..#.#...........#..#.......#..#.#.,#.....#.#.........#............#.#..##.#,.....##....#.##....#.....#..##....#..#..,.#.......#......#.......#....#....#..#..,...#........#.#.##..#.#..#..#........#..,#........#.#......#..###....##..#......#,...#....#...#.....#.....#.##.#..#...#...,#.#.....##....#...........#.....#...#...";

        // Example 3
        //static string puzzleInput = ".#..#..###,####.###.#,....###.#.,..###.##.#,##.##.#.#.,....###..#,..#.#..#.#,#..#.#.###,.##...##.#,.....#.#..";

        //static string puzzleInput  = "#.........,...#......,...#..#...,.####....#,..#.#.#...,.....#....,..###.#.##,.......#..,....#...#.,...#..#..#";

        static string[] splitInput;
        static bool[,] boardOfAsteroids;
        static int heightOfBoard;
        static int lengthOfBoard;
        static List<int[]> directions;

        static int[] currentCoordinates = new int[] { -1, -1 };
        static int[] bestCoordinates = new int[] { -1, -1 };

        static int orientationIndex;
        static bool[,] currentBoard;

        static public int[] ReturnCoordinatesOf200thDestroyedAsteroid()
        {
            int[] coordinatesOfDestroyedAsteroid = new int[2];

            for (int destroyedCount = 0; destroyedCount < 200; destroyedCount++)
            {
                coordinatesOfDestroyedAsteroid = DestroyAsteroid();

                if (coordinatesOfDestroyedAsteroid[0] == -1 && coordinatesOfDestroyedAsteroid[1] == -1) { destroyedCount -= 1; }
                else
                {
                    //Console.WriteLine($"Destroyed count : {destroyedCount + 1}");
                    //PrintBoardOfAsteroids(currentBoard, coordinatesOfDestroyedAsteroid);
                    //Console.WriteLine();
                }
            }
            return coordinatesOfDestroyedAsteroid;
        }
        static public int[] DestroyAsteroid()
        {
            int[] coordinates = new int[] { -1, -1 };
            int directionDepth = 0;
            int[] array = directions[orientationIndex];

            while (true)
            {
                directionDepth += 1;
                try
                {
                    if (currentBoard[(array[0] * directionDepth) + bestCoordinates[0], (array[1] * directionDepth) + bestCoordinates[1]] == true)
                    {
                        currentBoard[(array[0] * directionDepth) + bestCoordinates[0], (array[1] * directionDepth) + bestCoordinates[1]] = false;
                        coordinates[0] = (array[0] * directionDepth) + bestCoordinates[0];
                        coordinates[1] = (array[1] * directionDepth) + bestCoordinates[1];
                        //Console.WriteLine($"{coordinates[0]},{coordinates[1]}");
                        break;
                    }
                }
                catch (IndexOutOfRangeException) { break; }
            }
            orientationIndex = (orientationIndex + 1) % directions.Count;
            return coordinates;
        }

        static public int ReturnBestAsteroidNum()
        {
            splitInput = puzzleInput.Split(',');
            MakeBoardOfAsteroids();
            PrintBoardOfAsteroids(boardOfAsteroids, new int[] { -1, -1 });
            int asteroidNum = ReturnAsteroidNum(boardOfAsteroids);
            directions = ReturnDirectionsList();
            int bestNum = 0;
            int asteroidCheckedNum = 0;
            int currentAsteroidNum;

            for (int heightCount = 0; heightCount < heightOfBoard; heightCount++)
            {
                for (int lengthCount = 0; lengthCount < lengthOfBoard; lengthCount++)
                {
                    if (boardOfAsteroids[lengthCount, heightCount] == true)
                    {
                        currentCoordinates[0] = lengthCount; currentCoordinates[1] = heightCount;
                        currentAsteroidNum = ReturnCurrentViewedAsteroidNum(lengthCount, heightCount, directions);
                        asteroidCheckedNum += 1;
                        if (currentAsteroidNum > bestNum) { bestNum = currentAsteroidNum; bestCoordinates[0] = lengthCount; bestCoordinates[1] = heightCount; }
                    }
                }
            }
            currentBoard = boardOfAsteroids;
            currentBoard[bestCoordinates[0], bestCoordinates[1]] = false;
            return bestNum;
        }
        static public int ReturnCurrentViewedAsteroidNum(int coordinateX, int coordinateY, List<int[]> directions)
        {
            bool[,] testBoard = new bool[lengthOfBoard, heightOfBoard];

            int directionDepth;
            foreach (int[] array in directions)
            {
                directionDepth = 0;
                while (true)
                {
                    directionDepth += 1;
                    try
                    {
                        if (boardOfAsteroids[(array[0] * directionDepth) + coordinateX, (array[1] * directionDepth) + coordinateY] == true)
                        {
                            testBoard[(array[0] * directionDepth) + coordinateX, (array[1] * directionDepth) + coordinateY] = true;
                            break;
                        }
                    }
                    catch (IndexOutOfRangeException) { break; }
                }
            }
            return ReturnAsteroidNum(testBoard);
        }


        static public List<int[]> ReturnDirectionsList()
        {
            List<int[]> directions = new List<int[]>();
            double containsValueDivided;
            double newValueDivided;
            bool present;

            // |_|_|x|
            // |_|_|_|
            // |_|_|_|            
            for (double widthCount = 1; widthCount < lengthOfBoard; widthCount++)
            {
                for (double heightCount = -heightOfBoard + 1; heightCount < 0; heightCount++)
                {
                    present = false;
                    newValueDivided = widthCount / heightCount;
                    foreach (int[] array in directions)
                    {
                        containsValueDivided = (double)array[0] / (double)array[1];
                        if (containsValueDivided == newValueDivided) { present = true; break; }
                    }
                    if (!present) { directions.Add(new int[] { (int)widthCount, (int)heightCount }); }
                    continue;
                }
            }
            directions.Insert(0, new int[] { 0, -1 });
            directions.Insert(directions.Count, new int[] { 1, 0 });

            // |_|_|_|
            // |_|_|_|
            // |_|_|x|
            List<int[]> tempDirections = new List<int[]>();
            for (double heightCount = 1; heightCount < heightOfBoard; heightCount++)
            {
                for (double widthCount = lengthOfBoard - 1; widthCount > 0; widthCount--)
                {
                    present = false;
                    newValueDivided = widthCount / heightCount;
                    foreach (int[] array in tempDirections)
                    {
                        containsValueDivided = (double)array[0] / (double)array[1];
                        if (containsValueDivided == newValueDivided) { present = true; break; }
                    }
                    if (!present)
                    {
                        directions.Add(new int[] { (int)widthCount, (int)heightCount });
                        tempDirections.Add(new int[] { (int)widthCount, (int)heightCount });
                    }
                    continue;
                }
            }
            directions.Insert(directions.Count, new int[] { 0, 1 });

            // |_|_|_|
            // |_|_|_|
            // |x|_|_|
            tempDirections = new List<int[]>();
            for (double widthCount = -1; widthCount > -lengthOfBoard; widthCount--)
            {
                for (double heightCount = heightOfBoard - 1; heightCount > 0; heightCount--)
                {
                    present = false;
                    newValueDivided = widthCount / heightCount;
                    foreach (int[] array in tempDirections)
                    {
                        containsValueDivided = (double)array[0] / (double)array[1];
                        if (containsValueDivided == newValueDivided) { present = true; break; }
                    }
                    if (!present)
                    {
                        directions.Add(new int[] { (int)widthCount, (int)heightCount });
                        tempDirections.Add(new int[] { (int)widthCount, (int)heightCount });
                    }
                    continue;
                }
            }
            directions.Insert(directions.Count, new int[] { -1, 0 });

            // |x|_|_|
            // |_|_|_|
            // |_|_|_|
            tempDirections = new List<int[]>();
            for (double heightCount = -1; heightCount > -heightOfBoard + 1; heightCount--)
            {
                for (double widthCount = -lengthOfBoard + 1; widthCount < 0; widthCount++)
                {
                    present = false;
                    newValueDivided = widthCount / heightCount;
                    foreach (int[] array in tempDirections)
                    {
                        containsValueDivided = (double)array[0] / (double)array[1];
                        if (containsValueDivided == newValueDivided) { present = true; break; }
                    }
                    if (!present)
                    {
                        directions.Add(new int[] { (int)widthCount, (int)heightCount });
                        tempDirections.Add(new int[] { (int)widthCount, (int)heightCount });
                    }
                    continue;
                }
            }
            directions = directions.OrderBy(x => Math.Atan2(x[1], x[0])).ToList();
            for (int count = 0; count < directions.Count; count++) { if (directions[count][0] == 0 && directions[count][1] == -1) { orientationIndex = count; } }
            return directions;
        }
        static public void MakeBoardOfAsteroids()
        {
            lengthOfBoard = splitInput[0].Length;
            heightOfBoard = splitInput.Length;
            boardOfAsteroids = new bool[lengthOfBoard, heightOfBoard];
            for (int heightCount = 0; heightCount < heightOfBoard; heightCount++)
            {
                for (int lengthCount = 0; lengthCount < lengthOfBoard; lengthCount++)
                {
                    if (splitInput[heightCount][lengthCount] == '#') { boardOfAsteroids[lengthCount, heightCount] = true; }
                }
            }
        }
        static public void PrintBoardOfAsteroids(bool[,] board, int[] destroyedCoordinates)
        {
            for (int heightCount = 0; heightCount < heightOfBoard; heightCount++)
            {
                for (int lengthCount = 0; lengthCount < lengthOfBoard; lengthCount++)
                {
                    if (bestCoordinates[0] == lengthCount && bestCoordinates[1] == heightCount) { Console.BackgroundColor = ConsoleColor.Green; Console.Write('@'); Console.ResetColor(); continue; }
                    if (destroyedCoordinates[0] == lengthCount && destroyedCoordinates[1] == heightCount) { Console.BackgroundColor = ConsoleColor.Red; Console.Write('*'); Console.ResetColor(); continue; }
                    if (board[lengthCount, heightCount] == true) { Console.Write('#'); }
                    else { Console.Write('.'); }
                }
                Console.WriteLine();
            }
        }
        static public int ReturnAsteroidNum(bool[,] board)
        {
            int asteroidCount = 0;
            foreach (bool value in board) { if (value == true) { asteroidCount += 1; } }
            return asteroidCount;
        }

        public void main()
        {
            // Part 1
            Console.WriteLine($"The best number of asteroids viewed : {ReturnBestAsteroidNum()}");

            // Part 2
            int[] coordiantesTwoHunderedthAsteroid = ReturnCoordinatesOf200thDestroyedAsteroid();
            Console.WriteLine($"The coordinates of 200th asteroid are : {coordiantesTwoHunderedthAsteroid[0]},{coordiantesTwoHunderedthAsteroid[1]}");
            Console.WriteLine($"Final answer : {(coordiantesTwoHunderedthAsteroid[0] * 100) + coordiantesTwoHunderedthAsteroid[1]}");
            Console.ReadLine();
        }
    }
}