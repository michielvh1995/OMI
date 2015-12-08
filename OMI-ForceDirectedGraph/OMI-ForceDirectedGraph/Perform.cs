#undef HC
#undef FR
#define Eades


using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace OMI_ForceDirectedGraph
{
    class Perform
    {
        private static Vertex[] GenerateVertices(int verticesAmt)
        {
            Vertex[] vertices = new Vertex[verticesAmt];

            for (int i = 0; i < verticesAmt; i++)
            {
                // Random Position
                int x = StaticRandom.Rand(100, 400);
                int y = StaticRandom.Rand(100, 400);

                // Random Connections 
                int connections = 1 + StaticRandom.Rand(0, 2);
                HashSet<int> connectionSet = new HashSet<int>();

                for (int c = 0; c < connections; c++)
                {
                    int conn = StaticRandom.Rand(0, verticesAmt);
                    if (conn != i)
                        connectionSet.Add(conn);
                }

                // The ID is its position in the array
                vertices[i] = new Vertex(i, new Vector(x, y), connectionSet);
            }

            // Each of the connections should go both ways.
            for (int i = 0; i < verticesAmt; i++)
                foreach (int connected in vertices[i].connectedVertexIDs)
                    vertices[connected].AddConnection(i);

            return vertices;
        }


        // Main function:
        private static Vertex[] updateForces(int VerticesAmt, Vertex[] Vertices, double aWeight, double rWeight, double k = 0)
        {
            // Works the same way as array, but can be used concurrently (as we're both reading and writing)
            var forcesDict = new Dictionary<int, Vector>();
            var closed = new Dictionary<int, bool>();

            // Needs to be instantiated to something
            for (int i = 0; i < VerticesAmt; i++)
                forcesDict[i] = new Vector(0, 0);

            for (int i = 0; i < VerticesAmt; i++)
                closed[i] = false;

#if FR
            // Omdat FurchtRein raar is:
            // radius <- rWeight,
            // rWeight == aWeight <- aWeight
            // k <- k
            Double FRConstant = Algorithms.FruchtReinConstant(k, rWeight);
#endif

            for (int i = 0; i < VerticesAmt; i++)
            {
                foreach (int connection in Vertices[i].connectedVertexIDs)
                {
                    if (closed[connection])
                        continue;

#if HC
                    Vector aForce = Algorithms.HCAttractive(Vertices[i], Vertices[connection], aWeight);
#elif FR

                    Vector aForce = new Vector(0, 0);//Algorithms.FruchtReinAttractive(Vertices[i], Vertices[connection], FRConstant, aWeight);
#elif Eades
                    Vector aForce = Algorithms.EadesAttractive(Vertices[i],Vertices[connection],aWeight,k);
#endif

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


#if HC
                    Vector rForce = Algorithms.HCRepulsive(Vertices[i], Vertices[j], rWeight);
#elif FR
                    Vector rForce = Algorithms.FruchtRein(Vertices[i], Vertices[j], k, rWeight, aWeight);
                         //Algorithms.FruchtReinRepulsive(Vertices[i], Vertices[j], FRConstant, aWeight);
#elif Eades
                    Vector rForce = Algorithms.EadesRepulsive(Vertices[i], Vertices[j], rWeight);
#endif
                    forcesDict[i] += rForce;
                }
                closed[i] = true;
            }

            // Apply the forces
            for (int i = 0; i < VerticesAmt; i++)
            {
                Vertices[i].ApplyForce(forcesDict[i]);
            }

            return Vertices;
        }

        // Loop over all possible combinations between aWeight and rWeight (delta a,r = 1)
        public static void ExecuteTests()
        {
            int verticesAmt = 10;

            var qualityVar = new ConcurrentBag<Tuple<double[], double[]>>();

            Parallel.For(1, 10, aw10 =>
            {
                double aW = (double)aw10 / 10;
                for (double rW = 0.1; rW < 1; rW += 0.1)
                {
                    Vertex[] vertices = GenerateVertices(verticesAmt);

                    for (int i = 0; i < 1000; i++)
                        vertices = updateForces(verticesAmt, vertices, aW, rW);

                    // And put the quality of the graph into a Tuple along with its a and r weights
                    Tuple<double[], double[]> outTuple = new Tuple<double[], double[]>(new[] { aW, rW }, QualityTest.TestAll(vertices));

                    qualityVar.Add(outTuple);

                    // Store the Graph in a file with the parameters
                    Save.SaveGraph(new[] { aW, rW }, vertices);
                }
            });

            // Save the string array into a file
            Save.SaveStrings(getQualitiesStrings(qualityVar));
        }

        // Turn a ConcurrentBag<Tuple..> into an array of strings
        private static string[] getQualitiesStrings(ConcurrentBag<Tuple<double[], double[]>> bag)
        {
            int size = bag.Count;
            string[] output = new string[size];

            for (int i = 0; i < size; i++)
            {
                Tuple<double[], double[]> fst;
                bag.TryTake(out fst);

                String qualityString = "aW " + fst.Item1[0] + "|aR " + fst.Item1[1] +
                   "|" + String.Join(",", fst.Item2) + ";";

                output[i] = qualityString;
            }
            return output;
        }

        // Loop over all possible combinations between k, aWeight and rWeight (delta a,r = 1)
        public static void ExecuteTestsTri()
        {
            int verticesAmt = 10;

            var qualityVar = new ConcurrentBag<Tuple<double[], double[]>>();

            Parallel.For(1, 10, aw10 =>
            {
                double aW = (double)aw10 / 10;
                for (double rW = 0.1; rW < 1; rW += 0.1)
                {
                    for (double k = 0.1; k < 1; k += 0.1)
                    {
                        Vertex[] vertices = GenerateVertices(verticesAmt);

                        // The amount of updateForces iterations
                        for (int i = 0; i < 1000; i++)
                            vertices = updateForces(verticesAmt, vertices, aW, rW, k);

                        // And put the quality of the graph into a Tuple along with its a and r weights
                        var outTuple = new Tuple<double[], double[]>(new[] { aW, rW, k }, QualityTest.TestAll(vertices));

                        qualityVar.Add(outTuple);

                        // Store the Graph in a file with the parameters
                        Save.SaveGraph(new[] { aW, rW, k }, vertices);
                    }
                }
            });

            // Save the string array into a file
            Save.SaveStrings(getQualitiesStringsTri(qualityVar));
        }

        private static string[] getQualitiesStringsTri(ConcurrentBag<Tuple<double[], double[]>> bag)
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

    public static class StaticRandom
    {
        static int seed = Environment.TickCount;

        static readonly ThreadLocal<Random> random =
            new ThreadLocal<Random>(() => new Random(Interlocked.Increment(ref seed)));

        public static int Rand(int minBound = 0, int maxBound = 1000)
        {
            return random.Value.Next(minBound, maxBound);
        }
    }
}
