using AltBuild.LinkedPath;
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Reflection;

namespace AltBuild.LinkedPath
{
    /// <summary>
    /// Attribute helper static method.
    /// </summary>
    public static class AttributeHelper
    {
        /// <summary>
        /// Get from member or type attribute.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="memberInfo"></param>
        /// <returns></returns>
        public static T[] GetAttributesEx<T>(MemberInfo memberInfo)
            where T : class
        {
            // Get IndexerConverterAttribute from Member.
            var attributes = memberInfo.GetAttributes<T>();
            if (attributes.Length > 0)
                return attributes;

            // Get MemberType.
            var returnType = memberInfo.GetReturnType();

            // Get StoreConverterAttribute from member type definition.
            attributes = returnType.GetAttributes<T>();
            if (attributes.Length > 0)
                return attributes;

            // Return null.
            return null;
        }
    }
}
