using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeveloperKit.Report
{
    public class VirusInfo
    {
        private List<KeyValuePair<string, object>> information;
        private string urlToDataBase;
        private string filePath;
        private byte[] signature;

        public VirusInfo()
        {
            information = new List<KeyValuePair<string, object>>();

        }

        public byte[] Signature { get { return signature; } set { signature = value; } }

        public string UrlToDataBase { get { return urlToDataBase; } set { urlToDataBase = value; } }

        public string FilePath { get { return filePath; } set { filePath = value; } }

        public IEnumerable<KeyValuePair<string, object>> Inforamation
        {
            get
            {
                return information;
            }
        }
        public void addInfo(string key, object value)
        {
            information.Add(new KeyValuePair<string, object>(key,value));
        }



    }
}
