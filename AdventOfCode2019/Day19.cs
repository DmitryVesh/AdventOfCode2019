using System;
using System.Collections.Generic;

namespace Day19
{
    class IntCodeComputer
    {
        long relativeBase = 0;
        long currentOpcodeIndex = 0;
        string[] memory;
        string opcode;

        long inputCount = 0;
        long outputCount = 0;

        string[] inputs;
        long output;

        public IntCodeComputer(string puzzleInput, int[] initInput)
        {
            inputs = new string[] { initInput[0].ToString(), initInput[1].ToString() };
            memory = puzzleInput.Split(',');
            int lengthOfCode = memory.Length;
            Array.Resize<string>(ref memory, lengthOfCode * 100);
            long lengthOfCodeNew = memory.Length;
            for (int count = 0; count < lengthOfCodeNew - 1; count++) { if (memory[count] == null) { memory[count] = "0"; } }
        }
        private void OutputProcedure()
        {
            long parameter1 = GetValueFromMode(currentOpcodeIndex + 1, 2);
            output = parameter1;
            currentOpcodeIndex += 2;
            outputCount += 1;
        }
        private void InputProcedure()
        {
            //while (true)
            //{
            //for (int count = 0; count < 2; count++)
            //{
            //string input = null;
            long index = GetAddressFromMode(currentOpcodeIndex + 1, 2);
            if (inputCount % 2 == 0) { memory[index] = inputs[0]; }
            else { memory[index] = inputs[1]; }
            currentOpcodeIndex += 2;
            inputCount += 1;
            //Console.WriteLine(inputCount);
            //}
            //OutputProcedure();
            //if (inputCount % 100 == 0) { PrintBoard(); Console.ReadLine(); }
            //}

        }
        private bool EndProgram()
        {
            return true;
        }

        public long Run()
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
            return output;
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

    class Day19Code
    {
        static string puzzleInput = "109,424,203,1,21101,11,0,0,1105,1,282,21101,0,18,0,1106,0,259,1202,1,1,221,203,1,21101,0,31,0,1105,1,282,21102,1,38,0,1106,0,259,20101,0,23,2,22102,1,1,3,21101,1,0,1,21101,0,57,0,1106,0,303,1202,1,1,222,21002,221,1,3,21001,221,0,2,21102,1,259,1,21101,80,0,0,1105,1,225,21102,1,117,2,21102,1,91,0,1105,1,303,1202,1,1,223,20102,1,222,4,21101,0,259,3,21101,0,225,2,21101,225,0,1,21101,118,0,0,1105,1,225,21001,222,0,3,21101,20,0,2,21102,1,133,0,1105,1,303,21202,1,-1,1,22001,223,1,1,21101,0,148,0,1106,0,259,2101,0,1,223,20102,1,221,4,21001,222,0,3,21101,0,16,2,1001,132,-2,224,1002,224,2,224,1001,224,3,224,1002,132,-1,132,1,224,132,224,21001,224,1,1,21102,195,1,0,105,1,108,20207,1,223,2,21002,23,1,1,21102,-1,1,3,21101,0,214,0,1105,1,303,22101,1,1,1,204,1,99,0,0,0,0,109,5,1201,-4,0,249,22102,1,-3,1,22101,0,-2,2,21202,-1,1,3,21102,1,250,0,1106,0,225,22102,1,1,-4,109,-5,2105,1,0,109,3,22107,0,-2,-1,21202,-1,2,-1,21201,-1,-1,-1,22202,-1,-2,-2,109,-3,2106,0,0,109,3,21207,-2,0,-1,1206,-1,294,104,0,99,21202,-2,1,-2,109,-3,2105,1,0,109,5,22207,-3,-4,-1,1206,-1,346,22201,-4,-3,-4,21202,-3,-1,-1,22201,-4,-1,2,21202,2,-1,-1,22201,-4,-1,1,21201,-2,0,3,21101,343,0,0,1105,1,303,1105,1,415,22207,-2,-3,-1,1206,-1,387,22201,-3,-2,-3,21202,-2,-1,-1,22201,-3,-1,3,21202,3,-1,-1,22201,-3,-1,2,21201,-4,0,1,21101,0,384,0,1105,1,303,1105,1,415,21202,-4,-1,-4,22201,-4,-3,-4,22202,-3,-2,-2,22202,-2,-4,-4,22202,-3,-2,-3,21202,-4,-1,-2,22201,-3,-2,1,22101,0,1,-4,109,-5,2105,1,0";
        static Queue<int> inputQ = new Queue<int>();
        static int[] lastInput = new int[2];
        static int height = 1500;
        static int width = 1500;
        static HashSet<string> board = new HashSet<string>();
        static int squareWidth = 100;
        static int squareHeight = 100;
        static List<int[]> foundList = new List<int[]>();

        public void main()
        {
            for (int countHeight = 0; countHeight < height; countHeight++) { for (int countWidth = 0; countWidth < width; countWidth++) { inputQ.Enqueue(countWidth); inputQ.Enqueue(countHeight); } }
            while (true)
            {
                try { lastInput[0] = inputQ.Dequeue(); lastInput[1] = inputQ.Dequeue(); } catch (InvalidOperationException) { break; }
                IntCodeComputer computer = new IntCodeComputer(puzzleInput, lastInput);
                UpdateBoard(computer.Run());
            }
            // Part 1
            Console.WriteLine($"The number of points affected by tractor beam : {ReturnNumPointsAffected()}");
            Console.WriteLine("part1 done");
            // Part 2
            Console.WriteLine($"The final answer : {ReturnAnswerToPart2()}");
            Console.ReadLine();
        }
        public int ReturnAnswerToPart2()
        {
            int result;
            bool found = false;
            int[] leftMost = null;
            int difInLeftMostRightMost = 0;
            for (int countHeight = 0; countHeight < height; countHeight++)
            {
                bool beamPresent = false;
                leftMost = new int[] { -1, -1 };
                int[] rightMost = { -1, -1 };
                int beamWidth = 0;
                int beamHeight;

                for (int countWidth = 0; countWidth < width; countWidth++)
                {
                    string key1 = countWidth.ToString() + ',' + countHeight.ToString();
                    if (!beamPresent && board.Contains(key1)) { beamPresent = true; leftMost[0] = countWidth; leftMost[1] = countHeight; beamWidth = 1; }
                    else if (beamPresent && !board.Contains(key1)) { beamPresent = false; rightMost[0] = countWidth - 1; rightMost[1] = countHeight; }
                    else if (beamPresent && board.Contains(key1)) { beamWidth += 1; }
                    else if (!beamPresent && rightMost[0] != -1) { break; }
                }

                if (beamWidth >= squareWidth)
                {
                    difInLeftMostRightMost = 0;
                    bool incompleteLine;
                    while (rightMost[0] - (leftMost[0] + difInLeftMostRightMost) > squareWidth - 2)
                    {
                        beamHeight = 1;
                        for (int beamDepthCount = countHeight + 1; beamDepthCount < height; beamDepthCount++)
                        {
                            beamWidth = 0;
                            incompleteLine = false;
                            for (int beamWidthCount = leftMost[0] + difInLeftMostRightMost; beamWidthCount < width; beamWidthCount++)
                            {
                                string key2 = beamWidthCount.ToString() + ',' + beamDepthCount.ToString();
                                if (!board.Contains(key2) && beamWidth < squareWidth) { incompleteLine = true; break; }
                                else if (board.Contains(key2)) { beamWidth += 1; }
                                if (beamWidth == 10) { break; }
                            }
                            if (incompleteLine) { break; }
                            beamHeight += 1;
                            if (beamHeight == squareHeight) { Console.WriteLine($"top left coordinates : {leftMost[0] + difInLeftMostRightMost},{countHeight}"); found = true; break; }
                        }
                        if (found) { break; }
                        difInLeftMostRightMost += 1;
                    }
                    if (found) { break; }
                }
                if (countHeight % 200 == 0) { Console.WriteLine("Soon"); }
                if (found) { break; }
            }

            foundList = ReturnListOfClosestCoordinates(leftMost, difInLeftMostRightMost);
            //PrintBoard();
            int[] closestCoordinate = ReturnClosestCoordinate(foundList);
            Console.WriteLine($"Closest coordinate : {closestCoordinate[0]},{closestCoordinate[1]}");
            result = (closestCoordinate[0] * 10000) + closestCoordinate[1];
            return result;
        }
        public List<int[]> ReturnListOfClosestCoordinates(int[] topLeftCoordinates, int numFromLeft)
        {
            List<int[]> coordinatesToCheck = new List<int[]>();
            for (int countHeight = 0; countHeight < squareHeight; countHeight++) { coordinatesToCheck.Add(new int[] { topLeftCoordinates[0] + numFromLeft, topLeftCoordinates[1] + countHeight }); }
            for (int countWidth = 0; countWidth < squareWidth; countWidth++) { coordinatesToCheck.Add(new int[] { topLeftCoordinates[0] + numFromLeft + countWidth, topLeftCoordinates[1] }); }
            return coordinatesToCheck;
        }
        public int[] ReturnClosestCoordinate(List<int[]> listCoordinates)
        {
            int[] closest = new int[] { 9999, 9999 };
            foreach (int[] element in listCoordinates) { if (closest[0] + closest[1] > element[0] + element[1]) { closest = element; } }
            return closest;
        }

        public int ReturnNumPointsAffected()
        {
            return board.Count;
        }
        public void UpdateBoard(long droneState)
        {
            if (droneState == 1)
            {
                string key = lastInput[0].ToString() + ',' + lastInput[1].ToString();
                board.Add(key);
            }
        }
        public void PrintBoard()
        {
            for (int countHeight = 0; countHeight < height; countHeight++)
            {
                if (countHeight < 10) { Console.Write($"  {countHeight} "); }
                else if (countHeight < 100) { Console.Write($" {countHeight} "); }
                else { Console.Write($"{countHeight} "); }
                for (int countWidth = 0; countWidth < width; countWidth++)
                {
                    foreach (int[] element in foundList) { if (element[0] == countWidth && element[1] == countHeight) { Console.ForegroundColor = ConsoleColor.Red; } }
                    if (board.Contains(countWidth.ToString() + ',' + countHeight.ToString())) { Console.Write('#'); }
                    else { Console.Write('.'); }
                    Console.ResetColor();
                }
                Console.WriteLine();
            }
        }
    }
}