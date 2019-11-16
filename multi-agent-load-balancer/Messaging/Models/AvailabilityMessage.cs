using System;
using System.Collections.Generic;
using System.Text;

namespace multi_agent_load_balancer.Messaging.Models
{
    class AvailabilityMessage
    {
        public string FilePath { get; set; }
        public bool IsAvailable { get; set; }
    }
}
