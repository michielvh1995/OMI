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

        // A total of up to 25 vertices are allowed in the graph
        internal Vertex[] Vertices = new Vertex[25];
        private Random rndGen = new Random();

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
            for (int i = 0; i < 25; i++)
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
            for (int i = 0; i < 25; i++)
            {
                // Random Position
                int x = rndGen.Next(10, 500);
                int y = rndGen.Next(10, 500);

                // Random Connections 
                int connections = 1 + rndGen.Next(2);
                HashSet<int> connectionSet = new HashSet<int>();

                for (int c = 0; c < connections; c++)
                    connectionSet.Add(rndGen.Next(25));

                // The ID is its position in the array
                Vertices[i] = new Vertex(i, new Vector(x, y), connectionSet);
            }

            // And now for some hacky magic:
            // Each connection goes both ways:
            for (int i = 0; i < 25; i++)
                for (int j = i; j < 25; j++)
                    if (Vertices[i].ConnectedWith(Vertices[j]))
                    {
                        Vertices[j].AddConnection(Vertices[i]);
                        if(Vertices[j].ConnectedWith(Vertices[i]) && !Vertices[i].ConnectedWith(Vertices[j]))
                            Console.WriteLine("HELP");
                    }
        }


        // Main function:
        private void updateForces()
        {
            bool[] closed = new bool[25];
            for (int i = 0; i < 5; i++)
            {
                var vert = Vertices[i];
                foreach (var connection in vert.connectedVertexIDs)
                {
                    if (closed[connection])
                        continue;

                    // Roep de bereken functies aan voor de forces
                    var force = new Vector(1, 1); // Algorithms.EadesForce(null, null, 0, 0, 0, 0);

                    vert.ApplyForce(force);
                    Vertices[connection].ApplyForce(-force);
                }
                closed[i] = true;
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
        }

        private void ApplyForcesButton_Click(object sender, EventArgs e)
        {
            this.updateForces();
            pictureBox1.Invalidate();
        }
    }
}
