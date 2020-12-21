using DeveloperKit.Report;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace GUI
{
    class Reporter
    {
        private TextBlock reportB;
        public Reporter(TextBlock report)
        {
            this.reportB = report;
        }

        public void ShowResult(IEnumerable<AntivirusReport> reports)
        {
            int n = 0;
            foreach (AntivirusReport report in reports)
            {

                foreach (VirusInfo info in report.VirusInfos)
                {
                    reportB.Text += $"File name - {info.FilePath}\n";
                    reportB.Text += $"Signature: \n";
                    //Console.WriteLine($"File name - {info.FilePath}");
                    //Console.WriteLine($"Signature: ");
                    if (info.Signature != null)
                    {
                        foreach (byte b in info.Signature)
                        {
                            if (n > 31)
                            {
                                reportB.Text += '\n';
                                //Console.WriteLine();
                                n = 0;
                            }

                            reportB.Text += b.ToString("X2") + " " + '\n';
                            //Console.Write(b.ToString("X2") + " ");
                            n++;
                        }
                    }

                    reportB.Text += '\n';
                    reportB.Text += $"URL to database: {info.UrlToDataBase}\n";
                    reportB.Text += "Information from analyzer: \n";
                    //Console.WriteLine();
                    //Console.WriteLine($"URL to database: {info.UrlToDataBase}");
                    //Console.WriteLine("Information from analyzer: ");
                    foreach (KeyValuePair<string, object> pair in info.Inforamation)
                    {
                        reportB.Text += $"{pair.Key} {pair.Value}\n";
                        //Console.WriteLine($"{pair.Key} {pair.Value}");
                    }

                    reportB.Text += "----------------------------------------------------------------\n";
                    //Console.WriteLine("----------------------------------------------------------------");
                }

                reportB.Text += "============================================\n";
                //Console.WriteLine("============================================");
            }

        }
    }
}