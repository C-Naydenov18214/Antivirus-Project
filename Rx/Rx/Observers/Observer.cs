using Microsoft.Diagnostics.Tracing;
using Microsoft.Diagnostics.Tracing.Parsers.Kernel;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Rx.Observers
{
    class Observer<T> : IObserver<T> where T : TraceEvent
    {

        public int Id { get; }
        public ConcurrentDictionary<int, T> dictionary;
        private AutoResetEvent _event;

        public Observer(int id, AutoResetEvent _event)
        {
            this.dictionary = new ConcurrentDictionary<int, T>();
            this.Id = id;
            this._event = _event;
        }

        public void OnCompleted()
        {
            _event.Set();
            Console.WriteLine($"observer {Id} completed");
        }

        public void OnError(Exception error)
        {
            Console.WriteLine(error.Message);
        }

        public void OnNext(T value)
        {
            var ok = dictionary.TryAdd(value.ProcessID, value);
            if (!ok)
            {
                Console.WriteLine($"key {value.ProcessID} already exists");
            }
        }
    }
}
