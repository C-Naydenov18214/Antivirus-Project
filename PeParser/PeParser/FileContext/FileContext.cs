using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeveloperKit.Context
{
    public class FileContext
    {
        private FileInfo info;
        private PeFileContext peInfo;

        public FileContext(FileInfo fileInfo, PeFileContext peInfo)
        {
            this.info = fileInfo;
            this.peInfo = peInfo;
        }

        public FileInfo FileInfo { get { return info; } }
        public PeFileContext PeInfo { get { return peInfo; } }

    }
}
