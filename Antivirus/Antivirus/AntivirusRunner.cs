using Reporter;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Antivirus
{
    public delegate AntivirusReport Analyze(string path);
    class AntivirusRunner
    {
        private string dllInterface = "IAnalyzer";
        private string interfaceMethod = "Analyze";
        public void Run(string[] args)
        {

           
            List<Task> tasks = ArrayToTasks(args);

            foreach(Task task in tasks)
            {
                AntivirusReport report = StartSearch(task);
                Console.WriteLine(report.GetName());
            } 

            
        }

        private List<Task> ArrayToTasks(string[] args)
        {
            int length = args.Length;
            List<Task> tasks = new List<Task>(length / 2);
          
            for (int i = 0; i < length-1;i+=2) 
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

            Assembly asm = loader.LoadAssambly(task.GetAnalyzerPath());
            Type type = finder.FindType(asm, this.dllInterface);
            Analyze analizer = CreateAnalyzer(type);
            
            
            return antivirus.CheckFile(analizer, task.GetTargetPath());
        }
        private Analyze CreateAnalyzer(Type type) 
        {
            MethodInfo analyze = type.GetMethod(this.interfaceMethod);
            object obj = Activator.CreateInstance(type);
            Analyze analizer;
            analizer = Delegate.CreateDelegate(typeof(Analyze), obj, analyze) as Analyze; 
            return analizer;
        }
    }
}
