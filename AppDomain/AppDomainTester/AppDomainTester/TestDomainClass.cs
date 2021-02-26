using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AppDomainTester
{
    class TestDomainClass
    {
        public static void CreateAndTestDomain()
        {
            //domain test start
            string callingDomainName = Thread.GetDomain().FriendlyName;
            Console.WriteLine($"callingDomainName = {callingDomainName}");
            string exeAssembly = Assembly.GetEntryAssembly().FullName;
            Console.WriteLine($"assembly = {exeAssembly}");
            // Construct and initialize settings for a second AppDomain.
            AppDomainSetup ads = new AppDomainSetup();
            ads.ApplicationBase = AppDomain.CurrentDomain.BaseDirectory;
            ads.DisallowBindingRedirects = false;
            ads.DisallowCodeDownload = true;

            ads.ConfigurationFile =
                AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;
            // Create the second AppDomain.
            AppDomain.MonitoringIsEnabled = true;
            AppDomain ad2 = AppDomain.CreateDomain("Domain for application", null, ads);
           
            
            // Create an instance of MarshalbyRefType in the second AppDomain.
            // A proxy to the object is returned.
            TestClass mbrt =
                (TestClass)ad2.CreateInstanceAndUnwrap(
                    exeAssembly,
                    typeof(TestClass).FullName
                );
            // Call a method on the object via the proxy, passing the
            // default AppDomain's friendly name in as a parameter.
            mbrt.TestMethod(callingDomainName);
            AppDomain.Unload(ad2);
            try
            {
                // Call the method again. Note that this time it fails
                // because the second AppDomain was unloaded.
                mbrt.TestMethod(callingDomainName);
                Console.WriteLine("Sucessful call.");
            }
            catch (AppDomainUnloadedException)
            {
                Console.WriteLine("Failed call; this is expected.");
            }
            //domain test end

        }

    }
}
