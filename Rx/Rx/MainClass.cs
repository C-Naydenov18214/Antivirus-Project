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

namespace Rx
{
    class MainClass
    {
        public static void Main(string[] args) 
        {
            var obs1 = new EventObserver<ImageLoadTraceData>(0,null);
            var obs2 = new EventObserver<ProcessTraceData>(0,null);
            var obs3 = new EventObserver<ProcessTraceData>(0,null);
            
            var dllEvents = new TraceEventsObservable<ImageLoadTraceData>();
            var processStartEvents = new TraceEventsObservable<ProcessTraceData>();
            var processEndEvents = new TraceEventsObservable<ProcessTraceData>();
            processStartEvents.ObserveOn(TaskPoolScheduler.Default).Subscribe(obs2);
            processEndEvents.ObserveOn(TaskPoolScheduler.Default).Subscribe(obs3);
            dllEvents.ObserveOn(TaskPoolScheduler.Default).Subscribe(obs1);
            var judgeEvents =  new BaseObservable<JudgeEvent>();
            var judge = new Judge<JudgeEvent>(0, null,1, 2, 34);


        }


    }
}
