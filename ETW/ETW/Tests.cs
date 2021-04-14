using Rx.MainModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETW
{
    class Tests
    {
        public static string image = "Image/Load";
        public static string write = "FileIO/Write";

        public static void TestVarient(IObservable<IGroupedObservable<string, FileEvent>> groupedByFile)
        { 
            var groupedByProcID = groupedByFile.Select(gElem =>
            {
                var res = gElem.Select(elem => elem);
                var r = res.GroupBy(elem => elem.ProcessID);
                return r;

            }).Merge();//   Subscribe(g => ProcessGroup(g));
            var dlls = groupedByProcID.Select(elem =>
            {
                var tmp = elem.Select(el => el);
                var res = tmp.Where(el => el.EventName.CompareTo(image) == 0);
                return res;
            }).Merge();
            var writes = groupedByProcID.Select(elem =>
            {
                var tmp = elem.Select(el => el);
                var res = tmp.Where(el => el.EventName.CompareTo(write) == 0);
                return res;
            }).Merge();
            
            dlls.Subscribe(w => Console.WriteLine($"\t{w.ProcessID} : {w.ProcessName} : {w.EventName} : {w.TimeStamp}"));
            writes.Subscribe(w => Console.WriteLine($"\t{w.ProcessID} : {w.ProcessName} : {w.EventName} : {w.TimeStamp}"));
            dlls.CombineLatest(writes,ConcatFunc);
        }

        public static ResultEvent ConcatFunc(FileEvent dll, FileEvent write)
        {
            return new ResultEvent(dll, write);
        
        }
 

    }
}
