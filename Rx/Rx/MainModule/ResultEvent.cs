using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rx.MainModule
{
    public class ResultEvent
    {
        public FileEvent DllEvent { get; }
        public FileEvent WriteEvent { get; }


        public ResultEvent(FileEvent dll, FileEvent write)
        {
            this.DllEvent = dll;
            this.WriteEvent = write;
      
        }
    }
}
