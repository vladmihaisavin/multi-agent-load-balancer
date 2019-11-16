using ActressMas;
using multi_agent_load_balancer.Agents.Base;
using multi_agent_load_balancer.Messaging.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace multi_agent_load_balancer.Agents
{
    public class ProcessorAgent : ExtendedConcurrentAgent
    {
        private string _outputDirectory;
        private Random _rand = new Random();
        private BlockingCollection<string> _pendingFiles = new BlockingCollection<string>(2); 
        public ProcessorAgent(string name) : base(name)
        {
        }

        public override void Setup()
        {
            var message = new CustomMessage
            {
                MessageContent = this.Name,
                Type = Messaging.MessageType.NewProcessor
            };
            Send("distributor", message);
            _outputDirectory = Path.Combine(RunSettings.WorkingDirectory,
                    RunSettings.Configuration["Directory.Results"]);
            if (!Directory.Exists(_outputDirectory))
            {
                Directory.CreateDirectory(_outputDirectory);
            }
            Task.Factory.StartNew(() => HandleFiles());
        }

        private void HandleFiles()
        {

            foreach (var filePath in _pendingFiles.GetConsumingEnumerable())
            {
                ProcessFile(filePath);
            }
        }

        public override void Act(Message message)
        {
            var custom = ParseMessage<CustomMessage>(message.Content);
            if(custom.Type == Messaging.MessageType.NewFileToProcess)
            {
                HandleNewFile(custom.MessageContent);
            }
        }

        private void HandleNewFile(string filePath)
        {
            if(_pendingFiles.Count < 2)
            {
                _pendingFiles.TryAdd(filePath);
                Console.WriteLine($"[{this.Name}] Enqueued a new file for processing.");
            }
            else
            {
                var custom = new CustomMessage
                {
                    Type = Messaging.MessageType.Overload,
                    MessageContent = filePath
                };
                Send("dispatcher", custom);
                Console.WriteLine($"[{this.Name}] Can't handle any more files. Sending to dispatcher...");
            }
        }

        private void ProcessFile(string filePath)
        {
            if (!File.Exists(filePath)) return;
            var charCounter = new Dictionary<char, int>();
            using (var sr = new StreamReader(filePath))
            {
                do
                {
                    var character = (char)sr.Read();
                    if(!charCounter.TryGetValue(character, out int currentValue))
                    {
                        currentValue = 0;
                    }
                    charCounter[character] = currentValue + 1;
                } while (!sr.EndOfStream);
                var fileName = Path.GetFileNameWithoutExtension(filePath);
                var outputFilePath = Path.Combine(_outputDirectory,$"{fileName}.json");
                File.WriteAllText(outputFilePath, JsonSerializer.Serialize(
                    charCounter.OrderBy(x=>x.Key)
                    .ToDictionary(k=>k.Key.ToString(),v=>v.Value)));
                Thread.Sleep(_rand.Next(0,200));
            }
        }
    }
}
