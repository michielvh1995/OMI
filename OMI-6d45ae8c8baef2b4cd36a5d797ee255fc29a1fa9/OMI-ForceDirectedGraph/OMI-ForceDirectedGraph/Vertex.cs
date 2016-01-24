using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace OMI_ForceDirectedGraph
{
    /// <summary>
    /// The Vertex class, used in calculations
    /// </summary>
    public class Vertex
    {
        public Vector PositionVector { get; private set; }
        public HashSet<int> connectedVertexIDs;         // Should be private, for debugging reasons it's not
        private readonly double mass;                   // Currently in use to ensure the nodes wont fly too far 

        // Allows for easier computations
        public int ID;


        public Vertex()
        {
        }

        // Constructor function
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
            this.PositionVector += (forceVector / this.mass);
        }

        // Check whether 2 nodes are connected
        public bool ConnectedWith(Vertex otherVertex)
        {
            return this.connectedVertexIDs.Contains(otherVertex.ID);
        }

        public bool ConnectedWith(int otherID)
        {
            return this.connectedVertexIDs.Contains(otherID);
        }

        // Get the amount of connections, this node has.
        public int GetConnectionCount()
        {
            return this.connectedVertexIDs.Count;
        }

        public override string ToString()
        {
            string connections = string.Join(",", connectedVertexIDs);

            return ID + ":" + this.PositionVector.X + "," + this.PositionVector.X + "|" + connections + ";";
        }


        // Add a new connection
        // Should not be used when the nodes are properly generated
        public void AddConnection(Vertex otherVertex)
        {
            this.connectedVertexIDs.Add(otherVertex.ID);
        }
        public void AddConnection(int otherID)
        {
            this.connectedVertexIDs.Add(otherID);
        }


        public static Vector VectorBetween(Vertex node1, Vertex node2)
        {
            return node2.PositionVector - node1.PositionVector;
        }
    }
}
