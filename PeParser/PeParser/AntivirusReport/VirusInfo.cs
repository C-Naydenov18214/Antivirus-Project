using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeveloperKit.Report
{
    public class VirusInfo
    {
        private Dictionary<string, string> information;
        private string urlToDataBase;
        private string filePath;
        private byte[] signature;

        public VirusInfo()
        {
            information = new Dictionary<string, string>();

        }

        public byte[] Signature { get { return signature; } set { signature = value; } }

        public string UrlToDataBase { get { return urlToDataBase; } set { urlToDataBase = value; } }

        public string FilePath { get { return filePath; } set { filePath = value; } }

        public IDictionary<string, string> Inforamation
        {
            get
            {
                return information;
            }
        }
        public void addInfo(string key, string value)
        {
            information.Add(key, value);
        }



    }
}
