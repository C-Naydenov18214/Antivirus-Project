using Kit;
using Microsoft.Diagnostics.Tracing.Parsers.Kernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ETW.Patterns.Analyzer
{
    class CreateWriteAnalyzer : ARxAnalyzer
    {
        private IObservable<FileIOCreateTraceData> _creates;
        private IObservable<FileIOReadWriteTraceData> _writes;
        public CreateWriteAnalyzer(IObservable<FileIOCreateTraceData> creates, IObservable<FileIOReadWriteTraceData> writes, Subject<SuspiciousEvent> suspiciousEvents) : base(suspiciousEvents)
        {
            this._creates = creates;
            this._writes = writes;
            Console.WriteLine("Test analyzer created");
        }
        public override void Start()
        {


            var streamC = _creates.Where(el => el.FileName.EndsWith(".txt")).Select(el => new { PID = el.ProcessID, FName = el.FileName });
            var streamW = _writes.Where(el => el.FileName.EndsWith(".txt")).Select(el => new { PID = el.ProcessID, FName = el.FileName });

            var byPid = streamC.Merge(streamW).GroupBy(el => el.PID);

            streamC.SubscribeOn(Scheduler.Default).Subscribe(e =>
            {
                Console.WriteLine($"Create Analyzer Thread: {Thread.CurrentThread.ManagedThreadId} FILE = {e.FName}");
                //Console.WriteLine($"ID: {e.ProcessID} {e.ProcessName}");
                var r = new SuspiciousEvent();
                try
                {
                    r.ProcessId = e.PID;
                    this.SuspiciousEvents.OnNext(r);
                }
                catch (NullReferenceException ex)
                {
                    Console.WriteLine(ex.Message);
                }
            });
        }
    }
}
