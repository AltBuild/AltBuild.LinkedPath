using AltBuild.LinkedPath.Converters;
using AltBuild.LinkedPath.Parser;
using System;
using System.Reflection;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics;
using System.Xml.Linq;
using System.Runtime.CompilerServices;

namespace AltBuild.LinkedPath
{
    /// <summary>
    /// Results from object and PathMember
    /// </summary>
    public partial class InductInfo : IInductInfo
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T GetValue<T>()
        {
            TryGetValue(out T value);
            return value;
        }

        public bool TryGetValue<T>(out T value)
        {
            if (Frame.IsUnduct == false)
            {
                // Try direct casting
                if (ReturnValue is T atValue)
                {
                    value = atValue;
                    return true;
                }

                // Set null with true
                else if (ReturnValue == null)
                {
                    value = default(T);

                    var pathType = MemberInfo.GetPathType();
                    return (
                        (typeof(T).Equals(typeof(string)) ||
                        pathType.IsNullable && pathType.NullableUnderlyingType.Equals(typeof(T))) ||
                        (pathType.NativeType.Equals(typeof(T)))
                    );
                }

                else if (typeof(T).Equals(typeof(string)))
                {
                    string stringValue;

                    if (PathMember.Frame.AttributesIsValid && PathMember.Frame.Attributes.TryGetFormat(out string format))
                        stringValue = ReturnValue.ToString(format);
                    else
                        stringValue = ReturnValue.ToString();

                    if (stringValue is T atValue2)
                        value = atValue2;
                    else
                        value = default(T);

                    return true;
                }

                // Get value with converter
                else if (
                    Frame.Methods.HasFlag(InductMethods.NoInductiveConverter) == false &&
                    InductiveConverterFactory.TryGetConverter<T>(MemberInfo, out InductiveConverterBase converter))
                {
                    return converter.TryGetValueInner(PathMember, MemberInfo, ReturnValue, out value);
                }
            }

            value = default(T);
            return false;
        }

        /// <summary>
        /// Set target member value with default value.
        /// </summary>
        /// <returns>Successful: true, Failed: false</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetDefaultValue()
        {
            var defaultValue = Activator.CreateInstance(MemberInfo.GetReturnType());
            SetValue(defaultValue);
        }

        /// <summary>
        /// Set target member value.
        ///   Support: Indexer(Index only), Trace, Inductive converter
        ///   Not support: Indexer(etc.) 
        /// </summary>
        /// <param name="source">Definition object</param>
        /// <param name="value">value</param>
        /// <returns>Successful: true, Failed: false</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetValue(object value)
        {
            if (TrySetValue(BaseObject, value?.GetType(), value) == false)
                throw new InvalidOperationException();
        }

        /// <summary>
        /// Set target member value.
        ///   Support: Indexer(Index only), Trace, Inductive converter
        ///   Not support: Indexer(etc.) 
        /// </summary>
        /// <param name="source">Definition object</param>
        /// <param name="value">value</param>
        /// <returns>Successful: true, Failed: false</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetValue<T>(T value)
        {
            if (TrySetValue(BaseObject, typeof(T), value) == false)
                throw new InvalidOperationException();
        }

        /// <summary>
        /// Set target member value.
        ///   Support: Indexer(Index only), Trace, Inductive converter
        ///   Not support: Indexer(etc.) 
        /// </summary>
        /// <param name="source">Definition object</param>
        /// <param name="value">value</param>
        /// <returns>Successful: true, Failed: false</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TrySetValue(object value) =>
            TrySetValue(BaseObject, value?.GetType(), value);

        /// <summary>
        /// Set target member value.
        ///   Support: Indexer(Index only), Trace, Inductive converter
        ///   Not support: Indexer(etc.) 
        /// </summary>
        /// <param name="source">Definition object</param>
        /// <param name="value">value</param>
        /// <returns>Successful: true, Failed: false</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TrySetValue<T>(T value) =>
            TrySetValue(BaseObject, typeof(T), value);

        bool TrySetValue(object source, Type type, object value)
        {
            // Set value
            if (value == null || IsMatchType())
            {
                MemberInfo.SetValue(source, value);
                return true;
            }

            // Set value with converter
            else if (
                Frame.Methods.HasFlag(InductMethods.NoInductiveConverter) == false &&
                InductiveConverterFactory.TryGetConverter(type, MemberInfo, out InductiveConverterBase converter))
            {
                dynamic dynValue = value;
                return converter.TrySetValue(PathMember, MemberInfo, source, dynValue);
            }

            bool IsMatchType()
            {
                var returnType = MemberInfo.GetReturnType();
                return type.Equals(returnType) || type.Equals(Nullable.GetUnderlyingType(returnType));
            }

            return false;
        }

        public bool TryInvokeMethod() => TryInvokeMethod(out object returnValue);

        public bool TryInvokeMethod([MaybeNullWhen(false)] out object returnValue)
        {
            if (Frame.IsUnduct == false && MemberInfo is MethodInfo methodInfo)
            {
                if (BaseObject != null)
                {
                    // パラメーター生成
                    var parameterInfos = methodInfo.GetParameters();
                    object[] parameters = new object[parameterInfos.Length];

                    if (parameterInfos.Length > 0)
                        throw new InvalidProgramException();

                    //for (int i = 0; i < parameterInfos.Length; i++)
                    //    if (this.GetType().GetInterfaces().Contains(parameterInfos[i].ParameterType))
                    //        parameters[i] = this;

                    returnValue = methodInfo.Invoke(BaseObject, parameters);
                    return true;
                }
            }

            returnValue = null;
            return false;
        }

        public T GetAttribute<T>()
        {
            if (Frame.IsUnduct)
                return default(T);

            else
                return MemberInfo.GetAttribute<T>();
        }

        public bool TryGetAttribute<T>(out T attribute)
        {
            if (Frame.IsUnduct == false)
            {
                attribute = MemberInfo.GetAttribute<T>();
                return (attribute != null);
            }

            attribute = default(T);
            return false;
        }

        /// <summary>
        /// Last InductInfo
        /// </summary>
        InductInfo Last
        {
            get
            {
                var next = PathMember?.Child;

                // To Child.
                if (next != null && Frame.Methods.HasFlag(InductMethods.NoDescendant) == false)
                    return Create(Frame, next, ReturnValue, ReturnType).Last;

                // Return this.
                else
                    return this;
            }
        }

        public override string ToString() => ReturnValue.ToString(PathElements.Default);
    }
}
