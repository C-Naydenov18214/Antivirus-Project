using System;
using System.Collections.Generic;

namespace Kit
{
    public abstract class ARxAnalyzer
    {
        public readonly IObservable<SuspiciousEvent> SuspiciousEvents;

        protected ARxAnalyzer(IObservable<SuspiciousEvent> suspiciousEvents)
        {
            SuspiciousEvents = suspiciousEvents;
        }

        public abstract void Start();
    }
}