using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Kit;

namespace App
{
    public class TypeFinder
    {
        public static Type GetType(Assembly assembly)
        {
            var types = assembly.GetTypes();

            return types.FirstOrDefault(t => t.IsSubclassOf(typeof(ARxAnalyzer)));
        }
    }
}