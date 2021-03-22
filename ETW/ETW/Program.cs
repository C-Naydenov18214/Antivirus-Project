using System;
using System.Threading.Tasks;
using ETW.Tracer;

namespace ETW
{
    sealed class Program
    {
        private static void Main(string[] args)
        {
            var eventTracer = new EventTracer(Console.Out);
            var task = Task.Run(eventTracer.Run);
            task.Wait();
        }
    }
}
