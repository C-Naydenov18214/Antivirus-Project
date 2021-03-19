using Microsoft.Diagnostics.Tracing;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rx.Observers;

namespace Rx.MainModule
{
    public class Judge<T> where T : TraceEvent
    {
        List<EventObserver<T>> dictionaries;
        List<int> _ids;
        public Judge(params int[] processIds)
        {
            this._ids = new List<int>(processIds);
            dictionaries = new List<EventObserver<T>>();
        }

        public void AddSource(EventObserver<T> obs)
        {
            dictionaries.Add(obs);
        }

        public void test(T arg) 
        {
        
        
        }

        public void Work()
        {
            /*можно попробовать придумать что то с логикой событий, чтобы определение банить или нет было более элегантным*/
            bool ban = false;
            while (true)
            {
                foreach (EventObserver<T> obs in dictionaries)
                {
                    ban = obs.ContainsKey(_ids[0]);
                }
                if (ban)
                {
                    Console.WriteLine($"Pls ban process {_ids[0]}");
                }

            }


        }






    }
}
