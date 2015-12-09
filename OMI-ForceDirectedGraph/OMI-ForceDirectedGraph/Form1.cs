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
using System.Diagnostics;
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

        
        // Displaying the vertices
        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            return;
        }

        private void GenerateButton_Click(object sender, EventArgs e)
        {
        }

        private void ApplyForcesButton_Click(object sender, EventArgs e)
        {
        }

        // Used when there are only 2 vars required for the tests
        private void button1_Click(object sender, EventArgs e)
        {
            // Parallelised the execution of the tests.
            Stopwatch timer = Stopwatch.StartNew();
            
            PerformHC HookeCoulomb = new PerformHC();
            HookeCoulomb.ExecuteTests();

            Console.WriteLine(timer.ElapsedMilliseconds + " millis");
            timer.Stop();
        }

        // Used when you need 3 input vars for the tests
        private void button2_Click(object sender, EventArgs e)
        {
            // Parallelised the execution of the tests.
            Stopwatch timer = Stopwatch.StartNew();
            
            PerformEades Eades = new PerformEades();
            Eades.ExecuteTests();
            
            Console.WriteLine(timer.ElapsedMilliseconds + " millis");
            timer.Stop();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            // Parallelised the execution of the tests.
            Stopwatch timer = Stopwatch.StartNew();

            PerformFR FruchtmanReingold = new PerformFR();
            FruchtmanReingold.ExecuteTests();

            Console.WriteLine(timer.ElapsedMilliseconds + " millis");
            timer.Stop();
        }
    }
}
