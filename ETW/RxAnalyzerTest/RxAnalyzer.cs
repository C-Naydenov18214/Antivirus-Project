using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using App;
using Kit;
using Microsoft.Diagnostics.Tracing.Parsers.Kernel;
using Unity;

namespace RxAnalyzerTest
{
    public class RxAnalyzer : ARxAnalyzer
    {
        private IObservable<FileIOCreateTraceData> wObservable;
        private IObservable<FileIOReadWriteTraceData> rObservable;

        public RxAnalyzer(IObservable<FileIOCreateTraceData> wObservable, IObservable<FileIOReadWriteTraceData> rObservable, Subject<SuspiciousEvent> suspiciousEvents) : base(suspiciousEvents)
        {
            this.wObservable = wObservable;
            this.rObservable = rObservable;
        }

        public override void Start()
        {
            if (wObservable != null)
            {
                Console.WriteLine("create not null!");
            }

            if (rObservable != null)
            {
                Console.WriteLine("read not null!");
            }

            if (SuspiciousEvents != null)
            {
                Console.WriteLine("susp not null!");
            }
        }
    }
}
