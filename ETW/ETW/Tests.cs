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
            var dllsProcID = groupedByProcID.Select(procElems =>
            {
                var tmp = procElems.Select(el => el);
                var res = tmp.Where(el => el.EventName.CompareTo(image) == 0).GroupBy(elem => elem.ProcessID);
                return res;
            }).Merge();
            var writesProcID = groupedByProcID.Select(procElem =>
            {
                var tmp = procElem.Select(el => el);
                var res = tmp.Where(el => el.EventName.CompareTo(write) == 0).GroupBy(elem => elem.ProcessID);
                return res;
            }).Merge();
            
            //var merged = dllsProcID.Merge(writesProcID);

            /*dllsProcID.Subscribe(el =>
            {
                Console.WriteLine("Key = " + el.Key);
                el.Subscribe(w => Console.WriteLine($"\t{w.ProcessID} : {w.ProcessName} : {w.EventName} : {w.TimeStamp}"));
            });*/
            //dlls.Subscribe(w => Console.WriteLine($"\t{w.ProcessID} : {w.ProcessName} : {w.EventName} : {w.TimeStamp}"));
            //writes.Subscribe(w => Console.WriteLine($"\t{w.ProcessID} : {w.ProcessName} : {w.EventName} : {w.TimeStamp}"));

            //var resultEvents = dllsProcID.   C .CombineLatest(writesProcID,ConcatFunc);
            //resultEvents.Subscribe(el => PrintResultEvent(el));
        }


        public static void PrintResultEvent(ResultEvent eve) 
        {
            var w = eve.DllEvent;
            var w1 = eve.WriteEvent;
            Console.WriteLine($"\t{w.ProcessID} : {w.ProcessName} : {w.EventName} : {w.TimeStamp} <------> {w1.ProcessID} : {w1.ProcessName} : {w1.EventName} : {w1.TimeStamp}");


        }

        public static ResultEvent ConcatFunc(FileEvent dll, FileEvent write)
        {
            return new ResultEvent(dll, write);
        
        }
 

    }
}
