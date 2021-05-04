using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using App;
using Kit;
using Unity;

namespace RxAnalyzerTest
{
    public class RxAnalyzer : ARxAnalyzer
    {
        private Subject<WriteEvent> wObservable;
        private Subject<ReadEvent> rObservable;

        public RxAnalyzer(Subject<WriteEvent> wObservable, Subject<ReadEvent> rObservable, Subject<SuspiciousEvent> suspiciousEvents) : base(suspiciousEvents)
        {
            this.wObservable = wObservable;
            this.rObservable = rObservable;
        }

        public override void Start()
        {
            if (wObservable != null && rObservable != null && SuspiciousEvents != null)
            {
                Console.WriteLine("Not null!");
            }
            else
            {
                Console.WriteLine("Null found!");
            }
        }
    }
}
