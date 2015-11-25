using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace OMI_ForceDirectedGraph
{
    /// <summary>
    /// The Vertex class, used in calculations
    /// 
    /// What is this? An HashSet!? That's right! It's a list with only unique values, also, it's OP as fuck.
    /// </summary>
    public class Vertex
    {
        public Vector PositionVector { get; private set; }
        public HashSet<int> connectedVertexIDs;
        private readonly double mass;

        // Allows for easier computations
        public int ID;

        // Constructors
        public Vertex(int id)
        {
            this.ID = id;
            this.mass = 1;
        }

        public Vertex(int id, Vector position)
        {
            this.ID = id;
            this.PositionVector = position;
            this.mass = 1;
        }

        public Vertex(int id, Vector position, HashSet<int> connections)
        {
            this.ID = id;
            this.PositionVector = position;
            this.connectedVertexIDs = connections;
            this.mass = 1;
        }

        // Apply force function, requires the addition vector
        public void ApplyForce(Vector forceVector)
        {
            this.PositionVector += forceVector / this.mass;
        }

        // Check whether 2 nodes are connected
        public bool ConnectedWith(Vertex otherVertex)
        {
            return this.connectedVertexIDs.Contains(otherVertex.ID);
        }

        public int GetConnectionCount()
        {
            return this.connectedVertexIDs.Count;
        }


        public void AddConnection(Vertex otherVertex)
        {
            this.connectedVertexIDs.Add(otherVertex.ID);
        }
    }
}
