using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using ETW.Patterns;
using ETW.Patterns.Analyzer;
using ETW.Tracer;
using Kit;
using Rx.MainModule;

namespace ETW
{
    sealed class Program
    {
        public static ConcurrentDictionary<int, ConcurrentBag<InternalEvent>> dict = new ConcurrentDictionary<int, ConcurrentBag<InternalEvent>>();
        public static ConcurrentDictionary<string, ConcurrentBag<FileEvent>> dictFileEvents = new ConcurrentDictionary<string, ConcurrentBag<FileEvent>>();
        public static ConcurrentDictionary<int, ProcessStatus> suspicions = new ConcurrentDictionary<int, ProcessStatus>();
        static int c = 0;
        static Mutex mutex = new Mutex();
        private static void Main(string[] args)
        {
            var eventTracer = new EventTracer(Console.Out);
            var task = Task.Run(eventTracer.Test);
            Thread.Sleep(1000);
            //Tests.TestVarient(procGroups);
            //WriteLoadPattern.TestVarient(procGroups);
            //Создаем выходной потом подозрительных событий 
            var sub = new Subject<SuspiciousEvent>();
            //создаем анализатор с нужными событиями 
            var dllAnalyzer = new DllLoadAnalyzer(eventTracer.Dlls,sub);
            var createsAnalyzer = new CreateWriteAnalyzer(eventTracer.Creates,eventTracer.Writes, sub);
            dllAnalyzer.Start();
            createsAnalyzer.Start();
            //Cript.Test();
            Console.WriteLine("Wait");
            //подписываемся на выходной поток подозрительных событий, оно и без SubscribeOn работает 
            sub.SubscribeOn(Scheduler.Default).Subscribe(e => Console.WriteLine($"Main Thread: {Thread.CurrentThread.ManagedThreadId} susp val = {e.ProcessId}"));
            task.Wait();

            Console.ReadLine();
        }






        /*public static void TestProcessGroup(IGroupedObservable<int, InternalEvent> group)
        {
            var res = group.GroupBy(i => i.EventName);//.Select(n => new InternalEvent() { EventName = n.Key.EventName, ProcessID = n.Key.ProcessID });

        }*/

        public static void ProcessGroup(IGroupedObservable<int, InternalEvent> group)
        {
            Console.WriteLine($"{dictFileEvents.Count}. Group key = {group.Key}");
            group.Subscribe(data => AddEvent(data));
        }

        public static void ProcessGroup(IGroupedObservable<string, FileEvent> group)
        {
            Console.WriteLine($"{dictFileEvents.Count}. Group key = {group.Key}");
            group.Window(TimeSpan.FromMilliseconds(200)).Subscribe(w => ProcessWindow(w));
            group.Subscribe(data => AddEvent(data));
        }

        public static void ProcessWindow(IObservable<FileEvent> window)
        {
            /*var em = window.ToEnumerable<FileEvent>();
            bool isLoad = false;
            bool isWrite = false;
            Console.WriteLine($"\t HERE");
            foreach (var el in em)
            {
                PrintWindowElem(el);
            }*/
            window.Subscribe(w => PrintWindowElem(w));

        }

        private static void AnalyzeWindow(IObservable<FileEvent> window)
        {
            var v = window.GroupBy(i => i.ProcessID).Subscribe(g => ProcessGroup(g));//.Subscribe для обработки одельного элемента;
            //window.Subscribe(w => AnalyzeGroupElement(w));
        }
        public static void ProcessGroup(IGroupedObservable<int, FileEvent> group)
        {

            group.Subscribe(el => AnalyzeGroupElement(el));
        }

        private static void AnalyzeGroupElement(FileEvent elem)
        {
            ProcessStatus curProc;
            if (suspicions.ContainsKey(elem.ProcessID))
            {
                curProc = suspicions[elem.ProcessID];
            }
            else
            {


            }

        }

        private static void PrintWindowElem(FileEvent w)
        {
            mutex.WaitOne();
            Console.WriteLine($"\t{c}. {w.ProcessID} : {w.ProcessName} : {w.EventName} : {w.TimeStamp}");
            c++;
            mutex.ReleaseMutex();

        }
        public static void AddEvent(InternalEvent elem)
        {
            if (dict.ContainsKey(elem.ProcessID))
            {
                dict[elem.ProcessID].Add(elem);
            }
            else
            {
                var bag = new ConcurrentBag<InternalEvent>();
                bag.Add(elem);
                dict.TryAdd(elem.ProcessID, bag);

            }
        }

        public static void AddEvent(FileEvent elem)
        {
            if (dictFileEvents.ContainsKey(elem.FileName))
            {
                dictFileEvents[elem.FileName].Add(elem);
            }
            else
            {
                var bag = new ConcurrentBag<FileEvent>();
                bag.Add(elem);
                dictFileEvents.TryAdd(elem.FileName, bag);

            }
        }

        private static void AddOrUpdate(ConcurrentDictionary<object, object> dict, object elem)
        {


        }
    }


}
