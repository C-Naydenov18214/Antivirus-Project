using System;
using System.Threading.Tasks;
using ETW.Tracer;
using Rx.MainModule;

namespace ETW
{
    sealed class Program
    {
        private static void Main(string[] args)
        {
            var dllInput = new BaseObservable<InternalEvent>();
            var fwInput =  new BaseObservable<InternalEvent>();
            var frInput = new BaseObservable<InternalEvent>();

            var eventTracer = new EventTracer(Console.Out);
            eventTracer.setInputs(dllInput, fwInput, frInput);
            var task = Task.Run(eventTracer.Run);
            task.Wait();
        }
    }
}
