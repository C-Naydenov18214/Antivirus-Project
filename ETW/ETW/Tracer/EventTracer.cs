﻿using System;
using System.Diagnostics;
using System.IO;
using Microsoft.Diagnostics.Tracing.Parsers;
using Microsoft.Diagnostics.Tracing.Parsers.Kernel;
using Microsoft.Diagnostics.Tracing.Session;

namespace ETW.Tracer
{
    /// <summary>
    /// Kernel event listener.
    /// </summary>
    class EventTracer
    {
        private readonly TextWriter _out;
        private TraceEventSession _kernelSession;
        private bool _isStopped;

        public EventTracer()
        {
            _out = Console.Out;
        }

        public EventTracer(TextWriter @out)
        {
            _out = @out;
        }

        /// <summary>
        /// Start event tracing.
        /// </summary>
        public void Run()
        {
            if (!(TraceEventSession.IsElevated() ?? false))
            {
                _out.WriteLine("Please run program as Administrator");
                Debugger.Break();
                return;
            }

            Console.CancelKeyPress += delegate (object sender, ConsoleCancelEventArgs cancelArgs)
            {
                _isStopped = true;
                _out.WriteLine("Stopping all ETW sessions...");
                _kernelSession?.Dispose();
                cancelArgs.Cancel = true;
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

#if DEBUG
            _out.WriteLine("ImageLoadEvent from pid {0} caught", data.ProcessID);
#endif
            // ...
        }

        private void FileWriteEvent(FileIOReadWriteTraceData data)
        {
            if (_isStopped)
            {
                return;
            }

#if DEBUG
            _out.WriteLine("FileWriteEvent from pid {0} caught", data.ProcessID);
#endif
            // ...
        }

        private void FileReadEvent(FileIOReadWriteTraceData data)
        {
            // HasExtension(...) is to avoid cached files
            if (_isStopped || !Path.HasExtension(data.FileName))
            {
                return;
            }

#if DEBUG
            _out.WriteLine("FileReadEvent from pid {0} caught", data.ProcessID);
#endif
            // ...
        }
    }
}
