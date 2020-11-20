using PeParser;
using Reporter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HashDll
{
    public class HashAnalyzer : IAnalyzer
    {
        public AntivirusReport Analyze(string filePath)
        {
            PeHeaderReader reader = new PeHeaderReader();
            PeFile file = reader.ReadPeFile(filePath);
            foreach (ImageSectionHeader header in file.ImageSectionHeaders)
            {
                Console.WriteLine(header.SectionHeader.Name);
            }

            byte[] bytes = file.GetSection(".virus").SectionBytes;
            foreach (byte b in bytes)
            {
                Console.Write(Convert.ToString(b, 16));
            }
            return new AntivirusReport(filePath);
        }

        public void PrintMessage(string mes)
        {
            Console.WriteLine(mes);
        }


    }



}
