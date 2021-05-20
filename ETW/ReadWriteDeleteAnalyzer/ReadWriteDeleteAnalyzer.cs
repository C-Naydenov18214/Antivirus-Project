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

            var byPid = streamR.Merge(streamW).GroupBy(el => el.PID);
            byPid = streamR.Merge(streamW).Merge(streamD).GroupBy(el => el.PID);
            byPid.SelectMany(pidGr =>
            {
                string message = null;
                var read = pidGr.Where(el => el.Action.CompareTo("FileIO/Read") == 0);
                var write = pidGr.Where(el => el.Action.CompareTo("FileIO/Write") == 0);
                var delete = pidGr.Where(el => el.Action.CompareTo("FileIO/Delete") == 0);
                
                
                
                
                var firstPart = read.CombineLatest(write,
                    (r,w) => (r,w))
                .Select(x => new
                {
                    FName = x.r.FName,
                    ProcName = x.w.ProcName,
                    PID = x.r.PID,
                    writes = x.w.FName
                }).GroupBy(el => el.FName).SelectMany(gr =>
                {
                    return gr.FirstOrDefaultAsync();
             
                });


                var res = firstPart.CombineLatest(delete).Where(p => p.First.FName.CompareTo(p.Second.FName) == 0).Select(p => new
                {
                    PID = p.First.PID,
                    FRead = p.First.FName,
                    FDelet = p.Second.FName,
                    FWrite = p.First.writes,
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
