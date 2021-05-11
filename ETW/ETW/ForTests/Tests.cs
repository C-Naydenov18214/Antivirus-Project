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
using ETW.Provider;
using ETW.Reflection;
using ETW.Tracer;
using Kit;
using Microsoft.Diagnostics.Tracing.Parsers.Kernel;
using Rx.MainModule;

namespace ETW.ForTests
{
    class Tests
    {
        public static void Run()
        {

            var eventTracer = new EventTracer(Console.Out);

            var task = Task.Run(eventTracer.Test);
            Thread.Sleep(1000);
            Type dllAType = typeof(DllLoadAnalyzer);
            Type createAType = typeof(CreateWriteAnalyzer);
            //Создаем выходной потом подозрительных событий 
            var sub = new Subject<SuspiciousEvent>();
            var dllList = ReflectionKit.GetConstructerArgs(dllAType, eventTracer);
            var createList = ReflectionKit.GetConstructerArgs(createAType, eventTracer);
            //var dllAnalyzer = new DllLoadAnalyzer(((EventProvider<ImageLoadTraceData>)providers[0]).Events,sub);

            object dllAnalyzer = Activator.CreateInstance(dllAType, dllList[0], sub);
            object createAnalyzer = Activator.CreateInstance(createAType, createList[0],createList[1], sub);
            dllAType.GetMethod("Start").Invoke(dllAnalyzer, null);
            createAType.GetMethod("Start").Invoke(createAnalyzer, null);
            Console.WriteLine("Wait");
            //подписываемся на выходной поток подозрительных событий, оно и без SubscribeOn работает 
            sub.SubscribeOn(Scheduler.Default).Subscribe(e => Console.WriteLine($"Main Thread: {Thread.CurrentThread.ManagedThreadId} susp val = {e.ProcessId}"));
            task.Wait();

            Console.ReadLine();

        }


    }
}
