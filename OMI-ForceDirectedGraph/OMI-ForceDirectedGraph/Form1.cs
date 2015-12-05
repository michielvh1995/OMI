#define DEBUG

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
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
    public partial class Form1 : Form
    {
        #region Console
        // Allowing the use of the console.
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AllocConsole();
        #endregion

        // The maximum amount of vertices in a graph
        public const int VerticesAmt = 5;

        public enum AlgorithmType { HookeCoulomb, Eades, FruchtRein };

        // Display object for drawing the graphs
        private readonly Display display = new Display();
        
        // A total of up to verticesAmt vertices are allowed in the graph
        internal Vertex[] Vertices = new Vertex[VerticesAmt];
        private readonly Random rndGen = new Random();

        // Saves all test results as graph qualities indexed by a combination of the used algorithm, the index of the graph used in the graph list and the choices for the constants.
        // Note that for the Hooke-Coulomb algorithm the third (last) constant value in the key will always be 0
        private Dictionary<Tuple<AlgorithmType, int, double, double, double>, double> testResults = new Dictionary<Tuple<AlgorithmType, int, double, double, double>, double>();

        // Define the weights for the repulsive and attractive forces
        private double rWeight = 1;
        private double aWeight = 1;
        private double logAWeight = 1;
        private double FRWeight = 1;
        private double FRConstant = 1;
        private double radius = 1;

        private Vector rWeightRange = new Vector(0, 1);
        private Vector aWeightRange = new Vector(0, 1);
        private Vector logAWeightRange = new Vector(0, 1);
        private Vector FRWeightRange = new Vector(0, 1);
        private Vector FRConstantRange = new Vector(0, 1);
        private Vector radiusRange = new Vector(0, 1);

        // each run we will have up to a maximum of 100 iterations of calculating and apply force before we stop it
        private int maxIterations = 100;
        // We'll try a maximum of 10 settings for each parameter, spread linearly across its range
        private int maxConstantSettings = 10;
        // We'll try out the algorithms and algorithm settings on a maximum of 10 distinct graphs
        private int maxGraphs = 10;

        public Form1()
        {
            AllocConsole();
            InitializeComponent();
        }

        /// <summary>
        /// Just here to test whether everything works
        /// </summary>
        void testFunctions()
        {
            // Whether all connections are correct
            bool worker = true;
            for (int i = 0; i < VerticesAmt; i++)
            {
                foreach (int connection in Vertices[i].connectedVertexIDs)
                {
                    if (!Vertices[connection].ConnectedWith(i))
                        worker = false;
                }
            }
            Console.WriteLine(worker);

            int cnt = 0;
            for (int i = 0; i < VerticesAmt; i++)
            {
                for (int j = i + 1; j < VerticesAmt; j++)
                {
                    if (Vertices[i].ConnectedWith(j))
                        cnt++;
                }
            }
            Console.WriteLine(cnt);

            foreach (var v in Vertices)
            {
                Console.Write(v.GetConnectionCount() + " ");
            }
            Console.WriteLine();
        }

        // A helper function to linearly interpolate between two values
        private double lerp(double start, double end, double index)
        {
            return (1 - index) * start + index * end;
        }

        // Conduct the full set of tests for different algorithms, graphs and constant values
        private void conductTests()
        {
            Vertex[][] graphs = new Vertex[maxGraphs][];

            for (int i = 0; i < maxGraphs; i++)
            {
                GenerateVertices();
                graphs[i] = new Vertex[Vertices.Length];
                this.Vertices.CopyTo(graphs[i], 0);
            }

            //Hooke-Coulomb algorithm
            for (int i = 0; i < maxGraphs; i++)
            {
                Vertices = graphs[i];
                for (int j = 0; j < maxConstantSettings; j++)
                {
                    aWeight = lerp(aWeightRange.X, aWeightRange.Y, j * (1d / (maxConstantSettings - 1)));
                    for (int k = 0; k < maxConstantSettings; k++)
                    {
                        rWeight = lerp(rWeightRange.X, rWeightRange.Y, k * (1d / (maxConstantSettings - 1)));
                        for (int l = 0; l < maxIterations; l++)
                        {
                            updateForces(AlgorithmType.HookeCoulomb);
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
                    aWeight = lerp(aWeightRange.X, aWeightRange.Y, j * (1d / (maxConstantSettings - 1)));
                    for (int k = 0; k < maxConstantSettings; k++)
                    {
                        logAWeight = lerp(logAWeightRange.X, logAWeightRange.Y, k * (1d / (maxConstantSettings - 1)));
                        for (int l = 0; l < maxIterations; l++)
                        {
                            rWeight = lerp(rWeightRange.X, rWeightRange.Y, l * (1d / (maxConstantSettings - 1)));
                            for (int m = 0; m < maxIterations; m++)
                            {
                                updateForces(AlgorithmType.Eades);
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
                    FRWeight = lerp(FRWeightRange.X, FRWeightRange.Y, j * (1d / (maxConstantSettings - 1)));
                    for (int k = 0; k < maxConstantSettings; k++)
                    {
                        FRConstant = lerp(FRConstantRange.X, FRConstantRange.Y, k * (1d / (maxConstantSettings - 1)));
                        for (int l = 0; l < maxIterations; l++)
                        {
                            radius = lerp(radiusRange.X, radiusRange.Y, l * (1d / (maxConstantSettings - 1)));
                            for (int m = 0; m < maxIterations; m++)
                            {
                                updateForces(AlgorithmType.FruchtRein);
                            }
                            testResults.Add(Tuple.Create<AlgorithmType, int, double, double, double>(AlgorithmType.FruchtRein, i, FRWeight, FRConstant, radius), QualityTest.qualityTest(Vertices));
                        }
                    }
                }
            }
        }

        // Generate verticesAmt vertices, each with a random position and up to 10 random connections
        private void GenerateVertices()
        {
            for (int i = 0; i < VerticesAmt; i++)
            {
                // Random Position
                int x = rndGen.Next(230, 270);
                int y = rndGen.Next(230, 270);

                // Random Connections 
                int connections = 1 + rndGen.Next(2);
                HashSet<int> connectionSet = new HashSet<int>();

                for (int c = 0; c < connections; c++)
                {
                    int conn = rndGen.Next(VerticesAmt);
                    if (conn != i)
                        connectionSet.Add(conn);
                }

                // The ID is its position in the array
                Vertices[i] = new Vertex(i, new Vector(x, y), connectionSet);
            }

            // Each of the connections should go both ways.
            for (int i = 0; i < VerticesAmt; i++)
            {
                foreach (int connected in Vertices[i].connectedVertexIDs)
                {
                    Vertices[connected].AddConnection(i);
                }
            }
        }

        // General function to calculate the attractive force for each algorithm
        private Vector attractiveForce(AlgorithmType type, Vertex node1, Vertex node2, double k)
        {
            switch (type)
            {
                case AlgorithmType.HookeCoulomb:
                    return Algorithms.HCAttractive(node1, node2, aWeight);
                    break;
                case AlgorithmType.Eades:
                    return Algorithms.EadesAttractive(node1, node2, aWeight, logAWeight);
                    break;
                case AlgorithmType.FruchtRein:
                    return Algorithms.FruchtReinAttractive(node1, node2, k, FRWeight);
                    break;
                default:
                    return new Vector(0, 0);
            }
        }

        // General function to calculate the repulsive force for each algorithm
        private Vector repulsiveForce(AlgorithmType type, Vertex node1, Vertex node2, double k)
        {
            switch (type)
            {
                case AlgorithmType.HookeCoulomb:
                    return Algorithms.HCRepulsive(node1, node2, rWeight);
                    break;
                case AlgorithmType.Eades:
                    return Algorithms.EadesRepulsive(node1, node2, rWeight);
                    break;
                case AlgorithmType.FruchtRein:
                    return Algorithms.FruchtReinRepulsive(node1, node2, k, FRWeight);
                    break;
                default:
                    return new Vector(0, 0);
            }
        }

        // Main function:
        private void updateForces(AlgorithmType type)
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

                    Vector aForce = attractiveForce(type, Vertices[i], Vertices[connection], k);

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

                    Vector rForce = repulsiveForce(type, Vertices[i], Vertices[j], k);

                    forcesDict[i] += rForce;
                }
                closed[i] = true;
            });

            for (int i = 0; i < VerticesAmt; i++)
            {
                Vertices[i].ApplyForce(forcesDict[i]);
            }
        }

        // Displaying the vertices
        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            if (Vertices[0] == null) return;

            display.DrawGraph(e.Graphics, Vertices);
        }

        private void GenerateButton_Click(object sender, EventArgs e)
        {
            this.GenerateVertices();
            pictureBox1.Invalidate();

#if DEBUG   // Testing
            this.testFunctions();
#endif
        }

        private void ApplyForcesButton_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < maxIterations; i++)
                this.updateForces(AlgorithmType.HookeCoulomb);
            pictureBox1.Invalidate();

#if DEBUG   // Testing
            for (int i = 0; i < VerticesAmt; i++)
                Console.WriteLine(Vertices[i].PositionVector);
            Console.WriteLine();
#endif
            Console.WriteLine(QualityTest.GetEdgeCrossings(Vertices));
        }
    }
}
