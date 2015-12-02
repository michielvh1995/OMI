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

        // Display object for drawing the graphs
        Display display = new Display();

        // Object for testing the quality of the graph
        QualityTest qualityTest = new QualityTest();

        // A total of up to 25 vertices are allowed in the graph
        int vertexAmount = 25;
        internal Vertex[] Vertices;
        private Random rndGen = new Random();

        // Define the weights for the repulsive and attractive forces
        private int rWeight = 1;
        private int aWeight = 1;

        // each run we will have up to a maximum of 100 iterations of calculating and apply force before we stop it
        private int maxIterations = 100;

        public Form1()
        {
            Vertices = new Vertex[vertexAmount];
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
            for (int i = 0; i < vertexAmount; i++)
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
            Vertices = new Vertex[vertexAmount];
            
            for (int i = 0; i < vertexAmount; i++)
            {
                // Random Position
                int x = rndGen.Next(10, 500);
                int y = rndGen.Next(10, 500);

                // Random Connections 
                int connections = 1 + rndGen.Next(2);
                HashSet<int> connectionSet = new HashSet<int>();

                for (int c = 0; c < connections; c++)
                    connectionSet.Add(rndGen.Next(vertexAmount));

                // The ID is its position in the array
                Vertices[i] = new Vertex(i, new Vector(x, y), connectionSet);
            }

            // And now for some hacky magic:
            // Each connection goes both ways:
            for (int i = 0; i < vertexAmount; i++)
                for (int j = 1; j < vertexAmount; j++)
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

            bool[] closed = new bool[vertexAmount];
            var forcesDict = new Dictionary<int, Vector>(vertexAmount);

            for (int i = 0; i < vertexAmount; i++)
            {
                var vert = Vertices[i];

                foreach (int connection in vert.connectedVertexIDs)
                {
                    if (closed[connection])
                        continue;

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
            closed = new bool[vertexAmount];

            for (int i = 0; i < vertexAmount; i++)
            {
                if (closed[i])
                    continue;

                for (int j = i + 1; j < vertexAmount; j++)
                {
                    Vector oldForce = new Vector(0, 0);
                    Vector aForce = Algorithms.HCAttractive(Vertices[i], Vertices[j], aWeight);

                    forcesDict.TryGetValue(i, out oldForce);

                    forcesDict[i] = oldForce + aForce;
                    forcesDict[j] = oldForce - aForce;
                }
            }

            for (int i = 0; i < vertexAmount; i++)
            {
                if (forcesDict.ContainsKey(i))
                    Vertices[i].ApplyForce(forcesDict[i]);

                Console.WriteLine(Vertices[i].PositionVector);
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
            Console.WriteLine(qualityTest.GetEdgeCrossings(Vertices));
        }

        private void ApplyForcesButton_Click(object sender, EventArgs e)
        {
            this.updateForces();
            pictureBox1.Invalidate();
        }
    }
}
