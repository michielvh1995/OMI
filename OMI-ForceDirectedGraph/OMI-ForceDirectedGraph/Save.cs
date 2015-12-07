using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.IO.Pipes;

namespace OMI_ForceDirectedGraph
{
    class Save
    {
        public static bool SaveGraph(double[] paramStrings, Vertex[] vertices)
        {
            for (int i = 0; i < 2; i++)
            {
                paramStrings[i] *= 10;
            }
            String fileName = "/graph" + String.Join("", paramStrings) + ".txt";

            Console.WriteLine(Directory.GetCurrentDirectory());


            // Check whether the file already exists, we don't want to overwrite it
            if (File.Exists(Directory.GetCurrentDirectory() + fileName))
                return false;

            String[] vertexStrings = new string[vertices.Length];

            for (int i = 0; i < vertices.Length; i++)
                vertexStrings[i] = vertices[i].ToString();

            File.WriteAllLines(Directory.GetCurrentDirectory() + fileName, vertexStrings);

            return true;
        }



    }
}
