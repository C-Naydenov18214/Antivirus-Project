using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Rx
{
    class Program
    {
        static void Main(string[] args)
        {
            Persons persons = new Persons();
            var observer1 = new Observer(1);
            var observer2 = new Observer(2);
            var obs1 = persons.ObserveOn(NewThreadScheduler.Default).Subscribe(observer1);
            var obs2 = persons.ObserveOn(NewThreadScheduler.Default).Subscribe(observer2);
            int i = 0;
            while (true)
            {
                //persons.Subscribe(observer);
                persons.AddPerson(new Person(i, "cab"));
                i++;
            }
            persons.Stop();
            
            Console.WriteLine($"main thread = {Thread.CurrentThread.ManagedThreadId}");
            Console.ReadLine();
            
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
