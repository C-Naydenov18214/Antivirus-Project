using ETW.Provider;
using ETW.Tracer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETW.Reflection
{
    public class ReflectionKit
    {
        public static List<Type> GetConstructorTypes(Type type)
        {
            var res = new List<Type>();
            var constructors = type.GetConstructors();
            var c = constructors.FirstOrDefault();

            var parameters = c.GetParameters();
            foreach (var p in parameters)
            {
                res.Add(p.ParameterType);
            }
            return res;

        }


        public static dynamic Convert(dynamic src, Type dest)
        {
            return System.Convert.ChangeType(src, dest);
        }


        public static List<object> GetConstructorArgs(Type analyzerType, EventTracer eventTracer)
        {
            var types = GetConstructorTypes(analyzerType);
            List<KeyValuePair<object, Type>> providersPair = new List<KeyValuePair<object, Type>>();
            var providersArgs = types.GetRange(0, types.Count - 1);
            foreach (var t in providersArgs)
            {
                Type providerClass = typeof(EventProvider<>);
                Type constructedProvider = providerClass.MakeGenericType(t.GetGenericArguments()[0]);
                object provider = Activator.CreateInstance(constructedProvider, eventTracer.AllEvents);
                providersPair.Add(new KeyValuePair<object, Type>(provider, constructedProvider));
            }

            List<object> providers = new List<object>(providersPair.Count);
            foreach (var p in providersPair)
            {
                object provider = Convert(p.Key, p.Value);
                providers.Add(provider);
            }

            return providers;
        }
    }
}
