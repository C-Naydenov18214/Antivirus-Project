using Kit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;
using Microsoft.Diagnostics.Tracing.Parsers.Kernel;
using System.Threading.Tasks;
using System.Reactive.Linq;
using System.Reactive;

namespace ReadWriteDeleteAnalyzer
{
    public class ReadWriteDeleteAnalyzer : ARxAnalyzer
    {
        private IObservable<FileIOReadWriteTraceData> _writes;
        private IObservable<FileIOReadWriteTraceData> _reads;
        private IObservable<FileIOInfoTraceData> _deletes;

        public ReadWriteDeleteAnalyzer(IObservable<FileIOReadWriteTraceData> reads, IObservable<FileIOReadWriteTraceData> writes, IObservable<FileIOInfoTraceData> deletes, Subject<SuspiciousEvent> suspiciousEvents) : base(suspiciousEvents)
        {
            this._reads = reads;
            this._writes = writes;
            this._deletes = deletes;

        }
        public override void Start()
        {
            
            var streamW = _writes.Where(el => el.FileName.EndsWith(".txt")).Select(el => new { PID = el.ProcessID, FName = el.FileName, Action = el.EventName, ProcName = el.ProcessName });
            var streamR = _reads.Where(el => el.FileName.EndsWith(".txt")).Select(el => new { PID = el.ProcessID, FName = el.FileName, Action = el.EventName, ProcName = el.ProcessName });
            var streamD = _deletes.Where(el => el.FileName.EndsWith(".txt")).Select(el => new { PID = el.ProcessID, FName = el.FileName, Action = el.EventName, ProcName = el.ProcessName });
            var byPid = streamR.Merge(streamW).Merge(streamD).GroupBy(el => el.PID);
            byPid.SelectMany(pidGr =>
            {
                string message = null;
                var read = pidGr.Where(el => el.Action.CompareTo("FileIO/Read") == 0).Publish().RefCount();
                var write = pidGr.Where(el => el.Action.CompareTo("FileIO/Write") == 0).Publish().RefCount();
                var delete = pidGr.Where(el => el.Action.CompareTo("FileIO/Delete") == 0).Publish().RefCount();
                var firstPart = read.GroupJoin(write,
                    _ => Observable.Timer(TimeSpan.FromTicks(1)),
                    _ => Observable.Never<Unit>().TakeUntil(read.LastOrDefaultAsync().CombineLatest(write.LastOrDefaultAsync())),
                    (w, r) => (w, r))
                .SelectMany(x => x.r.Aggregate(new HashSet<int>(), (acc, v) => { acc.Add(v.PID); message = v.FName; return acc; }, acc => new
                {
                    FromName = x.w.FName,
                    ToFile = message,
                    procName = x.w.ProcName,
                    writeBy = x.w.PID,
                    creates = acc
                })).Publish().RefCount();

                var res = delete.GroupJoin(firstPart,
                    _ => Observable.Timer(TimeSpan.FromTicks(1)),
                    _ => Observable.Never<Unit>().TakeUntil(delete.LastOrDefaultAsync().CombineLatest(firstPart.LastOrDefaultAsync())/*.Where(a => a.First.FName.CompareTo(a.Second.FromName) == 0)*/),
                    (d, r) => (d, r))
                    .SelectMany(x => x.r.Aggregate(new HashSet<int>(), (acc, v) => { acc.Add(v.writeBy); message = v.FromName; return acc; }, acc => new
                    {
                        FromName = $"\n\t read {message}-> write -> delete {x.d.FName}",
                        procName = x.d.ProcName,
                        writeBy = x.d.PID,
                        creates = acc
                    })).Where(x => x.creates.Contains(x.writeBy) && x.creates.Count != 0); ;
                return res;

            }).Subscribe(e =>
            {
                //Console.WriteLine($"Create Analyzer Thread: {Thread.CurrentThread.ManagedThreadId} FILE = {e.fname}");

                //Console.WriteLine($"-------\t{e.fname}\tload: {e.writeBy}\twrites: {String.Join(", ", e.creates.Select(i => i.ToString()))}");
                var r = new SuspiciousEvent();
                try
                {
                    r.ProcessId = e.writeBy;
                    r.EventInfo = e.FromName;
                    r.Length = r.EventInfo.Length;
                    r.ProcName = e.procName;
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
