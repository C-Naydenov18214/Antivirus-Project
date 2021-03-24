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
    public class BaseObserver<I, O> : IObserver<I> where O : InternalEvent
    {
        public int Id { get; }
        public ConcurrentDictionary<int, I> dictionary;
        protected AutoResetEvent _event;

        public BaseObservable<O> OutputStream { get; set; }


        public BaseObserver(int id, AutoResetEvent _event)
        {
            this.dictionary = new ConcurrentDictionary<int, I>();
            this.Id = id;
            this._event = _event;
            this.OutputStream = new BaseObservable<O>();
        }
        public virtual void OnCompleted()
        {
            if (OutputStream != null)
            {
                OutputStream.Stop();
            }
            _event.Set();
            Console.WriteLine($"observer {Id} completed");
        }

        public virtual void OnError(Exception error)
        {
            if (OutputStream != null)
            {
                OutputStream.Stop();
            }
            Console.WriteLine(error.Message);
        }

        public virtual void OnNext(I value)
        {
            Console.WriteLine($"obs {Id} got value = {(value as InternalEvent).EventType}");
            if (OutputStream != null)
            {
                ToNextObserver((value as O));
            }
        }

        protected virtual void ToNextObserver(O inEvent)
        {
            OutputStream.AddEvent(inEvent);
            Console.WriteLine($"obs {Id} sent value = {inEvent.EventType}");
        }


        public virtual void ConnectTo(BaseObserver<O, O> to)
        {
            if (OutputStream == null)
            {
                OutputStream = new BaseObservable<O>();
            }
            OutputStream.ObserveOn(TaskPoolScheduler.Default).Subscribe(to);
            Console.WriteLine($"obs {Id} connected to obs {to.Id}");
        }


        public virtual bool ContainsKey(int id)
        {
            return dictionary.ContainsKey(id);
        }

        public virtual I GetValue(int id)
        {
            I value;
            dictionary.TryGetValue(id, out value);
            return value;
        }
    }
}

