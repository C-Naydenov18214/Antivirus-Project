using ETW.Tracer;

namespace ETW
{
    sealed class Program
    {
        private static void Main(string[] args)
        {
            var eventTracer = new EventTracer();
            eventTracer.Run();
        }
    }
}
