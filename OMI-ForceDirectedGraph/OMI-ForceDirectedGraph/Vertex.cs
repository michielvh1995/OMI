using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace WindowsFormsApplication1
{
    class Vertex
    {
        public Vector coordinates;
        public List<Vertex> connections;

        public Vertex(double x, double y)
        {
            coordinates = new Vector(x, y);
            connections = new List<Vertex>();
        }

        public Vertex(Vector coordinates)
        {
            this.coordinates = coordinates;
            connections = new List<Vertex>();
        }

        public static Vector VectorBetween(Vertex from, Vertex to)
        {
            return Vertex.VectorBetween(from.coordinates, to.coordinates);
        }

        public static Vector VectorBetween(Vector from, Vector to)
        {
            return to - from;
        }
    }
}
