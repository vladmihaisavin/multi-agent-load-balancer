using multi_agent_load_balancer.Agents.Base;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Timers;

namespace multi_agent_load_balancer.Agents
{
    public class DistributorAgent : ExtendedConcurrentAgent
    {
        public DistributorAgent(string name)
        {
            Name = name;
        }
        private Timer _timer = new Timer();
        private Dictionary<string, string> _aliveWorkers;
        private int _fileIndex = 0;
        private string _pendingWork;
        private Random _rand = new Random();
        public override void Setup()
        {
            _pendingWork = Path.Combine(RunSettings.WorkingDirectory,
                RunSettings.Configuration["Directory.PendingWork"]);
            if (!Directory.Exists(_pendingWork))
            {
                Directory.CreateDirectory(_pendingWork);
            }

            _timer.AutoReset = true;
            _timer.Interval = Convert.ToInt32(RunSettings.Configuration["File.Generation.MsInterval"]);
            _timer.Elapsed += GenerateFile;
            _timer.Start();
        }

        private void GenerateFile(object sender, ElapsedEventArgs e)
        {
            var fileName = $"file_{_fileIndex++}.txt";
            var filePath = Path.Combine(_pendingWork, fileName);
            var text = new StringBuilder();
            var charLength = _rand.Next(RunSettings.MinCharLength, RunSettings.MaxCharLength);
            while (text.Length < charLength)
            {
                text.Append((char)(_rand.Next(0, 26)+'a'));
            }
            File.AppendAllText(filePath, text.ToString());
        }
    }
}
