using Reporter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Antivirus
{
    class Antivirus
    {
        public AntivirusReport CheckFile(Analyze analizer,string path)
        {
            return analizer(path);
        }
    }
}
