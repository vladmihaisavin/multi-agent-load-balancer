using System;
using System.Collections.Generic;
using System.Text;

namespace multi_agent_load_balancer.Messaging.Models
{
    public class CustomMessage
    {
        public MessageType Type { get; set; }
        public string MessageContent { get; set; }
    }
}
