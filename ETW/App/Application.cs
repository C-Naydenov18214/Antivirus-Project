using System;
using System.Collections.Generic;
using System.Reflection;
using App.IoC;
using Kit;
using Unity;

namespace App
{
    public class Application
    {
        /// <summary>
        /// Run application.
        /// </summary>
        /// <param name="args">Rx DLLs</param>
        public static void Run(string[] args)
        {
            IUnityContainer container = new UnityContainer();
            ContainerConfigurator.Configure(container);

            Assembly assembly;
            foreach (var value in args)
            {
                assembly = Assembly.LoadFile(value);
                var type = TypeFinder.GetType(assembly, "IRxAnalyzer");
                var analyzer = (IRxAnalyzer)container.Resolve(type);
                analyzer.Start();
            }
            Console.ReadKey();
        }
    }
}