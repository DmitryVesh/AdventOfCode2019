using System;
using System.IO;
using System.Threading;
using System.Collections.Generic;


namespace Day6
{
    class Day6Code
    {
        //static string dir = "D:\\C# projects\\Day6Orbits\\input.txt";
        //static Dictionary<string, List<string>> graphOfOrbits = new Dictionary<string, List<string>>();
        //static int totalNumOfOrbits = 0;

        //static List<string> nodes = new List<string>();



        public void main()
        {
            string dir = "D:\\C# projects\\AdventOfCode2019\\inputFiles\\day6.txt";
            Dictionary<string, List<string>> graphOfOrbits = new Dictionary<string, List<string>>();
            //int totalNumOfOrbits = 0;

            List<string> nodes = new List<string>();
            string[] splitLine;
            string startNode = null;
            using (StreamReader sr = new StreamReader(dir))
            {
                while (!sr.EndOfStream)
                {
                    splitLine = sr.ReadLine().Split(')');
                    //splitLine[1] orbits splitLine[0]

                    if (!graphOfOrbits.ContainsKey(splitLine[1])) { graphOfOrbits.Add(splitLine[1], new List<string> { splitLine[0] }); }
                    else { graphOfOrbits[splitLine[1]].Add(splitLine[0]); }

                    bool present = false;
                    foreach (string element in nodes) { if (element == splitLine[0]) { present = true; break; } }
                    if (!present) { nodes.Add(splitLine[0]); }
                    present = false;
                    foreach (string element in nodes) { if (element == splitLine[1]) { present = true; break; } }
                    if (!present) { nodes.Add(splitLine[1]); }

                    // COM doesn't orbit nothing... so start node is what orbits it...
                    if (splitLine[0] == "COM") { startNode = splitLine[1]; }
                }
                Console.WriteLine("The total number of nodes : ({0})", nodes.Count);
            }
            //int count = 0;
            //foreach(KeyValuePair<string, List<string>> entry in graphOfOrbits) { AddAllOrbitsToAnEntry(entry); count += 1; if (count == 4) { break; } } 
            string someNode = startNode;

            List<List<string>> nodesInfront = new List<List<string>> { new List<string> { "COM", startNode } };
            //totalNumOfOrbits = 1;
            Dictionary<string, List<string>> newgraphOfOrbits = new Dictionary<string, List<string>>(graphOfOrbits);
            List<string> nodesNeedToVisit = new List<string>();
            bool end = false;
            int counter = 1;
            int numOfOrbitsPerObject = 0;
            int branchCount = 0;
            while (!end)
            {
                foreach (KeyValuePair<string, List<string>> entry in graphOfOrbits)
                {
                    foreach (string value in entry.Value)
                    {
                        if (value == someNode)
                        {
                            numOfOrbitsPerObject += 1;
                            //Console.WriteLine("AAYYYY"); 
                            for (int count = nodesInfront.Count - 2; count > -1; count--)
                            {
                                newgraphOfOrbits[entry.Key].Add(nodesInfront[branchCount][count]);
                            }

                            nodesNeedToVisit.Add(entry.Key);

                            break;
                        }
                    }
                }
                numOfOrbitsPerObject = 0;
                Thread.Sleep(10);
                foreach (string element in nodesNeedToVisit)
                {
                    someNode = element;

                    nodesInfront.Add(new List<string> { someNode });
                    //Console.WriteLine(someNode);
                    //if (counter % 12 == 0) { OutputAllGraphConnections(newgraphOfOrbits); Console.ReadLine(); }
                    //OutputAllGraphConnections(newgraphOfOrbits);
                    //Console.ReadLine();
                    //Console.Clear();

                    counter += 1;
                }
                nodesNeedToVisit.Clear();
            }



        }

        public static void OutputAllGraphConnections(Dictionary<string, List<string>> graphOfOrbits)
        {
            foreach (KeyValuePair<string, List<string>> entry in graphOfOrbits)
            {
                Console.WriteLine();
                Console.Write("Key : ({0})  -  Values", entry.Key);
                foreach (string element in entry.Value) { Console.Write(" {0},", element); }
            }
        }

        public static void OutputCurrentGraphConnections(KeyValuePair<string, List<string>> entry)
        {
            Console.WriteLine();
            Console.Write("Key : ({0})  -  Values", entry.Key);
            foreach (string element in entry.Value) { Console.Write(" {0},", element); }
            Console.WriteLine();
        }

        public static void AddAllOrbitsToAnEntry(KeyValuePair<string, List<string>> entry, int totalNumOfOrbits, Dictionary<string, List<string>> graphOfOrbits)
        {
            OutputCurrentGraphConnections(entry);
            int count = 0;
            int lengthOfValue = 0;
            Console.WriteLine("Total number of orbits ({0})", totalNumOfOrbits);
            while (count <= lengthOfValue)
            {

                string value = entry.Value[count];
                List<string> newValue = null;
                if (graphOfOrbits.ContainsKey(value))
                {
                    graphOfOrbits.TryGetValue(value, out newValue);
                    if (newValue != null)
                    {
                        foreach (string element in newValue) { entry.Value.Add(element); lengthOfValue += 1; totalNumOfOrbits += 1; }
                    }
                }
                count += 1;
            }
            OutputCurrentGraphConnections(entry);
        }
    }
}