using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TestAssembly
{
    class MainClass : MarshalByRefObject
    {
        static void Main(string[] args)
        {
          
        }
        public int TestMethod(string callingDomainName)
        {
            // Get this AppDomain's settings and display some of them.
            AppDomainSetup ads = AppDomain.CurrentDomain.SetupInformation;
            Console.WriteLine("AppName={0}, AppBase={1}, ConfigFile={2}",
                ads.ApplicationName,
                ads.ApplicationBase,
                ads.ConfigurationFile
            );
            // Display the name of the calling AppDomain and the name
            // of the second domain.
            // NOTE: The application's thread has transitioned between
            // AppDomains.
            Console.WriteLine("Calling from '{0}' to '{1}'.",
                callingDomainName,
                Thread.GetDomain().FriendlyName
            );
            AppDomain curDoMain = AppDomain.CurrentDomain;
            Console.WriteLine($"Total Allocated Memory Size befor = {curDoMain.MonitoringTotalAllocatedMemorySize}");
            Int64[] integers = new Int64[100000];
            Console.WriteLine($"Total Allocated Memory Size after = {curDoMain.MonitoringTotalAllocatedMemorySize}");


            return callingDomainName.Length;
        }

    }
}
