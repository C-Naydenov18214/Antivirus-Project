using Rx.MainModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;

namespace ETW.Patterns
{
    public class Cript
    {

        public static void Test()
        {
            var readSubj = new Subject<FileEvent>();
            var writeSubj = new Subject<FileEvent>();
            var closeSubj = new Subject<FileEvent>();

            var byTidPid = readSubj.Merge<FileEvent>(writeSubj).Merge(closeSubj)
                .Do(x => Console.WriteLine($"merge: {x.ThreadID} {x.ProcessID}"))
                .GroupBy(el => new { tID = el.ThreadID, pID = el.ProcessID });
            byTidPid.SelectMany(gr =>
            {
                var read = gr.Where(el => el.EventName.CompareTo("FileIO/Read") == 0).Publish().RefCount();
                var write = gr.Where(el => el.EventName.CompareTo("FileIO/Write") == 0).Publish().RefCount();
                var gpJoin = read.GroupJoin(write,
                    _ => Observable.Timer(TimeSpan.FromTicks(1)),
                    _ => Observable.Never<Unit>().TakeUntil(read.LastOrDefaultAsync().CombineLatest(write.LastOrDefaultAsync())),
                    (l, w) => (l, w))
                .SelectMany(x => x.w.Aggregate(new HashSet<int>(), (acc, v) => { acc.Add(v.ProcessID); return acc; }, acc => new { fname = x.l.FileName, readBy = x.l.ProcessID, writes = acc }))
                .Where(x => x.writes.Count != 0);
                return gpJoin;
                //??

            }).Subscribe(x => Console.WriteLine($"-------\t{x.fname}\tread: {x.readBy}\twrites: {String.Join(", ", x.writes.Select(i => i.ToString()))}"));


            writeSubj.OnNext(new FileEvent("FileIO/Write",1 ,1, $"proc name = {1}", $"a.exe", 1));
            writeSubj.OnNext(new FileEvent("FileIO/Write", 1, 2, $"proc name = {1}", $"a.exe", 1));
            writeSubj.OnNext(new FileEvent("FileIO/Write", 1, 3, $"proc name = {1}", $"a.exe", 1));

            readSubj.OnNext(new FileEvent("FileIO/Read",1, 4, $"proc name = {1}", $"a.exe", 1));
            readSubj.OnNext(new FileEvent("FileIO/Read",1, 5, $"proc name = {1}", $"a.exe",  1));
            readSubj.OnNext(new FileEvent("FileIO/Read", 1, 6, $"proc name = {1}", $"a.exe", 1));
            readSubj.OnNext(new FileEvent("FileIO/Read", 1, 1, $"proc name = {1}", $"a.exe", 1));
            readSubj.OnNext(new FileEvent("FileIO/Read", 1, 2, $"proc name = {1}", $"a.exe", 1));
            readSubj.OnNext(new FileEvent("FileIO/Read", 1, 3, $"proc name = {1}", $"a.exe", 1));
            closeSubj.OnNext(new FileEvent("FileIO/Close", 1, $"proc name = {1}", $"a.exe",  1));
            closeSubj.OnNext(new FileEvent("FileIO/Close", 5, $"proc name = {1}", $"a.exe",  1));
        }
    }
}
