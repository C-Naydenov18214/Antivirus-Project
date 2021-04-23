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
            Console.WriteLine("Enter path to DLL.");
            string path = Console.ReadLine();
            args = new string[1];
            args[0] = path;
            Application.Run(args);
        }
    }
}
