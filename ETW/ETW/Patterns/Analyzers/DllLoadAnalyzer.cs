using Kit;
using Microsoft.Diagnostics.Tracing.Parsers.Kernel;
using System.Reactive.Subjects;
using System.Reactive.Linq;
using System;
using System.Reactive.Concurrency;
using System.Threading;

namespace ETW.Patterns
{
    public class DllLoadAnalyzer : ARxAnalyzer
    {
        private IObservable<ImageLoadTraceData> _loads;

        public DllLoadAnalyzer(IObservable<ImageLoadTraceData> loads, Subject<SuspiciousEvent> suspiciousEvents) : base(suspiciousEvents)
        {
            this._loads = loads;
            Console.WriteLine("Test analyzer created");
        }
        public override void Start()
        {
            var s = this._loads.Where(el => el.FileName.Contains("Rx.dll"));
            s.SubscribeOn(Scheduler.Default)/*.ObserveOn(NewThreadScheduler.Default)*/.Subscribe(e =>
            {
                if (e != null)
                {
                    Console.WriteLine($"Dll Analyzer Thread: {Thread.CurrentThread.ManagedThreadId} FILE = {e.FileName}");
                    //Console.WriteLine($"ID: {e.ProcessID} {e.ProcessName}");
                    var r = new SuspiciousEvent();
                    try
                    {
                        r.ProcessId = e.ProcessID;
                        this.SuspiciousEvents.OnNext(r);
                    }
                    catch (NullReferenceException ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            });
        }
    }
}
