using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Reflection;

namespace AltBuild.LinkedPath
{
    /// <summary>
    /// Extended definition of Object.
    /// </summary>
    public static class TypeExtensions2
    {
        /// <summary>
        /// Is the member defined in Type?
        /// </summary>
        /// <param name="type">Target type</param>
        public static bool IsMemberDefinedIn(this Type type, string memberName)
        {
            foreach (var member in type.GetMembers())
                if (member.Name == memberName && member.DeclaringType.Equals(type))
                    return true;

            return false;
        }

        /// <summary>
        /// Is the method defined in Type?
        /// </summary>
        /// <param name="type">Target type</param>
        public static bool IsMethdDefinedIn(this Type type, string methodName, Type[] parametersType = null)
        {
            foreach (var method in type.GetMethods())
            {
                if (method.Name == methodName && method.DeclaringType.Equals(type))
                {
                    if (parametersType != null && IsTypeMatch(method.GetParameters()) == false)
                        continue;

                    return true;
                }
            }
            return false;

            bool IsTypeMatch(ParameterInfo[] parameters)
            {
                if (parameters.Length != parametersType.Length)
                    return false;

                for (int i = 0; i < parameters.Length; i++)
                    if (parameters[i].ParameterType.Equals(parametersType[i]) == false)
                        return false;

                return true;
            }
        }
    }
}
