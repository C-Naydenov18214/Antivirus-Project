using System;
using System.Collections.Generic;

namespace App
{
    public class Dashboard
    {
        private readonly Dictionary<int, int> _dictionary;

        public Dashboard()
        {
            _dictionary = new Dictionary<int, int>();
        }

        public void AddOrUpdate(int key, int newValue)
        {
            if (_dictionary.TryGetValue(key, out var val))
            {
                _dictionary[key] = val + newValue;
            }
            else
            {
                _dictionary.Add(key, newValue);
            }
        }

        public void Show()
        {
            foreach (var pair in _dictionary)
            {
                Console.WriteLine("Process ID = {0}, Suspicious Events Count = {1}", pair.Key, pair.Value);
            }
        }
    }
}