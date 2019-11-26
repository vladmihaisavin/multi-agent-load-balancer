using ActressMas;
using multi_agent_load_balancer.Agents.Base;
using multi_agent_load_balancer.Messaging.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace multi_agent_load_balancer.Agents
{
    public class DispatcherAgent : ExtendedConcurrentAgent
    {
        private List<string> _workers = new List<string>();
        private List<HelperAgent> _helpers = new List<HelperAgent>();

        private Dictionary<string, Dictionary<string, bool>> _workerAvailabilityPerFile = new Dictionary<string, Dictionary<string, bool>>();
        public DispatcherAgent(string name) : base(name)
        {
        }
        public override AgentType AgentType => AgentType.Dispatcher;
        public override void Act(Message message)
        {
            if(message.ConversationId == ((int) Messaging.MessagingConversationId.AvailabilityMessage).ToString())
            {
                var answer = ParseMessage<AvailabilityMessage>(message.Content);
                if(!_workerAvailabilityPerFile.TryGetValue(answer.FilePath,out Dictionary<string, bool> workerAnswers))
                {
                    workerAnswers = new Dictionary<string, bool>();
                }
                //save answer
                workerAnswers[message.Sender] = answer.IsAvailable;
                _workerAvailabilityPerFile[answer.FilePath] = workerAnswers;
                //wait for all the other agents to answer
                HandleAnswers(answer.FilePath);
            }
            else
            {
                var custom = ParseMessage<CustomMessage>(message.Content);
                if (custom.Type == Messaging.MessageType.NewProcessor)
                {
                    _workers.Add(custom.MessageContent);
                }
                else if (custom.Type == Messaging.MessageType.Overload)
                {
                    var otherAgents = _workers.Where(x => x != message.Sender);
                    var question = new CustomMessage
                    {
                        Type = Messaging.MessageType.CheckAvailability,
                        MessageContent = custom.MessageContent
                    };
                    foreach (var agent in otherAgents)
                    {
                        //ask the other agents if they can handle this file
                        Send(agent, question);
                        Log($"Checking {agent} availability...", custom.MessageContent);
                    }
                }
                else if (custom.Type == Messaging.MessageType.HelperFinishedWork)
                {
                    var helper = _helpers.FirstOrDefault(x => x.Name == custom.MessageContent);
                    helper?.Stop();
                    Log("Killed helper", custom.MessageContent, ConsoleColor.Red);
                }
            }
        }

        private void HandleAnswers(string filePath)
        {
            //take answers we saved for a specific file path
            var allAnswers = _workerAvailabilityPerFile[filePath] ?? new Dictionary<string, bool>();
            //if there are less than workers count - 1 then it means we didn't receive all the answers yet
            if(allAnswers.Count < _workers.Count - 1){ return; }
            //get the workers who answered positively
            var availableWorkers = allAnswers.Where(x => x.Value).Select(x => x.Key).ToList();
            if(availableWorkers.Count > 0)
            {
                var randomIdx = StaticRandom.Next(0, availableWorkers.Count);
                var agentToSend = availableWorkers.ElementAt(randomIdx);
                var newFileToProcess = new CustomMessage
                {
                    Type = Messaging.MessageType.NewFileToProcess,
                    MessageContent = filePath
                };
                //send the file to a randomly selected worker from the available workers
                Send(agentToSend, newFileToProcess);
                Log($"Redirecting the file to {agentToSend}", filePath, ConsoleColor.Magenta);
            }
            else
            {
                Log("Create helper agent", filePath, ConsoleColor.Green);
                var helper = new HelperAgent(filePath);
                _helpers.Add(helper);
                //add helper agent to the environment
                this.Environment.Add(helper);
                helper.Start();
            }

        }
    }
}
