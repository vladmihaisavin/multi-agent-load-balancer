using multi_agent_load_balancer.Agents.Base;
using multi_agent_load_balancer.Messaging.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace multi_agent_load_balancer.Agents
{
    public class HelperAgent : ProcessorAgent
    {
        public HelperAgent(string name) : base(name)
        {
        }

        public override void Setup()
        {
            HandleNewFile(this.Name);
            var message = new CustomMessage
            {
                Type = Messaging.MessageType.HelperFinishedWork,
                MessageContent = this.Name
            };
            Send("dispatcher", message);
        }
    }
}
