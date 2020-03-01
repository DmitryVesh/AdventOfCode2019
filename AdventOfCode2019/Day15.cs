using System;
using System.Collections.Generic;
using System.Threading;

namespace Day15
{
    class IntCodeComputer
    {
        long relativeBase = 0;
        long currentOpcodeIndex = 0;
        string[] memory;
        string opcode;

        long inputCount = 0;
        long outputCount = 0;
        Dictionary<string, int> tileCoordinates = new Dictionary<string, int>();
        string currentCoordinates = "0,0";
        int lastMoveIndex = -1;
        List<int> lastMoveList = new List<int>();

        int height;
        int width;
        string[,] board;
        int[] biggestCoordinates;
        HashSet<string> alreadyFilledTiles;
        HashSet<string> stuckCoordinates = new HashSet<string>();

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
                else if (opcodeInstruction == "03") { end = InputProcedure(); }
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
        private void UpdateBoard()
        {
            string[] coordinates;
            int[] coordinatesInt;
            foreach (KeyValuePair<string, int> pair in tileCoordinates)
            {
                coordinates = pair.Key.Split(',');
                coordinatesInt = new int[] { int.Parse(coordinates[0]) + 25, int.Parse(coordinates[1]) + 25 };
                board[coordinatesInt[0], coordinatesInt[1]] = pair.Value.ToString();
                if (pair.Value == 0 && !stuckCoordinates.Contains(pair.Key)) { stuckCoordinates.Add(pair.Key); }
            }
        }
        private void PrintTiles()
        {
            string[] coordiantes = currentCoordinates.Split(',');
            int coordiantesX = int.Parse(coordiantes[0]);
            int coordiantesY = int.Parse(coordiantes[1]);
            Console.WriteLine(currentCoordinates);
            for (int countHeight = height - 1; countHeight > -1; countHeight--)
            {
                for (int countWidth = width - 1; countWidth > -1; countWidth--)
                {
                    if (countWidth == coordiantesX + 25 && countHeight == coordiantesY + 25) { Console.BackgroundColor = ConsoleColor.Red; Console.Write('A'); }
                    else if (board[countWidth, countHeight] == "0") { Console.BackgroundColor = ConsoleColor.White; Console.Write('#'); }
                    else if (board[countWidth, countHeight] == "1") { Console.BackgroundColor = ConsoleColor.Blue; Console.Write('.'); }
                    else if (board[countWidth, countHeight] == "2") { Console.BackgroundColor = ConsoleColor.Green; Console.Write('@'); }
                    else if (board[countWidth, countHeight] == "3") { Console.BackgroundColor = ConsoleColor.Green; Console.Write('O'); }
                    else { Console.Write(' '); }
                    Console.ResetColor();
                }
                Console.WriteLine();
            }
        }
        public void UpdateTiles(long output)
        {
            string[] coordiantes = currentCoordinates.Split(',');
            int currentTileCoordinatesXInt = int.Parse(coordiantes[0]);
            int currentTileCoordinatesYInt = int.Parse(coordiantes[1]);

            if (lastMoveList[lastMoveIndex] == 1) { currentTileCoordinatesYInt += 1; }
            else if (lastMoveList[lastMoveIndex] == 2) { currentTileCoordinatesYInt -= 1; }
            else if (lastMoveList[lastMoveIndex] == 3) { currentTileCoordinatesXInt -= 1; }
            else { currentTileCoordinatesXInt += 1; }

            string newCoordinates = currentTileCoordinatesXInt.ToString() + ',' + currentTileCoordinatesYInt.ToString();

            if (output == 0) { if (!tileCoordinates.ContainsKey(newCoordinates)) { tileCoordinates.Add(newCoordinates, 0); } }
            else if (output == 1) { if (!tileCoordinates.ContainsKey(newCoordinates)) { tileCoordinates.Add(newCoordinates, 1); } currentCoordinates = newCoordinates; }
            else { if (!tileCoordinates.ContainsKey(newCoordinates)) { tileCoordinates.Add(newCoordinates, 2); } currentCoordinates = newCoordinates; }
            MakeBoard();
            UpdateBoard();
        }
        private void OutputProcedure()
        {
            long parameter1 = GetValueFromMode(currentOpcodeIndex + 1, 2);
            UpdateTiles(parameter1);
            //PrintTiles();
            currentOpcodeIndex += 2;
            outputCount += 1;
            //Console.WriteLine($"Output count : {outputCount}");
        }
        private bool InputProcedure()
        {
            int input;
            long index = GetAddressFromMode(currentOpcodeIndex + 1, 2);
            if (inputCount == 0) { tileCoordinates.Add("0,0", 1); }
            /*
            while (true)
            {
                input = Console.ReadLine();
                if (input == "w") { input = 1; break; }
                else if (input == "s") { input = 2; break; }
                else if (input == "d") { input = 3; break; }
                else if (input == "a") { input = 4; break; }
                else { Console.WriteLine("Error incorrect input."); }
            }
            */

            ///*
            input = GetMoveDirection();
            lastMoveList.Add(input);
            PrintTiles();
            lastMoveIndex += 1;
            if (inputCount == 2458)
            {
                PrintTiles();
                Thread.Sleep(3000);
                OutputTimeForOxygenToFillRoom();
                return true;
            }
            //*/

            memory[index] = input.ToString();
            currentOpcodeIndex += 2;
            inputCount += 1;
            return false;
        }
        private void OutputTimeForOxygenToFillRoom()
        {
            int timeToFillRoomCount = 0;
            int numOfFilledtiles = 1;
            string coordinatesOfOxygenSystem = ReturnCoordiantesOfOxygenSystem();
            Console.WriteLine($"coordinates of oxygen system : {coordinatesOfOxygenSystem}");
            int maxNumOfEmptyTiles = ReturnNumOfEmptyTiles();
            Console.WriteLine($"number of empty tiles : {maxNumOfEmptyTiles}");
            List<string> filledOxygenTileCoordinates = new List<string> { coordinatesOfOxygenSystem };
            alreadyFilledTiles = new HashSet<string> { coordinatesOfOxygenSystem };
            while (true)
            {
                filledOxygenTileCoordinates = AddOxygenToRoomOneMin(filledOxygenTileCoordinates);
                timeToFillRoomCount += 1;

                numOfFilledtiles += filledOxygenTileCoordinates.Count;
                Console.WriteLine(numOfFilledtiles);
                if (numOfFilledtiles == maxNumOfEmptyTiles) { break; }
            }
            Console.WriteLine($"The time taken to fill the room is : {timeToFillRoomCount} minutes");
            PrintTiles();
        }
        private List<string> AddOxygenToRoomOneMin(List<string> tileCoordinatesSpreadFrom)
        {
            PrintTiles();
            List<string> tilesCoordinatesSpreadTo = new List<string>();

            foreach (string coordinateFrom in tileCoordinatesSpreadFrom)
            {
                string[] splitCoordinates = coordinateFrom.Split(',');
                int coordinateX = int.Parse(splitCoordinates[0]);
                int coordinateY = int.Parse(splitCoordinates[1]);

                string newCoordiantesXAdd1Y = (coordinateX + 1).ToString() + ',' + coordinateY.ToString();
                string newCoordiantesXMinus1Y = (coordinateX - 1).ToString() + ',' + coordinateY.ToString();
                string newCoordiantesXYAdd1 = coordinateX.ToString() + ',' + (coordinateY + 1).ToString();
                string newCoordiantesXYMinus1 = coordinateX.ToString() + ',' + (coordinateY - 1).ToString();
                try { if (!alreadyFilledTiles.Contains(newCoordiantesXAdd1Y) && tileCoordinates[newCoordiantesXAdd1Y] == 1) { tilesCoordinatesSpreadTo.Add(newCoordiantesXAdd1Y); alreadyFilledTiles.Add(newCoordiantesXAdd1Y); } }
                catch (KeyNotFoundException) { }
                try { if (!alreadyFilledTiles.Contains(newCoordiantesXMinus1Y) && tileCoordinates[newCoordiantesXMinus1Y] == 1) { tilesCoordinatesSpreadTo.Add(newCoordiantesXMinus1Y); alreadyFilledTiles.Add(newCoordiantesXMinus1Y); } }
                catch (KeyNotFoundException) { }
                try { if (!alreadyFilledTiles.Contains(newCoordiantesXYAdd1) && tileCoordinates[newCoordiantesXYAdd1] == 1) { tilesCoordinatesSpreadTo.Add(newCoordiantesXYAdd1); alreadyFilledTiles.Add(newCoordiantesXYAdd1); } }
                catch (KeyNotFoundException) { }
                try { if (!alreadyFilledTiles.Contains(newCoordiantesXYMinus1) && tileCoordinates[newCoordiantesXYMinus1] == 1) { tilesCoordinatesSpreadTo.Add(newCoordiantesXYMinus1); alreadyFilledTiles.Add(newCoordiantesXYMinus1); } }
                catch (KeyNotFoundException) { }


            }
            foreach (string tile in tilesCoordinatesSpreadTo) { tileCoordinates[tile] = 3; }
            UpdateBoard();
            return tilesCoordinatesSpreadTo;
        }


        private int ReturnNumOfEmptyTiles()
        {
            int emptyTileCount = 1;
            foreach (KeyValuePair<string, int> pair in tileCoordinates) { if (pair.Value == 1) { emptyTileCount += 1; } }
            return emptyTileCount;
        }
        private string ReturnCoordiantesOfOxygenSystem()
        {
            foreach (KeyValuePair<string, int> pair in tileCoordinates) { if (pair.Value == 2) { return pair.Key; } }
            return "";
        }
        private int GetMoveDirection()
        {
            int move;
            string[] coordinates = currentCoordinates.Split(',');
            int coordinateX = int.Parse(coordinates[0]);
            int coordinateY = int.Parse(coordinates[1]);

            string newCoordiantesXAdd1Y = (coordinateX + 1).ToString() + ',' + coordinateY.ToString();
            string newCoordiantesXMinus1Y = (coordinateX - 1).ToString() + ',' + coordinateY.ToString();
            string newCoordiantesXYAdd1 = coordinateX.ToString() + ',' + (coordinateY + 1).ToString();
            string newCoordiantesXYMinus1 = coordinateX.ToString() + ',' + (coordinateY - 1).ToString();

            if (!tileCoordinates.ContainsKey(newCoordiantesXAdd1Y)) { move = 4; }
            else if (!tileCoordinates.ContainsKey(newCoordiantesXMinus1Y)) { move = 3; }
            else if (!tileCoordinates.ContainsKey(newCoordiantesXYAdd1)) { move = 1; }
            else if (!tileCoordinates.ContainsKey(newCoordiantesXYMinus1)) { move = 2; }
            else
            {
                if (!stuckCoordinates.Contains(newCoordiantesXAdd1Y)) { move = 4; }
                else if (!stuckCoordinates.Contains(newCoordiantesXMinus1Y)) { move = 3; }
                else if (!stuckCoordinates.Contains(newCoordiantesXYAdd1)) { move = 1; }
                else { move = 2; }
                stuckCoordinates.Add(currentCoordinates);
            }
            return move;
        }
        private void MakeBoard()
        {
            //string[] coordinates;
            //int[] coordinatesInt;
            //smallestCoordinates = new int[] { -50, -50 };
            biggestCoordinates = new int[] { 50, 50 };

            //foreach (KeyValuePair<string, int> pair in tileCoordinates)
            //{
            //    coordinates = (pair.Key).Split(',');
            //    coordinatesInt = new int[] { int.Parse(coordinates[0]), int.Parse(coordinates[1]) };
            //    if (coordinatesInt[0] < smallestCoordinates[0]) { smallestCoordinates[0] = coordinatesInt[0]; }
            //    if (coordinatesInt[1] < smallestCoordinates[1]) { smallestCoordinates[1] = coordinatesInt[1]; }
            //    if (coordinatesInt[0] > biggestCoordinates[0]) { biggestCoordinates[0] = coordinatesInt[0]; }
            //    if (coordinatesInt[1] > biggestCoordinates[1]) { biggestCoordinates[1] = coordinatesInt[1]; }
            //}

            height = biggestCoordinates[1];
            width = biggestCoordinates[0];
            board = new string[width, height];
        }
        private bool EndProgram()
        {
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

    class Day15Code
    {
        //Actual
        static string puzzleInput = "3,1033,1008,1033,1,1032,1005,1032,31,1008,1033,2,1032,1005,1032,58,1008,1033,3,1032,1005,1032,81,1008,1033,4,1032,1005,1032,104,99,102,1,1034,1039,1001,1036,0,1041,1001,1035,-1,1040,1008,1038,0,1043,102,-1,1043,1032,1,1037,1032,1042,1106,0,124,1001,1034,0,1039,1001,1036,0,1041,1001,1035,1,1040,1008,1038,0,1043,1,1037,1038,1042,1106,0,124,1001,1034,-1,1039,1008,1036,0,1041,101,0,1035,1040,1002,1038,1,1043,1001,1037,0,1042,1105,1,124,1001,1034,1,1039,1008,1036,0,1041,101,0,1035,1040,102,1,1038,1043,101,0,1037,1042,1006,1039,217,1006,1040,217,1008,1039,40,1032,1005,1032,217,1008,1040,40,1032,1005,1032,217,1008,1039,5,1032,1006,1032,165,1008,1040,33,1032,1006,1032,165,1102,2,1,1044,1106,0,224,2,1041,1043,1032,1006,1032,179,1102,1,1,1044,1105,1,224,1,1041,1043,1032,1006,1032,217,1,1042,1043,1032,1001,1032,-1,1032,1002,1032,39,1032,1,1032,1039,1032,101,-1,1032,1032,101,252,1032,211,1007,0,44,1044,1106,0,224,1102,1,0,1044,1106,0,224,1006,1044,247,1002,1039,1,1034,1001,1040,0,1035,101,0,1041,1036,1002,1043,1,1038,1002,1042,1,1037,4,1044,1105,1,0,84,9,40,28,41,90,52,26,39,35,81,12,9,28,1,68,11,25,73,16,24,68,64,5,17,2,41,90,36,41,40,53,79,14,68,21,27,2,8,6,23,58,78,99,5,21,82,34,95,7,19,87,68,47,33,76,57,21,56,58,13,42,88,30,48,69,36,96,83,86,16,69,31,27,57,27,67,21,75,13,6,98,7,47,22,82,96,68,18,90,6,13,26,55,64,30,86,13,8,71,65,39,76,92,28,32,99,26,99,12,71,67,15,63,21,94,9,8,39,78,50,16,14,71,73,29,21,91,69,1,88,69,41,94,26,10,67,24,4,23,1,93,72,39,11,53,42,55,41,89,16,66,50,58,75,28,26,55,8,26,60,84,14,33,3,89,15,21,94,3,40,70,15,18,83,27,90,63,65,62,12,6,75,96,60,39,99,43,69,23,19,43,18,84,39,20,82,93,43,20,70,64,74,36,75,89,14,91,65,4,49,36,57,41,11,71,18,29,46,56,40,93,18,13,83,7,31,63,14,45,60,67,22,40,34,31,31,55,92,10,65,40,70,65,9,38,51,18,92,49,84,52,13,98,42,37,90,20,80,17,47,81,92,39,90,46,19,6,28,47,32,17,72,26,62,85,31,5,67,1,22,66,43,77,5,81,39,59,19,98,10,73,89,20,80,23,37,68,6,76,2,99,24,14,71,35,54,56,32,80,95,10,76,80,9,32,54,98,56,57,24,28,87,36,68,19,53,30,84,8,11,59,38,77,4,56,37,32,32,51,9,41,51,88,90,9,23,78,11,32,12,23,9,88,96,11,43,36,52,71,2,30,73,43,1,76,4,10,91,15,53,77,33,91,40,85,71,27,92,53,34,79,39,23,60,38,54,37,91,79,39,27,33,92,25,83,86,9,74,25,47,78,21,74,31,41,63,43,75,47,19,69,15,34,62,58,23,67,92,19,4,80,49,8,73,79,20,13,34,39,88,31,55,64,35,39,76,65,35,20,45,6,89,72,60,40,9,73,35,91,54,30,24,60,3,86,11,18,83,25,2,10,50,82,29,59,88,43,16,88,21,13,10,51,90,4,92,37,19,91,74,31,86,33,64,89,91,15,51,3,30,54,36,2,11,76,15,57,35,64,80,2,7,67,11,31,35,60,82,32,96,20,17,71,1,69,97,72,26,63,34,81,21,83,9,88,16,14,94,99,63,17,73,40,55,64,24,49,86,43,81,71,18,99,47,1,11,25,78,51,76,81,5,41,88,41,51,18,95,15,77,10,53,28,7,68,43,72,18,25,83,53,54,6,97,15,18,67,73,10,28,14,88,35,99,18,76,2,12,45,37,84,76,32,32,2,12,69,24,18,31,76,55,43,97,53,25,54,85,28,9,5,38,65,48,96,35,5,89,1,72,58,43,11,18,54,15,74,58,32,74,23,79,56,39,96,93,39,87,75,14,25,11,73,93,34,35,52,34,53,85,7,91,28,70,32,68,94,66,32,52,12,19,9,75,99,11,73,32,94,39,63,39,28,63,39,22,67,3,73,54,39,17,81,16,62,71,74,6,12,81,3,13,6,56,43,41,18,13,99,90,13,25,26,89,6,76,82,6,9,72,23,68,95,25,56,65,39,54,7,70,57,23,34,97,21,5,53,17,71,26,97,67,9,86,90,98,38,49,27,62,79,26,50,37,66,1,96,25,89,26,98,53,55,4,80,18,57,37,73,27,57,13,82,54,50,11,56,57,84,12,88,43,84,24,51,17,76,13,46,0,0,21,21,1,10,1,0,0,0,0,0,0";

        public void main()
        {
            IntCodeComputer computer = new IntCodeComputer(puzzleInput);
            computer.Run();
            Console.ReadLine();
        }
    }
}