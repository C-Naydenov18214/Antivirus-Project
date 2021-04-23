using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace App
{
    public class TypeFinder
    {
        public static Type GetType(Assembly assembly, string interfaceName)
        {
            var types = assembly.GetTypes();

            return (from type in types let analyzer = type.GetInterface(interfaceName) where analyzer != null select type).FirstOrDefault();
        }
    }
}