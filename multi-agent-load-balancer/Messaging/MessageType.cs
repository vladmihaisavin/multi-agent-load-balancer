using System;
using System.Collections.Generic;
using System.Text;

namespace multi_agent_load_balancer.Messaging
{
    public enum MessageType
    {
        NewFileToProcess,
        NewProcessor,
        Overload,
        CheckAvailability,
        HelperFinishedWork
    }
}
