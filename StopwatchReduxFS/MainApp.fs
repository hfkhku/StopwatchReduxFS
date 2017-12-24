open System
open System.Windows
open System.Windows.Controls
open System.Windows.Markup
open Stopwatch.Models
open App
open System.Windows.Navigation
open Stopwatch.Actions
open Stopwatch.Models
open System.Windows.Data
open System.Windows.Documents
open System.Windows.Input
open System.Windows.Media
open System.Windows.Media.Imaging
open System.Windows.Navigation
open System.Windows.Controls.Primitives
open System.Reactive.Linq
open FsXaml

type MainPage = XAML< "MainPageView.xaml" >

type ResultPage = XAML< "ResultPageView.xaml" >

// Application Entry point
[<STAThread>]
[<EntryPoint>]
let main (_) = 
    let a = App()
    let mainPage = MainPage()
    let resultPage = ResultPage()
    let mutable timerSubscription : IDisposable = null
    let mainWindowView = 
        Application.LoadComponent(new System.Uri("/App;component/mainwindow.xaml", UriKind.Relative)) :?> NavigationWindow
    let txtNowSpan = mainPage.txtNowSpan
    let btnStartStopReset = mainPage.btnStartStopReset
    let btnLap = mainPage.btnLap
    let lvLap = mainPage.lvLap
    let chbIsShowed = mainPage.chbIsShowed
    let lvLapResult = resultPage.lvLap
    let btnBack = resultPage.btnBack
    App.Store.ObserveOnDispatcher().Subscribe(fun state -> 
       txtNowSpan.Text <- state.NowSpan.ToString(state.DisplayFormat)
       btnStartStopReset.Content <- state.ButtonLabel
       btnLap.IsEnabled <- state.Mode = StopwatchMode.Start
       lvLap.ItemsSource <- state.LapTimeList
       if state.Mode = StopwatchMode.Stop then 
           let nowSpan = state.NowSpan
           let maxLapTime = state.MaxLapTime
           let minLapTime = state.MinLapTime
           let r = 
               MessageBox.Show
                   ("All time: " + nowSpan.ToString(state.DisplayFormat) + "\r\nMax laptime: " 
                    + string maxLapTime.TotalMilliseconds + " ms\nMin laptime: " + string minLapTime.TotalMilliseconds 
                    + "ms\n\nShow all lap result?", "Confirmation", MessageBoxButton.OKCancel)
           if r = MessageBoxResult.OK then mainWindowView.Navigate(resultPage) |> ignore
       lvLapResult.ItemsSource <- state.LapTimeList)
    |> ignore
    //表示切替チェックボックス
    chbIsShowed.Events()
               .Checked.Subscribe(fun _ -> 
               App.Store.Dispatch(new TimeFormatAction(Format = Constants.TimeSpanFormat)) |> ignore) |> ignore
    chbIsShowed.Events()
               .Unchecked.Subscribe(fun _ -> 
               App.Store.Dispatch(new TimeFormatAction(Format = Constants.TimeSpanFormatNoMillsecond)) |> ignore) 
    |> ignore
    //start,stop,resetボタン
    btnStartStopReset.Events().Click.Subscribe(fun e -> 
                     let mode = App.Store.GetState().Mode
                     let scheduler = App.Store.GetState().TimerScheduler
                     if mode = StopwatchMode.Init then 
                         timerSubscription <- Observable.Interval(TimeSpan.FromMilliseconds(10.), scheduler)
                                                        .Subscribe(fun _ -> 
                                                        App.Store.Dispatch
                                                            (new TimerAction(Now = scheduler.Now.DateTime.ToLocalTime())) 
                                                        |> ignore)
                     else 
                         if mode = StopwatchMode.Start then 
                             timerSubscription.Dispose()
                             timerSubscription <- null
                     App.Store.Dispatch(new ChangeModeAction()) |> ignore)
    |> ignore
    //lapボタン
    btnLap.Events().Click.Subscribe(fun _ -> App.Store.Dispatch(new LapAction()) |> ignore) |> ignore
    //backボタン
    btnBack.Events().Click.Subscribe(fun _ -> mainWindowView.Navigate(mainPage) |> ignore) |> ignore
    mainWindowView.Navigate(mainPage) |> ignore
    a.Run(mainWindowView)
