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

        // Display object for drawing the graphs
        private readonly Display display = new Display();

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
            for (int i = 0; i < Tests.VerticesAmt; i++)
            {
                foreach (int connection in Tests.Vertices[i].connectedVertexIDs)
                {
                    if (!Tests.Vertices[connection].ConnectedWith(i))
                        worker = false;
                }
            }
            Console.WriteLine(worker);

            int cnt = 0;
            for (int i = 0; i < Tests.VerticesAmt; i++)
            {
                for (int j = i + 1; j < Tests.VerticesAmt; j++)
                {
                    if (Tests.Vertices[i].ConnectedWith(j))
                        cnt++;
                }
            }
            Console.WriteLine(cnt);

            foreach (var v in Tests.Vertices)
            {
                Console.Write(v.GetConnectionCount() + " ");
            }
            Console.WriteLine();
        }

        // Displaying the vertices
        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            if (Tests.Vertices[0] == null) return;

            display.DrawGraph(e.Graphics, Tests.Vertices);
        }

        private void GenerateButton_Click(object sender, EventArgs e)
        {
            Tests.GenerateVertices(Tests.VerticesAmt);
            pictureBox1.Invalidate();

#if DEBUG   // Testing
            this.testFunctions();
#endif
        }

        private void ApplyForcesButton_Click(object sender, EventArgs e)
        {
            // Calculate and apply the forces n times, where n = maxIterations
            for (int i = 0; i < Tests.maxIterations; i++)
                Tests.UpdateForces(Tests.AlgorithmType.HookeCoulomb);
            pictureBox1.Invalidate();

#if DEBUG   // Testing
            for (int i = 0; i < Tests.VerticesAmt; i++)
                Console.WriteLine(Tests.Vertices[i].PositionVector);
            Console.WriteLine();
#endif
            Console.WriteLine(QualityTest.GetEdgeCrossings(Tests.Vertices));

            // Store the Graph in a file with the parameters
            Console.WriteLine(Save.SaveGraph(new[] { Tests.aWeight, Tests.rWeight }, Tests.Vertices));
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Parallelised the execution of the tests.
            Parallel.For(0, 10, aw10 =>
            {
                double aW = (double)aw10 / 10;
                for (double rW = 0; rW < 1; rW += 1)
                {
                    Tests.GenerateVertices(Tests.VerticesAmt);

                    for (int i = 0; i < 100; i++)
                        Tests.UpdateForces(Tests.AlgorithmType.HookeCoulomb);

                    Console.WriteLine(QualityTest.GetEdgeCrossings(Tests.Vertices));

                    // Store the Graph in a file with the parameters
                    Console.WriteLine(Save.SaveGraph(new[] { Tests.aWeight, Tests.rWeight }, Tests.Vertices));
                }
            });

        }
    }
}
