using System;
using System.Reactive.Subjects;
using Kit;
using Unity;
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
    }
}