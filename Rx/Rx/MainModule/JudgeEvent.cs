using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rx.MainModule
{
    public class JudgeEvent
    {
        public int EventType { get; }
        public int ProcessID { get; }


        public JudgeEvent(int eventType, int processID) 
        {
            this.EventType = eventType;
            this.ProcessID = processID;
        }

    }
}
