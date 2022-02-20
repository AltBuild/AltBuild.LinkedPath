using System;
using System.Reflection;
using System.Linq;

namespace AltBuild.LinkedPath.Parser
{
    /// <summary>
    /// PathOperatorType Code Attribute
    /// </summary>
    public static class PathOperatorTypeExtensions
    {
        public static string GetOperatorCode(this PathOperatorType type)
        {
            var attribute = type.GetType().GetMember(type.ToString()).First().GetCustomAttribute<CodeAttribute>();
            if (attribute != null)
                return attribute.Name;

            else
                return null;
        }
    }
}
