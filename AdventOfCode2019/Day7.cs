using System;
using System.Collections.Generic;


namespace Day7
{
    class Day7Code
    {
        static int[] AmpsLastIndex = { 0, 0, 0, 0, 0 };
        static bool[] AmpsEnd = { false, false, false, false, false };
        static string[] AmpsCode = new string[5];

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

            int final;
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
                if (first == second) { originalCode[saveIndex] = "1"; }
                else { originalCode[saveIndex] = "0"; }
            }
            return originalCode;
        }

        public static string ReturnSomeValue(string input1, string input2, int AmpNum)
        {
            string line = AmpsCode[AmpNum];
            int currentOpcodeIndex = AmpsLastIndex[AmpNum];
            line = line.Substring(1);
            string output = "";
            int inputCounter = 0;
            if (AmpsLastIndex[AmpNum] != 0) { inputCounter += 1; }

            string[] originalCode = line.Split(',');

            string opcode;
            string opcodeInstruction;
            int opcodeLen;

            string[] instructionSet = { "01", "02", "03", "04", "99", "05", "06", "07", "08" };
            int instructionIndex;

            bool end = false;

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
                    string input;
                    if (inputCounter == 0) { input = input1; }
                    else { input = input2; }
                    originalCode[int.Parse(originalCode[currentOpcodeIndex + 1])] = input;
                    currentOpcodeIndex += 2;
                    inputCounter += 1;
                }

                // 04 Outputs value at some address
                else if (instructionIndex == 3)
                {
                    output = originalCode[int.Parse(originalCode[currentOpcodeIndex + 1])];
                    currentOpcodeIndex += 2;
                    AmpsLastIndex[AmpNum] = currentOpcodeIndex;
                    string outputLine = "";
                    foreach (string element in originalCode)
                    {
                        outputLine += "," + element;
                    }
                    AmpsCode[AmpNum] = outputLine;
                    return output;
                }

                // 99 Halts the program
                else if (instructionIndex == 4) { AmpsEnd[AmpNum] = true; return output; }

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

            return output;
        }

        public void main()
        {
            // Actual
            string line = ",3,8,1001,8,10,8,105,1,0,0,21,38,47,64,89,110,191,272,353,434,99999,3,9,101,4,9,9,102,3,9,9,101,5,9,9,4,9,99,3,9,1002,9,5,9,4,9,99,3,9,101,2,9,9,102,5,9,9,1001,9,5,9,4,9,99,3,9,1001,9,5,9,102,4,9,9,1001,9,5,9,1002,9,2,9,1001,9,3,9,4,9,99,3,9,102,2,9,9,101,4,9,9,1002,9,4,9,1001,9,4,9,4,9,99,3,9,101,1,9,9,4,9,3,9,101,1,9,9,4,9,3,9,1002,9,2,9,4,9,3,9,102,2,9,9,4,9,3,9,101,2,9,9,4,9,3,9,101,1,9,9,4,9,3,9,1001,9,2,9,4,9,3,9,102,2,9,9,4,9,3,9,1001,9,1,9,4,9,3,9,101,2,9,9,4,9,99,3,9,101,2,9,9,4,9,3,9,1002,9,2,9,4,9,3,9,102,2,9,9,4,9,3,9,101,2,9,9,4,9,3,9,1002,9,2,9,4,9,3,9,101,2,9,9,4,9,3,9,1002,9,2,9,4,9,3,9,101,2,9,9,4,9,3,9,1001,9,2,9,4,9,3,9,102,2,9,9,4,9,99,3,9,1001,9,2,9,4,9,3,9,1001,9,2,9,4,9,3,9,101,1,9,9,4,9,3,9,1001,9,1,9,4,9,3,9,1001,9,1,9,4,9,3,9,1002,9,2,9,4,9,3,9,102,2,9,9,4,9,3,9,1002,9,2,9,4,9,3,9,101,1,9,9,4,9,3,9,101,1,9,9,4,9,99,3,9,102,2,9,9,4,9,3,9,1001,9,1,9,4,9,3,9,1001,9,1,9,4,9,3,9,1002,9,2,9,4,9,3,9,102,2,9,9,4,9,3,9,1001,9,1,9,4,9,3,9,1001,9,2,9,4,9,3,9,102,2,9,9,4,9,3,9,1001,9,1,9,4,9,3,9,1002,9,2,9,4,9,99,3,9,101,1,9,9,4,9,3,9,102,2,9,9,4,9,3,9,1001,9,2,9,4,9,3,9,1001,9,2,9,4,9,3,9,102,2,9,9,4,9,3,9,102,2,9,9,4,9,3,9,1001,9,2,9,4,9,3,9,1002,9,2,9,4,9,3,9,1002,9,2,9,4,9,3,9,1002,9,2,9,4,9,99";

            // Example 1
            //string line = ",3,26,1001,26,-4,26,3,27,1002,27,2,27,1,27,26,27,4,27,1001,28,-1,28,1005,28,6,99,0,0,5";

            // Exampl 2
            //string line = ",3,52,1001,52,-5,52,3,53,1,52,56,54,1007,54,5,55,1005,55,26,1001,54,-5,54,1105,1,12,1,53,54,53,1008,54,0,55,1001,55,1,55,2,53,55,53,4,53,1001,56,-1,56,1005,56,6,99,0,0,0,0,10";
            for (int count = 0; count < 5; count++) { AmpsCode[count] = line; }

            int bestThruster = 0;
            int[] bestSetting = new int[5];

            for (int count1 = 5; count1 < 10; count1++)
            {
                for (int count2 = 5; count2 < 10; count2++)
                {
                    if (count1 == count2) { continue; }
                    for (int count3 = 5; count3 < 10; count3++)
                    {
                        if (count3 == count2) { continue; }
                        else if (count3 == count1) { continue; }
                        for (int count4 = 5; count4 < 10; count4++)
                        {
                            if (count4 == count3) { continue; }
                            else if (count4 == count2) { continue; }
                            else if (count4 == count1) { continue; }
                            for (int count5 = 5; count5 < 10; count5++)
                            {
                                if (count5 == count4) { continue; }
                                else if (count5 == count3) { continue; }
                                else if (count5 == count2) { continue; }
                                else if (count5 == count1) { continue; }

                                Console.WriteLine($"Order {count5}, {count4}, {count3}, {count2}, {count1}");

                                bool end = false;
                                string Amp1, Amp2, Amp3, Amp4, Amp5; ;
                                List<string> LastAmp5Values = new List<string>();

                                Amp1 = ReturnSomeValue(count5.ToString(), "0", 0);
                                while (!end)
                                {
                                    Amp2 = ReturnSomeValue(count4.ToString(), Amp1, 1);
                                    Amp3 = ReturnSomeValue(count3.ToString(), Amp2, 2);
                                    Amp4 = ReturnSomeValue(count2.ToString(), Amp3, 3);
                                    Amp5 = ReturnSomeValue(count1.ToString(), Amp4, 4);
                                    Console.WriteLine($"Amp1 {Amp1}, Amp2 {Amp2}, Amp3 {Amp3}, Amp4 {Amp4}, Amp5 {Amp5}");
                                    if (Amp5 != "") { LastAmp5Values.Add(Amp5); }
                                    if (AmpsEnd[4] == true) { break; }
                                    else { Amp1 = ReturnSomeValue(count5.ToString(), Amp5, 0); }

                                }
                                Amp5 = LastAmp5Values[LastAmp5Values.Count - 1];
                                Console.WriteLine($" Last value : ({Amp5})");
                                Console.WriteLine();
                                int Amp5Int = int.Parse(Amp5);
                                if (Amp5Int > bestThruster)
                                {
                                    bestThruster = Amp5Int;
                                    bestSetting[0] = count5;
                                    bestSetting[1] = count4;
                                    bestSetting[2] = count3;
                                    bestSetting[3] = count2;
                                    bestSetting[4] = count1;
                                }
                                for (int count = 0; count < 5; count++) { AmpsCode[count] = line; AmpsEnd[count] = false; AmpsLastIndex[count] = 0; }

                            }
                        }
                    }
                }
            }

            Console.WriteLine($"The best output value is : ({bestThruster})");
            foreach (int element in bestSetting) { Console.Write($"{element},"); }
        }
    }
}