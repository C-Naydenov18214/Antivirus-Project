﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using ETW.Patterns;
using ETW.Tracer;
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

            //MainClass.Main(null);
            /*List<AutoResetEvent> events = new List<AutoResetEvent>(6);
            for (int i = 0; i < 7; i++)
            {
                events.Add(new AutoResetEvent(false));
            }

            var first_f = new BaseObserver<InternalEvent, InternalEvent>(1, events[0]);
            var second_f = new BaseObserver<InternalEvent, InternalEvent>(2, events[1]);

            var first_s = new BaseObserver<InternalEvent, InternalEvent>(3, events[2]);
            var second_s = new BaseObserver<InternalEvent, InternalEvent>(4, events[3]);

            var first_t = new BaseObserver<InternalEvent, InternalEvent>(5, events[4]);
            var second_t = new BaseObserver<InternalEvent, InternalEvent>(6, events[5]);

            var killer = new Killer<InternalEvent, InternalEvent>(7, events[6]);

            var dllInput = new BaseObservable<InternalEvent>();
            var fwInput = new BaseObservable<InternalEvent>();
            var frInput = new BaseObservable<InternalEvent>();

            dllInput.ObserveOn(TaskPoolScheduler.Default).Subscribe(first_f);
            first_f.ConnectTo(second_f);
            second_f.ConnectTo(killer);

            fwInput.ObserveOn(TaskPoolScheduler.Default).Subscribe(first_s);
            first_s.ConnectTo(second_s);
            second_s.ConnectTo(killer);

            frInput.ObserveOn(TaskPoolScheduler.Default).Subscribe(first_t);
            first_t.ConnectTo(second_t);
            second_t.ConnectTo(killer);

            var eventTracer = new EventTracer(Console.Out);
            eventTracer.SetInputs(dllInput, fwInput, frInput);
            var task = Task.Run(eventTracer.Run);
            task.Wait();
            
            dllInput.Stop();
            fwInput.Stop();
            frInput.Stop();
            AutoResetEvent.WaitAll(events.ToArray());
            killer.Result();
            Console.ReadLine();*/
            List<FileEvent> dllList = new List<FileEvent>(100);
            List<FileEvent> wrList = new List<FileEvent>(100);
            for (int i = 0; i < 100; i++)
            {
                dllList.Add(new FileEvent("Image/Load", i, $"some name {i}", $"some File {i}", (ulong) i, i));
                wrList.Add(new FileEvent("FileIO/Write", i, $"some name {i}", $"some File {i}", (ulong)i, i));

            }
           // var eventTracer = new EventTracer(Console.Out);
            //var task = Task.Run(eventTracer.Test);
           // Thread.Sleep(1000);
            var dllObs = dllList.ToObservable();
            var wrObs = wrList.ToObservable();

            var procGroups = dllObs.Merge(wrObs).GroupBy(el => el.FileName);//eventTracer.mergedGroups;
            //Tests.TestVarient(procGroups);
            //WriteLoadPattern.TestVarient(procGroups);
            ReadWritePattern.TestVarient(procGroups);
            //Cript.Test();
            //Console.WriteLine("Wait");
            //procGroups.Subscribe(group => ProcessGroup(group));
            //task.Wait();
            /*int i = 0;
            Console.WriteLine($"Process count = {dictFileEvents.Count}");
            foreach (var dictElem in dictFileEvents)
            {
                Console.WriteLine($"{i++}. Process ID = {dictElem.Key}");
                foreach (var bag in dictElem.Value)
                {
                    Console.WriteLine($"\t{bag.ProcessID} {bag.ProcessName} {bag.EventName} {bag.TimeStamp}");
                }

            }*/
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
