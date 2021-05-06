using Kit;
using Microsoft.Diagnostics.Tracing.Parsers.Kernel;
using System.Reactive.Subjects;
using System.Reactive.Linq;
using System;
using System.Reactive.Concurrency;
using System.Threading;

namespace ETW.Patterns
{
    public class TestAnalyzer : ARxAnalyzer
    {
        private IObservable<ImageLoadTraceData> _loads;

        public TestAnalyzer(IObservable<ImageLoadTraceData> loads,Subject<SuspiciousEvent> suspiciousEvents) : base(suspiciousEvents)
        {
            this._loads = loads;
            var s = suspiciousEvents;
            Console.WriteLine("Test analyzer created");
        }
        public override void Start()
        { 
            var s = this._loads.Where(el => el.FileName.Contains("Rx.dll"));
            s.SubscribeOn(ThreadPoolScheduler.Instance)/*.ObserveOn(Scheduler.Default)*/.Subscribe(e => {
                if (e != null)
                {
                    Console.WriteLine($"Analyzer Thread: {Thread.CurrentThread.ManagedThreadId}");
                    //Console.WriteLine($"ID: {e.ProcessID} {e.ProcessName}");
                    var r = new SuspiciousEvent();
                    r.ProcessId = e.ProcessID;
                    this.SuspiciousEvents.OnNext(r);
                }
                });
            
        }
    }
}
