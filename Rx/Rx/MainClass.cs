using Microsoft.Diagnostics.Tracing;
using Microsoft.Diagnostics.Tracing.Parsers.Kernel;
using Rx.MainModule;
using Rx.Observable;
using Rx.Observers;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Rx
{
    public class MainClass
    {
        public static void Main(string[] args) 
        {
            /*var obs1 = new EventObserver<ImageLoadTraceData,InternalEvent>(0,null);
            var obs2 = new EventObserver<ProcessTraceData, InternalEvent>(0,null);
            var obs3 = new EventObserver<ProcessTraceData, InternalEvent>(0,null);
           
            var dllEvents = new TraceEventsObservable<ImageLoadTraceData>();
            var processStartEvents = new TraceEventsObservable<ProcessTraceData>();
            var processEndEvents = new TraceEventsObservable<ProcessTraceData>();
            processStartEvents.ObserveOn(TaskPoolScheduler.Default).Subscribe(obs2);
            processEndEvents.ObserveOn(TaskPoolScheduler.Default).Subscribe(obs3);
            dllEvents.ObserveOn(TaskPoolScheduler.Default).Subscribe(obs1);
            var killerEvents =  new BaseObservable<InternalEvent>();
            var killer = new Killer<InternalEvent, InternalEvent>(0, null,1, 2, 34);
            killerEvents.ObserveOn(TaskPoolScheduler.Default).Subscribe(killer); */
            ConnectionTest();




        }



        public static void ConnectionTest() 
        {
            List<AutoResetEvent> events = new List<AutoResetEvent>(2);
            for (int i = 0; i < 3; i++)
            {
                events.Add(new AutoResetEvent(false));
            }
            var first = new BaseObserver<InternalEvent, InternalEvent>(1,events[0]);
            var second = new BaseObserver<InternalEvent, InternalEvent>(2, events[1]);
            var killer = new Killer<InternalEvent, InternalEvent>(3, events[2]);
            var input = new BaseObservable<InternalEvent>();
            
            input.ObserveOn(TaskPoolScheduler.Default).Subscribe(first);
            first.ConnectTo(second);
            second.ConnectTo(killer);
            for (int i = 0; i < 10; i++) 
            {
                input.AddEvent(new InternalEvent(new TraceEventID(), i * 5,656d)); 
            }
            input.Stop();
            AutoResetEvent.WaitAll(events.ToArray());


        }


    }
}
