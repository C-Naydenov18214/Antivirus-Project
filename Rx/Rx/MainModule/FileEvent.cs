using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rx.MainModule
{
    public class FileEvent
    {
        public virtual string FileName { get; }
        public virtual string ProcessName { get; }
        public virtual int ProcessID { get; }
        public virtual int ThreadID { get; }
        public virtual double TimeStamp { get; set; }
        public virtual string EventName { get; set; }
        public virtual ulong FileKey { get; }

        public FileEvent(string eventName, int threadID, int processID, string processName, string fileName, double timeStamp)
        {
            EventName = eventName;
            ProcessID = processID;
            ProcessName = processName;
            FileName = fileName;
            TimeStamp = timeStamp;
            ThreadID = threadID;
        }
        public FileEvent(string eventName, int processID, string processName, string fileName, double timeStamp)
        {
            EventName = eventName;
            ProcessID = processID;
            ProcessName = processName;
            FileName = fileName;
            TimeStamp = timeStamp;
        }
        public FileEvent(string eventName, int processID, string processName, string fileName, ulong fileKey, double timeStamp)
        {
            EventName = eventName;
            ProcessID = processID;
            ProcessName = processName;
            FileName = fileName;
            FileKey = fileKey;
            TimeStamp = timeStamp;
        }

        public override string ToString()
        {
            return $"ProcessID= {ProcessID}, Action= {EventName}";
        }



    }
}
