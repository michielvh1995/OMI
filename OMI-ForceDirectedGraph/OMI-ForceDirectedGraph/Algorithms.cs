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

            Vector forceVector = r * (distance - 25);

            return aWeight * forceVector;
        }

        /// <summary>
        /// Calculates the translation for Vertex node1 based on the repulsive and attractive forces between it and node2.
        /// The attractive force is calculated according to a logarithmic variation on Hooke's law while the repulsive force is calculated according to Coulomb's law.
        /// The total translation for node1 is then the sum of the translations on node1 based on every other node.
        /// </summary>
        /// <param name="node1">The vertex to be translated</param>
        /// <param name="node2">The vertex that's interacting with node1 via repulsive and attractive forces</param>
        /// <param name="c1">A constant that determines the strength of the attractive force</param>
        /// <param name="c2">A constant that determines the logarithmic scaling factor of the attractive force</param>
        /// <param name="c3">A constant that determines the strength of the repulsive force</param>
        /// <param name="s">The ratio between the size of the translation and the size of the combined attractive and repulsive force</param>
        /// <returns>The translation vector to be applied to node1</returns>
        public static Vector EadesForce(Vertex node1, Vertex node2, double c1, double c2, double c3, double s)
        {
            Vector r = Vertex.VectorBetween(node2, node1);
            Vector rn = r;
            rn.Normalize();
            double d = r.Length;

            Vector fAtt = node1.ConnectedWith(node2) ? c1 * Math.Log(d / c2, 2) * rn : new Vector(0, 0);
            Vector fRep = (c3 / (d * d)) * rn;

            return s * (fAtt + fRep);
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

        /// <summary>
        /// Calculates the translation for Vertex node1 based on the repulsive and attractive forces between it and node2.
        /// The attractive and repulsive forces are calculated according to the optimal vertex distribution based on the algorithm of Fruchterman and Reingold
        /// The total translation for node1 is then the sum of the translations on node1 based on every other node.
        /// </summary>
        /// <param name="node1">The vertex to be translated</param>
        /// <param name="node2">The vertex that's interacting with node1 via repulsive and attractive forces</param>
        /// <param name="c">A constant that determines the weight of the optimal vertex distribution parameter</param>
        /// <param name="radius">A constant that determines the radius around the vertex to count the vertices in</param>
        /// <param name="s">The ratio between the size of the translation and the size of the combined attractive and repulsive force</param>
        /// <returns>The translation vector to be applied to node1</returns>
        public static Vector FruchtRein(Vertex node1, Vertex node2, double c, double radius, double s)
        {
            Vector r = Vertex.VectorBetween(node1, node2);
            Vector rn = r;
            rn.Normalize();
            double d = r.Length;

            double k = c * Math.Sqrt((Math.PI * radius * radius) / (1)); // Function to count number of objects in radius around Vertex v here
            Vector fAtt = node1.ConnectedWith(node2) ? ((d * d) / k) * rn : new Vector(0, 0);
            Vector fRep = (-(k * k) / d) * rn;

            return s * (fAtt + fRep);
        }

        public static double FruchtReinConstant(double c, double radius)
        {
            return c * Math.Sqrt((Math.PI * radius * radius) / (1));
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
        public double Cooling(double t0, int n, int iteration)
        {
            return Math.Pow(t0, 1 - iteration / n);
        }
    }
}