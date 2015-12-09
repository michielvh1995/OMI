using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace OMI_ForceDirectedGraph
{
    internal abstract class AbsPerform
    {
        internal Vertex[] GenerateVertices(int verticesAmt)
        {
            Vertex[] vertices = new Vertex[verticesAmt];

            for (int i = 0; i < verticesAmt; i++)
            {
                // Random Position
                int x = StaticRandom.Rand(100, 400);
                int y = StaticRandom.Rand(100, 400);

                // Random Connections 
                int connections = StaticRandom.Rand(0, 2);
                HashSet<int> connectionSet = new HashSet<int>();

                if (i > 0)
                    connectionSet.Add(StaticRandom.Rand(0, i - 1));

                for (int c = 0; c < connections; c++)
                {
                    int conn = StaticRandom.Rand(0, verticesAmt);
                    if (conn != i)
                        connectionSet.Add(conn);
                }

                // The ID is its position in the array
                vertices[i] = new Vertex(i, new Vector(x, y), connectionSet);
            }

            // Each of the connections should go both ways.
            for (int i = 0; i < verticesAmt; i++)
                foreach (int connected in vertices[i].connectedVertexIDs)
                    vertices[connected].AddConnection(i);

            return vertices;
        }

        // Main function:
        internal abstract Vertex[] UpdateForces(int VerticesAmt, Vertex[] Vertices, double aWeight, double rWeight,
            double k = 0);

        // Loop over all possible combinations between aWeight and rWeight (delta a,r = 1)
        public abstract void ExecuteTests();
        
        // Turn a ConcurrentBag<Tuple..> into an array of strings
        internal abstract string[] GetQualitiesStrings(ConcurrentBag<Tuple<double[], double[]>> bag);
    }

}
