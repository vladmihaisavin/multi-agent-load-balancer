using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace multi_agent_load_balancer
{
    public static class RunSettings
    {
        static RunSettings()
        {
            WorkingDirectory = Directory.GetCurrentDirectory();
            var builder = new ConfigurationBuilder()
                .SetBasePath(WorkingDirectory)
                .AddJsonFile("appsettings.json");

            Configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", true, true)
                .Build();

        }
        public static string WorkingDirectory { get; } 
        public static int NumberOfWorkers => Convert.ToInt32(Configuration[nameof(NumberOfWorkers)]);
        public static int MinCharLength => Convert.ToInt32(Configuration["File.Generation.MinCharLength"]);
        public static int MaxCharLength => Convert.ToInt32(Configuration["File.Generation.MaxCharLength"]);
        public static IConfiguration Configuration { get; }
    }
}
