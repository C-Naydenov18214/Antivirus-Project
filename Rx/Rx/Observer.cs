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
        public void OnCompleted()
        {
            Console.WriteLine("The end");
        }

        public void OnError(Exception error)
        {
            Console.WriteLine(error.Message);
        }

        public void OnNext(Person value)
        {
            Console.WriteLine("Observer " + value.Name + " " + Thread.CurrentThread.ManagedThreadId);
        }
    }
}
