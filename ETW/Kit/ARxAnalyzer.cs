using System;
using System.Collections.Generic;
using System.Reactive.Subjects;

namespace Kit
{
    public abstract class ARxAnalyzer
    {
        public readonly Subject<SuspiciousEvent> SuspiciousEvents;

        protected ARxAnalyzer(Subject<SuspiciousEvent> suspiciousEvents)
        {
            SuspiciousEvents = suspiciousEvents;
        }

        public abstract void Start();
    }
}