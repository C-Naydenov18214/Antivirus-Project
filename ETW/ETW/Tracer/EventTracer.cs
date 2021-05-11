using System;
using System.Diagnostics;
using System.IO;
using System.Reactive.Linq;
using Microsoft.Diagnostics.Tracing;
using Microsoft.Diagnostics.Tracing.Parsers;
using Microsoft.Diagnostics.Tracing.Parsers.Kernel;
using Microsoft.Diagnostics.Tracing.Session;
using Rx.MainModule;


namespace ETW.Tracer
{
    /// <summary>
    /// Kernel event listener.
    /// </summary>
    public class EventTracer
    {
        private readonly TextWriter _out;
        private TraceEventSession _kernelSession;
        private bool _isStopped;

        public TraceEventSession GetKernelSession()
        {
            return _kernelSession;
        }

        public IObservable<IGroupedObservable<string, FileEvent>> mergedGroups;
        public IObservable<ImageLoadTraceData> Dlls { get; private set; }
        public IObservable<FileIOCreateTraceData> Creates { get; private set; }
        public IObservable<FileIOReadWriteTraceData> Writes { get; private set; }
        public IObservable<TraceEvent> AllEvents { get; private set; }
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
            //_out.WriteLine("ImageLoadEvent from pid {0} caught", data.ProcessID);
#endif
            // _dllInput?.AddEvent(new InternalEvent(data.ID, data.EventName, data.ProcessID, data.TimeStampRelativeMSec));
        }

        private void FileWriteEvent(FileIOReadWriteTraceData data)
        {
            if (_isStopped)
            {
                return;
            }

#if DEBUG
            //_out.WriteLine("FileWriteEvent from pid {0} caught", data.ProcessID);
#endif
            //_fwInput?.AddEvent(new InternalEvent(data.ID, data.EventName, data.ProcessID, data.TimeStampRelativeMSec));
        }

        private void FileReadEvent(FileIOReadWriteTraceData data)
        {
            // HasExtension(...) is to avoid cached files
            if (_isStopped || !Path.HasExtension(data.FileName))
            {
                return;
            }

#if DEBUG
            //_out.WriteLine("FileReadEvent from pid {0} caught", data.ProcessID);
#endif
            //_frInput?.AddEvent(new InternalEvent(data.ID, data.EventName, data.ProcessID, data.TimeStampRelativeMSec));
        }


        public void Test()
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
                //Observable.Start<ImageLoadTraceData>(h => _kernelSession.Source.Kernel.ImageLoad += h, h => _kernelSession.Source.Kernel.ImageLoad -= h);
                Dlls = Observable.FromEvent<ImageLoadTraceData>(h => _kernelSession.Source.Kernel.ImageLoad += h, h => _kernelSession.Source.Kernel.ImageLoad -= h).Publish().RefCount();
                Creates = Observable.FromEvent<FileIOCreateTraceData>(h => _kernelSession.Source.Kernel.FileIOCreate += h, h => _kernelSession.Source.Kernel.FileIOCreate -= h).Publish().RefCount();
                Writes = Observable.FromEvent<FileIOReadWriteTraceData>(h => _kernelSession.Source.Kernel.FileIOWrite += h, h => _kernelSession.Source.Kernel.FileIOWrite -= h).Publish().RefCount();/*.Where(i => i.FileName.EndsWith(".dll"))*//*.Select(i => Transformer.TransformToFileEvent(i));*/
                AllEvents = Observable.FromEvent<TraceEvent>(h => _kernelSession.Source.Kernel.All += h, h => _kernelSession.Source.Kernel.All -= h).Publish().RefCount();


                //var read = Observable.FromEvent<FileIOReadWriteTraceData>(h => _kernelSession.Source.Kernel.FileIORead += h, h => _kernelSession.Source.Kernel.FileIORead -= h)/*.Where(i => i.FileName.EndsWith(".dll"))*/.Select(i => Transformer.TransformToFileEvent(i));
                var close = Observable.FromEvent<FileIOSimpleOpTraceData>(h => _kernelSession.Source.Kernel.FileIOClose += h, h => _kernelSession.Source.Kernel.FileIOClose -= h);/*.Where(i => i.FileName.EndsWith(".dll"))*///.Select(i => Transformer.TransformToFileEvent(i));
                //close.Subscribe(el => Console.WriteLine(el.EventName));
                //mergedGroups = Dlls.Merge(write).GroupBy(i => i.FileName);
                _kernelSession.Source.Process();
            }



        }


        /*public InternalEvent Transform(TraceEvent data)
        {
            return new InternalEvent(data.ID, data.EventName, data.ProcessID, data.TimeStampRelativeMSec);
        }*/



    }
}
