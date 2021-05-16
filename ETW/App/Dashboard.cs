using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;

namespace App
{
    public class Dashboard
    {
        private readonly ConcurrentDictionary<int, int> _dictionary;

        public Dashboard()
        {
            _dictionary = new ConcurrentDictionary<int, int>();
        }

        public void AddOrUpdate(int key, int val)
        {
            _dictionary.AddOrUpdate(key, val, (oldKey, oldVal) => oldVal + 1);
        }

        public void Kill(int id)
        {
            Process.GetProcessById(id).Kill();
        }

        public void Show()
        {
            Console.Clear();
            Console.WriteLine("Enter process id in order to kill it. Enter 'exit' to stop the program.");
            Console.WriteLine();
            foreach (var pair in _dictionary)
            {
                Console.WriteLine("Process ID = {0}, Suspicious Events Count = {1}", pair.Key, pair.Value);
            }
        }
    }
}