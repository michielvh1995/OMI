using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;

namespace OMI_ForceDirectedGraph
{
    internal class Algorithms
    {
        /// <summary>
        /// Calculate the repulsive force between two vertices.
        /// This is done using Coulomb's Algorithm
        /// </summary>
        /// <param name="node1"></param>
        /// <param name="node2"></param>
        /// <param name="rWeight"></param>
        /// <returns></returns>
        public static Vector HCRepulsive(Vertex node1, Vertex node2, double rWeight)
        {
            // The vector between the two vertices (basically the line connecting them)
            Vector r = Vertex.VectorBetween(node1, node2);
            double distance = Math.Abs(r.Length);
            r.Normalize();

            Vector forceVector = -r / (distance * distance);

            return rWeight * forceVector;
        }

        /// <summary>
        /// Calculate the attractive force between two vertices.
        /// This is done using Hooke's Algorithm
        /// </summary>
        /// <param name="node1"></param>
        /// <param name="node2"></param>
        /// <param name="aWeight"></param>
        /// <returns></returns>
        public static Vector HCAttractive(Vertex node1, Vertex node2, double aWeight)
        {
            Vector r = Vertex.VectorBetween(node1, node2);
            double distance = Math.Abs(r.Length);
            r.Normalize();

            Vector forceVector = r * (distance - 1);

            return aWeight * forceVector;
        }

        public static Vector EadesRepulsive(Vertex node1, Vertex node2, double rWeight)
        {
            Vector r = Vertex.VectorBetween(node2, node1);
            double distance = Math.Abs(r.Length);
            r.Normalize();

            Vector forceVector = r / (distance * distance);

            return rWeight * forceVector;
        }

        public static Vector EadesAttractive(Vertex node1, Vertex node2, double aWeight, double aWeight2)
        {
            Vector r = Vertex.VectorBetween(node2, node1);
            double distance = Math.Abs(r.Length);
            r.Normalize();

            Vector forceVector = r * Math.Log(distance / aWeight2, 2);

            return aWeight * forceVector;
        }

        private static double CountVertices(Vertex node, Vertex[] graph, double radius)
        {
            int count = 0;
            foreach (Vertex v in graph)
                if (Math.Abs(Vertex.VectorBetween(node, v).Length) <= radius)
                    count++;

            return count;
        }

        public static double FruchtReinConstant(Vertex node, Vertex[] graph, double c, double radius)
        {
            return c * Math.Sqrt((Math.PI * radius * radius) / CountVertices(node, graph, radius));
        }

        public static Vector FruchtReinRepulsive(Vertex node1, Vertex node2, double k, double weight)
        {
            Vector r = Vertex.VectorBetween(node1, node2);
            double distance = Math.Abs(r.Length);
            r.Normalize();

            Vector forceVector = r * -(k * k) / distance;

            return weight * forceVector;
        }

        public static Vector FruchtReinAttractive(Vertex node1, Vertex node2, double k, double weight)
        {
            Vector r = Vertex.VectorBetween(node1, node2);
            double distance = Math.Abs(r.Length);
            r.Normalize();

            Vector forceVector = r * (distance * distance) / k;

            return weight * forceVector;
        }

        /// <summary>
        /// Calculates the ceiling for the size of the translation from the Fruchterman-Reingold algorithm based on how many times
        /// the algorithm has been iterated, how many times it will be iterated and a starting value.
        /// This starting value is lowered each iteration according to a hyperbolic scheme, meaning that it'll lower very quickly
        /// at first and very slowly towards the final few iterations.
        /// </summary>
        /// <param name="t0">The starting ceiling value for the cooling</param>
        /// <param name="n">The total number of planned iterations</param>
        /// <param name="iteration">The current iteration</param>
        /// <returns>A ceiling value for the size of the translation calculated from the Fruchterman-Reingold algorithm</returns>
        public static double Cooling(double t0, int n, int iteration)
        {
            return Math.Pow(t0, 1 - iteration / n);
        }
    }
}