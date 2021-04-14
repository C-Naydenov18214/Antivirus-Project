using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rx.MainModule
{
    public class ConcatedStreams
    {
        public IGroupedObservable<int, FileEvent> Dll { get; }
        public IGroupedObservable<int, FileEvent> Writers { get; }


        public ConcatedStreams(IGroupedObservable<int, FileEvent> dll, IGroupedObservable<int, FileEvent> writers)
        {
            this.Dll = dll;
            this.Writers = writers;
       
        }

    }
}
