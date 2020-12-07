using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Antivirus
{
    class DirectoreManager
    {

        private Queue<string> dirs = new Queue<string>();
        private List<string> files = new List<string>();
        public bool IsDirectory(string path)
        {
            if (Directory.Exists(path)) 
            {
                return true;
            }
            return false;
        
        }

        public bool IsFile(string path)
        {
            if(File.Exists(path))
            {
                return true;
            }
            return false;
        }

        public IEnumerable<string> GetFiles(string dirPath) 
        {
            dirs.Enqueue(dirPath);

            while(dirs.Count > 0)
            {
                string curDir = dirs.Dequeue();
                string[] fileEntries = Directory.GetFiles(curDir);
                foreach (string f in fileEntries)
                {
                    files.Add(f);
                }

                string[] subdirectoryEntries = Directory.GetDirectories(curDir);
                foreach (string subdirectory in subdirectoryEntries)
                    dirs.Enqueue(subdirectory);
            }
            return files;
        }

    }
}
