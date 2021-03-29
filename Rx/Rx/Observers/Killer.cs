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
    public class Killer<I, O> : BaseObserver<I, O> where O : InternalEvent where I : InternalEvent
    {
        List<int> _ids;
        private Dictionary<int, int> _dllEvents;
        private Dictionary<int, int> _fwEvents;
        private Dictionary<int, int> _frEvents;

        public Killer(int id, AutoResetEvent _event, params int[] processIds) : base(id, _event)
        {
            this._ids = new List<int>(processIds);
            _dllEvents = new Dictionary<int, int>();
            _fwEvents = new Dictionary<int, int>();
            _frEvents = new Dictionary<int, int>();
        }

        public override void OnCompleted()
        {
            Console.WriteLine("Work Comleted");
            base._event.Set();
        }

        public override void OnError(Exception error)
        {
            Console.WriteLine(error.Message);
            base._event.Set();
        }

        public override void OnNext(I value)
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

        private void AddToDcitionary(Dictionary<int,int> dict, InternalEvent value)
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

        public void Result() 
        {
            Console.WriteLine("Dll events:");
            ShowStattistics(_dllEvents);
            Console.WriteLine("FileIO/Read events:");
            ShowStattistics(_fwEvents);
            Console.WriteLine("FileIO/Write events:");
            ShowStattistics(_frEvents);
        }

        private void ShowStattistics(Dictionary<int, int> dict)
        {
            foreach (KeyValuePair<int, int> pair in dict)
            {

                Console.WriteLine($"process {pair.Key} {pair.Value} times");
            }
        }
    }
}
