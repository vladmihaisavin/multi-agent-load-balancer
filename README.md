# multi-agent-load-balancer
A multi-agent application for load balancing in a distributed system

The system will count the characters from various randomly generated text files and output the results in a predefined directory.

The **DistributorAgent** will generate files and will randomly send their respective paths to the **ProcessorAgents**.

Each **ProcessorAgent** can handle one file at a time. When the ProcessorAgent is assigned more work than it can handle, it will send an appropriate message to the **DispatcherAgent**.

In order to counteract the imbalance, the **DispatcherAgent** will ask the other **ProcessorAgents** which of them can handle extra work (which one is working below their capacity).
- If there are replies, the **DispatcherAgent** will assign the extra work to the replying ProcessorAgents according to their remaining capacity.
- If there are no replies, the **DispatcherAgent** will generate **HelperAgents**, which can handle the operations made on one file.
A **ProcessorAgent** may be aided by multiple **HelperAgents** according to its work load. Enough **HelperAgents** should be generated so as to cover the work that
the **ProcessorAgents** can't handle on their own.