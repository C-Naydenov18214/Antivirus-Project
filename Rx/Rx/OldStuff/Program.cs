using Microsoft.Diagnostics.Tracing.Parsers;
using Microsoft.Diagnostics.Tracing.Session;
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
          
        }

        
    }
}
