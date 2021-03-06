﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Windows;
using System.Windows.Forms;

namespace OMI_ForceDirectedGraph
{
    partial class Tests
    {
        #region fields
        // The names of the constants used in the tests
        // Used when writing the test results to a file
        private static readonly string[] HKConsts = new string[2] { "aWeight", "rWeight" };
        private static readonly string[] EaConsts = new string[3] { "aWeight", "logAWeight", "rWeight" };
        private static readonly string[] FRConsts = new string[3] { "FRWeight", "FRConstant", "radius" };
        private static readonly string[][] ConstNames = new string[3][] { HKConsts, EaConsts, FRConsts };
        #endregion

        // Generates a number of graphs and writes them to graphs.txt.
        // One-time use only! These graphs are the ones we'll use for all tests!
        public static void CreateGraphs()
        {
            if (File.Exists(SourceDirectory + @"\graphs.txt"))              // Prevent accidental overwriting of our test graphs
                return;

            List<string> lines = new List<string>();

            for (int i = 0; i < maxGraphs; i++)                                                 // Note that this leaves an empty line at the end of the file - this is intended!
            {                                                                                   // This allows LoadGraphs to separate the lines into parts belonging to each graph
                lines = lines.Concat(GraphToStrings(GenerateVertices(VerticesAmt))).ToList();   // and convert them only when it encounters the empty string, which delimits the end
                lines.Add("");                                                                  // of a graph.
            }

            File.WriteAllLines(SourceDirectory + @"\graphs.txt", lines);
        }

        // Reads the graphs from graphs.txt and converts them to a list of Vertex arrays
        public static List<Vertex[]> LoadGraphs()
        {
            string[] lines = File.ReadAllLines(SourceDirectory + @"\graphs.txt");
            List<Vertex[]> graphs = new List<Vertex[]>();
            List<string> currentGraph = new List<string>();

            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i] != "")
                {
                    currentGraph.Add(lines[i]);
                }
                else
                {
                    graphs.Add(StringsToGraph(currentGraph));
                    currentGraph = new List<string>();
                }
            }

            return graphs;
        }

        // Converts a list of vertices to a list of strings
        // Helper function for CreateGraphs
        private static List<string> GraphToStrings(Vertex[] vertices)
        {
            List<string> lines = new List<string>();
            string id, x, y, connections;

            for (int i = 0; i < vertices.Length; i++)
            {
                id = vertices[i].ID.ToString();
                x = vertices[i].PositionVector.X.ToString();
                y = vertices[i].PositionVector.Y.ToString();
                connections = string.Join(",", vertices[i].connectedVertexIDs.ToArray());

                lines.Add(id + " " + x + " " + y + " " + connections);
            }

            return lines;
        }

        // Converts a list of strings representing a graph as generated by GraphToStrings
        // back to a Vertex array
        // Helper function for LoadGraphs
        private static Vertex[] StringsToGraph(List<string> lines)
        {
            Vertex[] graph = new Vertex[lines.Count];
            string[] fields, connectionStrings;
            HashSet<int> connections;
            int id;
            Vector position;

            for (int i = 0; i < lines.Count; i++)
            {
                fields = lines[i].Split(' ');
                connectionStrings = fields[3].Split(',');

                id = Convert.ToInt32(fields[0]);
                position = new Vector(Convert.ToDouble(fields[1]), Convert.ToDouble(fields[2]));

                connections = new HashSet<int>();
                foreach (string s in connectionStrings)
                    connections.Add(Convert.ToInt32(s));

                graph[i] = new Vertex(id, position, connections);
            }

            return graph;
        }

        // Converts the test results to a list of strings that are written to results#.txt
        // where # represents the test number.
        // So the results of the first test will be written to results1.txt, the next one to results2.txt, etcetera
        private static void WriteTestResults()
        {
            // Divide the results dictionary into portions by the algorithm used
            Dictionary<Tuple<int, double, double, double>, double>[] AlgoDicts = new Dictionary<Tuple<int, double, double, double>, double>[3];
            for (int i = 0; i < AlgoDicts.Length; i++)
                AlgoDicts[i] = testResults.Where(kvp => kvp.Key.Item1 == (AlgorithmType)i).ToDictionary(kvp => new Tuple<int, double, double, double>(kvp.Key.Item2, kvp.Key.Item3, kvp.Key.Item4, kvp.Key.Item5), kvp => kvp.Value);

            // Further divide this into portions by the graph used
            // This is done to more easily separate the test results when writing to the file
            Dictionary<Tuple<double, double, double>, double>[][] AlgoGraphDicts = new Dictionary<Tuple<double, double, double>, double>[3][];
            Dictionary<Tuple<double, double, double>, double>[] GraphDicts;
            for (int i = 0; i < AlgoGraphDicts.Length; i++)
            {
                GraphDicts = new Dictionary<Tuple<double,double,double>,double>[maxGraphs];
                for (int j = 0; j < GraphDicts.Length; j++)
                {
                    GraphDicts[j] = AlgoDicts[i].Where(kvp => kvp.Key.Item1 == j).ToDictionary(kvp => new Tuple<double, double, double>(kvp.Key.Item2, kvp.Key.Item3, kvp.Key.Item4), kvp => kvp.Value);
                }
                AlgoGraphDicts[i] = GraphDicts;
            }

            // Generate the string list representation of the test results
            // For every algorithm the name of the algorithm is written down
            // and for every graph the graph number is written down, indented with one tab
            // Finally the actual constant values and the resulting quality are written down
            // separated by commas and indented by two tabs
            List<string> lines = new List<string>();
            string line;
            double[] constants;
            List<Tuple<double, double, double>> sortedKeys;
            for (int i = 0; i < AlgoGraphDicts.Length; i++)
            {
                lines.Add(((AlgorithmType)i).ToString());
                for (int j = 0; j < AlgoGraphDicts[i].Length; j++)
                {
                    lines.Add("\tGraph " + j);
                    sortedKeys = AlgoGraphDicts[i][j].Keys.OrderBy(kvp => kvp.Item1).ThenBy(kvp => kvp.Item2).ThenBy(kvp => kvp.Item3).ToList();
                    for (int k = 0; k < sortedKeys.Count; k++)
                    {
                        line = "\t\t";
                        constants = new double[3] { sortedKeys[k].Item1, sortedKeys[k].Item2, sortedKeys[k].Item3 };

                        for (int l = 0; l < ConstNames[i].Length; l++)
                            line = line + ConstNames[i][l] + " = " + constants[l] + ", ";
                        line = line + "Quality = " + AlgoGraphDicts[i][j][sortedKeys[k]];

                        lines.Add(line);
                    }
                }
            }

            // Find what test number we're on
            int resultCount = 1;
            while (File.Exists(SourceDirectory + @"\results" + resultCount + ".txt"))
                resultCount++;

            File.WriteAllLines(SourceDirectory + @"\results" + resultCount + ".txt", lines);
        }

        private static void WriteTestResults3()
        {
            //Calculate average quality over all graphs
            Dictionary<Tuple<AlgorithmType, double, double, double>, double> averageQualities = new Dictionary<Tuple<AlgorithmType, double, double, double>, double>();
            Tuple<AlgorithmType, double, double, double> key;
            foreach (KeyValuePair<Tuple<AlgorithmType, int, double, double, double>, double> kvp in testResults)
            {
                key = new Tuple<AlgorithmType, double, double, double>(kvp.Key.Item1, kvp.Key.Item3, kvp.Key.Item4, kvp.Key.Item5);
                if (!averageQualities.ContainsKey(key))
                    averageQualities.Add(key, kvp.Value);
                else
                    averageQualities[key] += kvp.Value;
            }

            Dictionary<Tuple<AlgorithmType, double, double, double>, double> averageQualities2 = new Dictionary<Tuple<AlgorithmType, double, double, double>, double>();
            foreach (Tuple<AlgorithmType, double, double, double> Akey in averageQualities.Keys)
            {
                averageQualities2.Add(Akey, averageQualities[Akey] / 10d);
            }
            averageQualities = averageQualities2;

            List<Tuple<AlgorithmType, double, double, double>> averageQualitiesSort = averageQualities.Keys.OrderBy(akey => akey.Item1).ThenBy(akey => akey.Item2).ThenBy(akey => akey.Item3).ThenBy(akey => akey.Item4).ToList();

            //Split the qualities by algorithm
            Dictionary<Tuple<double, double, double>, double>[] algoDicts = new Dictionary<Tuple<double, double, double>, double>[3];
            List<Tuple<double, double, double>>[] algoDictsSort = new List<Tuple<double, double, double>>[3];
            for (int i = 0; i < algoDicts.Length; i++)
            {
                algoDicts[i] = averageQualities.Where(kvp => kvp.Key.Item1 == (AlgorithmType)i).ToDictionary(kvp => new Tuple<double, double, double>(kvp.Key.Item2, kvp.Key.Item3, kvp.Key.Item4), kvp => kvp.Value);
                algoDictsSort[i] = algoDicts[i].Keys.OrderBy(adkey => adkey.Item1).ThenBy(adkey => adkey.Item2).ThenBy(adkey => adkey.Item3).ToList();
            }

            //Filter out the qualities for the lowest and highest logAWeight
            Dictionary<Tuple<AlgorithmType, double, double>, double> lowLog = new Dictionary<Tuple<AlgorithmType, double, double>, double>();
            Dictionary<Tuple<AlgorithmType, double, double>, double> highLog = new Dictionary<Tuple<AlgorithmType, double, double>, double>();

            foreach (KeyValuePair<Tuple<AlgorithmType, double, double, double>, double> kvp in averageQualities.Where(kvp => kvp.Key.Item3 == 0.0001d && kvp.Key.Item1 == AlgorithmType.Eades).ToDictionary(kvp => new Tuple<AlgorithmType, double, double, double>(kvp.Key.Item1, kvp.Key.Item2, kvp.Key.Item3, kvp.Key.Item4), kvp => kvp.Value))
            {
                lowLog.Add(new Tuple<AlgorithmType, double, double>(kvp.Key.Item1, kvp.Key.Item2, kvp.Key.Item4), kvp.Value);
            }

            foreach (KeyValuePair<Tuple<AlgorithmType, double, double, double>, double> kvp in averageQualities.Where(kvp => kvp.Key.Item3 == 2d && kvp.Key.Item1 == AlgorithmType.Eades).ToDictionary(kvp => new Tuple<AlgorithmType, double, double, double>(kvp.Key.Item1, kvp.Key.Item2, kvp.Key.Item3, kvp.Key.Item4), kvp => kvp.Value))
            {
                highLog.Add(new Tuple<AlgorithmType, double, double>(kvp.Key.Item1, kvp.Key.Item2, kvp.Key.Item4), kvp.Value);
            }

            List<Tuple<AlgorithmType, double, double>> lowLogSort = lowLog.Keys.OrderBy(lkey => lkey.Item1).ThenBy(lkey => lkey.Item2).ThenBy(lkey => lkey.Item3).ToList();
            List<Tuple<AlgorithmType, double, double>> highLogSort = highLog.Keys.OrderBy(hkey => hkey.Item1).ThenBy(hkey => hkey.Item2).ThenBy(hkey => hkey.Item3).ToList();

            List<string> lines = new List<string>();
            string line;
            for (int i = 0; i < algoDictsSort[algoDictsSort.Length - 1].Count; i++)
            {
                line = "";

                for (int j = 0; j < algoDictsSort.Length; j++)
                    if (i < algoDictsSort[j].Count)
                        line = line + algoDicts[j][algoDictsSort[j][i]] + ", ";
                    else
                        line = line + algoDicts[j].Values.Average() + ", ";
                if (i < lowLogSort.Count)
                    line = line + lowLog[lowLogSort[i]] + ", " + highLog[highLogSort[i]];

                lines.Add(line);
            }

            // Find what test number we're on
            int resultCount = 1;
            while (File.Exists(SourceDirectory + @"\results" + resultCount + ".txt"))
                resultCount++;

            File.WriteAllLines(SourceDirectory + @"\results" + resultCount + ".txt", lines);
        }

        private static void WriteTestResults2()
        {
            // Divide the results dictionary into portions by the algorithm used
            Dictionary<Tuple<int, double, double, double>, double>[] AlgoDicts = new Dictionary<Tuple<int, double, double, double>, double>[3];
            for (int i = 0; i < AlgoDicts.Length; i++)
                AlgoDicts[i] = testResults.Where(kvp => kvp.Key.Item1 == (AlgorithmType)i).ToDictionary(kvp => new Tuple<int, double, double, double>(kvp.Key.Item2, kvp.Key.Item3, kvp.Key.Item4, kvp.Key.Item5), kvp => kvp.Value);

            // Further divide this into portions by the graph used
            // This is done to more easily separate the test results when writing to the file
            Dictionary<Tuple<double, double, double>, double>[][] AlgoGraphDicts = new Dictionary<Tuple<double, double, double>, double>[3][];
            Dictionary<Tuple<double, double, double>, double>[] GraphDicts;
            for (int i = 0; i < AlgoGraphDicts.Length; i++)
            {
                GraphDicts = new Dictionary<Tuple<double, double, double>, double>[maxGraphs];
                for (int j = 0; j < GraphDicts.Length; j++)
                {
                    GraphDicts[j] = AlgoDicts[i].Where(kvp => kvp.Key.Item1 == j).ToDictionary(kvp => new Tuple<double, double, double>(kvp.Key.Item2, kvp.Key.Item3, kvp.Key.Item4), kvp => kvp.Value);
                }
                AlgoGraphDicts[i] = GraphDicts;
            }

            // Generate the string list representation of the test results
            // For every algorithm the name of the algorithm is written down
            // and for every graph the graph number is written down, indented with one tab
            // Finally the actual constant values and the resulting quality are written down
            // separated by commas and indented by two tabs
            List<string> lines = new List<string>();
            string algo, graph, line;
            double[] constants;
            List<Tuple<double, double, double>> sortedKeys;
            for (int i = 0; i < AlgoGraphDicts.Length; i++)
            {
                algo = ((AlgorithmType)i).ToString();
                for (int j = 0; j < AlgoGraphDicts[i].Length; j++)
                {
                    graph = ", " + j;
                    sortedKeys = AlgoGraphDicts[i][j].Keys.OrderBy(kvp => kvp.Item1).ThenBy(kvp => kvp.Item2).ThenBy(kvp => kvp.Item3).ToList();
                    for (int k = 0; k < sortedKeys.Count; k++)
                    {
                        line = ", ";
                        constants = new double[3] { sortedKeys[k].Item1, sortedKeys[k].Item2, sortedKeys[k].Item3 };

                        for (int l = 0; l < ConstNames[i].Length; l++)
                            line = line + constants[l] + ", ";
                        if ((AlgorithmType)(i) == AlgorithmType.HookeCoulomb)
                            line = line + "-1, ";
                        line = line + AlgoGraphDicts[i][j][sortedKeys[k]];

                        lines.Add(algo + graph + line);
                    }
                }
            }

            // Find what test number we're on
            int resultCount = 1;
            while (File.Exists(SourceDirectory + @"\results" + resultCount + ".txt"))
                resultCount++;

            File.WriteAllLines(SourceDirectory + @"\results" + resultCount + ".txt", lines);
        }
    }
}
