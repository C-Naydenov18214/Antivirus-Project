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
    public class Judge<T> : BaseObserver<T> where T : JudgeEvent
    {
        List<int> _ids;
        public Judge(int id, AutoResetEvent _event,params int[] processIds) : base(id,_event)
        {
            this._ids = new List<int>(processIds);
        }

        public override void OnCompleted()
        {
            Console.WriteLine("Work Comleted");
        }

        public override void OnError(Exception error)
        {
            Console.WriteLine(error.Message);
        }

        public override void OnNext(T value)
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
