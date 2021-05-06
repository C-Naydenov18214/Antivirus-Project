using ETW.Patterns;
using Kit;
using Microsoft.Diagnostics.Tracing.Parsers.Kernel;
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
            var suspSub = new Subject<int>();
            var byFile = readSubj.Merge<FileEvent>(writeSubj).Merge(closeSubj)
                .Do(x => Console.WriteLine($"merge: {x.FileName}"))
                .GroupBy(el => el.FileName);

            var pairs = byFile.SelectMany(fgr =>
            {
                var read = fgr.OfType<FR>().Publish().RefCount();
                var write = fgr.OfType<FW>().Publish().RefCount();
                var close = fgr.OfType<FC>().Publish().RefCount();

                var readWrite = ((read as IObservable<FileEvent>)).Merge(write);
                return close.GroupJoin(readWrite,
                    _ => Observable.Timer(TimeSpan.FromTicks(1)),
                    _ => Observable.Timer(TimeSpan.FromMilliseconds(100)),//Never<Unit>().TakeUntil(close.LastOrDefaultAsync().CombineLatest(readWrite.LastOrDefaultAsync())),
                    (cl, wr) => (cl, wr))
                .SelectMany(x => x.wr.Aggregate(new HashSet<FileEvent>(), (acc, v) => { acc.Add(v); return acc; }, acc => new { fName = x.cl.FileName, closeBy = x.cl.ProcessID, actions = acc }))
                .Where(x => x.actions.Count != 0);
            }).Subscribe(x =>
            {
                suspSub.OnNext(x.closeBy);
                Console.WriteLine($"-------\t{x.fName}\tclose: {x.closeBy}\tactions:\n\t{String.Join(",\n\t", x.actions.Select(i => i.ToString()))}");
            });


            //.Subscribe(x => Console.WriteLine($"-------\t{x.fname}\tread: {x.readBy}\twrites: {String.Join(", ", x.writes.Select(i => i.ToString()))}"));
            /*for (int i = 0; i < 100; i++)
            {

                writeSubj.OnNext(new FileEvent("FileIO/Write", i, $"proc name = {i}", $"File = {i}", (ulong)i, i));
                loadSubj.OnNext(new FileEvent("Image/Load", i, $"proc name = {i}", $"File = {i}", (ulong)i, i));

            }*/
            //x.writes.Contains(x.loadBy) &&
            suspSub.Subscribe(x => Console.WriteLine($"**********************************susp closed by {x}"));
            readSubj.OnNext(new FR("FileIO/Read", 0, 1, $"proc name = {1}", $"a.exe", 1));
            readSubj.OnNext(new FR("FileIO/Read", 0, 21, $"proc name = {1}", $"b.exe", 1));
            writeSubj.OnNext(new FW("FileIO/Write", 0, 1, $"proc name = {1}", $"a.exe", 1));
            writeSubj.OnNext(new FW("FileIO/Write", 0, 5, $"proc name = {1}", $"b.exe", 1));
            writeSubj.OnNext(new FW("FileIO/Write", 0, 6, $"proc name = {1}", $"b.exe", 1));

            closeSubj.OnNext(new FC("FileIO/Close", 0, 1, $"proc name = {1}", $"a.exe", 1));
            closeSubj.OnNext(new FC("FileIO/Close", 0, 5, $"proc name = {1}", $"b.exe", 1));

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



        class FW : FileEvent
        {
            public override string FileName { get; }
            public override string ProcessName { get; }
            public override int ProcessID { get; }
            public override int ThreadID { get; }
            public override double TimeStamp { get; set; }
            public override string EventName { get; set; }
            public override ulong FileKey { get; }

            public FW(string eventName, int threadID, int processID, string processName, string fileName, double timeStamp) : base(eventName, threadID, processID, processName, fileName, timeStamp)
            {
                EventName = eventName;
                ProcessID = processID;
                ProcessName = processName;
                FileName = fileName;
                TimeStamp = timeStamp;
                ThreadID = threadID;
            }

        }

        class FR : FileEvent
        {
            public override string FileName { get; }
            public override string ProcessName { get; }
            public override int ProcessID { get; }
            public override int ThreadID { get; }
            public override double TimeStamp { get; set; }
            public override string EventName { get; set; }
            public override ulong FileKey { get; }

            public FR(string eventName, int threadID, int processID, string processName, string fileName, double timeStamp) : base(eventName, threadID, processID, processName, fileName, timeStamp)
            {
                EventName = eventName;
                ProcessID = processID;
                ProcessName = processName;
                FileName = fileName;
                TimeStamp = timeStamp;
                ThreadID = threadID;
            }

        }
        public class FC : FileEvent
        {
            public override string FileName { get; }
            public override string ProcessName { get; }
            public override int ProcessID { get; }
            public override int ThreadID { get; }
            public override double TimeStamp { get; set; }
            public override string EventName { get; set; }
            public override ulong FileKey { get; }

            public FC(string eventName, int threadID, int processID, string processName, string fileName, double timeStamp) : base(eventName, threadID, processID, processName, fileName, timeStamp)
            {
                EventName = eventName;
                ProcessID = processID;
                ProcessName = processName;
                FileName = fileName;
                TimeStamp = timeStamp;
                ThreadID = threadID;
            }

        }

    }



}
