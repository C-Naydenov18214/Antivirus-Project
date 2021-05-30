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

            var streamW = _writes.Where(el => (el.ProcessName.CompareTo("WinRAR") == 0 || el.ProcessName.CompareTo("ReadAndWrite") == 0))/*.Where(el => el.FileName.EndsWith(".txt"))*/.Select(el => new { PID = el.ProcessID, FName = el.FileName, Action = el.EventName, ProcName = el.ProcessName });
            var streamR = _reads.Where(el => el.FileName.EndsWith(".txt") && (el.ProcessName.CompareTo("WinRAR") == 0 || el.ProcessName.CompareTo("ReadAndWrite") == 0)).Select(el => new { PID = el.ProcessID, FName = el.FileName, Action = el.EventName, ProcName = el.ProcessName });
            var streamD = _deletes.Where(el => el.FileName.EndsWith(".txt") && (el.ProcessName.CompareTo("WinRAR") == 0 || el.ProcessName.CompareTo("ReadAndWrite") == 0)).Select(el => new { PID = el.ProcessID, FName = el.FileName, Action = el.EventName, ProcName = el.ProcessName });


            var byPid = streamR.Merge(streamW).Merge(streamD).GroupBy(el => el.PID);




            HashSet<string> message = new HashSet<string>();
            byPid.SelectMany(pidGr =>
            {

                var read = pidGr.Where(el => el.Action.CompareTo("FileIO/Read") == 0).Publish().RefCount(); ;
                var write = pidGr.Where(el => el.Action.CompareTo("FileIO/Write") == 0).Publish().RefCount(); ;

                var delete = pidGr.Where(el => el.Action.CompareTo("FileIO/Delete") == 0).Publish().RefCount();
                var rd = read.Merge(delete).GroupBy(x => x.FName);
                var prd = rd.SelectMany(fGr =>
                {
                    var r = fGr.Where(el => el.Action.CompareTo("FileIO/Read") == 0).Publish().RefCount();
                    var d = fGr.Where(el => el.Action.CompareTo("FileIO/Delete") == 0).Publish().RefCount();

                    return d.GroupJoin(r,
                        _ => Observable.Timer(TimeSpan.FromTicks(1)),
                        _ => Observable.Never<Unit>().TakeUntil(d.LastOrDefaultAsync().CombineLatest(r.LastOrDefaultAsync())),
                        (dd, rr) => (dd, rr))
                    .SelectMany(x => x.rr.Aggregate(new HashSet<string>(), (acc, v) => { acc.Add(v.FName); return acc; }, acc => new
                    {
                        FName = x.dd.FName,
                        ProcName = x.dd.ProcName,
                        PID = x.dd.PID,
                        writes = acc
                    }
                )).Where(x => x.writes.Count != 0).Publish().RefCount();
                });

                var prdw = prd.GroupJoin(write,
                    _ => Observable.Timer(TimeSpan.FromTicks(1)),
                    _ => Observable.Timer(TimeSpan.FromMilliseconds(1000)),//Observable.Never<Unit>().TakeUntil(prd.LastOrDefaultAsync().CombineLatest(write.LastOrDefaultAsync())),
                    (rrd, w) => (rrd, w)).SelectMany(x => x.w.Aggregate(new HashSet<string>(), (acc, v) => { acc.Add(v.FName); return acc; }, acc => new
                    {
                        FDelete = x.rrd.FName,
                        PID = x.rrd.PID,
                        ProcName = x.rrd.ProcName,
                        FRead = x.rrd.writes,
                        FWrite = acc

                    }
                      )).Where(x => x.FWrite.Count != 0).Select(x => new
                      {
                          FDelete = x.FDelete,
                          PID = x.PID,
                          ProcName = x.ProcName,
                          FRead = String.Join(", ", x.FRead.Select(i => i)),
                          FWrite = String.Join(", ", x.FWrite.Select(i => i))
                      });


                /*.Where(x => x.writes.Count != 0)*/
                ;//.GroupBy(x => x.FName).SelectMany(gr => gr.FirstOrDefaultAsync()).Publish().RefCount();
                return prdw;//prd;

            })



            /*.Subscribe(e =>
            {
                var r = new SuspiciousEvent();
                try
                {
                    r.ProcessId = e.PID;
                    r.EventInfo = $"\n\tread -> {e.FName} -> delete NO \n\t# caught writes: {String.Join(", ", e.writes.Select(i => i))}";
                    //r.EventInfo = $"\n\tread -> {e.FName}";// -> delete {e.FDelet} \n\t# caught writes: {e.FWrite}";
                    r.Length = r.EventInfo.Length;
                    r.ProcName = e.ProcName;
                    this.SuspiciousEvents.OnNext(r);
                }
                catch (NullReferenceException ex)
                {
                    Console.WriteLine(ex.Message);
                }
            });*/
            .Subscribe(e =>
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
