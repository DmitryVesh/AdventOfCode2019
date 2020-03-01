using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace Day5
{
    class Day5Code
    {
        public static string[] ReturnCodeAfterInstruction(string mode, string opcode, string parameter1, string parameter2, string parameter3, int parameter3Index, string[] originalCode)
        {

            int first = 0;
            if (opcode[2] == '0') { first = int.Parse(originalCode[int.Parse(parameter1)]); }
            else if (opcode[2] == '1') { first = int.Parse(parameter1); }

            int second = 0;
            if (opcode[1] == '0') { second = int.Parse(originalCode[int.Parse(parameter2)]); }
            else if (opcode[1] == '1') { second = int.Parse(parameter2); }

            int saveIndex = 0;
            if (opcode[0] == '0') { saveIndex = int.Parse(parameter3); }
            else if (opcode[0] == '1') { saveIndex = parameter3Index; }

            int final = 0;
            if (mode == "Add") { final = first + second; originalCode[saveIndex] = final.ToString(); }
            else if (mode == "Multiply") { final = first * second; originalCode[saveIndex] = final.ToString(); }
            else if (mode == "Jump-if-true")
            {
                string[] currentIndex = { "" };
                if (first != 0) { currentIndex[0] = second.ToString(); }
                else if (first == 0) { currentIndex[0] = (parameter3Index - 3).ToString(); }
                return currentIndex;
            }
            else if (mode == "Jump-if-false")
            {
                string[] currentIndex = { "" };
                if (first == 0) { currentIndex[0] = second.ToString(); }
                else if (first != 0) { currentIndex[0] = (parameter3Index - 3).ToString(); }
                return currentIndex;
            }
            else if (mode == "less-than")
            {
                if (first < second) { originalCode[saveIndex] = "1"; }
                else { originalCode[saveIndex] = "0"; }
            }
            else if (mode == "equals")
            {
                Console.WriteLine("The first : {0}, the second {1}", first, second);
                if (first == second) { originalCode[saveIndex] = "1"; }
                else { originalCode[saveIndex] = "0"; }
            }
            return originalCode;
        }

        public void main()
        {
            string line = "3,225,1,225,6,6,1100,1,238,225,104,0,1102,40,93,224,1001,224,-3720,224,4,224,102,8,223,223,101,3,224,224,1,224,223,223,1101,56,23,225,1102,64,78,225,1102,14,11,225,1101,84,27,225,1101,7,82,224,1001,224,-89,224,4,224,1002,223,8,223,1001,224,1,224,1,224,223,223,1,35,47,224,1001,224,-140,224,4,224,1002,223,8,223,101,5,224,224,1,224,223,223,1101,75,90,225,101,9,122,224,101,-72,224,224,4,224,1002,223,8,223,101,6,224,224,1,224,223,223,1102,36,63,225,1002,192,29,224,1001,224,-1218,224,4,224,1002,223,8,223,1001,224,7,224,1,223,224,223,102,31,218,224,101,-2046,224,224,4,224,102,8,223,223,101,4,224,224,1,224,223,223,1001,43,38,224,101,-52,224,224,4,224,1002,223,8,223,101,5,224,224,1,223,224,223,1102,33,42,225,2,95,40,224,101,-5850,224,224,4,224,1002,223,8,223,1001,224,7,224,1,224,223,223,1102,37,66,225,4,223,99,0,0,0,677,0,0,0,0,0,0,0,0,0,0,0,1105,0,99999,1105,227,247,1105,1,99999,1005,227,99999,1005,0,256,1105,1,99999,1106,227,99999,1106,0,265,1105,1,99999,1006,0,99999,1006,227,274,1105,1,99999,1105,1,280,1105,1,99999,1,225,225,225,1101,294,0,0,105,1,0,1105,1,99999,1106,0,300,1105,1,99999,1,225,225,225,1101,314,0,0,106,0,0,1105,1,99999,1007,226,677,224,1002,223,2,223,1005,224,329,1001,223,1,223,1007,226,226,224,1002,223,2,223,1006,224,344,101,1,223,223,1107,677,226,224,102,2,223,223,1006,224,359,1001,223,1,223,108,677,677,224,1002,223,2,223,1006,224,374,1001,223,1,223,107,677,677,224,1002,223,2,223,1005,224,389,101,1,223,223,8,677,677,224,1002,223,2,223,1005,224,404,1001,223,1,223,108,226,226,224,1002,223,2,223,1005,224,419,101,1,223,223,1008,677,677,224,1002,223,2,223,1005,224,434,101,1,223,223,1008,226,226,224,1002,223,2,223,1005,224,449,101,1,223,223,7,677,226,224,1002,223,2,223,1006,224,464,1001,223,1,223,7,226,226,224,1002,223,2,223,1005,224,479,1001,223,1,223,1007,677,677,224,102,2,223,223,1005,224,494,101,1,223,223,1108,677,226,224,102,2,223,223,1006,224,509,1001,223,1,223,8,677,226,224,102,2,223,223,1005,224,524,1001,223,1,223,1107,226,226,224,102,2,223,223,1006,224,539,1001,223,1,223,1008,226,677,224,1002,223,2,223,1006,224,554,1001,223,1,223,1107,226,677,224,1002,223,2,223,1006,224,569,1001,223,1,223,1108,677,677,224,102,2,223,223,1005,224,584,101,1,223,223,7,226,677,224,102,2,223,223,1006,224,599,1001,223,1,223,1108,226,677,224,102,2,223,223,1006,224,614,101,1,223,223,107,226,677,224,1002,223,2,223,1005,224,629,101,1,223,223,108,226,677,224,1002,223,2,223,1005,224,644,101,1,223,223,8,226,677,224,1002,223,2,223,1005,224,659,1001,223,1,223,107,226,226,224,1002,223,2,223,1006,224,674,101,1,223,223,4,223,99,226";
            //string line = "3,9,8,9,10,9,4,9,99,-1,8";
            string[] originalCode = line.Split(',');

            string opcode;
            string opcodeInstruction;
            int opcodeLen;

            string[] instructionSet = { "01", "02", "03", "04", "99", "05", "06", "07", "08" };
            int instructionIndex;

            bool end = false;
            int currentOpcodeIndex = 0;

            while (!end)
            {
                opcode = originalCode[currentOpcodeIndex];
                opcodeLen = opcode.Length;

                if (opcodeLen < 5) { for (int count = opcodeLen; count < 5; count++) { opcode = "0" + opcode; } }
                opcodeInstruction = opcode.Substring(3);
                instructionIndex = -99;
                for (int count = 0; count < 9; count++) { if (instructionSet[count] == opcodeInstruction) { instructionIndex = count; break; } }

                // 01 Add numbers together
                if (instructionIndex == 0)
                {
                    originalCode = ReturnCodeAfterInstruction("Add", opcode, originalCode[currentOpcodeIndex + 1], originalCode[currentOpcodeIndex + 2], originalCode[currentOpcodeIndex + 3], currentOpcodeIndex + 3, originalCode);
                    currentOpcodeIndex += 4;
                }

                // 02 Multiply numbers together
                else if (instructionIndex == 1)
                {
                    originalCode = ReturnCodeAfterInstruction("Multiply", opcode, originalCode[currentOpcodeIndex + 1], originalCode[currentOpcodeIndex + 2], originalCode[currentOpcodeIndex + 3], currentOpcodeIndex + 3, originalCode);
                    currentOpcodeIndex += 4;
                }

                // 03 Takes input and saves at some address
                else if (instructionIndex == 2)
                {
                    Console.Write("Enter input : ");
                    string input = Console.ReadLine();
                    Console.WriteLine();
                    originalCode[int.Parse(originalCode[currentOpcodeIndex + 1])] = input;
                    currentOpcodeIndex += 2;
                }

                // 04 Outputs value at some address
                else if (instructionIndex == 3)
                {
                    Console.WriteLine(originalCode[int.Parse(originalCode[currentOpcodeIndex + 1])]);
                    currentOpcodeIndex += 2;
                }

                // 99 Halts the program
                else if (instructionIndex == 4) { end = true; Console.WriteLine("End"); }

                // 05 Jump-if-true
                else if (instructionIndex == 5)
                {
                    string[] indexHolder = ReturnCodeAfterInstruction("Jump-if-true", opcode, originalCode[currentOpcodeIndex + 1], originalCode[currentOpcodeIndex + 2], originalCode[currentOpcodeIndex + 3], currentOpcodeIndex + 3, originalCode);
                    int indexTemp = int.Parse(indexHolder[0]);
                    if (indexTemp == currentOpcodeIndex) { currentOpcodeIndex += 3; }
                    else { currentOpcodeIndex = indexTemp; }
                }

                // 06 jump-if-false
                else if (instructionIndex == 6)
                {
                    string[] indexHolder = ReturnCodeAfterInstruction("Jump-if-false", opcode, originalCode[currentOpcodeIndex + 1], originalCode[currentOpcodeIndex + 2], originalCode[currentOpcodeIndex + 3], currentOpcodeIndex + 3, originalCode);
                    int indexTemp = int.Parse(indexHolder[0]);
                    if (indexTemp == currentOpcodeIndex) { currentOpcodeIndex += 3; }
                    else { currentOpcodeIndex = indexTemp; }
                }

                // 07 first parameter is less than second parameter, stores 1 in address if true, if false stores 0 at address 
                else if (instructionIndex == 7)
                {
                    originalCode = ReturnCodeAfterInstruction("less-than", opcode, originalCode[currentOpcodeIndex + 1], originalCode[currentOpcodeIndex + 2], originalCode[currentOpcodeIndex + 3], currentOpcodeIndex + 3, originalCode);
                    currentOpcodeIndex += 4;
                }

                // 08 first parameter is equal to second parameter, stores 1 in address if true, if false stores 0 at address
                else if (instructionIndex == 8)
                {
                    originalCode = ReturnCodeAfterInstruction("equals", opcode, originalCode[currentOpcodeIndex + 1], originalCode[currentOpcodeIndex + 2], originalCode[currentOpcodeIndex + 3], currentOpcodeIndex + 3, originalCode);
                    currentOpcodeIndex += 4;
                }

                // Incorrect instructionIndex error
                else { throw new IndexOutOfRangeException(); }
            }
        }
    }
}
