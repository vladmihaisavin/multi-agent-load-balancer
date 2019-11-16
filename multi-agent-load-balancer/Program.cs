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
            var dispatcher = new DispatcherAgent("dispatcher");
            var env = new ConcurrentEnvironment();
            env.Add(distributor);
            env.Add(dispatcher);
            distributor.Start();
            dispatcher.Start();
            for (int i = 1; i <= RunSettings.NumberOfWorkers; i++)
            {
                var processor = new ProcessorAgent($"p{i}");
                env.Add(processor);
                processor.Start();
            }
            env.WaitAll();
            //while (true) { }
        }
    }
}
