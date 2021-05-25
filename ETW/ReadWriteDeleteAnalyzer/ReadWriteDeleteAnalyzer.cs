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

            var streamW = _writes/*.Where(el => el.FileName.EndsWith(".txt"))*/.Select(el => new { PID = el.ProcessID, FName = el.FileName, Action = el.EventName, ProcName = el.ProcessName });
            var streamR = _reads/*.Where(el => el.FileName.EndsWith(".txt"))*/.Select(el => new { PID = el.ProcessID, FName = el.FileName, Action = el.EventName, ProcName = el.ProcessName });
            var streamD = _deletes/*.Where(el => el.FileName.EndsWith(".txt"))*/.Select(el => new { PID = el.ProcessID, FName = el.FileName, Action = el.EventName, ProcName = el.ProcessName });

            var byPid = streamR.Merge(streamW).GroupBy(el => el.PID);
            byPid = streamR.Merge(streamW).Merge(streamD).GroupBy(el => el.PID);
            HashSet<string> message = new HashSet<string>();
            byPid.SelectMany(pidGr =>
            {

                var read = pidGr.Where(el => el.Action.CompareTo("FileIO/Read") == 0).Publish().RefCount(); ;
                var write = pidGr.Where(el => el.Action.CompareTo("FileIO/Write") == 0).Publish().RefCount(); ;

                var delete = pidGr.Where(el => el.Action.CompareTo("FileIO/Delete") == 0).Publish().RefCount(); ;
                //gewge
                var joined = read.GroupJoin(write,
                    _ => Observable.Timer(TimeSpan.FromMilliseconds(100)),//Observable.Timer(TimeSpan.FromMilliseconds(10)),//Observable.Timer(TimeSpan.FromTicks(1)),
                    _ => Observable.Never<Unit>().TakeUntil(read.LastOrDefaultAsync().CombineLatest(write.LastOrDefaultAsync())),
                    (r, w) => (r, w))
                .SelectMany(x => x.w.Aggregate(new HashSet<string>(), (acc, v) => { acc.Add(v.FName); return acc; }, acc => new
                {
                    FName = x.r.FName,
                    ProcName = x.r.ProcName,
                    PID = x.r.PID,
                    writes = acc
                }
                ))
                .Where(x => x.writes.Count != 0).GroupBy(x => x.FName).SelectMany(gr => gr.FirstOrDefaultAsync()).Publish().RefCount();//.Publish().RefCount(); //

                var test = joined.GroupJoin(delete,
                    _ => Observable.Timer(TimeSpan.FromMilliseconds(100)),//Observable.Timer(TimeSpan.FromTicks(1)),
                    _ => Observable.Never<Unit>().TakeUntil(joined.LastOrDefaultAsync().CombineLatest(delete.LastOrDefaultAsync())),
                    (j, d) => (j, d))
                .SelectMany(x => x.d.Aggregate(new HashSet<string>(), (acc, v) =>
                {
                    if (v.FName.CompareTo(x.j.FName) == 0)
                    {
                        acc.Add(v.FName);
                    }
                    return acc;

                }, acc => new
                {
                    FRead = x.j.FName,
                    PID = x.j.PID,
                    FDelete = acc,
                    FWrite = x.j.writes,
                    ProcName = x.j.ProcName
                })).Where(x => x.FDelete.Contains(x.FRead)).Select(x => new
                {
                    FRead = x.FRead,
                    PID = x.PID,
                    FDelete = String.Join(" ", x.FDelete.Select(i => i)),
                    FWrite = String.Join(", ", x.FWrite.LastOrDefault()),
                    ProcName = x.ProcName
                });
                //FDelete = String.Join(" ", x.d.Select(i => i.FName)),
                //    FWrite = String.Join(", ", x.j.writes.Select(i => i)),
                return test;

            }).Subscribe(e =>
            {
                var r = new SuspiciousEvent();
                try
                {
                    r.ProcessId = e.PID;
                    r.EventInfo = $"\n\tread -> {e.FRead} -> delete {e.FDelete} \n\t# caught writes: {e.FWrite}";
                    //r.EventInfo = $"\n\tread -> {e.FName}";// -> delete {e.FDelet} \n\t# caught writes: {e.FWrite}";
                    r.Length = r.EventInfo.Length;
                    r.ProcName = e.ProcName;
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
