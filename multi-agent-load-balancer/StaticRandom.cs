using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace multi_agent_load_balancer
{
    public static class StaticRandom
    {
        static int GetSeed()
        {
            return Environment.TickCount * Thread.CurrentThread.ManagedThreadId;
        }

        static readonly ThreadLocal<Random> random =
            new ThreadLocal<Random>(() => new Random(GetSeed()));

        public static int Next(int min, int max)
        {
            return random.Value.Next(min, max);
        }
    }
}
