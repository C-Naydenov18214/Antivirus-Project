using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using ETW.Tracer;
using Microsoft.Diagnostics.Tracing;
using Rx;
using Rx.MainModule;
using Rx.Observers;

namespace ETW
{
    sealed class Program
    {
        public static ConcurrentDictionary<int, ConcurrentBag<InternalEvent>> dict = new ConcurrentDictionary<int, ConcurrentBag<InternalEvent>>();

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

            var eventTracer = new EventTracer(Console.Out);
            var task = Task.Run(eventTracer.Test);
            Thread.Sleep(1000);
            var procGroups = eventTracer.mergedGroups;
            procGroups.Subscribe(group => ProcessGroup(group));
            task.Wait();
            int i = 0;
            Console.WriteLine($"Process count = {dict.Count}");
            foreach (var dictElem in dict)
            {
                Console.WriteLine($"{i++}. Process ID = {dictElem.Key}");
                foreach (var bag in dictElem.Value)
                {
                    Console.WriteLine($"{bag.EventType} {bag.EventName} {bag.TimeStamp}");
                }

            }
            Console.ReadLine();
        }


        public static void TestProcessGroup(IGroupedObservable<int, InternalEvent> group)
        {
            var res = group.GroupBy(i => i.EventName);//.Select(n => new InternalEvent() { EventName = n.Key.EventName, ProcessID = n.Key.ProcessID });

        }

        public static void ProcessGroup(IGroupedObservable<int, InternalEvent> group)
        {
            Console.WriteLine($"{dict.Count}. Group key = {group.Key}");
            group.Subscribe(data => AddEvent(data));//Console.WriteLine($"\t ProcessID = {data.ProcessID} Eventname = { data.EventName} EventType = { data.EventType} TimeStamp = {data.TimeStamp} "));
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
    }


}
