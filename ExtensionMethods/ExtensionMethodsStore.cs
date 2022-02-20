using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Reflection;

namespace AltBuild.LinkedPath
{
    /// <summary>
    ///  Cache all extension methods
    /// </summary>
    public class ExtensionMethodsStore : ConcurrentDictionary<Type, ExtensionMethodsOfEachType>
    {
        public ExtensionMethodsStore()
        {
            // Get the extension definition type array in AppDomain (All assemblies)
            var extensionDefinitionTypes = AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes().Where(t => t.IsDefined(typeof(ExtensionAttribute)))).ToArray();

            // Get all extension methods.
            var extensionMethods = extensionDefinitionTypes.SelectMany(e => e.GetMethods().Where(m => m.IsDefined(typeof(ExtensionAttribute))));

            // Create extension methods of each type.
            foreach (var methodInfo in extensionMethods)
            {
                // Get extensionMethods of each type.
                var methodsOfEachType = GetOrAdd(methodInfo.GetParameters()[0].ParameterType, t => new ExtensionMethodsOfEachType { Type = t });

                // Add methodInfo list.
                methodsOfEachType.MethodsOfEachName.GetOrAdd(methodInfo.Name, n => new List<MethodInfo>()).Add(methodInfo);
            }
        }

        /// <summary>
        /// Get target method info
        /// </summary>
        /// <param name="type">Target type</param>
        /// <param name="name">Method name</param>
        /// <param name="methods">Match method list</param>
        /// <returns>true: success, false: failed</returns>
        public bool TryGetMethodsInfo(Type type, string name, out IReadOnlyList<MethodInfo> methods)
        {
            if (TryGetValue(type, out var methodsOfEachType) && methodsOfEachType.TryGetMethodInfo(name, out methods) && methods.Count > 0)
                return true;

            methods = null;
            return false;
        }
    }
}
