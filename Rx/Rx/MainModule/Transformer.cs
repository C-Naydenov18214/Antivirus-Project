using Microsoft.Diagnostics.Tracing;
using Microsoft.Diagnostics.Tracing.Parsers.Kernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rx.MainModule
{
    public class Transformer
    {


        public static FileEvent TransformToFileEvent(TraceEvent data)
        {
            if (data is ImageLoadTraceData evI)
            {
                return new FileEvent(evI.EventName, evI.ProcessID, evI.ProcessName, evI.FileName, evI.TimeStampRelativeMSec);
            }

            if (data is FileIOReadWriteTraceData evRW)
            {

                return new FileEvent(evRW.EventName, evRW.ProcessID, evRW.ProcessName, evRW.FileName, evRW.FileKey, evRW.TimeStampRelativeMSec);

            }
            if (data is FileIOSimpleOpTraceData evClose)
            {
                return new FileEvent(evClose.EventName, evClose.ProcessID, evClose.ProcessName, evClose.FileName, evClose.FileKey, evClose.TimeStampRelativeMSec);
            }

            throw new Exception("Faild to create FileEvent");
        }
        public static ProcessStatus TransformToProcessStatus(FileEvent ev)
        {
            return new ProcessStatus(ev.ProcessID, 0);
        }
    }
}
