module ReminderActor

open System
open System.Diagnostics
open Orleankka
open Orleankka.Cluster
open Orleans
open Orleankka.FSharp

type ReminderActor() = 
    inherit Actor()

    override this.OnReminder id = 
        Trace.TraceInformation("######### Reminded on id " + id)
        TaskDone.Done

    member this.Handle(data:string) = task {
        let! registered = this.Reminders.IsRegistered "BrokenReminder"
        if not registered then 
            this.Reminders.Register("BrokenReminder", TimeSpan.Zero, TimeSpan.FromMinutes(1.2)) |> ignore
        else return ()
    }