using System;
using System.Threading;

namespace OMI_ForceDirectedGraph
{
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