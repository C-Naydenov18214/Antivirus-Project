using Microsoft.Diagnostics.Tracing;
using Rx.MainModule;
using System;
using System.Collections.Concurrent;
using System.Threading;

namespace Rx.Observers
{
    public class EventObserver<I, O> : BaseObserver<I, O> where I : TraceEvent where O : InternalEvent
    {
        public EventObserver(int id, AutoResetEvent _event) : base(id, _event)
        {
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
            Console.WriteLine($"obs {Id} got value");
            var e = new InternalEvent(value.ID);
            if (base.OutputStream != null)
            {
                ToNextObserver(e as O);
            }
        }

        protected override void ToNextObserver(O inEvent)
        {
            base.OutputStream.AddEvent(inEvent);
        }

        public override void ConnectTo(BaseObserver<O, O> to)
        {
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
