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
            var observer = new Observer();
            var obs = persons.ObserveOn(Scheduler.Default).Subscribe(observer);
            //persons.Subscribe(observer);
            persons.AddPerson(new Person(1, "gege"));
            persons.AddPerson(new Person(2, "ghewfe"));
            persons.AddPerson(new Person(3, "ghge"));
            persons.AddPerson(new Person(4, "e"));
            persons.AddPerson(new Person(6, "new"));
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
