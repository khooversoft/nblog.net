# XRun

XRun is a orchestrator that executes activities in a pipeline.  Multiple pipelines can be packaged as a
workflow and can be executed across platforms and environments.  XRun can be embedded into executables
such as command line, web sites, micro services, etc...

Examples of applications...

- Development tools for build and executing multiple processes / systems for debugging, performance stress testing, etc...
- Running release processes and integration testing in DevOps build servers.
  - Can be used with MSTest, nUnit, xUnit, scripts
- Run as a service - listen for file modifying events, HTTP calls, etc..., and executes a specific pipeline when a file is changed.


#### Goals / Strategy:
- Easier to develop then PowerShell scripts
- Functional language style (DSL)
- Extensible through PowerShell or .NET Core assemblies
- Work with Azure and AWS
- Hosting environments
  - .NET Core (NuGet)
  - Containers
  - Azure App Services / Functions    


#### Pipeline
A pipeline is one or more activities.  There are multiple types of activities.

1) Call operator, runs a command in a child process.
2) Call PowerShell operator, runs a script block.
3) Input, from keyboard, queue, etc...
4) Flow control, switch, for-each, and while.
5) Trigger, actively listening for OS/App specific events for executing pipelines.

A pipeline is a graph with directional edges between nodes.  Activity are nodes in the graph
and edges are ownership or dependency. Edges can have constraints to specific values such
as the result of a dependent activity (node).


#### Trigger
Trigger can start a pipeline or provide input to an activity.  Activities can wait for input, where triggers are one type.

Some types of triggers...
 - Calendar
 - Frequency timer (such as every n minutes)
 - HTTP call
 - Queue message available
 - File(s) updated


#### Workflow Package
A self contained package providing all dependencies required to run.

1) All binaries, scripts, data, etc..
2) Workflow / pipeline plans
3) Environment specific configurations, including resources such as key vault.