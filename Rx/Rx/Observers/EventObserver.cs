using Microsoft.Diagnostics.Tracing;
using Rx.MainModule;
using System;
using System.Collections.Concurrent;
using System.Threading;

namespace Rx.Observers
{
    public class EventObserver<I,O> : BaseObserver<I,O> where I : TraceEvent where O : InternalEvent
    {
        public EventObserver(int id, AutoResetEvent _event) : base(id, _event)
        {
            this.dictionary = new ConcurrentDictionary<int, I>();
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

        public override void OnNext(I value)
        {
            var ok = dictionary.TryAdd(value.ProcessID, value);
            if (!ok)
            {
                Console.WriteLine($"key {value.ProcessID} already exists");
            }
            /*process event som how*/

            var e = new InternalEvent(value.ID, value.ProcessID, value.TimeStampRelativeMSec);
            if (OutputStream != null)
            {
                ToNextObserver(e as O);
            }
        }

        protected override void ToNextObserver(O inEvent)
        {
            OutputStream.AddEvent(inEvent);
        }

        public override void ConnectTo(BaseObserver<O, O> to)
        {
            Console.WriteLine($"Made connetction to {to.Id}");
            base.ConnectTo(to);
        }


        public override bool ContainsKey(int id)
        {
            return dictionary.ContainsKey(id);
        }

        public override I GetValue(int id)
        {
            I value;
            dictionary.TryGetValue(id, out value);
            return value;
        }
    }
}
