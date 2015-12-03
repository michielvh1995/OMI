using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace OMI_ForceDirectedGraph
{
    internal class QualityTest
    {
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
}