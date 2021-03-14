using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Rx
{
    class Observer : IObserver<Person>
    {
        private AutoResetEvent _event;
        public int Id { get; set; }
        public Observer(int id,AutoResetEvent _event)
        {
            this._event = _event;
            this.Id = id;
        }

        public void OnCompleted()
        {
            _event.Set();
            Console.WriteLine("The end");

        }

        public void OnError(Exception error)
        {
            Console.WriteLine(error.Message);
        }

        public void OnNext(Person value)
        {
            Console.WriteLine($"Observer {Id} get " + value.Name + " in " + Thread.CurrentThread.ManagedThreadId);
        }
    }
}
