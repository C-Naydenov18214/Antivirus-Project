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
        public TraceEventID EventType { get; set; }
        public int ProcessID { get; set; }
        public double TimeStamp { get; set; }
        public string EventName { get; set; }


        public InternalEvent(TraceEventID eventType, string eventName, int processID, double timeStamp)
        {
            this.EventType = eventType;
            this.EventName = eventName;
            this.ProcessID = processID;
            this.TimeStamp = timeStamp;
        }

        public InternalEvent()
        {
        }

        public InternalEvent(TraceEventID eventType)
        {
            TraceEvent a;
            this.EventType = eventType;
        }

    }
}
