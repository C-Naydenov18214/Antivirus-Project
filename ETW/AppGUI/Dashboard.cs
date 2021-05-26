using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Threading;
using Kit;

namespace AppGUI
{
    public class Dashboard
    {
        private readonly ConcurrentDictionary<int, KeyValuePair<SuspiciousEvent, int>> _dictionary;
        private readonly TextBox _report;

        public Dashboard(TextBox report)
        {
            _dictionary = new ConcurrentDictionary<int, KeyValuePair<SuspiciousEvent, int>>();
            _report = report;
        }

        public void AddOrUpdate(int key, KeyValuePair<SuspiciousEvent, int> val)
        {
            _dictionary.AddOrUpdate(key, val, (oldKey, oldVal) => oldVal = new KeyValuePair<SuspiciousEvent, int>(new SuspiciousEvent(oldVal.Key.ProcessId, oldVal.Key.EventInfo + "\n" + val.Key.EventInfo, oldVal.Key.ProcName), oldVal.Value + 1));
        }

        public void Kill(int id)
        {
            Process.GetProcessById(id).Kill();
        }

        public void Show()
        {
            _report.Dispatcher.BeginInvoke(new Action(() =>
            {
                _report.Text = string.Empty;
            }));
            WriteLn("Enter process id in order to kill it.");
            WriteLn("");
            foreach (var pair in _dictionary)
            {
                WriteLn($"###\nProcess ID = {pair.Key}, Process name: {pair.Value.Key.ProcName}, Suspicious Events Count = {pair.Value.Value}, Event info: {pair.Value.Key.EventInfo}");
                
            }
        }
        public void WriteLn(string text)
        {
            _report.Dispatcher.BeginInvoke(new Action(() =>
            {
                _report.Text += text + Environment.NewLine;
            }));
        }
    }
}