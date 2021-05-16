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
                dashboard.AddOrUpdate(ev.ProcessId, new KeyValuePair<SuspiciousEvent, int>(ev,1));
                dashboard.Show();
            });
            IUnityContainer container = new UnityContainer();
            ContainerConfigurator.Initialization(container, SuspiciousEvents);

            foreach (var value in args)
            {
                LoadAnalyzer(value, eventTracer, container);
            }

            Console.WriteLine("Please provide path to dll. Enter 'load `path to dll`'");
            var line = Console.ReadLine();
            Console.WriteLine("Read line");
            while (!line.Equals("exit"))
            {
                if (int.TryParse(line, out var id))
                {
                    dashboard.Kill(id);
                }
                else if (line.Contains("load"))
                {
                    line = line.Replace("load", "").Replace(" ", "");
                    LoadAnalyzer(line, eventTracer, container);
                    line = Console.ReadLine();
                }
                else 
                {
                    Console.WriteLine("Enter valid id");
                    line = Console.ReadLine();
                }
            }
            Console.WriteLine("Stopping application...");
            eventTracer.GetKernelSession()?.Dispose();

            Console.ReadKey();
        }

        private static void LoadAnalyzer(string value, EventTracer eventTracer, IUnityContainer container)
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
    }
}