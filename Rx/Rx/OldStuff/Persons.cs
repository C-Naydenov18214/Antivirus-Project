using System;
using System.Collections.Generic;

namespace Rx
{
    public class Persons :IObservable<Person>
    {
        public Persons()
        {
            observers = new List<IObserver<Person>>();
        }

        private List<IObserver<Person>> observers;

        public IDisposable Subscribe(IObserver<Person> observer)
        {
            if (!observers.Contains(observer))
                observers.Add(observer);
            return new Unsubscriber(observers, observer);
        }

        private class Unsubscriber : IDisposable
        {
            private List<IObserver<Person>> _observers;
            private IObserver<Person> _observer;

            public Unsubscriber(List<IObserver<Person>> observers, IObserver<Person> observer)
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

        public void AddPerson(Person loc)
        {
            foreach (var observer in observers)
            {
                if (loc == null)
                    observer.OnError(new Exception());
                else
                    observer.OnNext(loc);
            }
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
