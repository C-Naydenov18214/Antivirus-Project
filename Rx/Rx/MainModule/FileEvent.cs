using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rx.MainModule
{
    public class FileEvent
    {
        public string FileName { get; }
        public string ProcessName { get; }
        public int ProcessID { get; }
        public int ThreadID { get; }
        public double TimeStamp { get; set; }
        public string EventName { get; set; }
        public ulong FileKey { get; }

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
