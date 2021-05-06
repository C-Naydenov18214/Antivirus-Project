using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SuspiciousProcess
{
    class Program
    {
        static void Main(string[] args)
        {
            Assembly asm = null;
            //while (true)
            {
                try
                {
                    asm = Assembly.LoadFrom(@"D:\Downloads\Studying\Antivirus-Project\Rx\Rx\bin\Debug\Rx.dll");
                    
                }
                catch (FileNotFoundException ex)
                {
                    Console.WriteLine(ex.Message);
                }
                var types = asm.GetTypes();
                Thread.Sleep(1500);
                /*foreach (var t in types)
                {
                    Console.WriteLine($"Type: {t.FullName}");
                }*/
            }
        }
    }
}
