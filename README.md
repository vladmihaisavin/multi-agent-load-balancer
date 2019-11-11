# multi-agent-load-balancer
A multi-agent application for load balancing in a distributed system

The system will process large data (i.e. encrypt a document, perform large matrix operations, various statistical calculations based on experimental data, etc.). 
Each component of the distributed system will be represented by a ProcessorAgent.
Each processor agent can handle a specific work load.

The system will contain two more types of agents:

The DistributorAgent will get the input data and randomly assign tasks to each ProcessorAgent. Obviously,
this will lead to an imbalance, as some ProcessorAgents may be assigned more work than others. Some of
the ProcessorAgents may be assigned more work than they can handle, in which case they will send an
appropriate message to the dispatcher. In order to counteract the imbalance, the DispatcherAgent will ask
the other ProcessorAgents which of them can handle extra work (which is working below their capacity).
- If there are replies, the Dispatcher will assign the extra work to the replying ProcessorAgents
according to their remaining capacity.
- If there are no replies, the Dispatcher will generate HelperAgents, which can perform a fixed (and
small) number of operations. Depending on the imbalance, HelperAgents will take over some of the
tasks assigned to the ProcessorAgents. A ProcessorAgent may be aided by multiple HelperAgents
according to its work load. Enough HelperAgents should be generated so as to cover the work that
the ProcessorAgents can't handle on their own.

Each agent should display complete details on their status, the processed data, generated agents,
operations distributed / carried out, where appropriate.
