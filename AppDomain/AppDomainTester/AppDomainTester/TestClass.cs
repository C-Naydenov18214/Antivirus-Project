using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Reflection;

namespace AppDomainTester
{
    class TestClass : MarshalByRefObject
    {

        public void TestMethod(string callingDomainName)
        {
            Assembly target = Assembly.LoadFrom(@"D:\Downloads\VisualStudio\VisualStudioProjects\Antivirus-Project\AppDomain\Application\TestAssembly\bin\Debug\TestAssembly.exe");
            Type[] types = target.GetTypes();
            foreach (Type t in types)
            {

                Console.WriteLine(t);
                if (t.FullName.Contains("MainClass"))
                {
                    MethodInfo method = t.GetMethod("TestMethod");
                    var instance = Activator.CreateInstance(t);
                    object[] param = { callingDomainName };
                    var res = method.Invoke(instance, param);
                    Console.WriteLine($"res ={res}");
                    break;
                }
            }
            /**/
        }

    }
}
