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

<<<<<<< HEAD
        public const int VerticesAmt = 5;
=======
        public const int verticesAmt = 25;
>>>>>>> origin/master

        // Display object for drawing the graphs
        private readonly Display display = new Display();

        // Object for testing the quality of the graph
        QualityTest qualityTest = new QualityTest();

        // A total of up to 25 vertices are allowed in the graph
        internal Vertex[] Vertices = new Vertex[VerticesAmt];
        private readonly Random rndGen = new Random();

        // Define the weights for the repulsive and attractive forces
        private int rWeight = 1;
        private int aWeight = 1;

        // each run we will have up to a maximum of 100 iterations of calculating and apply force before we stop it
        private int maxIterations = 100;

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

        // Generate 25 vertices, each with a random position and up to 10 random connections
        private void GenerateVertices()
        {
<<<<<<< HEAD
            for (int i = 0; i < VerticesAmt; i++)
<<<<<<< HEAD
<<<<<<< HEAD
<<<<<<< HEAD
            Vertices = new Vertex[verticesAmt];
            
            for (int i = 0; i < verticesAmt; i++)
=======
>>>>>>> parent of 30dbd5a... Merge remote-tracking branch 'origin/master'
=======
=======
            Vertices = new Vertex[verticesAmt];
            
            for (int i = 0; i < verticesAmt; i++)
>>>>>>> origin/master
>>>>>>> parent of 7a01c07... .
=======
            Vertices = new Vertex[verticesAmt];
            
            for (int i = 0; i < verticesAmt; i++)
>>>>>>> parent of 4496388... rollback
            {
                // Random Position
                int x = rndGen.Next(10, 500);
                int y = rndGen.Next(10, 500);

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

<<<<<<< HEAD
            // Each of the connections should go both ways.
            for (int i = 0; i < VerticesAmt; i++)
            {
                foreach (int connected in Vertices[i].connectedVertexIDs)
                {
                    Vertices[connected].AddConnection(i);
                }
            }
<<<<<<< HEAD
<<<<<<< HEAD
<<<<<<< HEAD
=======
=======
>>>>>>> parent of 7a01c07... .
=======
>>>>>>> parent of 4496388... rollback
            // And now for some hacky magic:
            // Each connection goes both ways:
            for (int i = 0; i < verticesAmt; i++)
                for (int j = 1; j < verticesAmt; j++)
                    if (Vertices[i].ConnectedWith(Vertices[j]))
                    {
                        Vertices[j].AddConnection(Vertices[i]);
                        if (Vertices[j].ConnectedWith(Vertices[i]) && !Vertices[i].ConnectedWith(Vertices[j]))
                            Console.WriteLine("HELP");
                    }
<<<<<<< HEAD
<<<<<<< HEAD
=======
>>>>>>> parent of 30dbd5a... Merge remote-tracking branch 'origin/master'
=======
>>>>>>> origin/master
>>>>>>> parent of 7a01c07... .
=======
>>>>>>> parent of 4496388... rollback
        }


        // Main function:
        private void updateForces()
        {
            // Works the same way as array, but can be used concurrently (as we're both reading and writing)
            var forcesDict = new ConcurrentDictionary<int, Vector>();
            var closed = new ConcurrentDictionary<int, bool>();

            // Needs to be instantiated to something
            for (int i = 0; i < VerticesAmt; i++)
                forcesDict[i] = new Vector(0, 0);

            for (int i = 0; i < VerticesAmt; i++)
                closed[i] = false;

            Parallel.For(0, VerticesAmt - 1, i =>
            {
                foreach (int connection in Vertices[i].connectedVertexIDs)
                {
                    if (closed[connection])
                        continue;

<<<<<<< HEAD
                    Vector aForce = Algorithms.HCAttractive(Vertices[i], Vertices[connection], aWeight);

                    forcesDict[i] += aForce;
                }

                closed[i] = true;
            });

            // Add Repulsive Forces Into the mix:
            for (int i = 0; i < VerticesAmt; i++)
                closed[i] = false;
=======
                    Vector oldForce = new Vector(0, 0);
                    Vector aForce = Algorithms.HCAttractive(vert, Vertices[connection], aWeight);

                    forcesDict.TryGetValue(i, out oldForce);

                    forcesDict[i] = oldForce + aForce;
                    forcesDict[connection] = oldForce - aForce;


                    // Stappenplan (met opvang):
                    // Alle nodes af gaan, 
                    // alle connecties bekijken & alle andere nodes af gaan voor repulsive
                    // Gooi de tegenovergestelde force bij de andere dingen (bij beide loops)
                    // Al gesloten nodes worden geskipt
                }

                closed[i] = true;
            }

            // Apply Repulsive Forces:
            closed = new bool[verticesAmt];
>>>>>>> origin/master

            Parallel.For(0, VerticesAmt - 1, i =>
            {
<<<<<<< HEAD
                for (int j = 0; j < VerticesAmt; j++)
                {
                    if (closed[j] || i == j)
                        continue;

                    Vector rForce = Algorithms.HCRepulsive(Vertices[i], Vertices[j], rWeight);

                    forcesDict[i] += rForce;
                }
                closed[i] = true;
            });

            for (int i = 0; i < VerticesAmt; i++)
=======
                if (closed[i])
                    continue;

                for (int j = i + 1; j < verticesAmt; j++)
                {
                    Vector oldForce = new Vector(0, 0);
                    Vector aForce = Algorithms.HCAttractive(Vertices[i], Vertices[j], aWeight);

                    forcesDict.TryGetValue(i, out oldForce);

                    forcesDict[i] = oldForce + aForce;
                    forcesDict[j] = oldForce - aForce;
                }
            }

            for (int i = 0; i < verticesAmt; i++)
>>>>>>> origin/master
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
<<<<<<< HEAD

#if DEBUG   // Testing
            this.testFunctions();
#endif
=======
            Console.WriteLine(qualityTest.GetEdgeCrossings(Vertices));
>>>>>>> origin/master
        }

        private void ApplyForcesButton_Click(object sender, EventArgs e)
        {
<<<<<<< HEAD
            for (int i = 0; i < 1000; i++)
                this.updateForces();
=======
            this.updateForces();
>>>>>>> origin/master
            pictureBox1.Invalidate();

#if DEBUG   // Testing
            for (int i = 0; i < VerticesAmt; i++)
                Console.WriteLine(Vertices[i].PositionVector);
            Console.WriteLine();
#endif
        }
    }
}
