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
    public class Killer<I,O> : BaseObserver<I,O> where O : InternalEvent
    {
        List<int> _ids;
        public Killer(int id, AutoResetEvent _event,params int[] processIds) : base(id,_event)
        {
            this._ids = new List<int>(processIds);
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
            Console.WriteLine("do some work");
            Work();
        }

        private void Work()
        {
            /*можно попробовать придумать что то с логикой событий, чтобы определение банить или нет было более элегантным*/
            bool ban = false;
            //while (true)
            //{


            //}


        }






    }
}
