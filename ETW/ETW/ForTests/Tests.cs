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
            //Получаем  типы анализаторов 
            Type dllAType = typeof(DllLoadAnalyzer);
            Type createAType = typeof(CreateWriteAnalyzer);
            //Создаем выходной потом подозрительных событий 
            var sub = new Subject<SuspiciousEvent>();
            //Получаем провайдеров для контруктора анализатора
            List<object> dllArgsList = ReflectionKit.GetConstructorArgs(dllAType, eventTracer);
            List<object> createArgsList = ReflectionKit.GetConstructorArgs(createAType, eventTracer);
            //добавляем выходной поток
            dllArgsList.Add(sub);
            createArgsList.Add(sub);
            //создаем анализаторы 
            object dllAnalyzer = Activator.CreateInstance(dllAType, args:dllArgsList.ToArray());
            object createAnalyzer = Activator.CreateInstance(createAType, args:createArgsList.ToArray());
            //статуем анализаторы 
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
