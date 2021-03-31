using Rx.MainModule;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Rx.Observers
{
    public class BaseObserver<I, O>  where O : InternalEvent
    {
        public static void OnCompleted()
        {
            Console.WriteLine($"observer completed");
        }

        public static void OnError(Exception error)
        {
            Console.WriteLine(error.Message);
        }

        public static void OnNext(I value)
        {
            var v = (value as InternalEvent);
            Console.WriteLine($"obs got value = {v.EventType} {v.EventName} {v.ProcessID} {v.TimeStamp}");
        }

    }
}

