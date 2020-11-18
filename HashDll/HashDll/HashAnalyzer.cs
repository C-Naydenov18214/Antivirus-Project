using Reporter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HashDll
{
    public class HashAnalyzer : IAnalyzer
    {
        public AntivirusReport Analyze(string block)
        {            
            return new AntivirusReport(block);
        }

        public void PrintMessage(string mes)
        {
            Console.WriteLine(mes);
        }


    }



}
