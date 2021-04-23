using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using App;
using Kit;

namespace RxAnalyzerTest
{
    public class RxAnalyzer : IRxAnalyzer
    {
        private IObservable<WriteEvent> wObservable;
        private IObservable<ReadEvent> rObservable;

        public RxAnalyzer(IObservable<WriteEvent> wObservable, IObservable<ReadEvent> rObservable)
        {
            this.wObservable = wObservable;
            this.rObservable = rObservable;
        }

        public void Start()
        {
            if (wObservable != null && rObservable != null)
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
