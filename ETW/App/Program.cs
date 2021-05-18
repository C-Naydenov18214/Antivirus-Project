using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;

namespace App
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter paths to DLL. Type 's' to start.");
            var list = new List<string>();
            string path = Console.ReadLine();
            while (path != "s")
            {
                list.Add(path);
                path = Console.ReadLine();
            }
            args = list.Select(i => i.ToString()).ToArray();
            Application.Run(args);
        }
    }
}
