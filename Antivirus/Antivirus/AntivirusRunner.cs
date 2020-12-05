
using DeveloperKit.Context;
using DeveloperKit.PeReader;
using DeveloperKit.Report;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Antivirus
{
    class AntivirusRunner
    {
        private string dllInterface = "IAnalyzer";
        private string interfaceMethod = "Analyze";
        private ConcurrentDictionary<string, FileContext> filesContext;
        private ConcurrentQueue<AntivirusReport> reports;

        public AntivirusRunner()
        {
            this.filesContext = new ConcurrentDictionary<string, FileContext>();
        }
        public void Run(string[] args)
        {
            List<Task> tasks = ArrayToTasks(args);
            ManualResetEvent[] handles = new ManualResetEvent[tasks.Count];
            reports = new ConcurrentQueue<AntivirusReport>();
            for (int i = 0; i < tasks.Count; i++)
            {
                handles[i] = new ManualResetEvent(false);
            }
            int m = 0;
            foreach (Task task in tasks)
            {

                ThreadPool.QueueUserWorkItem(new WaitCallback(x =>
                {
                    reports.Enqueue(StartSearch(task));
                    handles[m++].Set();
                }));
            }
            WaitHandle.WaitAll(handles);
            Reporter reporter = new Reporter();
            reporter.ShowResult(reports);
        }

        private List<Task> ArrayToTasks(string[] args)
        {
            int length = args.Length;
            List<Task> tasks = new List<Task>(length / 2);

            for (int i = 0; i < length - 1; i += 2)
            {
                tasks.Add(new Task(args[i], args[i + 1]));
            }
            return tasks;
        }

        private AntivirusReport StartSearch(Task task)
        {


            AssemblyLoader loader = new AssemblyLoader();
            TypeFinder finder = new TypeFinder();
            Antivirus antivirus = new Antivirus();
            PeReader reader = new PeReader();

            string target = task.GetTargetPath();
            Assembly asm = loader.LoadAssambly(task.GetAnalyzerPath());
            Type type = finder.FindType(asm, this.dllInterface);
            FileContext fileContext;
            lock (filesContext)
            {
                if (!filesContext.ContainsKey(target))
                {
                    Console.WriteLine($"new item {target}");
                    PeFileContext peFile = reader.ReadPeFile(target);
                    FileInfo fileInfo = new FileInfo(target);
                    fileContext = new FileContext(fileInfo, peFile);
                    filesContext.TryAdd(target, fileContext);
                }
                else
                {
                    filesContext.TryGetValue(target, out fileContext);
                    Console.WriteLine($"Same file {fileContext.FileInfo.FullName}");
                }
            }

            var instance = Activator.CreateInstance(type);
            MethodInfo analyze = type.GetMethod(this.interfaceMethod);

            return antivirus.CheckFile(instance, analyze, fileContext);
        }
    }
}
