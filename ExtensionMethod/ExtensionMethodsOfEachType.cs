using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Reflection;

namespace AltBuild.LinkedPath
{
    /// <summary>
    ///  Cache extension methods of each type.
    /// </summary>
    public class ExtensionMethodsOfEachType
    {
        /// <summary>
        /// Target type
        /// </summary>
        public Type Type { get; init; }

        /// <summary>
        /// Method of each name
        /// </summary>
        public ConcurrentDictionary<string, List<MethodInfo>> MethodsOfEachName { get; } = new();

        /// <summary>
        /// Get MethodInfo list from method name.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="methods"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryGetMethodInfo(string name, out IReadOnlyList<MethodInfo> methods)
        {
            if (MethodsOfEachName.TryGetValue(name, out var value) && value.Count > 0)
            {
                methods = value;
                return true;
            }

            methods = null;
            return false;
        }
    }
}
