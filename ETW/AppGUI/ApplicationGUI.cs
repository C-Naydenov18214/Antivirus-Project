using System;
using System.Collections.Generic;
using System.Reactive.Subjects;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using App;
using App.IoC;
using ETW.Provider;
using ETW.Reflection;
using ETW.Tracer;
using Kit;
using Unity;

namespace AppGUI
{
    public class ApplicationGUI
    {
        private static readonly Subject<SuspiciousEvent> SuspiciousEvents = new Subject<SuspiciousEvent>();
        public static readonly EventTracer EventTracer = new EventTracer();
        private static readonly IUnityContainer Container = new UnityContainer();
        public static Dashboard Dashboard;

        /// <summary>
        /// Run application.
        /// </summary>
        /// <param name="args">Rx DLLs</param>
        /// <param name="report">TextBox reference</param>
        public static void Run(string[] args, TextBox report)
        {
            var dashboard = new Dashboard(report);
            Dashboard = dashboard;
            var task = Task.Run(EventTracer.Test);
            Thread.Sleep(1000);
            SuspiciousEvents.Subscribe(ev =>
            {
                dashboard.AddOrUpdate(ev.ProcessId, new KeyValuePair<SuspiciousEvent, int>(ev, 1));
                dashboard.Show();
            });
            ContainerConfigurator.Initialization(Container, SuspiciousEvents);

            foreach (var value in args)
            {
                LoadAnalyzer(value);
            }
        }

        public static void LoadAnalyzer(string value)
        {
            var assembly = Assembly.LoadFile(value);
            var type = TypeFinder.GetType(assembly);
            var arguments = ReflectionKit.GetConstructorArgs(type, EventTracer);
            foreach (var provider in arguments)
            {
                var iProvider = provider as IEventProvider;
                iProvider?.Subscribe(Container);
            }
            var analyzer = (ARxAnalyzer)Container.Resolve(type);
            Task.Run(analyzer.Start);
        }
    }
}