using System.Windows;

namespace OMI_ForceDirectedGraph
{
    // Using a Vector3D class we can return a 2D vector (direction) and a force in the force-algorithms
    internal class Vector3D
    {
        private double X;
        private double Y;
        private double Z;

        protected Vector3D(double x, double y, double z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        protected Vector3D(Vector xyVector, double z)
        {
            this.X = xyVector.X;
            this.Y = xyVector.Y;
            this.Z = z;
        }
    }
}