# Message Net

Imagen a self configuring network that provides reliable messages, in order, that can support both greedy and broadcast
capabilities.  A reasonable queue system right?

The goal of this project is to make node to node(s) communication as simple as sending a message,
with all the tools to work with messages asynchronously.  Like simple networks, each node corporates as part of the network
of nodes.

Examples of application...
 - Any kind of service, container or VM base.
 - Functions or other server-less compute capability.


#### Goals
 - Self registrations based on security roles
 - Minimum configuration requirements
 - Support for asynchronously routing and eventing
 - Extensible model for supporting cross platform, focused on Azure & AWS
 - Support for no-down time roll outs of versions on clusters such as AKS.


#### Architecture Notes
 - Queue based communication.
 - Asynchronous routing (short & long).
 - Standard node identification (IDs) for custom network protocol.

Message Net is an organization of nodes in a graph.  Nodes are nodes and edges a one direction channel.

Each node has one input channel.  Node send messages to other node by sending a message that's node input channel.
Greedy scaled out patterns are handled by creating nodes with the same node id.

Note: channels that support broadcast (topic/subscribers)

#### QueueId

QueueId provides a network segmentation where node id is part of.

QueueId = "{namespace}/{networkId}/{nodeId}"

 - Namespace is the general namespace for one or more networks
 - Network id provide role segmentation such as "prod", "test", "experimentation", etc...
 - Node Id provides compute node's id, multiple nodes can have the same id for scaled out greedy patterns.

