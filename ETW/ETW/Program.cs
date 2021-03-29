using System;
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
        private static void Main(string[] args)
        {
            List<AutoResetEvent> events = new List<AutoResetEvent>(2);
            for (int i = 0; i < 3; i++)
            {
                events.Add(new AutoResetEvent(false));
            }
            var first = new BaseObserver<InternalEvent, InternalEvent>(1, events[0]);
            var second = new BaseObserver<InternalEvent, InternalEvent>(2, events[1]);
            var killer = new Killer<InternalEvent, InternalEvent>(3, events[2]);
            var input = new BaseObservable<InternalEvent>();

            input.ObserveOn(TaskPoolScheduler.Default).Subscribe(first);
            first.ConnectTo(second);
            second.ConnectTo(killer);
            /*for (int i = 0; i < 10; i++)
            {
                input.AddEvent(new InternalEvent(new TraceEventID(), i * 5, 656d));
            }
            input.Stop();
            AutoResetEvent.WaitAll(events.ToArray());*/
            //var dllInput = new BaseObservable<InternalEvent>();
            //MainClass.Main(null);
            /*List<AutoResetEvent> events = new List<AutoResetEvent>(6);
            for (int i = 0; i < 7; i++)
            {
                events.Add(new AutoResetEvent(false));
            }

            var first_f = new EventObserver<TraceEvent, InternalEvent>(1, events[0]);
            var second_f = new BaseObserver<InternalEvent, InternalEvent>(2, events[1]);

            var first_s = new EventObserver<TraceEvent, InternalEvent>(3, events[2]);
            var second_s = new BaseObserver<InternalEvent, InternalEvent>(4, events[3]);

            var first_t = new EventObserver<TraceEvent, InternalEvent>(5, events[4]);
            var second_t = new BaseObserver<InternalEvent, InternalEvent>(6, events[5]);

            var killer = new Killer<InternalEvent, InternalEvent>(7, events[6]);

            var dllInput = new BaseObservable<TraceEvent>();
            var fwInput = new BaseObservable<TraceEvent>();
            var frInput = new BaseObservable<TraceEvent>();

            dllInput.ObserveOn(TaskPoolScheduler.Default).Subscribe(first_f);
            first_f.ConnectTo(second_f);
            second_f.ConnectTo(killer);

            fwInput.ObserveOn(TaskPoolScheduler.Default).Subscribe(first_s);
            first_s.ConnectTo(second_s);
            second_s.ConnectTo(killer);

            frInput.ObserveOn(TaskPoolScheduler.Default).Subscribe(first_t);
            first_t.ConnectTo(second_t);
            second_t.ConnectTo(killer);*/

            var eventTracer = new EventTracer(Console.Out);
            eventTracer.setInputs(input);
            var task = Task.Run(eventTracer.Run);
            task.Wait();

            input.Stop();
            AutoResetEvent.WaitAll(events.ToArray());
        }
    }
}
