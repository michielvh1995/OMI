using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Forms;
using System.Drawing;

namespace OMI_ForceDirectedGraph
{
    internal class QualityTest
    {
        public static List<double> GetEdgeLengths(Vertex[] vertices)
        {
            List<double> edgeLengths = new List<double>();

            EdgeSet done = new EdgeSet();

            foreach (Vertex vertex in vertices)
            {
                foreach (var connectedVert in vertex.connectedVertexIDs)
                {
                    Vertex[] edge = { vertex, vertices[connectedVert] };
                    if (done.Add(edge))
                    {
                        double a = Math.Abs((float)edge[0].PositionVector.X - (float)edge[1].PositionVector.X);
                        double b = Math.Abs((float)edge[0].PositionVector.Y - (float)edge[1].PositionVector.Y);

                        double edgeLength = Math.Sqrt(Math.Pow(a, 2) + Math.Pow(b, 2));

                        edgeLengths.Add(edgeLength);
                    }
                }
            }

            return edgeLengths;
        }

        public static int GetEdgeCrossings(Vertex[] vertices)
        {
            int edgeCrossings = 0;
            Console.WriteLine("Getting edge crossings...");

            EdgeCrossingSet done = new EdgeCrossingSet();
            foreach (var vertex in vertices)
            {
                foreach (var connectedVert in vertex.connectedVertexIDs)
                {
                    Vertex[] edge = { vertex, vertices[connectedVert] };
                    foreach (var otherEdgeVert1 in vertices)
                    {
                        foreach (var otherEdgeVert2 in otherEdgeVert1.connectedVertexIDs)
                        {
                            Vertex[] otherEdge = { otherEdgeVert1, vertices[otherEdgeVert2] };
                            if (CheckCross(edge, otherEdge))
                            {
                                if (done.Add(edge, otherEdge)) edgeCrossings++;
                            }
                        }
                    }
                }
            }
            
            return edgeCrossings;
        }        

        // Check whether two line segments cross each other
        // Source: http://csharphelper.com/blog/2014/08/determine-where-two-lines-intersect-in-c/
        private static bool CheckCross(Vertex[] line1, Vertex[] line2)
        {
            PointF p1 = new PointF((float)line1[0].PositionVector.X, (float)line1[0].PositionVector.Y);
            PointF p2 = new PointF((float)line1[1].PositionVector.X, (float)line1[1].PositionVector.Y);
            PointF p3 = new PointF((float)line2[0].PositionVector.X, (float)line2[0].PositionVector.Y);
            PointF p4 = new PointF((float)line2[1].PositionVector.X, (float)line2[1].PositionVector.Y);

            if (p1 == p2 || p2 == p3 || p1 == p4 || p2 == p4 || p1 == p3 || p3 == p4)
                return false;

            // Get the segments' parameters.
            float dx12 = p2.X - p1.X;
            float dy12 = p2.Y - p1.Y;
            float dx34 = p4.X - p3.X;
            float dy34 = p4.Y - p3.Y;

            // Solve for t1 and t2
            float denominator = (dy12 * dx34 - dx12 * dy34);
            float t1 = ((p1.X - p3.X) * dy34 + (p3.Y - p1.Y) * dx34) / denominator;
            if (float.IsInfinity(t1))
            {
                return false;
            }

            float t2 = ((p3.X - p1.X) * dy12 + (p1.Y - p3.Y) * dx12) / -denominator;

            // The segments intersect if t1 and t2 are between 0 and 1.
            return ((t1 >= 0) && (t1 <= 1) && (t2 >= 0) && (t2 <= 1));
        }

        // Calculates the total amount of unique edges
        public static double getTotalEdges(Vertex[] vertices)
        {
            List<Tuple<int, int>> edgeList = new List<Tuple<int, int>>();
            Tuple<int, int> index, inverseIndex;

            for (int i = 0; i < vertices.Length; i++)
            {
                foreach(int id in vertices[i].connectedVertexIDs)
                {
                    index = Tuple.Create<int, int>(i, id);
                    inverseIndex = Tuple.Create<int, int>(id, i);
                    if (!edgeList.Contains(index) && !edgeList.Contains(inverseIndex))
                        edgeList.Add(index);
                }
            }

            return edgeList.Count;
        }

        // Calculates the standard deviation of a set of data
        private static double standardDeviation(double[] data)
        {
            int l = data.Length;
            double avg = data.Average();
            return Math.Sqrt(data.Select(d => (d - avg) * (d - avg)).Sum() / (l - 1));
        }

        // Calculates the dispersion of the edge lengths for each vertex using the coefficient of variation (standard deviation / median)
        // This is usually a value between 0 and 1, though it can be up to sqrt(n - 1) with n the size fo the data set
        public static double edgeLengthDispersion(Vertex[] vertices)
        {
            Dictionary<Tuple<int, int>, double> edgeDict = new Dictionary<Tuple<int, int>, double>();
            Tuple<int, int> index, inverseIndex;

            for (int i = 0; i < vertices.Length; i++)
            {
                foreach (int id in vertices[i].connectedVertexIDs)
                {
                    index = Tuple.Create<int, int>(i, id);
                    inverseIndex = Tuple.Create<int, int>(id, i);
                    if (!edgeDict.ContainsKey(index) && !edgeDict.ContainsKey(inverseIndex))
                        edgeDict.Add(index, Math.Abs(Vertex.VectorBetween(vertices[i], vertices[id]).Length));
                }
            }

            return standardDeviation(edgeDict.Values.ToArray()) / edgeDict.Values.Average();
        }

        // Calculates the length of the diagonal of the axis-aligned bounding box of a set of vertices
        private static double boundingBoxDiagonal(Vertex[] vertices)
        {
            double[] xPositions = vertices.Select(v => v.PositionVector.X).ToArray();
            double[] yPositions = vertices.Select(v => v.PositionVector.Y).ToArray();

            double minX = xPositions.Min();
            double maxX = xPositions.Max();
            double minY = yPositions.Min();
            double maxY = yPositions.Max();

            double deltaX = maxX - minX;
            double deltaY = maxY - minY;

            return Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
        }

        // Calculates the dispersion of the vertex densities (the amount of vertices in a fixed radius around the vertex)
        // for each vertex using the coefficient of variation (standard deviation / median).
        // This is usually a value between 0 and 1, though it can be up to sqrt(n - 1) with n the size of the data set
        public static double vertexDensityDispersion(Vertex[] vertices)
        {
            double radius = 0.2d * boundingBoxDiagonal(vertices) / 2d;
            double[] vertexCounts = new double[vertices.Length];

            for (int i = 0; i < vertexCounts.Length; i++ )
                foreach (Vertex w in vertices)
                    if (Math.Abs(Vertex.VectorBetween(vertices[i], w).Length) <= radius)
                        vertexCounts[i]++;

            return standardDeviation(vertexCounts) / vertexCounts.Average();
        }

        public static double qualityTest(Vertex[] vertices)
        {
            return GetEdgeCrossings(vertices) / getTotalEdges(vertices) +
                   edgeLengthDispersion(vertices) +
                   vertexDensityDispersion(vertices);
        }
    }

    // Class that stores a list of unique edge crossings
    // example:
    // EdgeCrossingSet varName = new EdgeCrossingSet();
    // varName.Add(edge1, edge2);
    //
    // *edge1 and edge2 are of type Vertex[] and each contain the two vertices of an edge.
    public class EdgeCrossingSet
    {
        List<HashSet<PointF>> set;

        public EdgeCrossingSet()
        {
            set = new List<HashSet<PointF>>();
        }

        public bool Add(Vertex[] Edge1, Vertex[] Edge2)
        {
            PointF[] edge1 = new PointF[2];
            PointF[] edge2 = new PointF[2];

            edge1 = VertToPoint(Edge1);
            edge2 = VertToPoint(Edge2);

            HashSet<PointF> crossing = new HashSet<PointF>();
            crossing.Add(edge1[0]);
            crossing.Add(edge1[1]);
            crossing.Add(edge2[0]);
            crossing.Add(edge2[1]);

            foreach (HashSet<PointF> storedCrossing in set)
            {
                if (crossing.SetEquals(storedCrossing))
                // Crossing already exists in set, so return false and don't add
                {
                    return false;
                }
            }

            set.Add(crossing);
            return true;
        }

        private PointF[] VertToPoint(Vertex[] input)
        {
            PointF[] edge = new PointF[2];

            edge[0].X = (float)input[0].PositionVector.X;
            edge[0].Y = (float)input[0].PositionVector.Y;
            edge[1].X = (float)input[1].PositionVector.X;
            edge[1].Y = (float)input[1].PositionVector.Y;

            return edge;
        }

        public void PrintSet()
        {
            Console.WriteLine("Set:");
            foreach (HashSet<PointF> storedCrossing in set)
            {
                Console.Write("[");
                foreach (var edge in storedCrossing)
                {
                    Console.Write("(" + edge.X + ", " + edge.Y + "), ");
                }
                Console.Write("]\n");
            }
        }
    }

    // Class that stores a list of unique edges
    public class EdgeSet
    {
        HashSet<PointF[]> set;

        public EdgeSet()
        {
            set = new HashSet<PointF[]>();            
        }

        public bool Add(Vertex[] Edge)
        {
            PointF[] addingEdge = VertToPoint(Edge);

            foreach (var edge in set)
            {
                if ((edge[0] == addingEdge[0] && edge[1] == addingEdge[1]) ||
                    (edge[1] == addingEdge[0] && edge[0] == addingEdge[1]))
                {
                    return false;
                }
            }

            set.Add(addingEdge);
            return true;
        }

        public void printSet()
        {
            foreach (var edge in set)
            {
                Console.WriteLine("(" + edge[0].X + ", " + edge[0].Y + ") - (" + edge[1].X + ", " + edge[1].Y + ")");
            }
        }

        private PointF[] VertToPoint(Vertex[] input)
        {
            PointF[] edge = new PointF[2];

            edge[0].X = (float)input[0].PositionVector.X;
            edge[0].Y = (float)input[0].PositionVector.Y;
            edge[1].X = (float)input[1].PositionVector.X;
            edge[1].Y = (float)input[1].PositionVector.Y;

            return edge;
        }
    }
}