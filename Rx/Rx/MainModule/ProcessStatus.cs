using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rx.MainModule
{
    public class ProcessStatus
    {
        public int ProcessID { get; }
        public int SuspicionLevel { get; }

        public int ReadCounter { get; set; }

        public int WriteCounter { get; set; }
        public ProcessStatus(int processID, int initialLevel)
        {
            this.ProcessID = processID;
            this.SuspicionLevel = initialLevel;
        }
    }
}
