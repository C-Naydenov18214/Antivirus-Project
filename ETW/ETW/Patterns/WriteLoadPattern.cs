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
    public class WriteLoadPattern
    {
        public  int TestVarient()
        {
            var loadSubj = new Subject<FileEvent>();
            var writeSubj = new Subject<FileEvent>();
            var loadObs = loadSubj;
            var writeObs = writeSubj;
            int counter = 0;
            var byFile = loadObs.Merge<FileEvent>(writeObs)
                /*.Do(x => Console.WriteLine($"merge: {x.FileName}"))*/
                .GroupBy(el => el.FileName);

            byFile.SelectMany(fgr =>
            {
                var load = fgr.Where(el => el.EventName.CompareTo("Image/Load") == 0).Publish().RefCount();
                var write = fgr.Where(el => el.EventName.CompareTo("FileIO/Write") == 0).Publish().RefCount();
                return load.GroupJoin(write,
                    _ => Observable.Timer(TimeSpan.FromTicks(1)),
                    _ => Observable.Never<Unit>().TakeUntil(load.LastOrDefaultAsync().CombineLatest(write.LastOrDefaultAsync())),
                    (l, w) => (l, w))
                .SelectMany(x => x.w.Aggregate(new HashSet<int>(), (acc, v) => { acc.Add(v.ProcessID); return acc; }, acc => new { fname = x.l.FileName, loadBy = x.l.ProcessID, writes = acc }))
                .Where(x => x.writes.Contains(x.loadBy) && x.writes.Count != 0);
            }).Subscribe(x =>
            {
                counter++;
                Console.WriteLine($"-------\t{x.fname}\tload: {x.loadBy}\twrites: {String.Join(", ", x.writes.Select(i => i.ToString()))}");
            });
            /*for (int i = 0; i < 10; i++)
            {
                
                writeSubj.OnNext(new FileEvent("FileIO/Write", i, $"proc name = {i}", $"File = {i}", (ulong)i, i));
                writeSubj.OnNext(new FileEvent("FileIO/Write", i+1000, $"proc name = {i}", $"File = {i}", (ulong)i, i));
                loadSubj.OnNext(new FileEvent("Image/Load", i, $"proc name = {i}", $"File = {i}", (ulong)i, i));

            }*/


            writeSubj.OnNext(new FileEvent("FileIO/Write", 1, $"proc name = {1}", $"a.exe", (ulong)1, 1));
            
            loadSubj.OnNext(new FileEvent("Image/Load", 1, $"proc name = {1}", $"a.exe", (ulong)1, 1));
            
            writeSubj.OnNext(new FileEvent("FileIO/Write", 20, $"proc name = {1}", $"a.exe", (ulong)1, 1));
            writeSubj.OnNext(new FileEvent("FileIO/Write", 21, $"proc name = {2}", $"a.exe", (ulong)2, 2));
            writeSubj.OnNext(new FileEvent("FileIO/Write", 22, $"proc name = {3}", $"b.exe", (ulong)3, 3));
            writeSubj.OnNext(new FileEvent("FileIO/Write", 4, $"proc name = {1}", $"a.exe", (ulong)1, 1));
            writeSubj.OnNext(new FileEvent("FileIO/Write", 23, $"proc name = {5}", $"b.exe", (ulong)4, 4));
            writeSubj.OnNext(new FileEvent("FileIO/Write", 24, $"proc name = {5}", $"c.exe", (ulong)5, 5));

            loadSubj.OnNext(new FileEvent("Image/Load", 4, $"proc name = {4}", $"a.exe", (ulong)4, 4));
           
            writeSubj.OnNext(new FileEvent("FileIO/Write", 25, $"proc name = {7}", $"a.exe", (ulong)7, 7));
            
            loadSubj.OnNext(new FileEvent("Image/Load", 4, $"proc name = {66}", $"a.exe", (ulong)66, 66));
            loadSubj.OnNext(new FileEvent("Image/Load", 5, $"proc name = {66}", $"d.exe", (ulong)66, 66));
            loadSubj.OnNext(new FileEvent("Image/Load", 6, $"proc name = {66}", $"b.exe", (ulong)66, 66));

            
            loadSubj.OnCompleted();
            writeSubj.OnCompleted();
            return counter;
        }
    }
}
