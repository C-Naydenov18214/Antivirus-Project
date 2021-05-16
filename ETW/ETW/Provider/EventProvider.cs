using Microsoft.Diagnostics.Tracing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;

namespace ETW.Provider
{
    public class EventProvider<TEventType> : IEventProvider
    {
        public IObservable<TEventType> Events { get; private set; }

        public EventProvider(IObservable<TraceEvent> events)
        {
            Events = events.OfType<TEventType>();
        }

        public void Subscribe(IUnityContainer container)
        {
            Console.WriteLine("Trying to register instance...");
            container.RegisterInstance(Events);
        }

        public void Unsubscribe(IUnityContainer container)
        {
            throw new NotImplementedException();
        }
    }
}
