using ETW.Patterns;
using Rx.MainModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading;
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

            var byFile = readSubj.Merge<FileEvent>(writeSubj).Merge(closeSubj)
                .Do(x => Console.WriteLine($"merge: {x.FileName}"))
                .GroupBy(el => el.FileName);

            var pairs = byFile.SelectMany(fgr =>
            {
                var read = fgr.Where(el => el.EventName.CompareTo("FileIO/Read") == 0).Publish().RefCount();
                var write = fgr.Where(el => el.EventName.CompareTo("FileIO/Write") == 0).Publish().RefCount();
                var close = fgr.Where(el => el.EventName.CompareTo("FileIO/Close") == 0).Publish().RefCount();
                var readWrite = fgr.Where(el => el.EventName.CompareTo("FileIO/Read") == 0 || el.EventName.CompareTo("FileIO/Write") == 0).Publish().RefCount();
                return close.GroupJoin(readWrite,
                    _ => Observable.Timer(TimeSpan.FromTicks(1)),
                    _ => Observable.Timer(TimeSpan.FromMilliseconds(100)).TakeUntil(close.LastOrDefaultAsync().CombineLatest(readWrite.LastOrDefaultAsync())),//Never<Unit>().TakeUntil(close.LastOrDefaultAsync().CombineLatest(readWrite.LastOrDefaultAsync())),
                    (cl,wr) => (cl,wr))
                .SelectMany(x => x.wr.Aggregate(new HashSet<FileEvent> (), (acc, v) => { acc.Add(v); return acc; }, acc => new { fName = x.cl.FileName, closeBy = x.cl.ProcessID, actions = acc }))
                .Where(x => x.actions.Count != 0);
            }).Subscribe(x => Console.WriteLine($"-------\t{x.fName}\tclose: {x.closeBy}\tactions:\n\t{String.Join(",\n\t", x.actions.Select(i => i.ToString()))}"));


            //.Subscribe(x => Console.WriteLine($"-------\t{x.fname}\tread: {x.readBy}\twrites: {String.Join(", ", x.writes.Select(i => i.ToString()))}"));
            /*for (int i = 0; i < 100; i++)
            {

                writeSubj.OnNext(new FileEvent("FileIO/Write", i, $"proc name = {i}", $"File = {i}", (ulong)i, i));
                loadSubj.OnNext(new FileEvent("Image/Load", i, $"proc name = {i}", $"File = {i}", (ulong)i, i));

            }*/
            //x.writes.Contains(x.loadBy) &&

            readSubj.OnNext(new FileEvent("FileIO/Read", 1, $"proc name = {1}", $"a.exe", (ulong)1, 1));
            readSubj.OnNext(new FileEvent("FileIO/Read", 21, $"proc name = {1}", $"b.exe", (ulong)1, 1));
            writeSubj.OnNext(new FileEvent("FileIO/Write", 1, $"proc name = {1}", $"b.exe", (ulong)1, 1));
            Thread.Sleep(80);
            
            Thread.Sleep(50);
            writeSubj.OnNext(new FileEvent("FileIO/Write", 5, $"proc name = {1}", $"b.exe", (ulong)1, 1));
            writeSubj.OnNext(new FileEvent("FileIO/Write", 6, $"proc name = {1}", $"b.exe", (ulong)1, 1));
            
            closeSubj.OnNext(new FileEvent("FileIO/Close", 1, $"proc name = {1}", $"a.exe", (ulong)1, 1));
            closeSubj.OnNext(new FileEvent("FileIO/Close", 5, $"proc name = {1}", $"b.exe", (ulong)1, 1));

            /* writeSubj.OnNext(new FileEvent("FileIO/Write", 20, $"proc name = {1}", $"a.exe", (ulong)1, 1));
             writeSubj.OnNext(new FileEvent("FileIO/Write", 21, $"proc name = {2}", $"a.exe", (ulong)2, 2));
             writeSubj.OnNext(new FileEvent("FileIO/Write", 22, $"proc name = {3}", $"b.exe", (ulong)3, 3));
             writeSubj.OnNext(new FileEvent("FileIO/Write", 23, $"proc name = {5}", $"b.exe", (ulong)4, 4));
             writeSubj.OnNext(new FileEvent("FileIO/Write", 24, $"proc name = {5}", $"c.exe", (ulong)5, 5));

             readSubj.OnNext(new FileEvent("FileIO/Read", 4, $"proc name = {4}", $"a.exe", (ulong)4, 4));

             writeSubj.OnNext(new FileEvent("FileIO/Write", 25, $"proc name = {7}", $"a.exe", (ulong)7, 7));
             writeSubj.OnNext(new FileEvent("FileIO/Write", 666, $"proc name = {1}", $"a.exe", (ulong)1, 1));
             readSubj.OnNext(new FileEvent("FileIO/Read", 4, $"proc name = {66}", $"a.exe", (ulong)66, 66));

             readSubj.OnNext(new FileEvent("FileIO/Read", 5, $"proc name = {66}", $"d.exe", (ulong)66, 66));
             readSubj.OnNext(new FileEvent("FileIO/Read", 6, $"proc name = {66}", $"b.exe", (ulong)66, 66));*/

            readSubj.OnCompleted();
            writeSubj.OnCompleted();

        }

    }
}
