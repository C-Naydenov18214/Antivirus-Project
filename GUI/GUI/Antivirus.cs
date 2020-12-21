using DeveloperKit.Context;
using DeveloperKit.Report;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GUI
{
    class Antivirus
    {
        public AntivirusReport CheckFile(object instance, MethodInfo analyze, FileContext context)
        {
            object[] param = { context };
            var res = analyze.Invoke(instance, param);
            return res as AntivirusReport;
        }
    }
}