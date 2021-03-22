using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Diagnostics.Tracing.Parsers;
using Microsoft.Diagnostics.Tracing.Parsers.Kernel;
using Microsoft.Diagnostics.Tracing.Session;

namespace ETW.Tracer
{
    class EventTracer
    {

        private TraceEventSession _kernelSession;
        private bool _isStopped;

        public void Run()
        {
            if (!(TraceEventSession.IsElevated() ?? false))
            {
                Console.WriteLine("Please run program as Administrator");
                return;
            }

            Console.CancelKeyPress += delegate
            {
                _isStopped = true;
                Console.WriteLine("Stopping all ETW sessions...");
                _kernelSession?.Dispose();
                Environment.Exit(0);
            };

            using (_kernelSession = new TraceEventSession(KernelTraceEventParser.KernelSessionName))
            {

                _kernelSession.EnableKernelProvider(KernelTraceEventParser.Keywords.All);
                _kernelSession.Source.Kernel.ImageLoad += ImageLoadEvent;
                _kernelSession.Source.Kernel.FileIOWrite += FileWriteEvent;
                _kernelSession.Source.Kernel.FileIORead += FileReadEvent;

                _kernelSession.Source.Process();
            }
        }

        private void ImageLoadEvent(ImageLoadTraceData data)
        {
            if (_isStopped)
            {
                return;
            }

            Console.WriteLine("ImageLoadEvent catched");
            // ...
        }

        private void FileWriteEvent(FileIOReadWriteTraceData data)
        {
            if (_isStopped)
            {
                return;
            }

            Console.WriteLine("FileWriteEvent catched");
            // ...
        }

        private void FileReadEvent(FileIOReadWriteTraceData data)
        {
            // HasExtension(...) is to avoid cached files
            if (_isStopped || !Path.HasExtension(data.FileName))
            {
                return;
            }

            Console.WriteLine("FileReadEvent catched");
            // ...
        }
    }
}
