using ActressMas;
using multi_agent_load_balancer.Agents;
using System;
using System.IO;

namespace multi_agent_load_balancer
{
    class Program
    {
        static void Main(string[] args)
        {
            var distributor = new DistributorAgent("distributor");
            var env = new ConcurrentEnvironment();
            env.Add(distributor);
            distributor.Start();
            env.WaitAll();
            //while (true) { }
        }
    }
}
