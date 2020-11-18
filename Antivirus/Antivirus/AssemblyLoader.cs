using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Antivirus
{
    class AssemblyLoader
    {
        public Assembly LoadAssambly(string path) 
        {
            Assembly asm;
            try
            {
                asm = Assembly.LoadFrom(path);
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine(ex.Message);
                asm = null;
            }
            return asm;
        }
    }
}
