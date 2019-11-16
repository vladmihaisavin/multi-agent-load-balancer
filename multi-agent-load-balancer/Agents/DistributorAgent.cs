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
        private Random _rand;

        public DistributorAgent(string name) : base(name)
        {
            Thread.Sleep(2);
            _rand = new Random(DateTime.Now.Millisecond);
        }

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
            File.WriteAllText(filePath, text.ToString());
            var processorIdx = _rand.Next(0, _workers.Count);
            //wait till we have at least one processor agent
            while(_workers.Count == 0) { Thread.Sleep(100); }
            var custom = new CustomMessage
            {
                Type = Messaging.MessageType.NewFileToProcess,
                MessageContent = filePath,
            };
            var processorName = _workers.ElementAt(processorIdx);
            Send(processorName, custom);
            Console.WriteLine($"[Distributor] Sending a file {fileName} to processor {processorName}");
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
