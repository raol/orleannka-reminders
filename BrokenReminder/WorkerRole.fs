namespace BrokenReminder

open System
open System.Collections.Generic
open System.Diagnostics
open System.Linq
open System.Net
open System.Reflection
open System.Threading
open Microsoft.WindowsAzure
open Microsoft.WindowsAzure.Diagnostics
open Microsoft.WindowsAzure.ServiceRuntime

open Orleankka
open Orleankka.Cluster

open Orleans
open Orleans.Runtime.Configuration

open ReminderActor

type Bootstrapper() = 
    interface IBootstrapper with
        member this.Run(properties:obj) = 
            let actor = ClusterActorSystem.Current.ActorOf<ReminderActor>("TestReminder")
            actor.Tell("")

type WorkerRole() =
    inherit RoleEntryPoint() 

    // This is a sample worker implementation. Replace with your logic.

    let log message (kind : string) = Trace.TraceInformation(message, kind)
    let mutable system: AzureClusterActorSystem = null

    override wr.Run() =
        log "Running cluster." "INFO"
        system.Run()

    override wr.OnStart() = 

        // Set the maximum number of concurrent connections 
        ServicePointManager.DefaultConnectionLimit <- 12
       
        // For information on handling configuration changes
        // see the MSDN topic at http://go.microsoft.com/fwlink/?LinkId=166357.
        let aa = RoleEnvironment.DeploymentId
        let config = ClusterConfiguration().LoadFromEmbeddedResource(Assembly.GetExecutingAssembly(), "Orleans.xml")
        system <- ActorSystem.Configure()
                             .Azure()
                             .Cluster()
                             .From(config)
                             .Register(typeof<ReminderActor>.Assembly)
                             .Run<Bootstrapper>()
                             .Done()


        base.OnStart()
