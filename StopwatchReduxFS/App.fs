module App

open System
open System.Windows
open System.Windows.Controls
open System.Windows.Markup
open Stopwatch.Models
open Redux
open Stopwatch.States
open Stopwatch
open System.Collections.ObjectModel
open System.Reactive.Concurrency

type App() =
    inherit Application()
    static member val Store = Store<ApplicationState>( Reducer<ApplicationState>( fun s->fun a-> Reducers.ReduceApplication(s,a)),{
            TimerScheduler= Scheduler.Default
            DisplayFormat= Constants.TimeSpanFormatNoMillsecond
            NowSpan= TimeSpan.Zero
            Mode= StopwatchMode.Init
            ButtonLabel= Constants.StartLabel
            StartTime= DateTime()
            Now= DateTime()
            LapTimeList=ObservableCollection<LapTime>()
            MaxLapTime= TimeSpan.Zero
            MinLapTime= TimeSpan.Zero
        })





