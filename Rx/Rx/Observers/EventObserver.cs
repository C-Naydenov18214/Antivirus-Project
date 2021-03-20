using Microsoft.Diagnostics.Tracing;
using System;
using System.Collections.Concurrent;
using System.Threading;

namespace Rx.Observers
{
    public class EventObserver<T> : BaseObserver<T> where T : TraceEvent
    {
        public EventObserver(int id, AutoResetEvent _event) : base(id,_event)
        {
            this.dictionary = new ConcurrentDictionary<int, T>();
        }

        public override void OnCompleted()
        {
            _event.Set();
            Console.WriteLine($"observer {Id} completed");
        }

        public override void OnError(Exception error)
        {
            Console.WriteLine(error.Message);
        }

        public override void OnNext(T value)
        {
            var ok = dictionary.TryAdd(value.ProcessID, value);
            if (!ok)
            {
                Console.WriteLine($"key {value.ProcessID} already exists");
            }
        }


        public override bool ContainsKey(int id) 
        {
            return dictionary.ContainsKey(id);
        }

        public override T GetValue(int id) 
        {
            T value;
            dictionary.TryGetValue(id, out value);
            return value;
        }
    }
}
