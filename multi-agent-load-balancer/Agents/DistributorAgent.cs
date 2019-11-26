using ActressMas;
using multi_agent_load_balancer.Agents.Base;
using multi_agent_load_balancer.Messaging.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Timers;
using Timer = System.Timers.Timer;

namespace multi_agent_load_balancer.Agents
{
    public class DistributorAgent : ExtendedConcurrentAgent
    {
        private Timer _timer = new Timer();
        private List<string> _workers = new List<string>();
        private int _fileIndex = 0;
        private string _pendingWork;

        public DistributorAgent(string name) : base(name)
        {
        }
        public override AgentType AgentType => AgentType.Distributor;

        public override void Setup()
        {
            _pendingWork = Path.Combine(RunSettings.WorkingDirectory,
                RunSettings.Configuration["Directory.PendingWork"]);
            if (!Directory.Exists(_pendingWork))
            {
                //create directory for generated files
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
            //assign a random length to the generated file file 
            var charLength = StaticRandom.Next(RunSettings.MinCharLength, RunSettings.MaxCharLength);
            while (text.Length < charLength)
            {
                text.Append((char)(StaticRandom.Next(0, 26)+'a'));
            }
            File.WriteAllText(filePath, text.ToString());
            //take a random processor agent 
            var processorIdx = StaticRandom.Next(0, _workers.Count);
            var custom = new CustomMessage
            {
                Type = Messaging.MessageType.NewFileToProcess,
                MessageContent = filePath,
            };
            var processorName = _workers.ElementAt(processorIdx);
            Send(processorName, custom);
            Log($"Sending a file {fileName} to processor {processorName}", filePath);
        }
        public override void Act(Message message)
        {
            var custom = ParseMessage<CustomMessage>(message.Content);
            if(custom.Type == Messaging.MessageType.NewProcessor)
            {
                _workers.Add(custom.MessageContent);
            }
        }
    }
}
