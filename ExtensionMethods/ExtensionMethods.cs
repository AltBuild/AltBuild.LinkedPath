using System;
using System.Collections.Generic;
using System.Reflection;

namespace AltBuild.LinkedPath
{
    /// <summary>
    /// Store the extension method list
    /// </summary>
    public static class ExtensionMethods
    {
        /// <summary>
        /// lock object.
        /// </summary>
        static object _lockObject = new();

        /// <summary>
        /// Empty method info.
        /// </summary>
        static IReadOnlyList<MethodInfo> Empty { get; } = new MethodInfo[0];

        /// <summary>
        /// Get the cache of the extension method
        /// </summary>
        static ExtensionMethodsStore ExtensionMethodsStore
        {
            get
            {
                if (_extensionMethodsStore == null)
                {
                    lock (_lockObject)
                    {
                        if (_extensionMethodsStore == null)
                        {
                            _extensionMethodsStore = new ExtensionMethodsStore();
                        }
                    }
                }
                return _extensionMethodsStore;
            }
        }
        static ExtensionMethodsStore _extensionMethodsStore = null;

        /// <summary>
        /// Get extension method list
        /// </summary>
        /// <param name="targetObject">Target type</param>
        /// <param name="methodName">Method name</param>
        /// <returns>Target MethodInfo list</returns>
        public static IReadOnlyList<MethodInfo> GetExtensionMethods(Type type, string methodName)
        {
            // Get Extension method list with Source object type.
            if (ExtensionMethodsStore.TryGetMethodsInfo(type, methodName, out var methods))
                return methods;

            // 2) Second: Get Extension method list with object type.
            else if (ExtensionMethodsStore.TryGetMethodsInfo(typeof(object), methodName, out methods))
                return methods;

            // 3) failed.
            return Empty;
        }
    }
}
