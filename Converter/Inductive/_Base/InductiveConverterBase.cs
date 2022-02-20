using AltBuild.LinkedPath.Parser;
using System;
using System.Reflection;

namespace AltBuild.LinkedPath.Converters
{
    /// <summary>
    /// InductieConverter base class.
    /// </summary>
    public abstract class InductiveConverterBase
    {
        /// <summary>
        /// Can this converter handle it?
        /// </summary>
        /// <param name="type">To type</param>
        /// <param name="pathMember">Path member.</param>
        /// <param name="memberInfo">Member infomation.</param>
        /// <returns>true: can, false: can not</returns>
        public virtual bool CanConvert(Type type, PathMember pathMember, MemberInfo memberInfo) => true;

        /// <summary>
        /// Obtain the key value of an object
        /// </summary>
        /// <param name="sourceObject">Source object</param>
        /// <param name="value">value</param>
        /// <returns>True: Successful. False: Failed.</returns>
        public virtual bool TryGetKeyValue(object sourceObject, out dynamic value)
        {
            value = sourceObject;
            return true;
        }

        /// <summary>
        /// Model to value
        /// </summary>
        /// <param name="memberInfo">Member infomation</param>
        /// <param name="sourceObject">from object</param>
        /// <param name="value">value</param>
        /// <returns>true if success, false if failed</returns>
        public virtual bool TryGetValue<T>(PathMember pathMember, MemberInfo memberInfo, object sourceObject, out T value)
        {
            object atObject = null;

            // Get the atValue from MemberInfo
            if (memberInfo != null)
                atObject = memberInfo.GetValue(sourceObject);

            // Get the atValue from PathMember
            else if (pathMember != null)
                atObject = sourceObject.Induct(pathMember).ReturnValue;

            // Can not the atValue. 
            else
                throw ThrowConvertExceptionOnGet(pathMember, memberInfo);

            return TryGetValueInner<T>(pathMember, memberInfo, atObject, out value);
        }

        /// <summary>
        /// Model to value
        /// </summary>
        /// <param name="memberInfo">Member infomation</param>
        /// <param name="atObject">from object</param>
        /// <param name="value">value</param>
        /// <returns>true if success, false if failed</returns>
        public virtual bool TryGetValueInner<T>(PathMember pathMember, MemberInfo memberInfo, object atObject, out T value) =>
            throw new NotImplementedException();

        /// <summary>
        /// value to model
        /// </summary>
        /// <typeparam name="T">value type</typeparam>
        /// <param name="pathMember">Path member.</param>
        /// <param name="memberInfo">Member infomation.</param>
        /// <param name="sourceObject">to Object</param>
        /// <param name="value">value</param>
        /// <returns>true if success, false if failed</returns>
        public virtual bool TrySetValue<T>(PathMember pathMember, MemberInfo memberInfo, object sourceObject, T value)
        {
            object atObject = null;

            // Get the atValue from MemberInfo
            if (memberInfo != null)
                atObject = memberInfo.GetValue(sourceObject);

            else if (pathMember != null)
                atObject = sourceObject.Induct(pathMember).ReturnValue;

            // Can not the atValue. 
            else
                throw ThrowConvertExceptionOnSet(pathMember, memberInfo, value);

            // Set 
            return TrySetValueInner<T>(pathMember, memberInfo, atObject, value);
        }

        /// <summary>
        /// value to model
        /// </summary>
        /// <typeparam name="T">value type</typeparam>
        /// <param name="pathMember">Path member.</param>
        /// <param name="memberInfo">Member infomation.</param>
        /// <param name="atObject">to Object</param>
        /// <param name="value">value</param>
        /// <returns>true if success, false if failed</returns>
        public virtual bool TrySetValueInner<T>(PathMember pathMember, MemberInfo memberInfo, object atObject, T value) =>
            throw new NotImplementedException();

        /// <summary>
        /// Model to new value
        /// </summary>
        /// <typeparam name="T">new value type</typeparam>
        /// <param name="memberInfo">Member infomation</param>
        /// <param name="sourceObject">from object</param>
        /// <param name="value">new value</param>
        /// <returns>true if success, false if failed</returns>
        public virtual bool TryCreateValue<T>(PathMember pathMember, MemberInfo memberInfo, object sourceObject, out T value) =>
            TryCreateValueInner<T>(pathMember, memberInfo, memberInfo.GetValue(sourceObject), out value);

        /// <summary>
        /// Model to new value
        /// </summary>
        /// <typeparam name="T">new value type</typeparam>
        /// <param name="pathMember">Path member.</param>
        /// <param name="memberInfo">Member infomation</param>
        /// <param name="atObject">from object</param>
        /// <param name="value">new value</param>
        /// <returns>true if success, false if failed</returns>
        public virtual bool TryCreateValueInner<T>(PathMember pathMember, MemberInfo memberInfo, object atObject, out T value) =>
            throw new NotImplementedException();

        public virtual bool TryContains(PathMember pathMember, object atObject, object value, out bool bIncluded) =>
            throw new NotImplementedException();

        /// <summary>
        /// Get linked value
        /// </summary>
        /// <param name="atObject"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public virtual bool TryGetLink(object atObject, Type atType, out object destineValue, out Type destineType)
        {
            destineValue = null;
            destineType = null;
            return false;
        }

        /// <summary>
        /// Throw convert exception on Import Method.
        /// </summary>
        /// <param name="pathMember">Path member.</param>
        /// <param name="memberInfo">Target model member infomation</param>
        /// <param name="value">Import value</param>
        /// <exception cref="InvalidConvertException"></exception>
        public virtual Exception ThrowConvertExceptionOnSet(PathMember pathMember, MemberInfo memberInfo, object value) =>
            new InvalidConvertException($"Other:{memberInfo?.Name}[{memberInfo?.GetReturnType()}]({value}) to Model:{memberInfo} converting error. on {GetType()}");

        /// <summary>
        /// Throw convert exception on Export Method.
        /// </summary>
        /// <param name="pathMember">Path member.</param>
        /// <param name="memberInfo">Target model member infomation</param>
        /// <exception cref="InvalidConvertException"></exception>
        public virtual Exception ThrowConvertExceptionOnGet(PathMember pathMember, MemberInfo memberInfo) =>
            new InvalidConvertException($"Model:{memberInfo} to Other:{memberInfo?.Name}[{memberInfo?.GetReturnType()}] converting error. on {GetType()}");
    }
}
