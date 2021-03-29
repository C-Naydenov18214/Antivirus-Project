using Microsoft.Diagnostics.Tracing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rx.MainModule
{
    public class InternalEvent
    {
        public TraceEventID EventType { get; }
        public int ProcessID { get; }
        public double TimeStamp { get; }
        public string EventName { get; }


        public InternalEvent(TraceEventID eventType,string eventName ,int processID, double timeStamp)
        {
            this.EventType = eventType;
            this.EventName = eventName;
            this.ProcessID = processID;
            this.TimeStamp = timeStamp;
        }

        public InternalEvent(TraceEventID eventType)
        {
            TraceEvent a;
            this.EventType = eventType;
        }

    }
}
