namespace Stopwatch.States

open System
open System.Collections.Generic
open System.Linq
open System.Reactive.Concurrency
open System.Text
open System.Threading.Tasks
open Stopwatch.Models
open System.Collections.ObjectModel

type ApplicationState = 
    { TimerScheduler : IScheduler
      DisplayFormat : string
      NowSpan : TimeSpan
      Mode : StopwatchMode
      ButtonLabel : string
      StartTime : DateTime
      Now : DateTime
      LapTimeList : ObservableCollection<LapTime>
      MaxLapTime : TimeSpan
      MinLapTime : TimeSpan }
