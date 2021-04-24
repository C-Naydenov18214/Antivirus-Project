using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETW.Patterns
{
    class Wtf
    {
        public HashSet<int> writes = new HashSet<int>();
        public HashSet<int> closes = new HashSet<int>();
        public int read;

        public Wtf(int read, HashSet<int> writes, HashSet<int> closes)
        {
            this.read = read;
            this.writes = writes;
            this.closes = closes;
        }

    }
}
