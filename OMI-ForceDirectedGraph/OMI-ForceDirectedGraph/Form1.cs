using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Forms;
using System.Runtime.InteropServices;

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

        public const int verticesAmt = 5;

        // Display object for drawing the graphs
        Display display = new Display();

        // A total of up to 25 vertices are allowed in the graph
        internal Vertex[] Vertices = new Vertex[verticesAmt];
        private Random rndGen = new Random();

        // Define the weights for the repulsive and attractive forces
        private int rWeight = 1;
        private int aWeight = 1;

        // each run we will have up to a maximum of 100 iterations of calculating and apply force before we stop it
        private int maxIterations = 100;

        public Form1()
        {
            AllocConsole();
            InitializeComponent();
            //this.testFunctions();
        }

        /// <summary>
        /// Just here to test whether everything works
        /// </summary>
        void testFunctions()
        {
            // Whether all connections are correct
            bool worker = true;
            for (int i = 0; i < verticesAmt; i++)
            {
                foreach (var v in Vertices[i].connectedVertexIDs)
                {
                    if (!worker)
                        break;

                    // i=1 -> 13,4,8
                    if (i == 13)
                        Console.WriteLine("a");

                    worker = Vertices[v].ConnectedWith(Vertices[i]);
                }
                if (!worker)
                    break;
            }
            Console.WriteLine(worker);
        }

        // Generate 25 vertices, each with a random position and up to 10 random connections
        private void GenerateVertices()
        {
            for (int i = 0; i < verticesAmt; i++)
            {
                // Random Position
                int x = rndGen.Next(230, 270);
                int y = rndGen.Next(230, 270);

                // Random Connections 
                int connections = 1 + rndGen.Next(2);
                HashSet<int> connectionSet = new HashSet<int>();

                for (int c = 0; c < connections; c++)
                {
                    int conn = rndGen.Next(verticesAmt);
                    if (conn != i)
                        connectionSet.Add(conn);
                }

                // The ID is its position in the array
                Vertices[i] = new Vertex(i, new Vector(x, y), connectionSet);
            }

            // And now for some hacky magic:
            // Each connection goes both ways:
            for (int i = 0; i < verticesAmt; i++)
                for (int j = i; j < verticesAmt; j++)
                    if (Vertices[i].ConnectedWith(Vertices[j]))
                    {
                        Vertices[j].AddConnection(Vertices[i]);
                        if (Vertices[j].ConnectedWith(Vertices[i]) && !Vertices[i].ConnectedWith(Vertices[j]))
                            Console.WriteLine("HELP");
                    }
        }


        // Main function:
        private void updateForces()
        {

            Console.WriteLine(Vertices[1].PositionVector);

            bool[] closed = new bool[verticesAmt];
            var forcesDict = new Dictionary<int, Vector>(verticesAmt);

            for (int i = 0; i < verticesAmt; i++)
                forcesDict[i] = new Vector(0, 0);

            for (int i = 0; i < verticesAmt; i++)
            {
                var vert = Vertices[i];

                foreach (int connection in vert.connectedVertexIDs)
                {
                    if (closed[connection])
                        continue;

                    Vector oldForce = new Vector();
                    Vector aForce = Algorithms.HCAttractive(vert, Vertices[connection], aWeight);

                    forcesDict.TryGetValue(i, out oldForce);
                    forcesDict[i] = oldForce + aForce;
                }

                closed[i] = true;
            }

            for (int i = 0; i < verticesAmt; i++)
            {

                Console.WriteLine(Vertices[i].PositionVector);
            }

            // Apply Repulsive Forces:
            closed = new bool[verticesAmt];

            for (int i = 0; i < verticesAmt; i++)
            {
                if (closed[i])
                    continue;

                for (int j = 0; j < verticesAmt; j++)
                {
                    if (closed[j] || i == j)
                        continue;

                    Vector oldForce = new Vector();
                    Vector aForce = Algorithms.HCRepulsive(Vertices[i], Vertices[j], aWeight);

                    forcesDict.TryGetValue(i, out oldForce);
                    forcesDict[i] = oldForce + aForce;

                    /*
                    Vector oldForce2 = new Vector(0, 0);
                    forcesDict.TryGetValue(j, out oldForce2);
                    forcesDict[j] = oldForce2 - aForce;
                    */
                }
            }


            for (int i = 0; i < verticesAmt; i++)
            {
                if (forcesDict.ContainsKey(i))
                    Vertices[i].ApplyForce(forcesDict[i]);

                Console.WriteLine(Vertices[i].PositionVector);
            }

            Console.WriteLine("");
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
        }

        private void ApplyForcesButton_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < 100; i++)
                this.updateForces();
            pictureBox1.Invalidate();
        }
    }
}
