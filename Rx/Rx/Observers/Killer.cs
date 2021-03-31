using Microsoft.Diagnostics.Tracing;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rx.Observers;
using System.Threading;

namespace Rx.MainModule
{
    public class Killer<I, O>  where O : InternalEvent where I : InternalEvent
    {
        private static Dictionary<int, int> _dllEvents= new Dictionary<int, int>();
        private static Dictionary<int, int> _fwEvents = new Dictionary<int, int>();
        private static Dictionary<int, int> _frEvents = new Dictionary<int, int>();


        public static void OnCompleted()
        {
            Console.WriteLine("Work Comleted");
        }

        public static void OnError(Exception error)
        {
            Console.WriteLine(error.Message);
        }

        public static void OnNext(I value)
        {
            switch (value.EventName)
            {
                case ("Image/Load"):
                    AddToDcitionary(_dllEvents, value);
                    break;
                case ("FileIO/Read"):
                    AddToDcitionary(_frEvents, value);
                    break;
                case ("FileIO/Write"):
                    AddToDcitionary(_fwEvents, value);
                    break;

            }
        }

        private static void AddToDcitionary(Dictionary<int,int> dict, InternalEvent value)
        {

            if (dict.ContainsKey(value.ProcessID))
            {
                dict[value.ProcessID]++;
            }
            else
            {
                dict.Add(value.ProcessID, 1);
            }
        }

        public static void Result() 
        {
            Console.WriteLine("Dll events:");
            ShowStattistics(_dllEvents);
            Console.WriteLine("FileIO/Read events:");
            ShowStattistics(_fwEvents);
            Console.WriteLine("FileIO/Write events:");
            ShowStattistics(_frEvents);
        }

        private static void ShowStattistics(Dictionary<int, int> dict)
        {
            foreach (KeyValuePair<int, int> pair in dict)
            {

                Console.WriteLine($"process {pair.Key} {pair.Value} times");
            }
        }
    }
}
