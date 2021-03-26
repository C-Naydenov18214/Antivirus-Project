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


        public InternalEvent(TraceEventID eventType, int processID, double timeStamp)
        {
            this.EventType = eventType;
            this.ProcessID = processID;
            this.TimeStamp = timeStamp;
        }

        public InternalEvent(TraceEventID eventType)
        {
            this.EventType = eventType;
        }

    }
}
