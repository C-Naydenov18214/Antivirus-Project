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
        public static void Configure(IUnityContainer container)
        {
            container.RegisterType<IObservable<WriteEvent>, Subject<WriteEvent>>(
                new ContainerControlledLifetimeManager());
            container.RegisterType<IObservable<ReadEvent>, Subject<ReadEvent>>(
                new ContainerControlledLifetimeManager());
        }

        public static void Configure(IUnityContainer container, Subject<SuspiciousEvent> suspiciousEvents, EventTracer eventTracer)
        {
            #region test
            container.RegisterType<IObservable<WriteEvent>, Subject<WriteEvent>>(
                new ContainerControlledLifetimeManager());
            container.RegisterType<IObservable<ReadEvent>, Subject<ReadEvent>>(
                new ContainerControlledLifetimeManager());
            #endregion

            container.RegisterInstance(suspiciousEvents, new ContainerControlledLifetimeManager());
            container.RegisterInstance(eventTracer.Dlls, new ContainerControlledLifetimeManager());
            container.RegisterInstance(eventTracer.Writes, new ContainerControlledLifetimeManager());
            container.RegisterInstance(eventTracer.Creates, new ContainerControlledLifetimeManager());
;       }
    }
}