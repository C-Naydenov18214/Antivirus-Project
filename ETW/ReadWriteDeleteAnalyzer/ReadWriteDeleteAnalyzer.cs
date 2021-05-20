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

            var streamW = _writes.Where(el => el.FileName.EndsWith(".txt")).Select(el => new { PID = el.ProcessID, FName = el.FileName, Action = el.EventName, ProcName = el.ProcessName }).Publish().RefCount();
            var streamR = _reads.Where(el => el.FileName.EndsWith(".txt")).Select(el => new { PID = el.ProcessID, FName = el.FileName, Action = el.EventName, ProcName = el.ProcessName });
            var streamD = _deletes.Where(el => el.FileName.EndsWith(".txt")).Select(el => new { PID = el.ProcessID, FName = el.FileName, Action = el.EventName, ProcName = el.ProcessName });

            var byPid = streamR.Merge(streamW).GroupBy(el => el.PID);
            byPid = streamR.Merge(streamW).Merge(streamD).GroupBy(el => el.PID);
            HashSet<string> message = new HashSet<string>();
            byPid.SelectMany(pidGr =>
            {

                var read = pidGr.Where(el => el.Action.CompareTo("FileIO/Read") == 0);
                var write = pidGr.Where(el => el.Action.CompareTo("FileIO/Write") == 0);

                var delete = pidGr.Where(el => el.Action.CompareTo("FileIO/Delete") == 0);
                //gewge
                var joined = read.GroupJoin(write,
                    _ => Observable.Timer(TimeSpan.FromMilliseconds(100)),//Observable.Timer(TimeSpan.FromTicks(1)),
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
                .Where(x => x.writes.Count != 0).GroupBy(x => x.FName).SelectMany(gr => gr.FirstOrDefaultAsync());

                var firstPart = read.CombineLatest(write,
                    (r, w) => (r, w))
                .Select(x =>
                {
                    //message.Clear();
                    message.Add(x.w.FName);
                    return new
                    {
                        FName = x.r.FName,
                        ProcName = x.w.ProcName,
                        PID = x.r.PID,
                        writes = String.Join(", ", message.Select(e => e))
                    };//x.w.FName });
                    //return new
                    //{
                    //    FName = x.r.FName,
                    //    ProcName = x.w.ProcName,
                    //    PID = x.r.PID,
                    //    writes = message.Length//x.w.FName
                    //};
                }).GroupBy(el => el.FName).SelectMany(gr =>
            {
                return gr.FirstOrDefaultAsync();

            });

                var res = joined.CombineLatest(delete).Where(p => p.First.FName.CompareTo(p.Second.FName) == 0).Select(p => new
                {
                    PID = p.First.PID,
                    FRead = p.First.FName,
                    FDelet = p.Second.FName,
                    FWrite = String.Join("\n\t\t\t\t\t ", p.First.writes.Select(i => i)),
                    ProcName = p.First.ProcName
                });
                return res;

            }).Subscribe(e =>
            {
                var r = new SuspiciousEvent();
                try
                {
                    r.ProcessId = e.PID;
                    r.EventInfo = $"\tread {e.FRead} -> delete {e.FDelet} # caught write {e.FWrite}";
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
