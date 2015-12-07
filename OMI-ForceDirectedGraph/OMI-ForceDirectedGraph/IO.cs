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
    }
}
