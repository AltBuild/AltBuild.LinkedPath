using System.Reflection;

namespace AltBuild.LinkedPath.Extensions
{
    /// <summary>
    /// Attribute helper static method.
    /// </summary>
    public static class AttributeExtensions
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
            // Get attribute from Member.
            var attributes = memberInfo.GetAttributes<T>();
            if (attributes.Length > 0)
                return attributes;

            // Get memberType.
            var returnType = memberInfo.GetReturnType();

            // Get attribute from member type definition.
            attributes = returnType.GetAttributes<T>();
            if (attributes.Length > 0)
                return attributes;

            // Return null.
            return null;
        }

        public static bool TryGetAttributesEx<T>(MemberInfo memberInfo, out T[] attributes)
            where T : class
        {
            attributes = GetAttributesEx<T>(memberInfo);
            return (attributes != null && attributes.Length > 0);
        }
    }
}
