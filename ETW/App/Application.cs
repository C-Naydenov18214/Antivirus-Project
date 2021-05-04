using System;
using System.Collections.Generic;
using System.Reactive.Subjects;
using System.Reflection;
using App.IoC;
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
            SuspiciousEvents.Subscribe(ev => dashboard.AddOrUpdate(ev.ProcessId, 1));

            IUnityContainer container = new UnityContainer();
            ContainerConfigurator.Configure(container, SuspiciousEvents);

            foreach (var value in args)
            {
                var assembly = Assembly.LoadFile(value);
                var type = TypeFinder.GetType(assembly);
                var analyzer = (ARxAnalyzer)container.Resolve(type);
                analyzer.Start();
            }
            Console.ReadKey();
        }
    }
}