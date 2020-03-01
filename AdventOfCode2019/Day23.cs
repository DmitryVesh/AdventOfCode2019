using System;
using System.Collections.Generic;

namespace Day23
{
    // 50 Computers, send packets to each other
    // Each computer runs NIC (puzzleInput)
    // Each computer has a network address, 0 to 49 = 50 total
    // First input instruction asks for its network address
    // After, each computer will run by sending and receiving packets
    // Packets contain 2 values, X & Y
    // Packets sent to a computer are queued and read in order received
    // To send packet, 3 output instructions used, network address, then X and Y
    // To receive packets, will use an input instruction, if queue is empty given -1
    // If queue is not empty, first input instruction is X, then Y


    class IntCodeComputer
    {
        public long relativeBase = 0;
        public long currentOpcodeIndex = 0;
        public string[] memory;

        public int outputCount;
        public int inputCount;
        public string[] output;
        int networkAddress;

        string opcode;

        public IntCodeComputer(string puzzleInput, int address)
        {
            networkAddress = address;
            output = new string[3];
            outputCount = 0; inputCount = 0;
            memory = puzzleInput.Split(',');
            int lengthOfCode = memory.Length;
            Array.Resize<string>(ref memory, lengthOfCode * 100);
            long lengthOfCodeNew = memory.Length;
            for (int count = 0; count < lengthOfCodeNew - 1; count++) { if (memory[count] == null) { memory[count] = "0"; } }
        }
        public string Run(string input)
        {
            int opcodeLen;
            string opcodeInstruction;

            while (true)
            {
                opcode = memory[currentOpcodeIndex];
                opcodeLen = opcode.Length;
                for (int count = opcodeLen; count < 5; count++) { opcode = "0" + opcode; }
                opcodeInstruction = opcode.Substring(3);

                if (opcodeInstruction == "01") { MathProcedure("Add"); }
                else if (opcodeInstruction == "02") { MathProcedure("Multiply"); }
                else if (opcodeInstruction == "03") { InputProcedure(input); break; }
                else if (opcodeInstruction == "04") { return OutputProcedure(); }
                else if (opcodeInstruction == "05") { JumpProcedure(true); }
                else if (opcodeInstruction == "06") { JumpProcedure(false); }
                else if (opcodeInstruction == "07") { MathProcedure("Less-than"); }
                else if (opcodeInstruction == "08") { MathProcedure("Equals"); }
                else if (opcodeInstruction == "09") { RelativeBaseProcedure(); }
                else if (opcodeInstruction == "99") { return "END"; }
                else { throw new Exception("Error in (Run) - (unknown opcode)"); }
            }

            return null;
        }

        private string OutputProcedure()
        {
            long parameter1 = GetValueFromMode(currentOpcodeIndex + 1, 2);
            currentOpcodeIndex += 2;
            outputCount += 1;
            return parameter1.ToString();
        }
        private void InputProcedure(string input)
        {
            if (inputCount == 0 && outputCount == 0) { memory[GetAddressFromMode(currentOpcodeIndex + 1, 2)] = networkAddress.ToString(); }
            else { memory[GetAddressFromMode(currentOpcodeIndex + 1, 2)] = input; }
            currentOpcodeIndex += 2;
            inputCount += 1;
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
                else { throw new Exception("Error in (JumpProcedure) - ()"); }
            }
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

    class Day23Code
    {
        static string puzzleInput = "3,62,1001,62,11,10,109,2257,105,1,0,1119,1088,1678,1851,2222,1647,606,1707,785,1445,1220,1886,1160,818,1546,882,1820,1950,2055,1779,1408,1919,1987,571,1476,985,709,2088,680,1515,2191,647,1348,2158,950,1191,742,1744,1317,1610,1286,2018,851,1016,919,1249,1377,2117,1579,1051,0,0,0,0,0,0,0,0,0,0,0,0,3,64,1008,64,-1,62,1006,62,88,1006,61,170,1106,0,73,3,65,21001,64,0,1,20102,1,66,2,21102,1,105,0,1106,0,436,1201,1,-1,64,1007,64,0,62,1005,62,73,7,64,67,62,1006,62,73,1002,64,2,133,1,133,68,133,102,1,0,62,1001,133,1,140,8,0,65,63,2,63,62,62,1005,62,73,1002,64,2,161,1,161,68,161,1101,1,0,0,1001,161,1,169,1002,65,1,0,1101,0,1,61,1101,0,0,63,7,63,67,62,1006,62,203,1002,63,2,194,1,68,194,194,1006,0,73,1001,63,1,63,1106,0,178,21102,210,1,0,105,1,69,1202,1,1,70,1101,0,0,63,7,63,71,62,1006,62,250,1002,63,2,234,1,72,234,234,4,0,101,1,234,240,4,0,4,70,1001,63,1,63,1106,0,218,1106,0,73,109,4,21102,1,0,-3,21102,0,1,-2,20207,-2,67,-1,1206,-1,293,1202,-2,2,283,101,1,283,283,1,68,283,283,22001,0,-3,-3,21201,-2,1,-2,1106,0,263,22101,0,-3,-3,109,-4,2105,1,0,109,4,21102,1,1,-3,21101,0,0,-2,20207,-2,67,-1,1206,-1,342,1202,-2,2,332,101,1,332,332,1,68,332,332,22002,0,-3,-3,21201,-2,1,-2,1106,0,312,22101,0,-3,-3,109,-4,2106,0,0,109,1,101,1,68,358,21001,0,0,1,101,3,68,366,21002,0,1,2,21102,1,376,0,1105,1,436,22102,1,1,0,109,-1,2105,1,0,1,2,4,8,16,32,64,128,256,512,1024,2048,4096,8192,16384,32768,65536,131072,262144,524288,1048576,2097152,4194304,8388608,16777216,33554432,67108864,134217728,268435456,536870912,1073741824,2147483648,4294967296,8589934592,17179869184,34359738368,68719476736,137438953472,274877906944,549755813888,1099511627776,2199023255552,4398046511104,8796093022208,17592186044416,35184372088832,70368744177664,140737488355328,281474976710656,562949953421312,1125899906842624,109,8,21202,-6,10,-5,22207,-7,-5,-5,1205,-5,521,21102,0,1,-4,21102,0,1,-3,21102,1,51,-2,21201,-2,-1,-2,1201,-2,385,471,20102,1,0,-1,21202,-3,2,-3,22207,-7,-1,-5,1205,-5,496,21201,-3,1,-3,22102,-1,-1,-5,22201,-7,-5,-7,22207,-3,-6,-5,1205,-5,515,22102,-1,-6,-5,22201,-3,-5,-3,22201,-1,-4,-4,1205,-2,461,1106,0,547,21101,0,-1,-4,21202,-6,-1,-6,21207,-7,0,-5,1205,-5,547,22201,-7,-6,-7,21201,-4,1,-4,1106,0,529,21202,-4,1,-7,109,-8,2106,0,0,109,1,101,1,68,563,21002,0,1,0,109,-1,2106,0,0,1101,50263,0,66,1101,0,3,67,1102,1,598,68,1102,302,1,69,1102,1,1,71,1102,604,1,72,1106,0,73,0,0,0,0,0,0,6,195758,1102,1,97879,66,1102,1,6,67,1102,633,1,68,1102,253,1,69,1102,1,1,71,1102,645,1,72,1105,1,73,0,0,0,0,0,0,0,0,0,0,0,0,11,35974,1101,34897,0,66,1102,1,1,67,1101,0,674,68,1101,556,0,69,1102,1,2,71,1101,676,0,72,1105,1,73,1,11,24,161373,20,297753,1102,1,55763,66,1102,1,1,67,1101,0,707,68,1101,556,0,69,1101,0,0,71,1101,0,709,72,1106,0,73,1,1753,1101,6389,0,66,1101,0,2,67,1101,0,736,68,1102,1,351,69,1102,1,1,71,1102,740,1,72,1106,0,73,0,0,0,0,255,15461,1102,1,353,66,1102,1,1,67,1101,769,0,68,1102,556,1,69,1102,7,1,71,1101,771,0,72,1105,1,73,1,1,49,48779,23,50263,45,209548,37,172358,24,107582,7,186562,4,47798,1102,1,68171,66,1102,1,2,67,1102,1,812,68,1102,1,302,69,1101,0,1,71,1101,0,816,72,1106,0,73,0,0,0,0,39,299361,1101,82963,0,66,1101,0,1,67,1102,1,845,68,1102,556,1,69,1102,2,1,71,1101,0,847,72,1105,1,73,1,10,15,100379,47,12377,1102,1,24391,66,1102,1,1,67,1102,1,878,68,1102,556,1,69,1101,0,1,71,1101,880,0,72,1105,1,73,1,21,24,53791,1101,100379,0,66,1102,4,1,67,1101,0,909,68,1101,0,302,69,1102,1,1,71,1101,917,0,72,1105,1,73,0,0,0,0,0,0,0,0,47,61885,1102,92363,1,66,1101,0,1,67,1102,946,1,68,1101,556,0,69,1102,1,1,71,1101,0,948,72,1106,0,73,1,160,47,37131,1102,1,90089,66,1101,3,0,67,1101,0,977,68,1102,302,1,69,1101,0,1,71,1102,1,983,72,1105,1,73,0,0,0,0,0,0,39,399148,1102,57557,1,66,1102,1,1,67,1101,1012,0,68,1101,0,556,69,1102,1,1,71,1101,1014,0,72,1106,0,73,1,733,37,86179,1102,72977,1,66,1101,0,1,67,1101,1043,0,68,1102,556,1,69,1101,0,3,71,1101,0,1045,72,1106,0,73,1,5,15,301137,15,401516,47,49508,1101,0,48779,66,1102,4,1,67,1101,0,1078,68,1102,302,1,69,1102,1,1,71,1101,0,1086,72,1106,0,73,0,0,0,0,0,0,0,0,6,587274,1102,99881,1,66,1102,1,1,67,1101,0,1115,68,1101,0,556,69,1102,1,1,71,1102,1,1117,72,1105,1,73,1,-95,37,258537,1102,15461,1,66,1101,0,1,67,1102,1146,1,68,1102,1,556,69,1102,1,6,71,1101,1148,0,72,1105,1,73,1,23892,8,136342,3,182738,3,274107,34,90089,34,180178,34,270267,1102,93253,1,66,1102,1,1,67,1101,0,1187,68,1101,556,0,69,1102,1,1,71,1102,1189,1,72,1105,1,73,1,569,23,100526,1102,1,37997,66,1102,1,1,67,1102,1218,1,68,1101,556,0,69,1102,1,0,71,1102,1220,1,72,1106,0,73,1,1682,1101,0,68399,66,1102,1,1,67,1102,1247,1,68,1102,556,1,69,1102,1,0,71,1101,0,1249,72,1105,1,73,1,1850,1102,52387,1,66,1101,4,0,67,1102,1,1276,68,1102,1,302,69,1102,1,1,71,1102,1,1284,72,1106,0,73,0,0,0,0,0,0,0,0,6,489395,1102,60659,1,66,1102,1,1,67,1102,1,1313,68,1102,1,556,69,1101,1,0,71,1102,1315,1,72,1105,1,73,1,293,7,93281,1101,0,85853,66,1102,1,1,67,1101,0,1344,68,1101,556,0,69,1102,1,1,71,1102,1,1346,72,1105,1,73,1,-183,17,378236,1102,1,97987,66,1102,1,1,67,1101,0,1375,68,1101,556,0,69,1101,0,0,71,1101,1377,0,72,1105,1,73,1,1347,1101,26371,0,66,1101,0,1,67,1101,0,1404,68,1101,0,556,69,1102,1,1,71,1102,1406,1,72,1105,1,73,1,-27,4,71697,1101,0,99251,66,1101,4,0,67,1101,0,1435,68,1102,302,1,69,1101,1,0,71,1101,0,1443,72,1106,0,73,0,0,0,0,0,0,0,0,39,199574,1101,40973,0,66,1102,1,1,67,1102,1,1472,68,1101,0,556,69,1101,1,0,71,1102,1474,1,72,1106,0,73,1,125,15,200758,1102,1,53791,66,1102,5,1,67,1101,0,1503,68,1102,302,1,69,1102,1,1,71,1101,0,1513,72,1105,1,73,0,0,0,0,0,0,0,0,0,0,6,391516,1101,0,31277,66,1102,1,1,67,1101,0,1542,68,1101,556,0,69,1102,1,1,71,1101,0,1544,72,1106,0,73,1,128,23,150789,1101,77291,0,66,1101,1,0,67,1102,1573,1,68,1102,556,1,69,1101,0,2,71,1101,0,1575,72,1106,0,73,1,191,49,97558,49,195116,1102,1,91753,66,1102,1,1,67,1101,0,1606,68,1102,1,556,69,1102,1,1,71,1102,1,1608,72,1106,0,73,1,61,17,94559,1102,99787,1,66,1102,4,1,67,1102,1637,1,68,1101,0,253,69,1102,1,1,71,1101,0,1645,72,1105,1,73,0,0,0,0,0,0,0,0,26,6389,1102,8081,1,66,1101,1,0,67,1101,0,1674,68,1102,556,1,69,1102,1,1,71,1101,1676,0,72,1105,1,73,1,8929,7,279843,1101,0,43997,66,1102,1,1,67,1102,1705,1,68,1101,0,556,69,1102,0,1,71,1101,0,1707,72,1106,0,73,1,1165,1102,1,93281,66,1102,4,1,67,1102,1,1734,68,1101,0,302,69,1102,1,1,71,1102,1,1742,72,1105,1,73,0,0,0,0,0,0,0,0,8,68171,1102,1,86179,66,1101,3,0,67,1102,1771,1,68,1101,302,0,69,1101,0,1,71,1102,1,1777,72,1105,1,73,0,0,0,0,0,0,6,97879,1101,14057,0,66,1102,1,1,67,1101,0,1806,68,1102,1,556,69,1101,0,6,71,1102,1808,1,72,1106,0,73,1,3,49,146337,45,52387,24,215164,11,17987,18,53974,20,99251,1102,1,69389,66,1102,1,1,67,1101,0,1847,68,1102,556,1,69,1102,1,1,71,1101,0,1849,72,1106,0,73,1,109,45,104774,1102,91369,1,66,1101,0,3,67,1102,1878,1,68,1101,302,0,69,1102,1,1,71,1101,1884,0,72,1105,1,73,0,0,0,0,0,0,39,99787,1101,0,17987,66,1101,0,2,67,1101,1913,0,68,1101,302,0,69,1102,1,1,71,1101,0,1917,72,1106,0,73,0,0,0,0,18,26987,1102,90173,1,66,1102,1,1,67,1102,1,1946,68,1101,0,556,69,1102,1,1,71,1101,0,1948,72,1106,0,73,1,-281,45,157161,1101,0,94559,66,1101,0,4,67,1101,0,1977,68,1101,0,302,69,1101,1,0,71,1101,1985,0,72,1106,0,73,0,0,0,0,0,0,0,0,6,293637,1102,13477,1,66,1101,0,1,67,1102,2014,1,68,1101,556,0,69,1101,1,0,71,1102,2016,1,72,1106,0,73,1,514,24,268955,1101,0,97871,66,1101,0,1,67,1102,2045,1,68,1102,556,1,69,1101,0,4,71,1102,2047,1,72,1106,0,73,1,2,17,189118,17,283677,47,24754,47,74262,1102,26987,1,66,1101,0,2,67,1102,2082,1,68,1102,1,302,69,1102,1,1,71,1101,2086,0,72,1106,0,73,0,0,0,0,20,198502,1101,92987,0,66,1101,0,1,67,1102,1,2115,68,1102,1,556,69,1102,0,1,71,1102,2117,1,72,1106,0,73,1,1857,1101,12377,0,66,1102,6,1,67,1101,0,2144,68,1101,302,0,69,1102,1,1,71,1101,2156,0,72,1106,0,73,0,0,0,0,0,0,0,0,0,0,0,0,26,12778,1102,69371,1,66,1102,1,1,67,1102,2185,1,68,1101,556,0,69,1102,2,1,71,1102,1,2187,72,1106,0,73,1,1889,20,397004,4,23899,1102,1,52861,66,1102,1,1,67,1101,0,2218,68,1101,556,0,69,1102,1,1,71,1102,2220,1,72,1105,1,73,1,599,7,373124,1101,23899,0,66,1102,1,3,67,1102,1,2249,68,1101,0,302,69,1102,1,1,71,1102,2255,1,72,1106,0,73,0,0,0,0,0,0,3,91369";
        static List<Queue<string[]>> inputQ;
        static int runTimes;
        static int loopCount;
        static string PreviousY = null;
        static string answerPart2 = null;
        static List<IntCodeComputer> listComputers;

        public static string[] ReturnFirstPacketSentTo255()
        {
            int numComputers = 50;
            listComputers = new List<IntCodeComputer>();
            //List<List<string[]>> inputQ = new List<List<string[]>>();
            inputQ = new List<Queue<string[]>>();

            for (int count = 0; count < numComputers; count++)
            {
                listComputers.Add(new IntCodeComputer(puzzleInput, count));
                inputQ.Add(new Queue<string[]>());
            }
            string input;
            string packetAddress = null;
            string packetX = null;
            string packetY = null;
            bool end = false;
            while (!end)
            {
                for (int count = 0; count < numComputers; count++)
                {
                    if (inputQ[count].Count > 0)
                    {
                        string[] inputPacket = inputQ[count].Dequeue();
                        for (int inputCount = 0; inputCount < 2; inputCount++)
                        {
                            listComputers[count].Run(inputPacket[inputCount]);
                        }
                        if (inputQ[count].Count > 0)
                        {
                            Console.WriteLine($"InputQ for computer : {count}");
                            foreach (string[] element in inputQ[count]) { Console.WriteLine($"      {element[0]}, {element[1]}"); }
                            Console.WriteLine();
                        }
                        else { Console.WriteLine($"InputQ for computer : {count} is now empty"); }

                        continue;
                    }
                    input = "-1";

                    packetAddress = listComputers[count].Run(input);
                    if (packetAddress == null) { continue; }

                    if (packetAddress == "END") { Console.WriteLine($"The computer numbered : {count} has stopped..."); Console.ReadLine(); }
                    packetX = listComputers[count].Run(input);
                    packetY = listComputers[count].Run(input);

                    if (packetAddress == "255")
                    {
                        end = true;
                        Console.ForegroundColor = ConsoleColor.Magenta;
                        Console.WriteLine($"sent packet : To ({packetAddress}), X ({packetX}), Y ({packetY})");
                        Console.ResetColor();
                        break;
                    }
                    else
                    {
                        Console.WriteLine($"sent packet : To ({packetAddress}), X ({packetX}), Y ({packetY})");
                        inputQ[int.Parse(packetAddress)].Enqueue(new string[] { packetX, packetY });
                    }
                }
            }
            return new string[] { packetAddress, packetX, packetY };
        }
        public static string ReturnAnswerPart2()
        {
            int numComputers = 50;
            listComputers = new List<IntCodeComputer>();
            inputQ = new List<Queue<string[]>>();

            for (int count = 0; count < numComputers; count++)
            {
                listComputers.Add(new IntCodeComputer(puzzleInput, count));
                inputQ.Add(new Queue<string[]>());
            }
            bool end = false;

            string NATPacketX = null;
            string NATPacketY = null;
            bool[] emptyQ = new bool[50];
            bool[] receivingEmptyPackets = new bool[50];

            loopCount = 0;
            string[] outputTotalPacket;

            runTimes = 0;
            while (!end)
            {
                for (int computerCount = 0; computerCount < numComputers; computerCount++)
                {

                    if (ReturnInputQPresence(computerCount))
                    {
                        receivingEmptyPackets[computerCount] = false;
                        outputTotalPacket = InputPacket(computerCount);
                        if (outputTotalPacket != null) { EnQPacket(outputTotalPacket); }
                        if (ReturnInputQPresence(computerCount))
                        {
                            Console.WriteLine($"InputQ for computer : {computerCount}");
                            foreach (string[] element in inputQ[computerCount]) { Console.WriteLine($"      {element[0]}, {element[1]}"); }
                            Console.WriteLine();
                            emptyQ[computerCount] = false;
                        }
                        else { Console.WriteLine($"InputQ for computer : {computerCount} is now empty"); emptyQ[computerCount] = true; }
                        continue;
                    }

                    emptyQ[computerCount] = true;
                    outputTotalPacket = OutputPacket(computerCount);

                    if (outputTotalPacket != null)
                    {
                        //receivingEmptyPackets[computerCount] = false;
                        if (outputTotalPacket[0] == "255")
                        {
                            NATPacketX = outputTotalPacket[1];
                            NATPacketY = outputTotalPacket[2];
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine($"Sent packet From ({computerCount}) : To (255), X ({NATPacketX}), Y ({NATPacketY})");
                            Console.ResetColor();
                        }
                        else
                        {
                            Console.WriteLine($"Sent packet From ({computerCount}): To ({outputTotalPacket[0]}), X ({outputTotalPacket[1]}), Y ({outputTotalPacket[2]})");
                            inputQ[int.Parse(outputTotalPacket[0])].Enqueue(new string[] { outputTotalPacket[1], outputTotalPacket[2] });
                            emptyQ[int.Parse(outputTotalPacket[0])] = false;
                            //emptyQ[computerCount] = false;
                        }
                    }
                    else { receivingEmptyPackets[computerCount] = true; }
                }
                runTimes += 1;
                loopCount += 1;

                IdleFunction(emptyQ, receivingEmptyPackets, loopCount, NATPacketX, NATPacketY);
                //if (answerPart2 != null) { break; }
            }
            return NATPacketY;
        }
        public static string[] OutputPacket(int computerCount)
        {
            string[] output = new string[3];
            for (int outputCount = 0; outputCount < 3; outputCount++)
            {
                output[outputCount] = listComputers[computerCount].Run("-1");
                if (output[0] == null) { break; }
            }
            if (output[0] != null) { return output; }
            return null;
        }
        public static string[] InputPacket(int computerCount)
        {
            string[] inputPacket = inputQ[computerCount].Dequeue();
            string[] output = new string[3];
            int outputCount = 0;
            for (int inputCount = 0; inputCount < 2; inputCount++)
            {
                output[outputCount] = listComputers[computerCount].Run(inputPacket[inputCount]);
                if (output[outputCount] != null) { Console.WriteLine($"            {computerCount} what {output[outputCount]}"); outputCount += 1; }
            }
            if (output[0] != null)
            {
                for (int count = 0; count < 3; count++)
                {
                    if (output[outputCount] == null)
                    {
                        output[outputCount] = listComputers[computerCount].Run("-1");
                        Console.WriteLine($"            {computerCount} nay {output[outputCount]}");
                    }
                }
                return output;
            }
            return null;
        }
        public static void EnQPacket(string[] output)
        {
            if (output[0] == "0")
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"Ran inner loop : {runTimes}");
                runTimes = 0;
                loopCount = 0;
                if (PreviousY == output[2]) { answerPart2 = output[2]; }
                PreviousY = output[2];
                PrintInputQ();
            }
            Console.WriteLine($"Sent pack To : ({output[0]}), X ({output[1]}), Y ({output[2]})");
            Console.ResetColor();

            inputQ[int.Parse(output[0])].Enqueue(new string[] { output[1], output[2] });
            if (output[0] == "0") { Console.ReadLine(); }
        }
        public static void IdleFunction(bool[] emptyQ, bool[] onlyReceivingPackets, int loopCount, string NATPacketX, string NATPacketY)
        {
            bool idleCheck = true;
            foreach (bool element in emptyQ) { if (element == false) { idleCheck = false; } }
            foreach (bool element in onlyReceivingPackets) { if (element == false) { idleCheck = false; } }
            if (idleCheck && loopCount > 1 && NATPacketX != null) { EnQPacket(new string[] {"0", NATPacketX, NATPacketY}); }
        }
        public static bool ReturnInputQPresence(int computerNum)
        {
            if (inputQ[computerNum].Count > 0) { return true; }
            return false;
        }
        public static void PrintInputQ()
        {
            int count = -1;
            foreach (Queue<string[]> queue in inputQ)
            {
                count += 1;
                if (queue.Count == 0) { Console.WriteLine($"Empty queue : {count}"); continue; }
                foreach(string[] packet in queue)
                {
                    Console.WriteLine($"     queue : {count} - {packet[0]}, {packet[1]}");
                }
                Console.WriteLine();
                
            }
        }

        public void main()
        {
            /*
            string YPacket255 = ReturnFirstPacketSentTo255()[2];
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine($"The Y value of packet sent to address 255 is : {YPacket255}");
            Console.ResetColor();
            */

            string AnswerPart2 = ReturnAnswerPart2();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"The first Y value delivered by NAT to address 0 twice in a row is : {AnswerPart2}");
            Console.ResetColor();

        }
    }
}
