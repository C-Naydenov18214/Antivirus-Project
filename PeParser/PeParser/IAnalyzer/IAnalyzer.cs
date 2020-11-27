using DeveloperKit;
using DeveloperKit.Context;
using DeveloperKit.Report;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeveloperKit.Analyzer
{
    public interface IAnalyzer
    {
        AntivirusReport Analyze(FileContext context);
    }
}
