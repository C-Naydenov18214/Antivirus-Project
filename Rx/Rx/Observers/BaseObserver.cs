using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Rx.Observers
{
    public class BaseObserver<T> : IObserver<T>
    {
        public int Id { get; }
        public ConcurrentDictionary<int, T> dictionary;
        protected AutoResetEvent _event;

        public BaseObserver(int id, AutoResetEvent _event)
        {
            this.dictionary = new ConcurrentDictionary<int, T>();
            this.Id = id;
            this._event = _event;
        }

        public virtual void OnCompleted()
        {
            _event.Set();
            Console.WriteLine($"observer {Id} completed");
        }

        public virtual void OnError(Exception error)
        {
            Console.WriteLine(error.Message);
        }

        public virtual void OnNext(T value)
        {
            Console.WriteLine("got value = " + value);
        }


        public virtual bool ContainsKey(int id)
        {
            return dictionary.ContainsKey(id);
        }

        public virtual  T GetValue(int id)
        {
            T value;
            dictionary.TryGetValue(id, out value);
            return value;
        }
    }
}

