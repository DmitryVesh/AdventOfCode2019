﻿using System;
using System.Collections.Generic;

namespace Day21
{
    //  6 registers : 
    //          4 (A, B, C, D) are for sensors, distances 1-2-3-4
    //          2 (T, J) T is temp register, J is jump register if true at the end of script, will jump 
    // Instructions : AND X Y, OR X Y, NOT X Y (Sets Y);
    // End instructions by : WALK + newline

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
        int currentWidth = 0;

        long hullDamage = 0;
        bool fallen = false;

        Queue<int> inputStream = new Queue<int>();
        int part;

        public IntCodeComputer(string puzzleInput, int initilisingPart)
        {
            part = initilisingPart;
            memory = puzzleInput.Split(',');
            int lengthOfCode = memory.Length;
            Array.Resize<string>(ref memory, lengthOfCode * 100);
            long lengthOfCodeNew = memory.Length;
            for (int count = 0; count < lengthOfCodeNew - 1; count++) { if (memory[count] == null) { memory[count] = "0"; } }
            string[] lines =

                (
                "NOT C J," +
                "AND D J," +
                "AND H J," +

                "NOT B T," +
                "AND D T," +
                "OR T J," +

                "NOT A T," +
                "OR T J," +

                "RUN"
                ).Split(',');

            List<List<int>> allFunctions = new List<List<int>>();
            foreach (string line in lines) { allFunctions.Add(ToAsciiList(line.Split())); }
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
            char[,] newBoard;
            if (asciiValue == 10) { widthOrHeight = false; }
            else { widthOrHeight = true; }

            if (widthOrHeight) { newBoard = new char[width + 1, height]; currentWidth += 1; }
            else { newBoard = new char[width, height + 1]; currentWidth = 0; }

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
                newBoard[currentWidth - 1, height - 1] = Convert.ToChar(asciiValue);
                if (currentWidth > width) { width += 1; }
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
        private long ReturnHullDamage()
        {
            return hullDamage;
        }
        private void OutputProcedure()
        {
            long parameter1 = GetValueFromMode(currentOpcodeIndex + 1, 2);
            if (parameter1 > 400) { hullDamage = parameter1; }
            else if (parameter1 == 64) { fallen = true; UpdateBoard(parameter1); }
            else { UpdateBoard(parameter1); }
            currentOpcodeIndex += 2;
            outputCount += 1;
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
                    if (characterCount > 1) { result.RemoveAt(result.Count - 1); result.Add(Convert.ToInt32(character)); result.Add(32); }
                    else { result.Add(Convert.ToInt32(character)); result.Add(32); }
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
            if (part == 1 && !fallen) { Console.WriteLine($"The amount of hull damage : {ReturnHullDamage()}"); }
            else { PrintBoard(); }
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

    class Day21Code
    {
        static string puzzleInput1 = "109,2050,21101,0,966,1,21101,0,13,0,1105,1,1378,21101,20,0,0,1106,0,1337,21102,1,27,0,1106,0,1279,1208,1,65,748,1005,748,73,1208,1,79,748,1005,748,110,1208,1,78,748,1005,748,132,1208,1,87,748,1005,748,169,1208,1,82,748,1005,748,239,21102,1,1041,1,21101,0,73,0,1105,1,1421,21102,78,1,1,21102,1,1041,2,21102,88,1,0,1106,0,1301,21101,0,68,1,21102,1041,1,2,21101,0,103,0,1106,0,1301,1102,1,1,750,1106,0,298,21102,1,82,1,21102,1041,1,2,21102,1,125,0,1105,1,1301,1101,0,2,750,1106,0,298,21101,79,0,1,21102,1041,1,2,21102,147,1,0,1105,1,1301,21102,1,84,1,21102,1,1041,2,21101,0,162,0,1106,0,1301,1102,3,1,750,1105,1,298,21101,0,65,1,21102,1041,1,2,21102,1,184,0,1106,0,1301,21102,1,76,1,21101,0,1041,2,21102,1,199,0,1105,1,1301,21101,75,0,1,21101,1041,0,2,21102,214,1,0,1106,0,1301,21102,221,1,0,1105,1,1337,21101,0,10,1,21101,0,1041,2,21102,236,1,0,1106,0,1301,1105,1,553,21102,85,1,1,21101,1041,0,2,21101,254,0,0,1105,1,1301,21102,78,1,1,21101,0,1041,2,21101,269,0,0,1106,0,1301,21101,276,0,0,1106,0,1337,21102,1,10,1,21101,0,1041,2,21101,0,291,0,1105,1,1301,1101,1,0,755,1105,1,553,21102,32,1,1,21102,1,1041,2,21102,313,1,0,1106,0,1301,21102,320,1,0,1106,0,1337,21101,0,327,0,1106,0,1279,2101,0,1,749,21102,1,65,2,21101,73,0,3,21102,346,1,0,1105,1,1889,1206,1,367,1007,749,69,748,1005,748,360,1101,1,0,756,1001,749,-64,751,1105,1,406,1008,749,74,748,1006,748,381,1101,-1,0,751,1106,0,406,1008,749,84,748,1006,748,395,1101,0,-2,751,1105,1,406,21102,1,1100,1,21102,1,406,0,1106,0,1421,21101,32,0,1,21101,0,1100,2,21101,0,421,0,1105,1,1301,21102,428,1,0,1106,0,1337,21102,1,435,0,1105,1,1279,1201,1,0,749,1008,749,74,748,1006,748,453,1101,0,-1,752,1106,0,478,1008,749,84,748,1006,748,467,1101,0,-2,752,1105,1,478,21101,1168,0,1,21101,0,478,0,1106,0,1421,21102,485,1,0,1105,1,1337,21101,10,0,1,21101,1168,0,2,21101,0,500,0,1105,1,1301,1007,920,15,748,1005,748,518,21102,1209,1,1,21101,518,0,0,1105,1,1421,1002,920,3,529,1001,529,921,529,1001,750,0,0,1001,529,1,537,102,1,751,0,1001,537,1,545,102,1,752,0,1001,920,1,920,1105,1,13,1005,755,577,1006,756,570,21102,1100,1,1,21102,570,1,0,1106,0,1421,21102,1,987,1,1106,0,581,21102,1,1001,1,21101,588,0,0,1105,1,1378,1101,0,758,594,101,0,0,753,1006,753,654,21002,753,1,1,21101,0,610,0,1106,0,667,21102,0,1,1,21102,1,621,0,1106,0,1463,1205,1,647,21102,1,1015,1,21102,635,1,0,1105,1,1378,21101,0,1,1,21101,0,646,0,1105,1,1463,99,1001,594,1,594,1106,0,592,1006,755,664,1101,0,0,755,1106,0,647,4,754,99,109,2,1102,1,726,757,22101,0,-1,1,21101,0,9,2,21102,1,697,3,21102,1,692,0,1106,0,1913,109,-2,2105,1,0,109,2,1002,757,1,706,1202,-1,1,0,1001,757,1,757,109,-2,2106,0,0,1,1,1,1,1,1,1,1,1,1,0,0,0,0,0,0,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,0,0,0,0,0,0,255,63,159,191,95,223,127,0,220,106,117,200,173,155,49,61,57,179,248,119,254,123,216,92,101,158,253,141,124,111,121,156,42,53,219,229,122,115,228,177,79,55,236,196,120,185,239,116,244,62,76,197,247,38,87,243,213,69,86,233,142,214,118,140,246,50,189,184,143,108,186,182,232,102,252,70,237,54,242,85,46,171,187,152,204,56,58,137,217,77,231,205,206,157,198,174,203,201,94,99,154,107,222,215,139,39,125,190,166,78,71,163,93,167,183,230,170,113,249,226,245,162,100,212,251,199,138,34,153,178,234,172,168,227,181,221,218,110,250,188,114,103,235,126,84,241,169,175,35,47,109,60,136,207,43,202,68,51,59,98,238,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,20,73,110,112,117,116,32,105,110,115,116,114,117,99,116,105,111,110,115,58,10,13,10,87,97,108,107,105,110,103,46,46,46,10,10,13,10,82,117,110,110,105,110,103,46,46,46,10,10,25,10,68,105,100,110,39,116,32,109,97,107,101,32,105,116,32,97,99,114,111,115,115,58,10,10,58,73,110,118,97,108,105,100,32,111,112,101,114,97,116,105,111,110,59,32,101,120,112,101,99,116,101,100,32,115,111,109,101,116,104,105,110,103,32,108,105,107,101,32,65,78,68,44,32,79,82,44,32,111,114,32,78,79,84,67,73,110,118,97,108,105,100,32,102,105,114,115,116,32,97,114,103,117,109,101,110,116,59,32,101,120,112,101,99,116,101,100,32,115,111,109,101,116,104,105,110,103,32,108,105,107,101,32,65,44,32,66,44,32,67,44,32,68,44,32,74,44,32,111,114,32,84,40,73,110,118,97,108,105,100,32,115,101,99,111,110,100,32,97,114,103,117,109,101,110,116,59,32,101,120,112,101,99,116,101,100,32,74,32,111,114,32,84,52,79,117,116,32,111,102,32,109,101,109,111,114,121,59,32,97,116,32,109,111,115,116,32,49,53,32,105,110,115,116,114,117,99,116,105,111,110,115,32,99,97,110,32,98,101,32,115,116,111,114,101,100,0,109,1,1005,1262,1270,3,1262,20101,0,1262,0,109,-1,2105,1,0,109,1,21101,1288,0,0,1106,0,1263,21002,1262,1,0,1102,1,0,1262,109,-1,2106,0,0,109,5,21102,1,1310,0,1105,1,1279,22101,0,1,-2,22208,-2,-4,-1,1205,-1,1332,22101,0,-3,1,21102,1332,1,0,1105,1,1421,109,-5,2105,1,0,109,2,21102,1,1346,0,1105,1,1263,21208,1,32,-1,1205,-1,1363,21208,1,9,-1,1205,-1,1363,1106,0,1373,21102,1,1370,0,1106,0,1279,1105,1,1339,109,-2,2105,1,0,109,5,1201,-4,0,1386,20102,1,0,-2,22101,1,-4,-4,21102,0,1,-3,22208,-3,-2,-1,1205,-1,1416,2201,-4,-3,1408,4,0,21201,-3,1,-3,1105,1,1396,109,-5,2106,0,0,109,2,104,10,22102,1,-1,1,21102,1436,1,0,1105,1,1378,104,10,99,109,-2,2106,0,0,109,3,20002,594,753,-1,22202,-1,-2,-1,201,-1,754,754,109,-3,2105,1,0,109,10,21101,5,0,-5,21102,1,1,-4,21101,0,0,-3,1206,-9,1555,21102,1,3,-6,21102,5,1,-7,22208,-7,-5,-8,1206,-8,1507,22208,-6,-4,-8,1206,-8,1507,104,64,1106,0,1529,1205,-6,1527,1201,-7,716,1515,21002,0,-11,-8,21201,-8,46,-8,204,-8,1105,1,1529,104,46,21201,-7,1,-7,21207,-7,22,-8,1205,-8,1488,104,10,21201,-6,-1,-6,21207,-6,0,-8,1206,-8,1484,104,10,21207,-4,1,-8,1206,-8,1569,21102,0,1,-9,1105,1,1689,21208,-5,21,-8,1206,-8,1583,21102,1,1,-9,1105,1,1689,1201,-5,716,1588,21002,0,1,-2,21208,-4,1,-1,22202,-2,-1,-1,1205,-2,1613,21201,-5,0,1,21102,1613,1,0,1106,0,1444,1206,-1,1634,22102,1,-5,1,21101,1627,0,0,1106,0,1694,1206,1,1634,21101,0,2,-3,22107,1,-4,-8,22201,-1,-8,-8,1206,-8,1649,21201,-5,1,-5,1206,-3,1663,21201,-3,-1,-3,21201,-4,1,-4,1105,1,1667,21201,-4,-1,-4,21208,-4,0,-1,1201,-5,716,1676,22002,0,-1,-1,1206,-1,1686,21101,0,1,-4,1105,1,1477,109,-10,2105,1,0,109,11,21102,0,1,-6,21102,1,0,-8,21102,1,0,-7,20208,-6,920,-9,1205,-9,1880,21202,-6,3,-9,1201,-9,921,1724,21002,0,1,-5,1001,1724,1,1732,21001,0,0,-4,21202,-4,1,1,21101,0,1,2,21101,9,0,3,21102,1,1754,0,1105,1,1889,1206,1,1772,2201,-10,-4,1766,1001,1766,716,1766,21001,0,0,-3,1105,1,1790,21208,-4,-1,-9,1206,-9,1786,22102,1,-8,-3,1106,0,1790,21201,-7,0,-3,1001,1732,1,1796,20102,1,0,-2,21208,-2,-1,-9,1206,-9,1812,22102,1,-8,-1,1105,1,1816,22102,1,-7,-1,21208,-5,1,-9,1205,-9,1837,21208,-5,2,-9,1205,-9,1844,21208,-3,0,-1,1105,1,1855,22202,-3,-1,-1,1106,0,1855,22201,-3,-1,-1,22107,0,-1,-1,1106,0,1855,21208,-2,-1,-9,1206,-9,1869,21201,-1,0,-8,1106,0,1873,21201,-1,0,-7,21201,-6,1,-6,1106,0,1708,22101,0,-8,-10,109,-11,2105,1,0,109,7,22207,-6,-5,-3,22207,-4,-6,-2,22201,-3,-2,-1,21208,-1,0,-6,109,-7,2106,0,0,0,109,5,1201,-2,0,1912,21207,-4,0,-1,1206,-1,1930,21102,0,1,-4,21202,-4,1,1,22102,1,-3,2,21101,1,0,3,21102,1,1949,0,1105,1,1954,109,-5,2106,0,0,109,6,21207,-4,1,-1,1206,-1,1977,22207,-5,-3,-1,1206,-1,1977,22101,0,-5,-5,1105,1,2045,22101,0,-5,1,21201,-4,-1,2,21202,-3,2,3,21102,1996,1,0,1106,0,1954,21201,1,0,-5,21102,1,1,-2,22207,-5,-3,-1,1206,-1,2015,21102,1,0,-2,22202,-3,-2,-3,22107,0,-4,-1,1206,-1,2037,22101,0,-2,1,21101,2037,0,0,105,1,1912,21202,-3,-1,-3,22201,-5,-3,-5,109,-6,2105,1,0";
        public void main()
        {
            IntCodeComputer computer;
            computer = new IntCodeComputer(puzzleInput1, 1);
            computer.Run();
            Console.ReadLine();
        }
    }
}