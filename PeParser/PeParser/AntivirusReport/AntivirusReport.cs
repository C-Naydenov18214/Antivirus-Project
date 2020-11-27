using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeveloperKit.Report
{
    public class AntivirusReport
    {
        private List<VirusInfo> virusInfos;


        public AntivirusReport()
        {
            virusInfos = new List<VirusInfo>();
        }

        public void addVirusInfo(VirusInfo info)
        {
            virusInfos.Add(info);
        }

        public IEnumerable<VirusInfo> VirusInfos
        {
            get { return virusInfos; }

        }

    }
}
