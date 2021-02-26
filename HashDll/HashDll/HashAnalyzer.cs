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
    public class HashAnalyzer :  MarshalByRefObject, IAnalyzer
    {

        public AntivirusReport Analyze(FileContext fileContext)
        {
            string path = Directory.GetCurrentDirectory();
            DatabaseController database = new DatabaseController(path + "\\Viruses.sqlite");
            database.Connect();
            AntivirusReport report = new AntivirusReport();
            IEnumerable<Virus> viruses = database.GetVirusesList();// { 96, 97, 104, 241, 18, 64, 0, 195, 204, 204, 204, 204, 204, 204, 204, 204 };//Сигнатура из базы данных
            foreach (Virus v in viruses)
            {
                string[] str = v.Signature.Split('-');
                byte[] virusBody = new byte[str.Length];

                for (int i = 0; i < str.Length; i++)
                {
                    virusBody[i] = Convert.ToByte(str[i], 16);
                }

                VirusInfo virusInfo = new VirusInfo();
                PeFileContext context = fileContext.PeInfo;
                foreach (ImageSectionHeader header in context.ImageSectionHeaders)
                {
                    bool isContain = IsContain(header.SectionBytes, virusBody);
                    if (isContain)
                    {
                        virusInfo.addInfo($"WARNING: Section: {new string(header.SectionHeader.Name)} contaions virus body", $"Virus ID: {v.Id}");
                    }
                    else
                    {
                        virusInfo.addInfo($"Section: {new string(header.SectionHeader.Name)} is ", "cleare");
                    }
                }
                try
                {
                    ImageSectionHeader header = context.GetSection(".virus");
                }
                catch (Exception e)
                {
                    virusInfo.addInfo("NO such section", ".virus");
                }
                Searcher searcher = new Searcher();
                var res = searcher.FindHeaderWithCharacteritic(context, Structures.DataSectionFlags.ContentCode, Structures.DataSectionFlags.MemoryExecute);
                if (res != null)
                {
                    foreach (ImageSectionHeader header in res)
                    {
                        virusInfo.addInfo("Sections with characteristic: ContentCode, MemoryExecute", new string(header.SectionHeader.Name));
                    }
                }
                virusInfo.FilePath = fileContext.FileInfo.FullName;
                virusInfo.Signature = virusBody;
                report.addVirusInfo(virusInfo);
            }
            return report;
        }

        private bool IsContain(byte[] array, byte[] subArray)
        {
            int index = 0;
            int count = 0;
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i] == subArray[count])
                {
                    count++;
                }
                else
                {
                    count = 0;
                }
                if (count == subArray.Length)
                {
                    index = i - count + 1;
                    return true;
                }
            }
            return false;
        }


        public void PrintMessage(string mes)
        {
            Console.WriteLine(mes);
        }


    }



}
