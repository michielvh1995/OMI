using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

/*
namespace WindowsFormsApplication1
{
    // Length == Strength
    class Algorithms
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="v"></param>
        /// <param name="w"></param>
        /// <param name="c1"></param>
        /// <param name="c2"></param>
        /// <param name="s"></param>
        /// <returns></returns>
        public Vector HookeCoulomb(Vertex v, Vertex w, double c1, double c2, double s)
        {
            Vector r = Vertex.VectorBetween(v, w);
            Vector rn = r;
            rn.Normalize();
            double d = r.Length;

            Vector fAtt = v.connections.Contains(w) ? rn * (-c1 * (d - 1)) : new Vector(0, 0);
            Vector fRep = (c2 / (d * d)) * rn;

            return s * (fAtt + fRep);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="v"></param>
        /// <param name="w"></param>
        /// <param name="c1"></param>
        /// <param name="c2"></param>
        /// <param name="c3"></param>
        /// <param name="s"></param>
        /// <returns></returns>
        public Vector Eades(Vertex v, Vertex w, double c1, double c2, double c3, double s)
        {
            Vector r = Vertex.VectorBetween(v, w);
            Vector rn = r;
            rn.Normalize();
            double d = r.Length;

            Vector fAtt = v.connections.Contains(w) ? c1 * Math.Log(d / c2, 2) * rn : new Vector(0, 0);
            Vector fRep = (c3 / (d * d)) * rn;

            return s * (fAtt + fRep);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="v"></param>
        /// <param name="w"></param>
        /// <param name="c"></param>
        /// <param name="radius"></param>
        /// <param name="s"></param>
        /// <returns></returns>
        public Vector FruchtRein(Vertex v, Vertex w, double c, double radius, double s)
        {
            Vector r = Vertex.VectorBetween(v, w);
            Vector rn = r;
            rn.Normalize();
            double d = r.Length;

            double k = c * Math.Sqrt((Math.PI * r * r) / (1)); // Function to count number of objects in radius around Vertex v here
            Vector fAtt = v.connections.Contains(w) ? ((d * d) / k) * rn : new Vector(0, 0);
            Vector fRep = (-(k * k) / d) * rn;

            return s * (fAtt + fRep);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="t0"></param>
        /// <param name="n"></param>
        /// <param name="iteration"></param>
        /// <returns></returns>
        public double Cooling(double t0, int n, int iteration)
        {
            return Math.Pow(t0, 1 - iteration / n);
        }
    }
}

*/

namespace OMI_ForceDirectedGraph
{
    internal class Algorithms
    {
        /// <summary>
        /// Calculates the translation for Vertex node1 based on the repulsive and attractive forces between it and node2.
        /// The attractive force is calculated according to Hooke's law while the repulsive force is calculated according to Coulomb's law.
        /// The total translation for node1 is then the sum of the translations on node1 based on every other node.
        /// </summary>
        /// <param name="node1">The vertex to be translated</param>
        /// <param name="node2">The vertex that's interacting with node1 via repulsive and attractive forces</param>
        /// <param name="c1">A constant that determines the strength of the attractive force</param>
        /// <param name="c2">A constant that determines the strength of the repulsive force</param>
        /// <param name="s">The ratio between the size of the translation and the size of the combined attractive and repulsive force</param>
        /// <returns>The translation vector to be applied to node1</returns>
        public static Vector HookeCoulombForce(Vertex node1, Vertex node2, double c1, double c2, double s)
        {
            Vector r = Vertex.VectorBetween(node2, node1);
            Vector rn = r;
            rn.Normalize();
            double d = r.Length;

            Vector fAtt = node1.ConnectedWith(node2) ? rn * (-c1 * (d - 1)) : new Vector(0, 0);
            Vector fRep = (c2 / (d * d)) * rn;

            return s * (fAtt + fRep);
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
