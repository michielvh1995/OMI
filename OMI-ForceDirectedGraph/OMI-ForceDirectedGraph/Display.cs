using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace OMI_ForceDirectedGraph
{
    public class Display
    {
        Pen pen1, pen2;
        SolidBrush brush;
        float nodeSize;

        public Display()
        {
            // Node visualization
            pen1 = new Pen(Color.Black, 1.5F);
            brush = new SolidBrush(Color.CornflowerBlue);
            nodeSize = 8f;

            // Connection visualization
            pen2 = new Pen(Color.Gray, 1.5F);
        }
        
        public void DrawGraph(Graphics g, Vertex[] vertices)
        {
            RectangleF node;

            foreach (var vertex in vertices)
            {
                // Draw the vertex
                PointF vertexPos = new PointF((float)vertex.PositionVector.X, (float)vertex.PositionVector.Y);
                node = new RectangleF(vertexPos.X - (nodeSize / 2f), vertexPos.Y - (nodeSize / 2f), nodeSize, nodeSize);
                g.FillEllipse(brush, node);
                g.DrawEllipse(pen1, node);

                // Draw the connections
                PointF connectedVertPos;
                foreach (var id in vertex.connectedVertexIDs)
                {
                    connectedVertPos = new PointF((float)vertices[id].PositionVector.X, (float)vertices[id].PositionVector.Y);
                    g.DrawLine(pen2, vertexPos, connectedVertPos);
                }
            }
        }
    }
}
