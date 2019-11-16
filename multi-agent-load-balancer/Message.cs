using System;
using System.Collections.Generic;
using System.Text;

namespace multi_agent_load_balancer
{
    public class Message
    {
        public MessageType Type { get; set; }
        public string Content { get; set; }
    }
}
