using System;
using System.Collections.Generic;

namespace Day11
{
    class IntCodeComputer
    {
        long relativeBase = 0;
        long currentOpcodeIndex = 0;
        string[] memory;
        string opcode;

        long paintedTileNum;
        long inputCount = 0;
        long outputCount = 0;
        Dictionary<string, int[]> paintedCoordinates = new Dictionary<string, int[]>();
        int[] currentCoordinates = { 0, 0 };
        // 0up   1right   2down   3left
        int orientation = 0;

        public IntCodeComputer(string puzzleInput)
        {
            memory = puzzleInput.Split(',');
            int lengthOfCode = memory.Length;
            Array.Resize<string>(ref memory, lengthOfCode * 100);
            long lengthOfCodeNew = memory.Length;
            for (int count = 0; count < lengthOfCodeNew - 1; count++) { if (memory[count] == null) { memory[count] = "0"; } }
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
        private void PrintTiles()
        {

            string[] coordinates;
            int[] coordinatesInt;

            int[] smallestCoordinates = { 9999, 9999 };
            int[] biggestCoordinates = { -9999, -9999 };


            foreach (KeyValuePair<string, int[]> pair in paintedCoordinates)
            {
                coordinates = (pair.Key).Split(',');
                coordinatesInt = new int[] { int.Parse(coordinates[0]), int.Parse(coordinates[1]) };
                if (coordinatesInt[0] < smallestCoordinates[0]) { smallestCoordinates[0] = coordinatesInt[0]; }
                if (coordinatesInt[1] < smallestCoordinates[1]) { smallestCoordinates[1] = coordinatesInt[1]; }
                if (coordinatesInt[0] > biggestCoordinates[0]) { biggestCoordinates[0] = coordinatesInt[0]; }
                if (coordinatesInt[1] > biggestCoordinates[1]) { biggestCoordinates[1] = coordinatesInt[1]; }
            }

            int height = biggestCoordinates[1] + 7;
            int width = biggestCoordinates[0] + 1;
            int[,] board = new int[width, height];

            foreach (KeyValuePair<string, int[]> pair in paintedCoordinates)
            {
                coordinates = (pair.Key).Split(',');
                coordinatesInt = new int[] { int.Parse(coordinates[0]) - smallestCoordinates[0], int.Parse(coordinates[1]) - smallestCoordinates[1] };
                board[coordinatesInt[0], coordinatesInt[1]] = pair.Value[1];
            }
            for (int countHeight = height - 1; countHeight > -1; countHeight--)
            {
                for (int countWidth = 0; countWidth < width - 1; countWidth++)
                {
                    if (board[countWidth, countHeight] == 1) { Console.BackgroundColor = ConsoleColor.White; Console.Write(' '); }
                    else if (board[countWidth, countHeight] == 0) { Console.BackgroundColor = ConsoleColor.Black; Console.Write(' '); }
                    Console.ResetColor();
                }
                Console.WriteLine();
            }
        }
        private bool EndProgram()
        {
            PrintTiles();
            return true;
        }
        private void PaintTile(long colour)
        {
            string currentCoordinatesStr = currentCoordinates[0].ToString() + "," + currentCoordinates[1].ToString();
            if (!paintedCoordinates.ContainsKey(currentCoordinatesStr)) { paintedCoordinates.Add(currentCoordinatesStr, new int[] { 1, int.Parse(colour.ToString()) }); paintedTileNum += 1; }
            else { paintedCoordinates[currentCoordinatesStr][0] += 1; paintedCoordinates[currentCoordinatesStr][1] = int.Parse(colour.ToString()); }

        }
        private void MoveRobot(long directionChange)
        {
            if (directionChange == 0) { orientation -= 1; if (orientation == -1) { orientation = 3; } }
            else if (directionChange == 1) { orientation += 1; if (orientation == 4) { orientation = 0; } }

            if (orientation == 0) { currentCoordinates[1] += 1; }
            else if (orientation == 1) { currentCoordinates[0] += 1; }
            else if (orientation == 2) { currentCoordinates[1] -= 1; }
            else if (orientation == 3) { currentCoordinates[0] -= 1; }
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
        private void OutputProcedure()
        {
            long parameter1 = GetValueFromMode(currentOpcodeIndex + 1, 2);
            if (outputCount % 2 == 0) { PaintTile(parameter1); }
            else { MoveRobot(parameter1); }
            currentOpcodeIndex += 2;
            outputCount += 1;
        }
        private void InputProcedure()
        {
            long index = GetAddressFromMode(currentOpcodeIndex + 1, 2);
            string currentCoordinatesStr = currentCoordinates[0].ToString() + "," + currentCoordinates[1].ToString();
            string input;
            if (inputCount == 0) { input = "1"; }
            else if (!paintedCoordinates.ContainsKey(currentCoordinatesStr)) { input = "0"; }
            else
            {
                if (paintedCoordinates[currentCoordinatesStr][1] == 0) { input = "0"; }
                else { input = "1"; }
            }
            memory[index] = input;
            currentOpcodeIndex += 2;
            inputCount += 1;
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

        private long GetValueFromMode(long parameterIndex, int parameterMode)
        {
            long result = 0;
            long parameter = long.Parse(memory[parameterIndex]);
            if (opcode[parameterMode] == '0') { result = long.Parse(memory[parameter]); }
            else if (opcode[parameterMode] == '1') { result = parameter; }
            else if (opcode[parameterMode] == '2') { result = long.Parse(memory[parameter + relativeBase]); }
            else { throw new Exception("Error in (GetValueFromMode)"); }
            //Console.WriteLine($"Result (value) : {result}");
            return result;
        }
        private long GetAddressFromMode(long parameterIndex, int parameterMode)
        {
            long result = 0;
            long parameter = long.Parse(memory[parameterIndex]);
            if (opcode[parameterMode] == '0') { result = parameter; }
            else if (opcode[parameterMode] == '1') { result = parameterIndex; }
            else if (opcode[parameterMode] == '2') { result = relativeBase + parameter; }
            else { throw new Exception("Error in (GetAddressFromMode)"); }
            //Console.WriteLine($"Result (address) : {result}");
            return result;
        }
    }

    class Day11Code
    {
        //Actual
        static string puzzleInput = "3,8,1005,8,319,1106,0,11,0,0,0,104,1,104,0,3,8,1002,8,-1,10,101,1,10,10,4,10,108,1,8,10,4,10,1001,8,0,28,2,1008,7,10,2,4,17,10,3,8,102,-1,8,10,101,1,10,10,4,10,1008,8,0,10,4,10,1002,8,1,59,3,8,1002,8,-1,10,101,1,10,10,4,10,1008,8,0,10,4,10,1001,8,0,81,1006,0,24,3,8,1002,8,-1,10,101,1,10,10,4,10,108,0,8,10,4,10,102,1,8,105,2,6,13,10,1006,0,5,3,8,1002,8,-1,10,101,1,10,10,4,10,108,0,8,10,4,10,1002,8,1,134,2,1007,0,10,2,1102,20,10,2,1106,4,10,1,3,1,10,3,8,102,-1,8,10,101,1,10,10,4,10,108,1,8,10,4,10,1002,8,1,172,3,8,1002,8,-1,10,1001,10,1,10,4,10,108,1,8,10,4,10,101,0,8,194,1,103,7,10,1006,0,3,1,4,0,10,3,8,1002,8,-1,10,1001,10,1,10,4,10,1008,8,1,10,4,10,101,0,8,228,2,109,0,10,1,101,17,10,1006,0,79,3,8,1002,8,-1,10,1001,10,1,10,4,10,108,0,8,10,4,10,1002,8,1,260,2,1008,16,10,1,1105,20,10,1,3,17,10,3,8,1002,8,-1,10,1001,10,1,10,4,10,1008,8,1,10,4,10,1002,8,1,295,1,1002,16,10,101,1,9,9,1007,9,1081,10,1005,10,15,99,109,641,104,0,104,1,21101,387365733012,0,1,21102,1,336,0,1105,1,440,21102,937263735552,1,1,21101,0,347,0,1106,0,440,3,10,104,0,104,1,3,10,104,0,104,0,3,10,104,0,104,1,3,10,104,0,104,1,3,10,104,0,104,0,3,10,104,0,104,1,21102,3451034715,1,1,21101,0,394,0,1105,1,440,21102,3224595675,1,1,21101,0,405,0,1106,0,440,3,10,104,0,104,0,3,10,104,0,104,0,21101,0,838337454440,1,21102,428,1,0,1105,1,440,21101,0,825460798308,1,21101,439,0,0,1105,1,440,99,109,2,22101,0,-1,1,21102,1,40,2,21101,0,471,3,21101,461,0,0,1106,0,504,109,-2,2106,0,0,0,1,0,0,1,109,2,3,10,204,-1,1001,466,467,482,4,0,1001,466,1,466,108,4,466,10,1006,10,498,1102,1,0,466,109,-2,2105,1,0,0,109,4,2101,0,-1,503,1207,-3,0,10,1006,10,521,21101,0,0,-3,21202,-3,1,1,22102,1,-2,2,21101,1,0,3,21102,540,1,0,1105,1,545,109,-4,2105,1,0,109,5,1207,-3,1,10,1006,10,568,2207,-4,-2,10,1006,10,568,22102,1,-4,-4,1106,0,636,22102,1,-4,1,21201,-3,-1,2,21202,-2,2,3,21102,587,1,0,1105,1,545,21201,1,0,-4,21101,0,1,-1,2207,-4,-2,10,1006,10,606,21102,0,1,-1,22202,-2,-1,-2,2107,0,-3,10,1006,10,628,22102,1,-1,1,21102,1,628,0,105,1,503,21202,-2,-1,-2,22201,-4,-2,-4,109,-5,2106,0,0";

        public void main()
        {
            IntCodeComputer Day11Part1 = new IntCodeComputer(puzzleInput);
            Day11Part1.Run();
            Console.WriteLine("Press enter to continue...");
            Console.ReadLine();
        }
    }
}