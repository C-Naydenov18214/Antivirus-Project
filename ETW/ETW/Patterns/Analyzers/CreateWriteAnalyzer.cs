using ETW.Provider;
using Kit;
using Microsoft.Diagnostics.Tracing.Parsers.Kernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
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
        public CreateWriteAnalyzer(EventProvider<FileIOCreateTraceData> creates, EventProvider<FileIOReadWriteTraceData> writes, Subject<SuspiciousEvent> suspiciousEvents) : base(suspiciousEvents)
        {
            this._creates = creates.Events;
            this._writes = writes.Events;
            Console.WriteLine("Test analyzer created");
        }
        public override void Start()
        {


            var streamC = _creates.Where(el => el.FileName.EndsWith(".txt")).Select(el => new { PID = el.ProcessID, FName = el.FileName, Action = el.EventName });
            var streamW = _writes.Where(el => el.FileName.EndsWith(".txt")).Select(el => new { PID = el.ProcessID, FName = el.FileName, Action = el.EventName });

            var byFile = streamC.Merge(streamW).GroupBy(el => el.FName);

            byFile.SelectMany(fgr =>
            {
                var create = fgr.Where(el => el.Action.CompareTo("FileIO/Create") == 0).Publish().RefCount();
                var write = fgr.Where(el => el.Action.CompareTo("FileIO/Write") == 0).Publish().RefCount();
                return write.GroupJoin(create,
                    _ => Observable.Timer(TimeSpan.FromTicks(1)),
                    _ => Observable.Never<Unit>().TakeUntil(write.LastOrDefaultAsync().CombineLatest(create.LastOrDefaultAsync())),
                    (l, w) => (l, w))
                .SelectMany(x => x.w.Aggregate(new HashSet<int>(), (acc, v) => { acc.Add(v.PID); return acc; }, acc => new
                {
                    fname = x.l.FName,
                    writeBy = x.l.PID,
                    creates = acc
                }))
                .Where(x => x.creates.Contains(x.writeBy) && x.creates.Count != 0);

            }).Subscribe(e =>
            {
                //Console.WriteLine($"Create Analyzer Thread: {Thread.CurrentThread.ManagedThreadId} FILE = {e.fname}");

                //Console.WriteLine($"-------\t{e.fname}\tload: {e.writeBy}\twrites: {String.Join(", ", e.creates.Select(i => i.ToString()))}");
                var r = new SuspiciousEvent();
                try
                {
                    r.ProcessId = e.writeBy;
                    this.SuspiciousEvents.OnNext(r);
                }
                catch (NullReferenceException ex)
                {
                    Console.WriteLine(ex.Message);
                }
            });


            //streamC.SubscribeOn(Scheduler.Default).Subscribe(e =>
            //{
            //    Console.WriteLine($"Create Analyzer Thread: {Thread.CurrentThread.ManagedThreadId} FILE = {e.FName}");
            //    //Console.WriteLine($"ID: {e.ProcessID} {e.ProcessName}");
            //    var r = new SuspiciousEvent();
            //    try
            //    {
            //        r.ProcessId = e.PID;
            //        this.SuspiciousEvents.OnNext(r);
            //    }
            //    catch (NullReferenceException ex)
            //    {
            //        Console.WriteLine(ex.Message);
            //    }
            //});
        }
    }
}
