
using System;
using System.Text;
using System.Threading.Tasks;

namespace Antivirus
{
    
    class MainClass
    {


        public static void Main(string[] args)
        {
           /* PeHeaderReader reader = new PeHeaderReader();
            PeFile file = reader.ReadPeFile(filePath);
            foreach (ImageSectionHeader header in file.ImageSectionHeaders)
            {
                Console.WriteLine(header.SectionHeader.Name);
            }*/

            //TEST
            /*args = new string[2];
            args[0] = @"D:\Visaul studio prejects\MalwareTests\HW\tests";
            args[1] = @"D:\Visaul studio prejects\Antivirus-Project\HashDll\HashDll\bin\Debug\HashDll.dll";*/
            /*args[2] = @"D:\Visaul studio prejects\Antivirus-Project\Antivirus\Antivirus\bin\Release\Antivirus.exe";
            args[3] = @"D:\Visaul studio prejects\Antivirus-Project\HashDll\HashDll\bin\Debug\HashDll.dll";*/
            /*args[4] = @"D:\Visaul studio prejects\MalwareTests\HW\testAddSec.exe";
            args[5] = @"D:\Visaul studio prejects\Antivirus-Project\HashDll\HashDll\bin\Debug\HashDll.dll";*/
            /*args[6] = @"D:\Visaul studio prejects\MalwareTests\HW\testAddSec.exe";
            args[7] = @"D:\Visaul studio prejects\Antivirus-Project\HashDll\HashDll\bin\Debug\HashDll.dll";
            args[8] = @"D:\Visaul studio prejects\MalwareTests\HW\testAddSec.exe";
            args[9] = @"D:\Visaul studio prejects\Antivirus-Project\HashDll\HashDll\bin\Debug\HashDll.dll";*/
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
