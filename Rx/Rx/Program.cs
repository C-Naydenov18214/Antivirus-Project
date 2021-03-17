using System;
using System.Collections.Generic;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading;

namespace Rx
{
    class Program
    {
        static void Main(string[] args)
        {
            Persons persons = new Persons();
            List<AutoResetEvent> events = new List<AutoResetEvent>(2);
            int i = 0;
            for (i = 0; i < 2; i++) {
                events.Add(new AutoResetEvent(false));
            }
            var observer1 = new Observer<Person>(1,events[0]);
            var observer2 = new Observer<Person>(2,events[1]);
            var obs1 = persons.ObserveOn(TaskPoolScheduler.Default).Subscribe(observer1);
            var obs2 = persons.ObserveOn(TaskPoolScheduler.Default).Subscribe(observer2);
            i = 0;
            while (i < 1000)
            {
                //persons.Subscribe(observer);
                persons.AddPerson(new Person(i, "cab"));
                i++;
            }
            persons.Stop();
            
            Console.WriteLine($"main thread = {Thread.CurrentThread.ManagedThreadId}");
            AutoResetEvent.WaitAll(events.ToArray());
            Console.WriteLine("MAIN THE END");
            
            //List<Person> list = new List<Person>();
            //list.Add(new Person(1, "gege"));
            //list.Add(new Person(2, "ghewfe"));
            //list.Add(new Person(3, "ghge"));
            //list.Add(new Person(4, "e"));

            //var v = Observable.ToObservable(list);//Where(p => p.Age % 2 == 0);
         
            //var obs = v.Subscribe(observer);
            //v.Append(new Person(6, "new")).Subscribe(observer);



        }
    }
}
