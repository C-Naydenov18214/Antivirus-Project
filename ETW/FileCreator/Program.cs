using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCreator
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var curDir = Directory.GetCurrentDirectory();
            var dirPath = @"D:\Downloads\Studying\Antivirus-Project\ETW\ReadAndWrite\bin\Debug\Files";
            DirectoryInfo dir = new DirectoryInfo(dirPath);
            if (!dir.Exists)
            {
                dir.Create();
            }
            try
            {
                for (int i = 0; i < 10; i++)
                {
                    string path = dir.FullName + @"\" + i.ToString() + ".txt";
                    Console.WriteLine(path);
                    using (StreamWriter sw = new StreamWriter(path, false))
                    {
                        await sw.WriteLineAsync($"This wrote { Process.GetCurrentProcess().Id} :)");
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }


        }
    }
}
