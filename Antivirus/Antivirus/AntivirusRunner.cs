
using DeveloperKit.Context;
using DeveloperKit.PeReader;
using DeveloperKit.Report;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Antivirus
{
    class AntivirusRunner
    {
        private string dllInterface = "IAnalyzer";
        private string interfaceMethod = "Analyze";
        private Dictionary<string, FileContext> filesContext;

        public AntivirusRunner()
        {
            this.filesContext = new Dictionary<string, FileContext>();
        }
        public void Run(string[] args)
        {


            List<Task> tasks = ArrayToTasks(args);

            foreach (Task task in tasks)
            {
                int n = 0; 
                AntivirusReport report = StartSearch(task);

                foreach (VirusInfo info in report.VirusInfos)
                {
                    Console.WriteLine(info.FilePath);
                    Console.WriteLine(info.Signature);
                    foreach (byte b in info.Signature)
                    {
                        if (n > 31)
                        {
                            Console.WriteLine();
                            n = 0;
                        }
                        Console.Write(b.ToString("X2") + " ");
                        n++;
                    }
                    Console.WriteLine();
                    Console.WriteLine(info.UrlToDataBase);
                    foreach (KeyValuePair<string, string> pair in info.Inforamation)
                    {
                        Console.WriteLine($"{pair.Key} {pair.Value}");
                    }
                }
            }


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

            if (!filesContext.ContainsKey(target))
            {
                PeFileContext context = reader.ReadPeFile(target);
                FileInfo fileInfo = new FileInfo(target);
                fileContext = new FileContext(fileInfo, context);
                filesContext.Add(target, fileContext);
            }
            else
            {
                filesContext.TryGetValue(target, out fileContext);
                Console.WriteLine($"Same file {fileContext.FileInfo.FullName}");
            }

            var instance = Activator.CreateInstance(type);
            MethodInfo analyze = type.GetMethod(this.interfaceMethod);

            return antivirus.CheckFile(instance, analyze, fileContext);
        }
    }
}
