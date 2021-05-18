using Kit;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;

namespace App
{
    public class Dashboard
    {
        private readonly ConcurrentDictionary<int, KeyValuePair<SuspiciousEvent, int>> _dictionary;

        public Dashboard()
        {
            _dictionary = new ConcurrentDictionary<int, KeyValuePair<SuspiciousEvent, int>>();
        }

        public void AddOrUpdate(int key, KeyValuePair<SuspiciousEvent, int> val)
        {
            _dictionary.AddOrUpdate(key, val, (oldKey, oldVal) => oldVal = new KeyValuePair<SuspiciousEvent, int>(oldVal.Key,oldVal.Value+1));
        }

        public void Kill(int id)
        {
            Process.GetProcessById(id).Kill();
        }

        public void Show()
        {
            Console.Clear();
            Console.WriteLine("Enter process id in order to kill it. Enter 'load `path to dll`', or 'exit' to stop the program.");
            Console.WriteLine();
            foreach (var pair in _dictionary)
            {
                Console.WriteLine($"Process ID = {pair.Key}, Process name: {pair.Value.Key.ProcName}, Suspicious Events Count = {pair.Value.Value}, Event info: {pair.Value.Key.EventInfo}");
            }
        }
    }
}