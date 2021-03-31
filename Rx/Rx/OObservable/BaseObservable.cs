using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rx.MainModule
{
    public class BaseObservable<T> : IObservable<T> 
    {
        protected List<IObserver<T>> observers;
        public BaseObservable()
        {
            observers = new List<IObserver<T>>();
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            if (!observers.Contains(observer))
                observers.Add(observer);
            return new Unsubscriber(observers, observer);
        }

        private class Unsubscriber : IDisposable
        {
            private List<IObserver<T>> _observers;
            private IObserver<T> _observer;

            public Unsubscriber(List<IObserver<T>> observers, IObserver<T> observer)
            {
                this._observers = observers;
                this._observer = observer;
            }

            public void Dispose()
            {
                if (_observer != null && _observers.Contains(_observer))
                    _observers.Remove(_observer);
            }
        }

        public void AddEvent(T newEvent)
        {
            //Console.WriteLine("Add event start");
            foreach (var observer in observers)
            {
                if (newEvent == null)
                    observer.OnError(new Exception());
                else
                    observer.OnNext(newEvent);
            }
            //Console.WriteLine("Add event end");
        }

        public void Stop()
        {
            foreach (var observer in observers.ToArray())
                if (observers.Contains(observer))
                    observer.OnCompleted();

            observers.Clear();
        }
    }
}
