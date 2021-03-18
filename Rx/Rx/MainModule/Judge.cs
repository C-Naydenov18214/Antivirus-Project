using Microsoft.Diagnostics.Tracing;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rx.MainModule
{
    class Judge<V> where V : TraceEvent
    {
        List<ConcurrentDictionary<int, V>> dictionaries;
        List<int> _ids;
        public Judge(params int[] processIds)
        {
            this._ids = new List<int>(processIds);
            dictionaries = new List<ConcurrentDictionary<int, V>>();
        }

        public void AddSource(ConcurrentDictionary<int, V> dictionary) 
        {
            dictionaries.Add(dictionary);
        }


        private void Work()
        {
            /*можно попробовать придумать что то с логикой событий, чтобы определение банить или нет было более элегантным*/
            bool ban = false;
            while (true) 
            {
                foreach (ConcurrentDictionary<int, V> dictionary in dictionaries)
                {
                    ban = dictionary.ContainsKey(_ids[0]);
                }
                if (ban) 
                {
                    Console.WriteLine($"Pls ban process {_ids[0]}");
                }
            
            }
        
        
        }






    }
}
