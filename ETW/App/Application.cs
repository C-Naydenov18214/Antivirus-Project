using System;
using System.Reactive.Subjects;
using System.Reflection;
using App.IoC;
using Kit;
using Unity;

namespace App
{
    public class Application
    {
        private static Subject<SuspiciousEvent> _suspiciousEvents;
        /// <summary>
        /// Run application.
        /// </summary>
        /// <param name="args">Rx DLLs</param>
        public static void Run(string[] args)
        {
            _suspiciousEvents = new Subject<SuspiciousEvent>();
            IUnityContainer container = new UnityContainer();
            ContainerConfigurator.Configure(container, _suspiciousEvents);

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