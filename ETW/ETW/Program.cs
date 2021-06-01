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

namespace ETW
{
    sealed class Program
    {
     
        static int c = 0;
        static Mutex mutex = new Mutex();
        private static void Main(string[] args)
        {

            //ETW.ForTests.Tests.Run();
            var t = new WriteLoadPattern();
            t.TestVarient();
            /*var eventTracer = new EventTracer(Console.Out);

            var task = Task.Run(eventTracer.Test);
            Thread.Sleep(1000);

            //Создаем выходной потом подозрительных событий 
            var sub = new Subject<SuspiciousEvent>();
            var dllProvider = new EventProvider<ImageLoadTraceData>(eventTracer.AllEvents);
            var writeProvider = new EventProvider<FileIOReadWriteTraceData>(eventTracer.AllEvents);
            var createProvider = new EventProvider<FileIOCreateTraceData>(eventTracer.AllEvents);

            var dllAnalyzer = new DllLoadAnalyzer(dllProvider, sub);
            var createsAnalyzer = new CreateWriteAnalyzer(createProvider.Events, writeProvider.Events, sub);
            dllAnalyzer.Start();
            createsAnalyzer.Start();
            Cript.Test();
            Console.WriteLine("Wait");
            //подписываемся на выходной поток подозрительных событий, оно и без SubscribeOn работает 
            sub.SubscribeOn(Scheduler.Default).Subscribe(e => Console.WriteLine($"Main Thread: {Thread.CurrentThread.ManagedThreadId} susp val = {e.ProcessId}"));
            task.Wait();

            Console.ReadLine();*/
        }
    }


}
