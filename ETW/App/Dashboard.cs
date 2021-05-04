using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

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
            _dictionary.AddOrUpdate(key, val, (oldKey, oldVal) => val);
        }

        public void Show()
        {
            Console.Clear();
            foreach (var pair in _dictionary)
            {
                Console.WriteLine("Process ID = {0}, Suspicious Events Count = {1}", pair.Key, pair.Value);
            }
        }
    }
}