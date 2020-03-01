﻿using System;
using System.Collections.Generic;

namespace Day17
{
    class IntCodeComputer
    {
        long relativeBase = 0;
        long currentOpcodeIndex = 0;
        string[] memory;
        string opcode;

        long inputCount = 0;
        long outputCount = 0;

        int height = 1;
        int width = 0;
        char[,] board = null;
        int currentWidth = -1;

        Queue<int> inputStream;
        int part;

        public IntCodeComputer(string puzzleInput, int initilisingPart)
        {
            part = initilisingPart;
            memory = puzzleInput.Split(',');
            int lengthOfCode = memory.Length;
            Array.Resize<string>(ref memory, lengthOfCode * 100);
            long lengthOfCodeNew = memory.Length;
            for (int count = 0; count < lengthOfCodeNew - 1; count++) { if (memory[count] == null) { memory[count] = "0"; } }
            // "R8,L12,R8,R12,L8,R10,R12,L8,R10,R8,L12,R8,R8,L8,L8,R8,R10,R8,L12,R8,R8,L12,R8,R8,L8,L8,R8,R10,R12,L8,R10,R8,L8,L8,R8,R10";
            string[] Main = { "A", "B", "B", "A", "C", "A", "A", "C", "B", "C" };
            string[] A = { "R", "8", "L", "12", "R", "8" };
            string[] B = { "R", "12", "L", "8", "R", "10" };
            string[] C = { "R", "8", "L", "8", "L", "8", "R", "8", "R", "10" };
            List<int> Mains = ToAsciiList(Main);
            List<int> As = ToAsciiList(A);
            List<int> Bs = ToAsciiList(B);
            List<int> Cs = ToAsciiList(C);
            List<List<int>> allFunctions = new List<List<int>> { Mains, As, Bs, Cs, new List<int> { 110, 10 } };
            inputStream = new Queue<int>();
            foreach (List<int> funcList in allFunctions) { foreach (int element in funcList) { inputStream.Enqueue(element); } }
        }
        private string ReturnInputFromQueue()
        {
            return inputStream.Dequeue().ToString();
        }

        public void Run()
        {
            int opcodeLen;
            bool end = false;
            while (!end)
            {
                opcode = memory[currentOpcodeIndex];
                opcodeLen = opcode.Length;
                for (int count = opcodeLen; count < 5; count++) { opcode = "0" + opcode; }
                string opcodeInstruction = opcode.Substring(3);

                if (opcodeInstruction == "01") { MathProcedure("Add"); }
                else if (opcodeInstruction == "02") { MathProcedure("Multiply"); }
                else if (opcodeInstruction == "03") { InputProcedure(); }
                else if (opcodeInstruction == "04") { OutputProcedure(); }
                else if (opcodeInstruction == "05") { JumpProcedure(true); }
                else if (opcodeInstruction == "06") { JumpProcedure(false); }
                else if (opcodeInstruction == "07") { MathProcedure("Less-than"); }
                else if (opcodeInstruction == "08") { MathProcedure("Equals"); }
                else if (opcodeInstruction == "09") { RelativeBaseProcedure(); }
                else if (opcodeInstruction == "99") { end = EndProgram(); }
                else { throw new Exception("Error in (Run) - (unknown opcode)"); }
            }
        }
        private void UpdateBoard(long asciiValue)
        {
            bool widthOrHeight;
            char[,] newBoard = null;
            if (asciiValue == 10) { widthOrHeight = false; }
            else { widthOrHeight = true; }

            if (widthOrHeight) { newBoard = new char[width + 1, height]; currentWidth += 1; }
            else { newBoard = new char[width, height + 1]; currentWidth = -1; }

            for (int heightCount = 0; heightCount < height; heightCount++)
            {
                for (int widthCount = 0; widthCount < width; widthCount++)
                {
                    newBoard[widthCount, heightCount] = board[widthCount, heightCount];
                }
            }

            if (!widthOrHeight) { height += 1; }
            else
            {
                newBoard[currentWidth, height - 1] = Convert.ToChar(asciiValue);
                if (height == 1) { width += 1; }
            }
            board = newBoard;
        }
        private void PrintBoard()
        {
            for (int heightCount = 0; heightCount < height; heightCount++)
            {
                for (int widthCount = 0; widthCount < width; widthCount++)
                {
                    Console.Write(board[widthCount, heightCount]);
                }
                Console.WriteLine();
            }
        }
        private void PrintBoard(List<int[]> intersections)
        {

            for (int heightCount = 0; heightCount < height; heightCount++)
            {
                for (int widthCount = 0; widthCount < width; widthCount++)
                {
                    bool found = false;
                    foreach (int[] element in intersections) { if (element[0] == widthCount && element[1] == heightCount) { Console.Write('O'); found = true; break; } }
                    if (!found) { Console.Write(board[widthCount, heightCount]); }

                }
                Console.WriteLine();
            }
        }
        private int ReturnSumAlignmentParameters()
        {
            int sum = 0;
            // '^' = 94
            // 'v' = 118
            // '<' == 60
            // '>' == 62

            List<int[]> listOfIntersectionCoordinates = new List<int[]>();
            List<int[]> listOfNeedToTraverse = new List<int[]>();
            HashSet<string> listOfAlreadyTraversed = new HashSet<string>();
            //listOfAlreadyTraversed.

            int[] coordinatesOfRobot = ReturnPositionOfRobot('^');
            int[] currentCoordinates = new int[] { coordinatesOfRobot[0], coordinatesOfRobot[1] };
            listOfNeedToTraverse.Add(currentCoordinates);
            listOfAlreadyTraversed.Add(currentCoordinates[0].ToString() + ',' + currentCoordinates[1].ToString());
            Console.WriteLine($"{coordinatesOfRobot[0]},{coordinatesOfRobot[1]}");

            while (true)
            {
                List<int[]> tempList = new List<int[]>();
                foreach (int[] coordinate in listOfNeedToTraverse)
                {
                    int[] newCoordinatesXAdd = new int[] { coordinate[0] + 1, coordinate[1] }; string stringXAdd = (coordinate[0] + 1).ToString() + ',' + coordinate[1].ToString();
                    int[] newCoordinatesYAdd = new int[] { coordinate[0], coordinate[1] + 1 }; string stringYAdd = coordinate[0].ToString() + ',' + (coordinate[1] + 1).ToString();
                    int[] newCoordinatesXMinus = new int[] { coordinate[0] - 1, coordinate[1] }; string stringXMinus = (coordinate[0] - 1).ToString() + ',' + coordinate[1].ToString();
                    int[] newCoordinatesYMinus = new int[] { coordinate[0], coordinate[1] - 1 }; string stringYMinus = coordinate[0].ToString() + ',' + (coordinate[1] - 1).ToString();

                    try
                    {
                        if (board[newCoordinatesXAdd[0], newCoordinatesXAdd[1]] == '#' && board[newCoordinatesXMinus[0], newCoordinatesXMinus[1]] == '#' &&
                            board[newCoordinatesYAdd[0], newCoordinatesYAdd[1]] == '#' && board[newCoordinatesYMinus[0], newCoordinatesYMinus[1]] == '#')
                        { listOfIntersectionCoordinates.Add(coordinate); }
                    }
                    catch (IndexOutOfRangeException) { }

                    try { if (!listOfAlreadyTraversed.Contains(stringXAdd) && board[newCoordinatesXAdd[0], newCoordinatesXAdd[1]] == '#') { tempList.Add(newCoordinatesXAdd); listOfAlreadyTraversed.Add(stringXAdd); } }
                    catch (IndexOutOfRangeException) { }

                    try { if (!listOfAlreadyTraversed.Contains(stringXMinus) && board[newCoordinatesXMinus[0], newCoordinatesXMinus[1]] == '#') { tempList.Add(newCoordinatesXMinus); listOfAlreadyTraversed.Add(stringXMinus); } }
                    catch (IndexOutOfRangeException) { }

                    try { if (!listOfAlreadyTraversed.Contains(stringYAdd) && board[newCoordinatesYAdd[0], newCoordinatesYAdd[1]] == '#') { tempList.Add(newCoordinatesYAdd); listOfAlreadyTraversed.Add(stringYAdd); } }
                    catch (IndexOutOfRangeException) { }

                    try { if (!listOfAlreadyTraversed.Contains(stringYMinus) && board[newCoordinatesYMinus[0], newCoordinatesYMinus[1]] == '#') { tempList.Add(newCoordinatesYMinus); listOfAlreadyTraversed.Add(stringYMinus); } }
                    catch (IndexOutOfRangeException) { }
                }
                listOfNeedToTraverse = tempList;
                if (listOfNeedToTraverse.Count == 0) { break; }
                //foreach (int[] needTraverse in listOfNeedToTraverse) { Console.WriteLine($"{needTraverse[0]},{needTraverse[1]}"); }
            }
            PrintBoard(listOfIntersectionCoordinates);
            foreach (int[] intersection in listOfIntersectionCoordinates) { sum += intersection[0] * intersection[1]; }
            return sum;
        }
        private int[] ReturnPositionOfRobot(char search)
        {
            int[] coordinates = new int[2];
            for (int heightCount = 0; heightCount < height; heightCount++)
            {
                for (int widthCount = 0; widthCount < width; widthCount++)
                {
                    if (board[widthCount, heightCount] == search) { coordinates[0] = widthCount; coordinates[1] = heightCount; break; }
                }
            }
            return coordinates;
        }
        private void OutputProcedure()
        {

            long parameter1 = GetValueFromMode(currentOpcodeIndex + 1, 2);
            if (outputCount == 3104 && part == 2) { Console.WriteLine($"The answer to part 2 : {parameter1}"); }
            else { UpdateBoard(parameter1); }
            currentOpcodeIndex += 2;
            outputCount += 1;
            //if (outputCount > 1523) { Console.ReadLine(); PrintBoard(); }
            //Console.WriteLine($"output count : {outputCount}");

        }
        private void InputProcedure()
        {
            string input = null;
            long index = GetAddressFromMode(currentOpcodeIndex + 1, 2);
            input = ReturnInputFromQueue();
            memory[index] = input;
            currentOpcodeIndex += 2;
            inputCount += 1;
        }
        private List<int> ToAsciiList(string[] array)
        {
            List<int> result = new List<int>();
            foreach (string element in array)
            {
                int characterCount = 0;
                foreach (char character in element)
                {
                    characterCount += 1;
                    if (characterCount > 1) { result.RemoveAt(result.Count - 1); result.Add(Convert.ToInt32(character)); result.Add(44); }
                    else { result.Add(Convert.ToInt32(character)); result.Add(44); }
                }
            }
            result.RemoveAt(result.Count - 1);
            result.Add(10);
            Console.WriteLine();
            return result;
        }
        private bool EndProgram()
        {
            // Part 1
            if (part == 1) { PrintBoard(); Console.WriteLine($"Sum of all alignment parameters : {ReturnSumAlignmentParameters()}"); }
            return true;
        }

        private void MathProcedure(string mode)
        {
            long parameter1 = GetValueFromMode(currentOpcodeIndex + 1, 2);
            long parameter2 = GetValueFromMode(currentOpcodeIndex + 2, 1);
            long parameter3 = GetAddressFromMode(currentOpcodeIndex + 3, 0);

            if (mode == "Add") { memory[parameter3] = (parameter1 + parameter2).ToString(); }
            else if (mode == "Multiply") { memory[parameter3] = (parameter1 * parameter2).ToString(); }
            else if (mode == "Less-than")
            {
                if (parameter1 < parameter2) { memory[parameter3] = "1"; }
                else { memory[parameter3] = "0"; }
            }
            else if (mode == "Equals")
            {
                if (parameter1 == parameter2) { memory[parameter3] = "1"; }
                else { memory[parameter3] = "0"; }
            }
            currentOpcodeIndex += 4;
        }
        private void RelativeBaseProcedure()
        {
            long parameter1 = GetValueFromMode(currentOpcodeIndex + 1, 2);
            relativeBase += parameter1;
            currentOpcodeIndex += 2;
        }
        private void JumpProcedure(bool mode)
        {
            long parameter1 = GetValueFromMode(currentOpcodeIndex + 1, 2);
            long parameter2 = GetValueFromMode(currentOpcodeIndex + 2, 1);
            if (mode == true)
            {
                if (parameter1 != 0) { currentOpcodeIndex = parameter2; }
                else if (parameter1 == 0) { currentOpcodeIndex += 3; }
            }
            else if (mode == false)
            {
                if (parameter1 == 0) { currentOpcodeIndex = parameter2; }
                else if (parameter1 != 0) { currentOpcodeIndex += 3; }
                else { throw new Exception("Error in (JumpProcedure)"); }
            }
        }
        private long GetValueFromMode(long parameterIndex, int parameterMode)
        {
            long result;
            long parameter = long.Parse(memory[parameterIndex]);
            if (opcode[parameterMode] == '0') { result = long.Parse(memory[parameter]); }
            else if (opcode[parameterMode] == '1') { result = parameter; }
            else if (opcode[parameterMode] == '2') { result = long.Parse(memory[parameter + relativeBase]); }
            else { throw new Exception("Error in (GetValueFromMode)"); }
            return result;
        }
        private long GetAddressFromMode(long parameterIndex, int parameterMode)
        {
            long result;
            long parameter = long.Parse(memory[parameterIndex]);
            if (opcode[parameterMode] == '0') { result = parameter; }
            else if (opcode[parameterMode] == '1') { result = parameterIndex; }
            else if (opcode[parameterMode] == '2') { result = relativeBase + parameter; }
            else { throw new Exception("Error in (GetAddressFromMode)"); }
            return result;
        }
    }

    class Day17Code
    {
        static string puzzleInput1 = "1,330,331,332,109,2952,1101,1182,0,16,1101,1467,0,24,102,1,0,570,1006,570,36,1002,571,1,0,1001,570,-1,570,1001,24,1,24,1106,0,18,1008,571,0,571,1001,16,1,16,1008,16,1467,570,1006,570,14,21101,0,58,0,1105,1,786,1006,332,62,99,21102,1,333,1,21101,73,0,0,1106,0,579,1101,0,0,572,1101,0,0,573,3,574,101,1,573,573,1007,574,65,570,1005,570,151,107,67,574,570,1005,570,151,1001,574,-64,574,1002,574,-1,574,1001,572,1,572,1007,572,11,570,1006,570,165,101,1182,572,127,101,0,574,0,3,574,101,1,573,573,1008,574,10,570,1005,570,189,1008,574,44,570,1006,570,158,1105,1,81,21101,340,0,1,1105,1,177,21101,477,0,1,1106,0,177,21101,514,0,1,21101,0,176,0,1105,1,579,99,21102,1,184,0,1106,0,579,4,574,104,10,99,1007,573,22,570,1006,570,165,101,0,572,1182,21101,375,0,1,21101,211,0,0,1106,0,579,21101,1182,11,1,21101,222,0,0,1105,1,979,21101,0,388,1,21102,233,1,0,1106,0,579,21101,1182,22,1,21101,0,244,0,1106,0,979,21101,401,0,1,21102,1,255,0,1106,0,579,21101,1182,33,1,21101,266,0,0,1105,1,979,21101,414,0,1,21102,1,277,0,1106,0,579,3,575,1008,575,89,570,1008,575,121,575,1,575,570,575,3,574,1008,574,10,570,1006,570,291,104,10,21101,1182,0,1,21101,313,0,0,1105,1,622,1005,575,327,1101,0,1,575,21102,327,1,0,1106,0,786,4,438,99,0,1,1,6,77,97,105,110,58,10,33,10,69,120,112,101,99,116,101,100,32,102,117,110,99,116,105,111,110,32,110,97,109,101,32,98,117,116,32,103,111,116,58,32,0,12,70,117,110,99,116,105,111,110,32,65,58,10,12,70,117,110,99,116,105,111,110,32,66,58,10,12,70,117,110,99,116,105,111,110,32,67,58,10,23,67,111,110,116,105,110,117,111,117,115,32,118,105,100,101,111,32,102,101,101,100,63,10,0,37,10,69,120,112,101,99,116,101,100,32,82,44,32,76,44,32,111,114,32,100,105,115,116,97,110,99,101,32,98,117,116,32,103,111,116,58,32,36,10,69,120,112,101,99,116,101,100,32,99,111,109,109,97,32,111,114,32,110,101,119,108,105,110,101,32,98,117,116,32,103,111,116,58,32,43,10,68,101,102,105,110,105,116,105,111,110,115,32,109,97,121,32,98,101,32,97,116,32,109,111,115,116,32,50,48,32,99,104,97,114,97,99,116,101,114,115,33,10,94,62,118,60,0,1,0,-1,-1,0,1,0,0,0,0,0,0,1,20,14,0,109,4,2102,1,-3,586,21001,0,0,-1,22101,1,-3,-3,21102,1,0,-2,2208,-2,-1,570,1005,570,617,2201,-3,-2,609,4,0,21201,-2,1,-2,1105,1,597,109,-4,2105,1,0,109,5,2101,0,-4,629,21001,0,0,-2,22101,1,-4,-4,21101,0,0,-3,2208,-3,-2,570,1005,570,781,2201,-4,-3,652,21002,0,1,-1,1208,-1,-4,570,1005,570,709,1208,-1,-5,570,1005,570,734,1207,-1,0,570,1005,570,759,1206,-1,774,1001,578,562,684,1,0,576,576,1001,578,566,692,1,0,577,577,21101,702,0,0,1105,1,786,21201,-1,-1,-1,1106,0,676,1001,578,1,578,1008,578,4,570,1006,570,724,1001,578,-4,578,21101,0,731,0,1105,1,786,1106,0,774,1001,578,-1,578,1008,578,-1,570,1006,570,749,1001,578,4,578,21102,1,756,0,1105,1,786,1105,1,774,21202,-1,-11,1,22101,1182,1,1,21101,0,774,0,1105,1,622,21201,-3,1,-3,1106,0,640,109,-5,2106,0,0,109,7,1005,575,802,20102,1,576,-6,20101,0,577,-5,1106,0,814,21101,0,0,-1,21102,1,0,-5,21101,0,0,-6,20208,-6,576,-2,208,-5,577,570,22002,570,-2,-2,21202,-5,45,-3,22201,-6,-3,-3,22101,1467,-3,-3,1202,-3,1,843,1005,0,863,21202,-2,42,-4,22101,46,-4,-4,1206,-2,924,21102,1,1,-1,1106,0,924,1205,-2,873,21101,0,35,-4,1105,1,924,1201,-3,0,878,1008,0,1,570,1006,570,916,1001,374,1,374,1201,-3,0,895,1102,1,2,0,1201,-3,0,902,1001,438,0,438,2202,-6,-5,570,1,570,374,570,1,570,438,438,1001,578,558,921,21001,0,0,-4,1006,575,959,204,-4,22101,1,-6,-6,1208,-6,45,570,1006,570,814,104,10,22101,1,-5,-5,1208,-5,33,570,1006,570,810,104,10,1206,-1,974,99,1206,-1,974,1101,0,1,575,21101,973,0,0,1106,0,786,99,109,-7,2105,1,0,109,6,21101,0,0,-4,21102,1,0,-3,203,-2,22101,1,-3,-3,21208,-2,82,-1,1205,-1,1030,21208,-2,76,-1,1205,-1,1037,21207,-2,48,-1,1205,-1,1124,22107,57,-2,-1,1205,-1,1124,21201,-2,-48,-2,1106,0,1041,21102,-4,1,-2,1106,0,1041,21102,1,-5,-2,21201,-4,1,-4,21207,-4,11,-1,1206,-1,1138,2201,-5,-4,1059,1202,-2,1,0,203,-2,22101,1,-3,-3,21207,-2,48,-1,1205,-1,1107,22107,57,-2,-1,1205,-1,1107,21201,-2,-48,-2,2201,-5,-4,1090,20102,10,0,-1,22201,-2,-1,-2,2201,-5,-4,1103,1201,-2,0,0,1106,0,1060,21208,-2,10,-1,1205,-1,1162,21208,-2,44,-1,1206,-1,1131,1105,1,989,21101,0,439,1,1105,1,1150,21102,477,1,1,1106,0,1150,21102,1,514,1,21102,1149,1,0,1106,0,579,99,21102,1157,1,0,1105,1,579,204,-2,104,10,99,21207,-3,22,-1,1206,-1,1138,1202,-5,1,1176,2102,1,-4,0,109,-6,2105,1,0,10,11,34,1,9,1,34,1,9,1,7,9,18,1,9,1,7,1,7,1,18,1,9,1,7,1,7,1,18,1,9,1,7,1,7,1,18,1,9,1,7,1,7,1,18,1,9,1,7,1,7,1,18,9,1,13,3,1,26,1,9,1,3,1,3,1,20,11,5,1,1,9,18,1,5,1,3,1,5,1,1,1,1,1,3,1,1,1,18,1,5,1,3,1,5,1,1,1,1,1,3,1,1,1,18,1,5,1,3,1,5,1,1,1,1,1,3,1,1,1,18,1,5,1,1,9,1,1,1,1,3,9,12,1,5,1,3,1,7,1,1,1,5,1,5,1,10,9,3,1,1,9,5,1,5,1,10,1,1,1,9,1,1,1,5,1,7,1,5,14,9,9,7,1,5,2,9,1,13,1,13,1,5,2,7,9,7,1,13,1,5,2,7,1,1,1,5,1,7,1,19,2,7,1,1,1,5,1,7,1,19,2,7,1,1,1,5,1,7,1,19,2,7,1,1,13,1,1,7,14,7,1,7,1,5,1,1,1,7,1,12,1,7,1,7,1,5,1,1,1,7,1,12,1,7,1,7,1,5,1,1,1,7,1,12,9,7,9,7,1,34,1,9,1,34,1,9,1,34,1,9,1,34,11,12";
        static string puzzleInput2 = "2,330,331,332,109,2952,1101,1182,0,16,1101,1467,0,24,102,1,0,570,1006,570,36,1002,571,1,0,1001,570,-1,570,1001,24,1,24,1106,0,18,1008,571,0,571,1001,16,1,16,1008,16,1467,570,1006,570,14,21101,0,58,0,1105,1,786,1006,332,62,99,21102,1,333,1,21101,73,0,0,1106,0,579,1101,0,0,572,1101,0,0,573,3,574,101,1,573,573,1007,574,65,570,1005,570,151,107,67,574,570,1005,570,151,1001,574,-64,574,1002,574,-1,574,1001,572,1,572,1007,572,11,570,1006,570,165,101,1182,572,127,101,0,574,0,3,574,101,1,573,573,1008,574,10,570,1005,570,189,1008,574,44,570,1006,570,158,1105,1,81,21101,340,0,1,1105,1,177,21101,477,0,1,1106,0,177,21101,514,0,1,21101,0,176,0,1105,1,579,99,21102,1,184,0,1106,0,579,4,574,104,10,99,1007,573,22,570,1006,570,165,101,0,572,1182,21101,375,0,1,21101,211,0,0,1106,0,579,21101,1182,11,1,21101,222,0,0,1105,1,979,21101,0,388,1,21102,233,1,0,1106,0,579,21101,1182,22,1,21101,0,244,0,1106,0,979,21101,401,0,1,21102,1,255,0,1106,0,579,21101,1182,33,1,21101,266,0,0,1105,1,979,21101,414,0,1,21102,1,277,0,1106,0,579,3,575,1008,575,89,570,1008,575,121,575,1,575,570,575,3,574,1008,574,10,570,1006,570,291,104,10,21101,1182,0,1,21101,313,0,0,1105,1,622,1005,575,327,1101,0,1,575,21102,327,1,0,1106,0,786,4,438,99,0,1,1,6,77,97,105,110,58,10,33,10,69,120,112,101,99,116,101,100,32,102,117,110,99,116,105,111,110,32,110,97,109,101,32,98,117,116,32,103,111,116,58,32,0,12,70,117,110,99,116,105,111,110,32,65,58,10,12,70,117,110,99,116,105,111,110,32,66,58,10,12,70,117,110,99,116,105,111,110,32,67,58,10,23,67,111,110,116,105,110,117,111,117,115,32,118,105,100,101,111,32,102,101,101,100,63,10,0,37,10,69,120,112,101,99,116,101,100,32,82,44,32,76,44,32,111,114,32,100,105,115,116,97,110,99,101,32,98,117,116,32,103,111,116,58,32,36,10,69,120,112,101,99,116,101,100,32,99,111,109,109,97,32,111,114,32,110,101,119,108,105,110,101,32,98,117,116,32,103,111,116,58,32,43,10,68,101,102,105,110,105,116,105,111,110,115,32,109,97,121,32,98,101,32,97,116,32,109,111,115,116,32,50,48,32,99,104,97,114,97,99,116,101,114,115,33,10,94,62,118,60,0,1,0,-1,-1,0,1,0,0,0,0,0,0,1,20,14,0,109,4,2102,1,-3,586,21001,0,0,-1,22101,1,-3,-3,21102,1,0,-2,2208,-2,-1,570,1005,570,617,2201,-3,-2,609,4,0,21201,-2,1,-2,1105,1,597,109,-4,2105,1,0,109,5,2101,0,-4,629,21001,0,0,-2,22101,1,-4,-4,21101,0,0,-3,2208,-3,-2,570,1005,570,781,2201,-4,-3,652,21002,0,1,-1,1208,-1,-4,570,1005,570,709,1208,-1,-5,570,1005,570,734,1207,-1,0,570,1005,570,759,1206,-1,774,1001,578,562,684,1,0,576,576,1001,578,566,692,1,0,577,577,21101,702,0,0,1105,1,786,21201,-1,-1,-1,1106,0,676,1001,578,1,578,1008,578,4,570,1006,570,724,1001,578,-4,578,21101,0,731,0,1105,1,786,1106,0,774,1001,578,-1,578,1008,578,-1,570,1006,570,749,1001,578,4,578,21102,1,756,0,1105,1,786,1105,1,774,21202,-1,-11,1,22101,1182,1,1,21101,0,774,0,1105,1,622,21201,-3,1,-3,1106,0,640,109,-5,2106,0,0,109,7,1005,575,802,20102,1,576,-6,20101,0,577,-5,1106,0,814,21101,0,0,-1,21102,1,0,-5,21101,0,0,-6,20208,-6,576,-2,208,-5,577,570,22002,570,-2,-2,21202,-5,45,-3,22201,-6,-3,-3,22101,1467,-3,-3,1202,-3,1,843,1005,0,863,21202,-2,42,-4,22101,46,-4,-4,1206,-2,924,21102,1,1,-1,1106,0,924,1205,-2,873,21101,0,35,-4,1105,1,924,1201,-3,0,878,1008,0,1,570,1006,570,916,1001,374,1,374,1201,-3,0,895,1102,1,2,0,1201,-3,0,902,1001,438,0,438,2202,-6,-5,570,1,570,374,570,1,570,438,438,1001,578,558,921,21001,0,0,-4,1006,575,959,204,-4,22101,1,-6,-6,1208,-6,45,570,1006,570,814,104,10,22101,1,-5,-5,1208,-5,33,570,1006,570,810,104,10,1206,-1,974,99,1206,-1,974,1101,0,1,575,21101,973,0,0,1106,0,786,99,109,-7,2105,1,0,109,6,21101,0,0,-4,21102,1,0,-3,203,-2,22101,1,-3,-3,21208,-2,82,-1,1205,-1,1030,21208,-2,76,-1,1205,-1,1037,21207,-2,48,-1,1205,-1,1124,22107,57,-2,-1,1205,-1,1124,21201,-2,-48,-2,1106,0,1041,21102,-4,1,-2,1106,0,1041,21102,1,-5,-2,21201,-4,1,-4,21207,-4,11,-1,1206,-1,1138,2201,-5,-4,1059,1202,-2,1,0,203,-2,22101,1,-3,-3,21207,-2,48,-1,1205,-1,1107,22107,57,-2,-1,1205,-1,1107,21201,-2,-48,-2,2201,-5,-4,1090,20102,10,0,-1,22201,-2,-1,-2,2201,-5,-4,1103,1201,-2,0,0,1106,0,1060,21208,-2,10,-1,1205,-1,1162,21208,-2,44,-1,1206,-1,1131,1105,1,989,21101,0,439,1,1105,1,1150,21102,477,1,1,1106,0,1150,21102,1,514,1,21102,1149,1,0,1106,0,579,99,21102,1157,1,0,1105,1,579,204,-2,104,10,99,21207,-3,22,-1,1206,-1,1138,1202,-5,1,1176,2102,1,-4,0,109,-6,2105,1,0,10,11,34,1,9,1,34,1,9,1,7,9,18,1,9,1,7,1,7,1,18,1,9,1,7,1,7,1,18,1,9,1,7,1,7,1,18,1,9,1,7,1,7,1,18,1,9,1,7,1,7,1,18,9,1,13,3,1,26,1,9,1,3,1,3,1,20,11,5,1,1,9,18,1,5,1,3,1,5,1,1,1,1,1,3,1,1,1,18,1,5,1,3,1,5,1,1,1,1,1,3,1,1,1,18,1,5,1,3,1,5,1,1,1,1,1,3,1,1,1,18,1,5,1,1,9,1,1,1,1,3,9,12,1,5,1,3,1,7,1,1,1,5,1,5,1,10,9,3,1,1,9,5,1,5,1,10,1,1,1,9,1,1,1,5,1,7,1,5,14,9,9,7,1,5,2,9,1,13,1,13,1,5,2,7,9,7,1,13,1,5,2,7,1,1,1,5,1,7,1,19,2,7,1,1,1,5,1,7,1,19,2,7,1,1,1,5,1,7,1,19,2,7,1,1,13,1,1,7,14,7,1,7,1,5,1,1,1,7,1,12,1,7,1,7,1,5,1,1,1,7,1,12,1,7,1,7,1,5,1,1,1,7,1,12,9,7,9,7,1,34,1,9,1,34,1,9,1,34,1,9,1,34,11,12";


        public void main()
        {
            IntCodeComputer computer = new IntCodeComputer(puzzleInput1, 1);
            computer.Run();
            computer = new IntCodeComputer(puzzleInput2, 2);
            computer.Run();
            Console.ReadLine();
        }
    }
}