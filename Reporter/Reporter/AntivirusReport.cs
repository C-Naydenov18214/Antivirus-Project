using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reporter
{
    public class AntivirusReport
    {

        private string name;
        public AntivirusReport(string message)
        {
            this.name = message;
        }

        public string GetName()
        {
            return this.name;
        }
    }
}
