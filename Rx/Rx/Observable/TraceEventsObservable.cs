using Microsoft.Diagnostics.Tracing;
using Rx.MainModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rx.Observable
{
    class TraceEventsObservable<T> : BaseObservable<T> where T : TraceEvent
    {
        public TraceEventsObservable() {
           
        
        }


    }
}
