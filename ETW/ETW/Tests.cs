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



            //var comb = dllsProcID.CombineLatest(writesProcID, ConcatFunc);
            //comb.Subscribe(Prin)

            //var merged = dllsProcID.Merge(writesProcID);

            /*дебаг статистики последовательностей*/
            
            /*groupedByProcID.Subscribe(el =>
            {
                Console.WriteLine("Key = " + el.Key);
                el.Subscribe(w => Console.WriteLine($"by id \t{w.ProcessID} : {w.ProcessName} : {w.EventName} : {w.TimeStamp}"));
            });


            groupedByFile.Subscribe(el =>
            {
                Console.WriteLine("Key = " + el.Key);
                el.Subscribe(w => Console.WriteLine($"by file \t{w.ProcessID} : {w.ProcessName} : {w.EventName} : {w.TimeStamp}"));
            });
            dllsProcID.Subscribe(el =>
            {
                el.Count().Subscribe(c => Console.WriteLine($"Dll {el.Key} counter = {c}"));
            });
            writesProcID.Subscribe(el => {
                el.Count().Subscribe(c => Console.WriteLine($"WR {el.Key} counter = {c}"));

            });*/



            var zipped = Observable.When(writesProcID.And(dllsProcID).Then((dll, wr) =>
            {
                return ConcatFunc(dll, wr);
            })).Select(concated =>
            {
                //Console.WriteLine($"\tin zipped dll = {concated.Dll.Key}, wr = {concated.Writers.Key}");
                /*
                 * var dll = concated.Dll.Select(el => el).CombineLatest(concated.Writers.Select(el => el),ConcatFunc);
                 * return dll;
                 * сокращеный вариант
                 */
                var dll = concated.Dll.Select(el => el);
                dll.Count().Subscribe(el => Console.WriteLine($"\tdll {concated.Dll.Key} in rezz = {el}")); // <- вот тут всегда нет dll элемента
                var wr = concated.Writers.Select(el => el);
                wr.Count().Subscribe(el => Console.WriteLine($"\twr {concated.Writers.Key} in rezz = {el}"));
                var res = dll.CombineLatest(wr, ConcatFunc);

                return res;


            }).Merge().Subscribe(el => PrintResultEvent(el));
            /*zipped.Count().Subscribe(el => Console.WriteLine($"\t \tzipped = {el}"));
            var rezz = zipped.Select(concated =>
            {
                //Console.WriteLine($"\tin zipped dll = {concated.Dll.Key}, wr = {concated.Writers.Key}");
                var dll = concated.Dll.Select(el => el);
                dll.Count().Subscribe(el => Console.WriteLine($"\tdll {concated.Dll.Key} in rezz = {el}"));
                var wr = concated.Writers.Select(el => el);
                wr.Count().Subscribe(el => Console.WriteLine($"\twr {concated.Writers.Key} in rezz = {el}"));
                var res = dll.CombineLatest(wr, ConcatFunc);

                return res;


            }).Merge();
            rezz.Count().Subscribe(el => Console.WriteLine($"\t \trezz = {el}"));
            rezz.Subscribe(el => PrintResultEvent(el));*/

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
            Console.WriteLine($"proc = {dll.ProcessID} {dll.EventName} | {write.ProcessID} {write.EventName}");
            return new ResultEvent(dll, write);
        
        }
        public static ConcatedStreams ConcatFunc(IGroupedObservable<int, FileEvent> dll, IGroupedObservable<int, FileEvent> writers)
        {
            //Console.WriteLine($"Created 1 {dll.Key} {writers.Key}");
            //dll.Count().Subscribe(c => Console.WriteLine(c));
            //dll.Subscribe(w => Console.WriteLine($"\t{w.ProcessID} : {w.ProcessName} : {w.EventName} : {w.TimeStamp}"));
            dll.Count().Subscribe(el => Console.WriteLine($"\tdll {dll.Key} in ConcatFunc = {el}"));
            writers.Count().Subscribe(el => Console.WriteLine($"\twr {writers.Key} in ConcatFunc = {el}"));
            return new ConcatedStreams(dll, writers);

        }
        /*public static ResultEvent ConcatFunc(FileEvent dll, FileEvent write)
        {
            return new ResultEvent(dll, write);

        }*/


    }
}
