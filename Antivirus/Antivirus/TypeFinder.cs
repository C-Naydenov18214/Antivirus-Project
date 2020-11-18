using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Antivirus
{
    class TypeFinder
    {
        public Type FindType(Assembly asm, string interfaceName)
        {
            Type[] types = asm.GetTypes();
            Type necessaryType = null;
            foreach (Type t in types)
            {
                Type analizer = t.GetInterface(interfaceName);
                if (analizer != null)
                {
                    necessaryType = t;
                    break;
                }
            }
            return necessaryType;
        }
        private IEnumerable GetDirectInterfaces(Type type)
        {
            var allInterfaces = new ArrayList();
            var childInterfaces = new ArrayList();

            foreach (var i in type.GetInterfaces())
            {
                allInterfaces.Add(i);
                foreach (var ii in i.GetInterfaces())
                    childInterfaces.Add(ii);
            }
            foreach (Type i in childInterfaces)
            {
                allInterfaces.Remove(i);
            }
            return allInterfaces;
        }
    }
}
