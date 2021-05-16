using System;
using System.Reactive.Subjects;
using ETW.Tracer;
using Kit;
using Unity;
using Unity.Injection;
using Unity.Lifetime;

namespace App.IoC
{
    public class ContainerConfigurator
    {
        public static void Initialization(IUnityContainer container)
        {
            container.RegisterType<IObservable<WriteEvent>, Subject<WriteEvent>>(
                new ContainerControlledLifetimeManager());
            container.RegisterType<IObservable<ReadEvent>, Subject<ReadEvent>>(
                new ContainerControlledLifetimeManager());
        }

        public static void Initialization(IUnityContainer container, Subject<SuspiciousEvent> suspiciousEvents)
        {
            container.RegisterInstance(suspiciousEvents, new ContainerControlledLifetimeManager());
        }
    }
}