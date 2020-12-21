using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GUI
{
    class Task
    {
        private string target;
        private string analyzer;

        public Task(string target, string analyzer)
        {
            this.analyzer = analyzer;
            this.target = target;
        }
        public string GetTargetPath()
        {
            return this.target;
        }

        public string GetAnalyzerPath()
        {
            return this.analyzer;
        }
    }
}