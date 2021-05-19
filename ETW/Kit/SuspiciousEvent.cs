namespace Kit
{
    public class SuspiciousEvent
    {
        public int ProcessId;
        public string EventInfo;
        public int Length;
        public string ProcName;


        public SuspiciousEvent() { }

        public SuspiciousEvent(int pid, string info, string name)
        {
            this.ProcessId = pid;
            this.EventInfo = info;
            this.ProcName = name;
        }
    }

}