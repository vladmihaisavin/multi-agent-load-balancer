using System;
using System.Collections.Generic;
using System.Text;

namespace multi_agent_load_balancer.Messaging.Models
{
    public class Message
    {
        public MessageType Type { get; set; }
        public string Content { get; set; }
    }
}
