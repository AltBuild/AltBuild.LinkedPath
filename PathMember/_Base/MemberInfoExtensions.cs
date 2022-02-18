using AltBuild.BaseExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Diagnostics;

namespace AltBuild.LinkedPath
{
    /// <summary>
    /// メンバー情報拡張
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
        ///  メンバーが扱う型を取得する
        /// </summary>
        /// <param name="memberInfo"></param>
        /// <returns></returns>
        public static Type GetReturnType(this MemberInfo memberInfo)
        {
            if (memberInfo != null)
            {
                switch (memberInfo.MemberType)
                {
                    case MemberTypes.Property:
                        return (memberInfo is PropertyInfo propertyInfo) && propertyInfo.CanRead ? propertyInfo.PropertyType : null;

                    case MemberTypes.Method:
                        return (memberInfo as MethodInfo).ReturnType;

                    case MemberTypes.Field:
                        return (memberInfo as FieldInfo).FieldType;
                }
            }
            return null;
        }

        public static object GetNativeValue(this MemberInfo memberInfo, object obj, PathMember pathMember = null)
        {
            if (PathHelper.TryGetMemberValue(out object returnValue, memberInfo, obj, pathMember))
                return returnValue;

            else
                return null;
        }

        public static void SetNativeValue(this MemberInfo memberInfo, object obj, object value)
        {
            Debug.Assert(memberInfo != null, "MemberInfo is null error.");

            if (memberInfo.MemberType == MemberTypes.Property)
                (memberInfo as PropertyInfo).SetValue(obj, value);

            else if (memberInfo.MemberType == MemberTypes.Field)
                (memberInfo as FieldInfo).SetValue(obj, value);

            // この処理は無くても良いが…
            // TODO: ↓これでよいか？
            else if (memberInfo.MemberType == MemberTypes.Method)
                (memberInfo as MethodInfo).Invoke(obj, value as object[]);

            //throw new InvalidProgramException("Method set Not supported.");
        }

        /// <summary>
        ///  メンバー値のコピー（IObjectCopy に対応）
        /// </summary>
        /// <param name="memberInfo"></param>
        /// <param name="srcObject"></param>
        /// <param name="dstObject"></param>
        /// <param name="withEntity"></param>
        /// <returns></returns>
        public static void Copy(this MemberInfo memberInfo, object srcObject, object dstObject, ICopyControl control)
        {
            // ソース側の値を取得
            if (PathHelper.TryGetMemberValue(out object srcValue, memberInfo, srcObject))
            {
                // ソース値が IObjectCopy を保持していれば 委譲
                if (srcValue is IObjectCopy srcValueWithObjectCopy)
                {
                    // デスティン側も IObjectCopy を保持していれば コピー
                    if (PathHelper.TryGetMemberValue(out object dstValue, memberInfo, dstObject))
                    {
                        // Copy.
                        if (dstValue is IObjectCopy dstValueWithObjectCopy)
                            srcValueWithObjectCopy.Copy(dstValueWithObjectCopy, control, memberInfo);

                        else
                            throw new InvalidProgramException("Copy type miss match.");
                    }
                    else
                        throw new InvalidProgramException("Copy member miss match.");
                }

                // その他の場合（IObjectCopy が無ければ）は単純コピーする
                else
                    PathHelper.TrySetValue(memberInfo, dstObject, srcValue);
            }
        }
    }
}
