using Rx.MainModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;

namespace ETW
{
    class ReadWritePattern
    {
        public static void TestVarient(IObservable<IGroupedObservable<string, FileEvent>> byFiles)
        {
            var readSubj = new Subject<FileEvent>();
            var writeSubj = new Subject<FileEvent>();
            var closeSubj = new Subject<FileEvent>();
            var readObs = readSubj;
            var writeObs = writeSubj;
            var byFile = readObs.Merge<FileEvent>(writeObs).Merge(closeSubj)
                .Do(x => Console.WriteLine($"merge: {x.FileName}"))
                .GroupBy(el => el.FileName);
            var str = "FileIO/Close";
            byFile.SelectMany(fgr =>
            {
            var read = fgr.Where(el => el.EventName.CompareTo("FileIO/Read") == 0).Publish().RefCount();
            var write = fgr.Where(el => el.EventName.CompareTo("FileIO/Write") == 0).Publish().RefCount();
            var close = fgr.Where(el => el.EventName.CompareTo("FileIO/Close") == 0).Publish().RefCount();
            var tmpCl = close.GroupJoin(write,
                _ => Observable.Timer(TimeSpan.FromTicks(1)),
                _ => Observable.Never<Unit>().TakeUntil(close.LastOrDefaultAsync().CombineLatest(write.LastOrDefaultAsync())),
                (cl, wr) => (cl, wr));
            //var tmpRes = tmpCl.Select(el => el.cl)
                return read.GroupJoin(write,
                    _ => Observable.Timer(TimeSpan.FromTicks(1)),
                    _ => Observable.Never<Unit>().TakeUntil(read.LastOrDefaultAsync().CombineLatest(write.LastOrDefaultAsync())),
                    (l, w) => (l, w))
                .SelectMany(x => x.w.Aggregate(new HashSet<int>(), (acc, v) => { acc.Add(v.ProcessID); return acc; }, acc => new { fname = x.l.FileName, loadBy = x.l.ProcessID, writes = acc }))
                .Where(x =>  x.writes.Count != 0);
            }).Subscribe(x => Console.WriteLine($"-------\t{x.fname}\tread: {x.loadBy}\twrites: {String.Join(", ", x.writes.Select(i => i.ToString()))}"));
            /*for (int i = 0; i < 100; i++)
            {

                writeSubj.OnNext(new FileEvent("FileIO/Write", i, $"proc name = {i}", $"File = {i}", (ulong)i, i));
                loadSubj.OnNext(new FileEvent("Image/Load", i, $"proc name = {i}", $"File = {i}", (ulong)i, i));

            }*/
            //x.writes.Contains(x.loadBy) &&
            readSubj.OnNext(new FileEvent("FileIO/Read", 1, $"proc name = {1}", $"a.exe", (ulong)1, 1));

            writeSubj.OnNext(new FileEvent("FileIO/Write", 20, $"proc name = {1}", $"a.exe", (ulong)1, 1));
            writeSubj.OnNext(new FileEvent("FileIO/Write", 21, $"proc name = {2}", $"a.exe", (ulong)2, 2));
            writeSubj.OnNext(new FileEvent("FileIO/Write", 22, $"proc name = {3}", $"b.exe", (ulong)3, 3));
            writeSubj.OnNext(new FileEvent("FileIO/Write", 23, $"proc name = {5}", $"b.exe", (ulong)4, 4));
            writeSubj.OnNext(new FileEvent("FileIO/Write", 24, $"proc name = {5}", $"c.exe", (ulong)5, 5));
            
            readSubj.OnNext(new FileEvent("FileIO/Read", 4, $"proc name = {4}", $"a.exe", (ulong)4, 4));
            
            writeSubj.OnNext(new FileEvent("FileIO/Write", 25, $"proc name = {7}", $"a.exe", (ulong)7, 7));
            writeSubj.OnNext(new FileEvent("FileIO/Write", 666, $"proc name = {1}", $"a.exe", (ulong)1, 1));
            readSubj.OnNext(new FileEvent("FileIO/Read", 4, $"proc name = {66}", $"a.exe", (ulong)66, 66));
            
            readSubj.OnNext(new FileEvent("FileIO/Read", 5, $"proc name = {66}", $"d.exe", (ulong)66, 66));
            readSubj.OnNext(new FileEvent("FileIO/Read", 6, $"proc name = {66}", $"b.exe", (ulong)66, 66));

            readSubj.OnCompleted();
            writeSubj.OnCompleted();

        }


    }
}
