using AltBuild.LinkedPath.Parser;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace AltBuild.LinkedPath
{
    /// <summary>
    /// Object member infomation extensions
    /// </summary>
    public static class MemberInfoExtensions
    {
        public static bool CanRead(this MemberInfo memberInfo)
        {
            if (memberInfo != null)
            {
                switch (memberInfo.MemberType)
                {
                    case MemberTypes.Property:
                        return (memberInfo as PropertyInfo).CanRead;

                    case MemberTypes.Method:
                        return (memberInfo as MethodInfo).ReturnType != typeof(void);

                    case MemberTypes.Field:
                        return true;
                }
            }
            return false;
        }

        public static bool CanWrite(this MemberInfo memberInfo)
        {
            if (memberInfo != null)
            {
                switch (memberInfo.MemberType)
                {
                    case MemberTypes.Property:
                        return (memberInfo as PropertyInfo).CanWrite;

                    case MemberTypes.Method:
                        return false;

                    case MemberTypes.Field:
                        return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 指定型のカスタム属性を取得
        /// （インターフェースも指定可能（但し最初に見つかったカスタム属性を返す））
        /// </summary>
        /// <typeparam name="T">型</typeparam>
        /// <param name="attribute">属性</param>
        /// <returns>true: Successful,  false: Failed</returns>
        public static bool TryGetAttribute<T>(this MemberInfo memberInfo, out T attribute)
        {
            attribute = memberInfo != null ? memberInfo.GetAttribute<T>() : default(T);
            return attribute != null;
        }

        /// <summary>
        /// 指定型のカスタム属性を取得
        /// （インターフェースも指定可能（但し最初に見つかったカスタム属性を返す））
        /// </summary>
        /// <typeparam name="T">型</typeparam>
        /// <returns>Success: T, Failed: default(T)</returns>
        public static T GetAttribute<T>(this MemberInfo memberInfo)
        {
            if (memberInfo != null)
            {
                foreach (var attribute in memberInfo.GetCustomAttributes())
                    if (attribute is T reqAttribute)
                        return reqAttribute;
            }

            return default(T);
        }

        /// <summary>
        /// 指定型のカスタム属性を取得
        /// （インターフェースも指定可能（該当タイプを全て取得））
        /// </summary>
        /// <typeparam name="T">型</typeparam>
        /// <returns>Success: T[], Failed: T[0])</returns>
        public static T[] GetAttributes<T>(this MemberInfo memberInfo)
        {
            List<T> results = new();

            if (memberInfo != null)
            {
                foreach (var attribute in memberInfo.GetCustomAttributes())
                    if (attribute is T reqAttribute)
                        results.Add(reqAttribute);
            }

            return results.ToArray();
        }

        /// <summary>
        ///  パスタイプを取得
        /// </summary>
        /// <param name="memberInfo"></param>
        /// <returns>パスタイプを取得。その他は Emptyを返す</returns>
        public static PathType GetPathType(this MemberInfo memberInfo)
        {
            Type returnType = GetReturnType(memberInfo);
            return returnType != null ? new PathType(returnType) : PathType.Empty;
        }

        /// <summary>
        ///  パスタイプを取得
        /// </summary>
        /// <param name="memberInfo"></param>
        /// <returns>パスタイプを取得。その他は Emptyを返す</returns>
        public static PathType GetPathType(this Type type)
        {
            return type != null ? new PathType(type) : PathType.Empty;
        }

        /// <summary>
        ///  メンバーが扱う型を取得する
        /// </summary>
        /// <param name="memberInfo"></param>
        /// <returns></returns>
        public static Type GetReturnType(this MemberInfo memberInfo)
        {
            switch (memberInfo.MemberType)
            {
                case MemberTypes.Property:
                    return (memberInfo is PropertyInfo propertyInfo) && propertyInfo.CanRead ? propertyInfo.PropertyType : null;

                case MemberTypes.Field:
                    return (memberInfo as FieldInfo).FieldType;

                case MemberTypes.Method:
                    return (memberInfo as MethodInfo).ReturnType;

                default:
                    return null;
            }
        }

        /// <summary>
        /// Get first from MemberInfo list.
        /// </summary>
        /// <param name="members"></param>
        /// <returns></returns>
        public static MemberInfo First(this MemberInfo[] members) => (members.Length > 0 ? members[0] : null);

        public static void SetValue(this MemberInfo memberInfo, object sourceObject, object destineValue)
        {
            switch (memberInfo.MemberType)
            {
                // case Property
                case MemberTypes.Property:
                    (memberInfo as PropertyInfo).SetValue(sourceObject, destineValue);
                    break;

                // case Field
                case MemberTypes.Field:
                    (memberInfo as FieldInfo).SetValue(sourceObject, destineValue);
                    break;

                case MemberTypes.Method:
                    (memberInfo as MethodInfo).Invoke(sourceObject, destineValue as object[]);
                    break;

                // case Method
                default:
                    throw new InvalidProgramException("Member handling is not supported. (TrySetValue)");
            }
        }

        public static object GetValue(this MemberInfo memberInfo, object sourceObject)
        {
            switch (memberInfo.MemberType)
            {
                // case Property
                case MemberTypes.Property:
                    return (memberInfo as PropertyInfo).GetValue(sourceObject);

                // case Field
                case MemberTypes.Field:
                    return (memberInfo as FieldInfo).GetValue(sourceObject);

                // case Method
                default:
                    throw new InvalidProgramException("Member handling is not supported. (TryGetValue)");
            }
        }

        public static void SetDefaultValue(this MemberInfo memberInfo, object obj)
        {
            object value = Activator.CreateInstance(memberInfo.GetReturnType());
            SetValue(memberInfo, obj, value);
        }

        /// <summary>
        ///  メンバー値のコピー（IObjectCopy に対応）
        /// </summary>
        /// <param name="memberInfo"></param>
        /// <param name="srcObject"></param>
        /// <param name="dstObject"></param>
        /// <param name="withEntity"></param>
        /// <returns></returns>
        public static void Copy(this MemberInfo memberInfo, object srcObject, object dstObject, IObjectCopyControl control)
        {
            // ソース側の値を取得
            object srcValue = memberInfo.GetValue(srcObject);

            // ソース値が IObjectCopy を保持していれば 委譲
            if (srcValue is IObjectCopy srcValueWithObjectCopy)
            {
                // デスティン側も IObjectCopy を保持していれば コピー
                object dstValue = memberInfo.GetValue(dstObject);

                // Copy.
                if (dstValue is IObjectCopy dstValueWithObjectCopy)
                    srcValueWithObjectCopy.Copy(dstValueWithObjectCopy, control, memberInfo);

                else
                    throw new InvalidProgramException("Copy type miss match.");
            }

            // その他の場合（IObjectCopy が無ければ）は単純コピーする
            else
            {
                memberInfo.SetValue(dstObject, srcValue);
            }
        }
    }
}
