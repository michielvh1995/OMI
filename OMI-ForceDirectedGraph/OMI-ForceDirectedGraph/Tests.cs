using System;
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
        private static readonly Random rndGen = new Random();

        public enum AlgorithmType { HookeCoulomb, Eades, FruchtRein };

        // A total of up to verticesAmt vertices are allowed in the graph
        internal static Vertex[] Vertices = new Vertex[VerticesAmt];

        // Saves all test results as graph qualities indexed by a combination of the used algorithm, the index of the graph used in the graph list and the choices for the constants.
        // Note that for the Hooke-Coulomb algorithm the third (last) constant value in the key will always be 0
        private static Dictionary<Tuple<AlgorithmType, int, double, double, double>, double> testResults = new Dictionary<Tuple<AlgorithmType, int, double, double, double>, double>();

        // Define the weights for the repulsive and attractive forces
        private static double rWeight = 1;
        private static double aWeight = 1;
        private static double logAWeight = 1;
        private static double FRWeight = 1;
        private static double FRConstant = 1;
        private static double radius = 1;

        private static Vector rWeightRange = new Vector(0, 1);
        private static Vector aWeightRange = new Vector(0, 1);
        private static Vector logAWeightRange = new Vector(0, 1);
        private static Vector FRWeightRange = new Vector(0, 1);
        private static Vector FRConstantRange = new Vector(0, 1);
        private static Vector radiusRange = new Vector(0, 1);

        // The maximum amount of vertices in a graph
        public const int VerticesAmt = 5;
        // each run we will have up to a maximum of 100 iterations of calculating and apply force before we stop it
        public const int maxIterations = 100;
        // We'll try a maximum of 10 settings for each parameter, spread linearly across its range
        public const int maxConstantSettings = 10;
        // We'll try out the algorithms and algorithm settings on a maximum of 10 distinct graphs
        public const int maxGraphs = 10;
        #endregion

        // Generates a specified number of vertices and their connections
        // Guaranteed is that the graph that these vertices form will be completely connected,
        // ie every vertex can be directly or indirectly reached by every other vertex
        public static Vertex[] GenerateVertices(int amount)
        {
            for (int i = 0; i < amount; i++)
            {
                // Random Position
                int x = rndGen.Next(230, 270);
                int y = rndGen.Next(230, 270);

                // Random Connections 
                int connections = rndGen.Next(2);
                HashSet<int> connectionSet = new HashSet<int>();
                HashSet<int> connectedVertices = new HashSet<int>();

                int connection;
                do
                { 
                    connection = connectedVertices.Count > 0 ? connectedVertices.ToList()[rndGen.Next(connectedVertices.Count)] : rndGen.Next(amount);
                } while (connection == i);
                connectionSet.Add(connection);
                connectedVertices.Add(i);

                for (int c = 0; c < connections; c++)
                {
                    int conn = rndGen.Next(amount);
                    if (conn != i)
                    {
                        connectionSet.Add(conn);
                        connectedVertices.Add(i);
                        connectedVertices.Add(conn);
                    }
                }

                // The ID is its position in the array
                Vertices[i] = new Vertex(i, new Vector(x, y), connectionSet);
            }

            // Each of the connections should go both ways.
            for (int i = 0; i < amount; i++)
            {
                foreach (int connected in Vertices[i].connectedVertexIDs)
                {
                    Vertices[connected].AddConnection(i);
                }
            }

            return Vertices;
        }

        // Conduct the full set of tests for different algorithms, graphs and constant values
        public static void ConductTests()
        {
            Vertex[][] graphs = LoadGraphs().ToArray();

            //Hooke-Coulomb algorithm
            for (int i = 0; i < maxGraphs; i++)
            {
                Vertices = graphs[i];
                for (int j = 0; j < maxConstantSettings; j++)
                {
                    aWeight = Lerp(aWeightRange.X, aWeightRange.Y, j * (1d / (maxConstantSettings - 1)));
                    for (int k = 0; k < maxConstantSettings; k++)
                    {
                        rWeight = Lerp(rWeightRange.X, rWeightRange.Y, k * (1d / (maxConstantSettings - 1)));
                        for (int l = 0; l < maxIterations; l++)
                        {
                            UpdateForces(AlgorithmType.HookeCoulomb);
                        }
                        testResults.Add(Tuple.Create<AlgorithmType, int, double, double, double>(AlgorithmType.HookeCoulomb, i, aWeight, rWeight, 0), QualityTest.qualityTest(Vertices));
                    }
                }
            }

            //Eades algorithm
            for (int i = 0; i < maxGraphs; i++)
            {
                Vertices = graphs[i];
                for (int j = 0; j < maxConstantSettings; j++)
                {
                    aWeight = Lerp(aWeightRange.X, aWeightRange.Y, j * (1d / (maxConstantSettings - 1)));
                    for (int k = 0; k < maxConstantSettings; k++)
                    {
                        logAWeight = Lerp(logAWeightRange.X, logAWeightRange.Y, k * (1d / (maxConstantSettings - 1)));
                        for (int l = 0; l < maxIterations; l++)
                        {
                            rWeight = Lerp(rWeightRange.X, rWeightRange.Y, l * (1d / (maxConstantSettings - 1)));
                            for (int m = 0; m < maxIterations; m++)
                            {
                                UpdateForces(AlgorithmType.Eades);
                            }
                            testResults.Add(Tuple.Create<AlgorithmType, int, double, double, double>(AlgorithmType.Eades, i, aWeight, logAWeight, rWeight), QualityTest.qualityTest(Vertices));
                        }
                    }
                }
            }

            //Fruchterman-Reingold algorithm
            for (int i = 0; i < maxGraphs; i++)
            {
                Vertices = graphs[i];
                for (int j = 0; j < maxConstantSettings; j++)
                {
                    FRWeight = Lerp(FRWeightRange.X, FRWeightRange.Y, j * (1d / (maxConstantSettings - 1)));
                    for (int k = 0; k < maxConstantSettings; k++)
                    {
                        FRConstant = Lerp(FRConstantRange.X, FRConstantRange.Y, k * (1d / (maxConstantSettings - 1)));
                        for (int l = 0; l < maxIterations; l++)
                        {
                            radius = Lerp(radiusRange.X, radiusRange.Y, l * (1d / (maxConstantSettings - 1)));
                            for (int m = 0; m < maxIterations; m++)
                            {
                                UpdateForces(AlgorithmType.FruchtRein);
                            }
                            testResults.Add(Tuple.Create<AlgorithmType, int, double, double, double>(AlgorithmType.FruchtRein, i, FRWeight, FRConstant, radius), QualityTest.qualityTest(Vertices));
                        }
                    }
                }
            }

            //WriteTestResults(); //Uncomment this line to write the test results to a file
        }

        // A helper function to linearly interpolate between two values
        private static double Lerp(double start, double end, double index)
        {
            return (1 - index) * start + index * end;
        }

        // General function to calculate the attractive force for each algorithm
        private static Vector AttractiveForce(AlgorithmType type, Vertex node1, Vertex node2, double k)
        {
            switch (type)
            {
                case AlgorithmType.HookeCoulomb:
                    return Algorithms.HCAttractive(node1, node2, aWeight);
                case AlgorithmType.Eades:
                    return Algorithms.EadesAttractive(node1, node2, aWeight, logAWeight);
                case AlgorithmType.FruchtRein:
                    return Algorithms.FruchtReinAttractive(node1, node2, k, FRWeight);
                default:
                    return new Vector(0, 0);
            }
        }

        // General function to calculate the repulsive force for each algorithm
        private static Vector RepulsiveForce(AlgorithmType type, Vertex node1, Vertex node2, double k)
        {
            switch (type)
            {
                case AlgorithmType.HookeCoulomb:
                    return Algorithms.HCRepulsive(node1, node2, rWeight);
                case AlgorithmType.Eades:
                    return Algorithms.EadesRepulsive(node1, node2, rWeight);
                case AlgorithmType.FruchtRein:
                    return Algorithms.FruchtReinRepulsive(node1, node2, k, FRWeight);
                default:
                    return new Vector(0, 0);
            }
        }

        // Main function:
        public static void UpdateForces(AlgorithmType type)
        {
            // Works the same way as array, but can be used concurrently (as we're both reading and writing)
            var forcesDict = new ConcurrentDictionary<int, Vector>();
            var closed = new ConcurrentDictionary<int, bool>();

            //Used only when using the FruchtRein algorithm
            double k = 0;

            // Needs to be instantiated to something
            for (int i = 0; i < VerticesAmt; i++)
                forcesDict[i] = new Vector(0, 0);

            for (int i = 0; i < VerticesAmt; i++)
                closed[i] = false;

            Parallel.For(0, VerticesAmt - 1, i =>
            {
                k = type == AlgorithmType.FruchtRein ? Algorithms.FruchtReinConstant(Vertices[i], FRConstant, radius) : 0;

                foreach (int connection in Vertices[i].connectedVertexIDs)
                {
                    if (closed[connection])
                        continue;

                    Vector aForce = AttractiveForce(type, Vertices[i], Vertices[connection], k);

                    forcesDict[i] += aForce;
                }

                closed[i] = true;
            });

            // Add Repulsive Forces Into the mix:
            for (int i = 0; i < VerticesAmt; i++)
                closed[i] = false;

            Parallel.For(0, VerticesAmt - 1, i =>
            {
                k = type == AlgorithmType.FruchtRein ? Algorithms.FruchtReinConstant(Vertices[i], FRConstant, radius) : 0;

                for (int j = 0; j < VerticesAmt; j++)
                {
                    if (closed[j] || i == j)
                        continue;

                    Vector rForce = RepulsiveForce(type, Vertices[i], Vertices[j], k);

                    forcesDict[i] += rForce;
                }
                closed[i] = true;
            });

            for (int i = 0; i < VerticesAmt; i++)
            {
                Vertices[i].ApplyForce(forcesDict[i]);
            }
        }
    }
}
