using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ReadAndWrite
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var curDir = Directory.GetCurrentDirectory();
            var dirPath = $"{curDir}\\Files";
            DirectoryInfo dir = new DirectoryInfo(dirPath);
            if (!dir.Exists)
            {
                dir.Create();
            }
            try
            {
                /*for (int i = 0; i < 10; i++)
                {
                    string path = dir.FullName + @"\" + i.ToString() + ".txt";
                    Console.WriteLine(path);
                    using (StreamWriter sw = new StreamWriter(path, false))
                    {
                        await sw.WriteLineAsync($"This wrote { Process.GetCurrentProcess().Id} :)");
                    }
                }*/
                for (int i = 0; i < 10; i++)
                {
                    string path = dir.FullName + @"\" + i.ToString() + ".txt";
                    using (StreamReader sr = new StreamReader(path))
                    {
                        var data = sr.ReadLine();
                        string pathToWrite = dir.FullName + @"\" + (i + 100).ToString() + ".txt";
                        using (StreamWriter sw = new StreamWriter(pathToWrite, false))
                        {
                            await sw.WriteLineAsync(data + " I did it again");
                        }
                        
                    }
                    File.Delete(path);
                    Thread.Sleep(1000);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
