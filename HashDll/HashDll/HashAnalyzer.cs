
using DeveloperKit.Analyzer;
using DeveloperKit.Context;
using DeveloperKit.PeReader;
using DeveloperKit.Report;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HashDll
{
    public class HashAnalyzer : IAnalyzer
    {

        public AntivirusReport Analyze(FileContext fileContext)
        {
            byte[] bytes = null;
            PeFileContext context = fileContext.PeInfo;
            foreach (ImageSectionHeader header in context.ImageSectionHeaders)
            {
                Console.WriteLine(header.SectionHeader.Name);
            }
            try
            {
                ImageSectionHeader header = context.GetSection(".virus");
                bytes = header.SectionBytes;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            int n = 0;
            
            Console.WriteLine();
            Searcher searcher = new Searcher();
            var res = searcher.FindHeaderWithChar(context);
            if (res != null)
            {
                foreach (ImageSectionHeader header in res)
                {
                    Console.Write("section " + new string(header.SectionHeader.Name) + "\n");
                }
            }

            AntivirusReport report = new AntivirusReport();
            VirusInfo virusInfo = new VirusInfo();
            virusInfo.addInfo("start", "0x0012f3");
            virusInfo.addInfo("length", 105);
            virusInfo.addInfo("status", "malicious");
            virusInfo.FilePath = fileContext.FileInfo.FullName;
            virusInfo.Signature = bytes;
            virusInfo.UrlToDataBase = "heh/gg/322";
            report.addVirusInfo(virusInfo);
            return report;
        }

        public void PrintMessage(string mes)
        {
            Console.WriteLine(mes);
        }


    }



}
