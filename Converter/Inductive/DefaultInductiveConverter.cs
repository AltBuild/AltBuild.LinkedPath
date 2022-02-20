using AltBuild.LinkedPath.Parser;
using System;
using System.Reflection;
using System.Diagnostics;

namespace AltBuild.LinkedPath.Converters
{
    /// <summary>
    /// Default InductiveConverter class.
    /// (Default operation when type is the same)
    /// </summary>
    public class DefaultInductiveConverter : InductiveConverterBase
    {
        /// <summary>
        /// Other to model
        /// </summary>
        /// <param name="pathMember">PathMember</param>
        /// <param name="memberInfo">Member infomation</param>
        /// <param name="sourceObject">Source object</param>
        /// <param name="value">value</param>
        /// <returns></returns>
        public override bool TrySetValue<T>(PathMember pathMember, MemberInfo memberInfo, object sourceObject, T value)
        {
            Debug.Assert(memberInfo != null);
            Debug.Assert(sourceObject != null);

            var pathType = memberInfo.GetPathType();
            if (pathType.NativeType.Equals(typeof(T)) == false && (pathType.IsNullable == false || pathType.NullableUnderlyingType.Equals(typeof(T)) == false))
            {
                if (value == null || (value is string strValue && string.IsNullOrWhiteSpace(strValue)))
                {
                    memberInfo.SetDefaultValue(sourceObject);
                    return true;
                }

                else if (TypeConverterExtensions.TryGet(pathType.NativeType, value, out object destineValue))
                {
                    memberInfo.SetValue(sourceObject, destineValue);
                    return true;
                }

                else
                    throw new InvalidConvertException($"Error, Invalid converter exception.[{typeof(T)} -> {pathType.NativeType}]");
            }

            memberInfo.SetValue(sourceObject, value);
            return true;
        }

        /// <summary>
        /// Model to other
        /// </summary>
        /// <param name="memberInfo">Member infomation</param>
        /// <param name="sourceObject">from object</param>
        /// <returns>successful: true, failed: false</returns>
        public override bool TryGetValueInner<T>(PathMember pathMember, MemberInfo memberInfo, object atObject, out T value)
        {
            if (atObject is T atValue)
            {
                value = atValue;
                return true;
            }

            else if (typeof(T).Equals(typeof(string)))
            {
                if (atObject?.ToString() is T atValueString)
                    value = atValueString;

                else
                    value = default(T);

                return true;
            }

            value = default(T);
            return false;
        }
    }
}
