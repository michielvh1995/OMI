using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace WindowsFormsApplication1
{
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

            double k = c * Math.Sqrt((Math.PI * r * r) / (1/*Function to count number of objects in radius around vertex v here*/));
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
