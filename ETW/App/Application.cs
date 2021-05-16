using System;
using System.Collections.Generic;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using App.IoC;
using ETW.Provider;
using ETW.Reflection;
using Kit;
using ETW.Tracer;
using Unity;

namespace App
{
    public class Application
    {
        private static readonly Subject<SuspiciousEvent> SuspiciousEvents = new Subject<SuspiciousEvent>();

        /// <summary>
        /// Run application.
        /// </summary>
        /// <param name="args">Rx DLLs</param>
        public static void Run(string[] args)
        {
            var dashboard = new Dashboard();
            var eventTracer = new EventTracer();
            var task = Task.Run(eventTracer.Test);
            Thread.Sleep(1000);
            SuspiciousEvents.Subscribe(ev =>
            {
                dashboard.AddOrUpdate(ev.ProcessId, 1);
                dashboard.Show();
            });
            IUnityContainer container = new UnityContainer();
            ContainerConfigurator.Initialization(container, SuspiciousEvents);

            foreach (var value in args)
            {
                var assembly = Assembly.LoadFile(value);
                var type = TypeFinder.GetType(assembly);
                var arguments = ReflectionKit.GetConstructorArgs(type, eventTracer);
                foreach (var provider in arguments)
                {
                    var iProvider = provider as IEventProvider;
                    iProvider?.Subscribe(container);
                }
                var analyzer = (ARxAnalyzer)container.Resolve(type);
                Task.Run(analyzer.Start);
            }

            var line = Console.ReadLine();
            while (!line.Equals("stop"))
            {
                if (int.TryParse(line, out var id))
                {
                    dashboard.Kill(id);
                }
                else
                {
                    Console.WriteLine("Enter valid id");
                }
            }
            Console.WriteLine("Stopping application...");
            eventTracer.GetKernelSession()?.Dispose();

            Console.ReadKey();
        }
    }
}