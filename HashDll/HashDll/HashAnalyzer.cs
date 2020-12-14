using Database;
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
            AntivirusReport report = new AntivirusReport();
            VirusInfo virusInfo = new VirusInfo();
            PeFileContext context = fileContext.PeInfo;
            foreach (ImageSectionHeader header in context.ImageSectionHeaders)
            {
                virusInfo.addInfo("Section", new string(header.SectionHeader.Name));
            }
            try
            {
                ImageSectionHeader header = context.GetSection(".virus");
                bytes = header.SectionBytes;
            }
            catch (Exception e)
            {
                virusInfo.addInfo("NO such section", ".virus");
            }
            int n = 0; 
            Searcher searcher = new Searcher();
            var res = searcher.FindHeaderWithCharacteritic(context,Structures.DataSectionFlags.ContentCode, Structures.DataSectionFlags.MemoryExecute);
            if (res != null)
            {
                foreach (ImageSectionHeader header in res)
                {
                    virusInfo.addInfo("Sections with characteristic: ContentCode, MemoryExecute", new string(header.SectionHeader.Name));
                }
            }
            virusInfo.FilePath = fileContext.FileInfo.FullName;
            virusInfo.Signature = bytes;
            virusInfo.UrlToDataBase = "https://vms.drweb.ru/search/";
            string path = Directory.GetCurrentDirectory();
            DatabaseController database = new DatabaseController(path + "\\Viruses.sqlite");
            database.Connect();
            database.Insert(new Virus(virusInfo));
            database.CloseConnection();
            report.addVirusInfo(virusInfo);
            return report;
        }

        public void PrintMessage(string mes)
        {
            Console.WriteLine(mes);
        }


    }



}
