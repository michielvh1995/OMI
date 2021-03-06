﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;

namespace OMI_ForceDirectedGraph
{
    internal class PerformEades : AbsPerform
    {
        internal override Vertex[] UpdateForces(int VerticesAmt, Vertex[] Vertices, double aWeight, double rWeight, double k = 0)
        {
            // Works the same way as array, but can be used concurrently (as we're both reading and writing)
            var forcesDict = new Dictionary<int, Vector>();
            var closed = new Dictionary<int, bool>();

            // Needs to be instantiated to something
            for (int i = 0; i < VerticesAmt; i++)
                forcesDict[i] = new Vector(0, 0);

            for (int i = 0; i < VerticesAmt; i++)
                closed[i] = false;


            for (int i = 0; i < VerticesAmt; i++)
            {
                foreach (int connection in Vertices[i].connectedVertexIDs)
                {
                    if (closed[connection])
                        continue;

                    Vector aForce = Algorithms.EadesAttractive(Vertices[i], Vertices[connection], aWeight, k);

                    forcesDict[i] += aForce;
                }

                closed[i] = true;
            }


            // Add Repulsive Forces Into the mix:
            for (int i = 0; i < VerticesAmt; i++)
                closed[i] = false;

            for (int i = 0; i < VerticesAmt; i++)
            {
                for (int j = 0; j < VerticesAmt; j++)
                {
                    if (closed[j] || i == j)
                        continue;

                    Vector rForce = Algorithms.EadesRepulsive(Vertices[i], Vertices[j], rWeight);

                    forcesDict[i] += rForce;
                }
                closed[i] = true;
            }

            // Apply the forces
            for (int i = 1; i < VerticesAmt; i++)
                Vertices[i].ApplyForce(forcesDict[i]);

            return Vertices;
        }

        public override void ExecuteTests()
        {
            int verticesAmt = 20;

            var qualityVar = new ConcurrentBag<Tuple<double[], double[]>>();

            Parallel.For(1, 10, aw10 =>
            {
                double aW = (double)aw10 / 10;
                for (double rW = 0.1; rW < 1; rW += 0.1)
                {
                    for (double k = 0.1; k < 1; k += 0.1)
                    {
                        Vertex[] vertices = base.GenerateVertices(verticesAmt);

                        // The amount of UpdateForces iterations
                        for (int i = 0; i < 1000; i++)
                            vertices = UpdateForces(verticesAmt, vertices, aW, rW, k);

                        // And put the quality of the graph into a Tuple along with its a and r weights
                        var outTuple = new Tuple<double[], double[]>(new[] { aW, rW, k }, QualityTest.TestAll(vertices));

                        qualityVar.Add(outTuple);

                        // Store the Graph in a file with the parameters
                        Save.SaveGraph(new[] { aW, rW, k }, vertices);
                    }
                }
            });

            // Save the string array into a file
            Save.SaveStrings(GetQualitiesStrings(qualityVar));
        }

        internal override string[] GetQualitiesStrings(ConcurrentBag<Tuple<double[], double[]>> bag)
        {
            int size = bag.Count;
            string[] output = new string[size];

            for (int i = 0; i < size; i++)
            {
                Tuple<double[], double[]> fst;
                bag.TryTake(out fst);

                String qualityString = "aW " + fst.Item1[0] + "|aR " + fst.Item1[1] +
                                       "|k " + fst.Item1[2] + "|" + String.Join(",", fst.Item2) + ";";

                output[i] = qualityString;
            }
            return output;
        }
    }
}