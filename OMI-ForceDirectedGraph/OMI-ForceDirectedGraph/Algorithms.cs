using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace WindowsFormsApplication1
{
    class Algorithms
    {
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

        public double Cooling(double t0, int n, int iteration)
        {
            return Math.Pow(t0, 1 - iteration / n);
        }
    }
}
