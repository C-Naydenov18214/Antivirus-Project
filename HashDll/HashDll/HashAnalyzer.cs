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
            int n = 0;
            foreach (byte b in bytes)
            {
                if (n > 15)
                {
                    Console.WriteLine();
                    n = 0;
                }
                Console.Write(b.ToString("X2") + " ");//Convert.ToString(b, 16));
                n++;
            }
            Console.WriteLine();
            Searcher searcher = new Searcher();
            var res = searcher.FindHeaderWithChar(file);
            if (res != null)
            {
                foreach (ImageSectionHeader header in res)
                {

                    Console.Write("section " + new string(header.SectionHeader.Name) + "\n");
                }
            }
           

            return new AntivirusReport(filePath);
        }

        public void PrintMessage(string mes)
        {
            Console.WriteLine(mes);
        }


    }



}
