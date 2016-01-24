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
        // Stores a graph in .txt format
        public static bool SaveGraph(double[] paramStrings, Vertex[] vertices) //, int[] qualityValues)
        {
            double[] nameDoubles = new double[paramStrings.Length];

            for (int i = 0; i < paramStrings.Length; i++)
                nameDoubles[i] = paramStrings[i] * 10;

            String fileName = "/output/graph" + String.Join("", nameDoubles) + ".txt";

            // Check whether the file already exists, we don't want to overwrite it
            if (File.Exists(Directory.GetCurrentDirectory() + fileName))
                return false;

            String[] vertexStrings = new string[vertices.Length];

            for (int i = 0; i < vertices.Length; i++)
                vertexStrings[i] = vertices[i].ToString();

            File.WriteAllLines(Directory.GetCurrentDirectory() + fileName, vertexStrings);

            return true;
        }

        public static bool SaveQuality(double[] aWeights, double[] rWeights, double[][] paramDoubles)
        {
            String fileName = "/output/qualities.txt";

            // Check whether the file already exists, we don't want to overwrite it
            if (File.Exists(Directory.GetCurrentDirectory() + fileName))
                return false;

            String[] qualityStrings = new string[aWeights.Length];

            for (int i = 0; i < aWeights.Length; i++)
            {
                qualityStrings[i] = "aW " + aWeights[i] + "|rW " + rWeights[i] + String.Join(",", paramDoubles[i]);
            }
            File.WriteAllLines(Directory.GetCurrentDirectory() + fileName, qualityStrings);

            return true;
        }

        public static bool SaveStrings(string[] strings)
        {
            String fileName = "/output/qualities.txt";

            // Check whether the file already exists, we don't want to overwrite it
            if (File.Exists(Directory.GetCurrentDirectory() + fileName))
                return false;

            File.WriteAllLines(Directory.GetCurrentDirectory() + fileName, strings);

            return true;
        }



    }
}
