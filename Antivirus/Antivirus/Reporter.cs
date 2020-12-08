using DeveloperKit.Report;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Antivirus
{
    class Reporter
    {

        public void ShowResult(IEnumerable<AntivirusReport> reports)
        {
            int n = 0;
            foreach (AntivirusReport report in reports)
            {

                foreach (VirusInfo info in report.VirusInfos)
                {
                    Console.WriteLine($"File name - {info.FilePath}");
                    Console.WriteLine($"Signature: ");
                    if (info.Signature != null)
                    {
                        foreach (byte b in info.Signature)
                        {
                            if (n > 31)
                            {
                                Console.WriteLine();
                                n = 0;
                            }
                            Console.Write(b.ToString("X2") + " ");
                            n++;
                        }
                    }
                    Console.WriteLine();
                    Console.WriteLine($"URL to database: {info.UrlToDataBase}");
                    Console.WriteLine("Information from analyzer: ");
                    foreach (KeyValuePair<string, object> pair in info.Inforamation)
                    {
                        Console.WriteLine($"{pair.Key} {pair.Value}");
                    }
                    Console.WriteLine("----------------------------------------------------------------");
                }
                Console.WriteLine("============================================");
            }

        }
    }
}
