using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace OMI_ForceDirectedGraph
{
    static class Program
    {


        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Dit.... mag... maar niet zo.
            // Tests.ConductTests();
            Application.Run(new Form1());
        }
    }
}
