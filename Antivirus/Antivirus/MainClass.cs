
using System;
using System.Text;
using System.Threading.Tasks;

namespace Antivirus
{
    
    class MainClass
    {

        
        public static void Main(string[] args) 
        {
            //TEST
            args = new string[10];
            args[0] = "TARGET_1";
            args[1] = @"D:\Visaul studio prejects\Antivirus-Project\HashDll\HashDll\bin\Debug\HashDll.dll";
            args[2] = "TARGET_2";
            args[3] = @"D:\Visaul studio prejects\Antivirus-Project\HashDll\HashDll\bin\Debug\HashDll.dll";
            args[4] = "TARGET_3";
            args[5] = @"D:\Visaul studio prejects\Antivirus-Project\HashDll\HashDll\bin\Debug\HashDll.dll";
            args[6] = "TARGET_4";
            args[7] = @"D:\Visaul studio prejects\Antivirus-Project\HashDll\HashDll\bin\Debug\HashDll.dll";
            args[8] = "TARGET_5";
            args[9] = @"D:\Visaul studio prejects\Antivirus-Project\HashDll\HashDll\bin\Debug\HashDll.dll";
            int len = args.Length;
            if (len < 2 || len % 2 != 0)
            {
                Console.WriteLine("Not enought arguments: couples required <filepath> <dllpath> ...");
                return;
            }

            AntivirusRunner runner = new AntivirusRunner();
            runner.Run(args);
        }      
    }
}
