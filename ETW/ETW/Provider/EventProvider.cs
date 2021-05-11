using Microsoft.Diagnostics.Tracing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETW.Provider
{
    public class EventProvider<EventType>
    {
        public IObservable<EventType> Events { get; private set; } 

        public EventProvider(IObservable<TraceEvent> events)
        {
            Events = events.OfType<EventType>();
        }

        public void Subscribe() 
        {
        
        }

        public void UnSubscribe()
        {
        
        
        }



    }
}
