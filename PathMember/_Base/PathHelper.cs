using AltBuild.BaseExtensions;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Runtime.CompilerServices;
using System.Diagnostics.CodeAnalysis;

namespace AltBuild.LinkedPath
{
    // DPath*** 関連の補助クラス
    public static class PathHelper
    {
        /// <summary>
        /// 処理対象となるバインディングフラグ
        /// </summary>
        ///                                   ↓ ToString() 拡張とかに InvokeMethod が必要
        public const BindingFlags Flags = BindingFlags.InvokeMethod | BindingFlags.FlattenHierarchy | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance;

        /// <summary>
        /// 属性を追加する
        /// </summary>
        /// <param name="line"></param>
        /// <param name="action"></param>
        public static void CreateKeyValues(string line, Action<string, string> action)
        {
            int indexOfIn = 0;
            char inChar = '\0';

            for (int i = 0; i < line.Length; i++)
            {
                var c = line[i];
                if (inChar != '\0')
                {
                    if (c == inChar)
                        inChar = '\0';

                    continue;
                }

                if (c is '\'' or '\"')
                {
                    inChar = c;
                    continue;
                }
                else if (c == ',')
                {
                    _create2(line.Substring(indexOfIn, i - indexOfIn));
                    indexOfIn = i + 1;
                }
            }

            // 最後の処理
            // line.Length = 0 でも 登録はする（IndexFlagOfPath の為）
            if (line.Length == 0 || indexOfIn < line.Length)
                _create2(line.Substring(indexOfIn));

            void _create2(string optionKV)
            {
                int indexOfSplit = optionKV.IndexOf('=');
                if (indexOfSplit < 0)
                {
                    action(optionKV.Trim(), null);
                }
                else
                {
                    var key = optionKV.Substring(0, indexOfSplit).Trim();
                    var value = optionKV.Substring(indexOfSplit + 1).Trim().Trim('\'', '\"');
                    if (string.IsNullOrEmpty(key))
                        action(null, value);
                    else
                        action(key, value);
                }
            }
        }

        /// <summary>
        /// パラメーターマッチ
        /// 元のパラメーターリスト（source）とメソッドの持つパラメーター情報（parameterInfos）がマッチする場合は、object[] parameters を生成する
        /// </summary>
        /// <param name="parameterInfos"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static bool TryParameters(object[] args, ParameterInfo[] parameterInfos, out object[] parameters)
        {
            // パラメーター無しを生成
            if (args == null || args.Length == 0)
            {
                if (parameterInfos.Length == 0)
                {
                    parameters = new object[0];
                    return true;
                }
            }

            // パラメーター有り
            else if (args.Length == parameterInfos.Length)
            {
                // パラメータ結果
                parameters = new object[args.Length];

                for (int i = 0; i < args.Length; i++)
                {
                    var parameterInfo = parameterInfos[i];
                    if (TypeConverterExtensions.TryGet(parameterInfo.ParameterType, args[i], out object destineValue))
                    {
                        parameters[i] = destineValue;
                    }
                    else
                    {
                        // failed
                        parameters = null;
                        return false;
                    }
                }

                // successful.
                return true;
            }

            parameters = null;
            return false;
        }

        ///// <summary>
        ///// Create DMemberInfo.
        ///// </summary>
        ///// <param name="source">Definition object</param>
        ///// <param name="memberInfo">Dreated DMemberInfo</param>
        ///// <returns>Successful: true, Failed: false</returns>
        //public static bool TryGetReturnType(out Type returnType, object source, string memberName, PathArgumentCollection arguments = null)
        //{
        //    // can't create.
        //    if (source == null || memberName == null)
        //    {
        //        returnType = null;
        //        return false;
        //    }

        //    // get target menbers
        //    var staticMemberInfos = source.GetType().GetMember(memberName, PathHelper.Flags);

        //    // creating...
        //    foreach (var staticMemberInfo in staticMemberInfos)
        //        if (TryGetReturnType(out returnType, staticMemberInfo, source, arguments))
        //            return true;

        //    returnType = null;
        //    return false;
        //}

        ///// <summary>
        ///// メンバーの戻り値を取得する
        ///// </summary>
        ///// <param name="returnType">取得したタイプ</param>
        ///// <param name="memberInfo">メンバー情報</param>
        ///// <param name="source">対象オブジェクト</param>
        ///// <param name="arguments">引数（メソッドの場合）</param>
        ///// <returns></returns>
        //public static bool TryGetReturnType(out Type returnType, MemberInfo memberInfo, object source, PathArgumentCollection arguments = null)
        //{
        //    switch (memberInfo.MemberType)
        //    {
        //        // case Property
        //        case MemberTypes.Property:
        //            if (memberInfo is PropertyInfo propertyInfo)
        //            {
        //                returnType = propertyInfo.PropertyType;
        //                return true;
        //            }
        //            break;

        //        // case Field
        //        case MemberTypes.Field:
        //            if (memberInfo is FieldInfo fieldInfo)
        //            {
        //                returnType = fieldInfo.FieldType;
        //                return true;
        //            }
        //            break;

        //        // case Method
        //        case MemberTypes.Method:
        //            object[] parameters = null;
        //            if (memberInfo is MethodInfo methodInfo && (arguments == null || arguments.TryParameters(methodInfo.GetParameters(), out parameters)))
        //            {
        //                try
        //                {
        //                    returnType = methodInfo.ReturnType;
        //                    return true;
        //                }
        //                catch (Exception)
        //                {
        //                    // メソッドの実行失敗
        //                }
        //            }
        //            break;
        //    }

        //    // unmatch member.
        //    returnType = null;
        //    return false;
        //}

        ///// <summary>
        ///// Create PathValue.
        ///// </summary>
        ///// <param name="dObjectMember">Dreated DObjectMember</param>
        ///// <param name="source">Definition object</param>
        ///// <param name="memberName">Member Name</param>
        ///// <param name="arguments">Member arguments(Method Only)</param>
        ///// <returns>Successful: true, Failed: false</returns>
        //public static bool TryGetValue<T>(out PathValue<T> dObjectMember, object source, string memberName, PathArgumentCollection arguments = null)
        //{
        //    // can't create.
        //    if (source == null || memberName == null)
        //    {
        //        dObjectMember = null;
        //        return false;
        //    }

        //    // get target menbers
        //    var staticMemberInfos = source.GetType().GetMember(memberName, PathHelper.Flags);

        //    // creating...
        //    foreach (var staticMemberInfo in staticMemberInfos)
        //    {
        //        if (TryGetValue<T>(out T returnValue, staticMemberInfo, source, arguments))
        //        {
        //            dObjectMember = new PathValue<T> { Source = source, MemberInfo = staticMemberInfo, Value = returnValue };
        //            return true;
        //        }
        //    }

        //    dObjectMember = null;
        //    return false;
        //}

        ///// <summary>
        ///// メンバーの値を取得する
        ///// </summary>
        ///// <param name="returnValue">取得した値</param>
        ///// <param name="memberInfo">メンバー情報</param>
        ///// <param name="source">対象オブジェクト</param>
        ///// <param name="arguments">引数（メソッドの場合）</param>
        ///// <returns></returns>
        //public static bool TryGetValue<T>(out T returnValue, MemberInfo memberInfo, object source, PathArgumentCollection arguments = null)
        //{
        //    switch (memberInfo.MemberType)
        //    {
        //        // case Property
        //        case MemberTypes.Property:
        //            if (memberInfo is PropertyInfo propertyInfo && propertyInfo.PropertyType.Equals(typeof(T)))
        //            {
        //                returnValue = (T)propertyInfo.GetValue(source);
        //                return true;
        //            }
        //            break;

        //        // case Field
        //        case MemberTypes.Field:
        //            if (memberInfo is FieldInfo fieldInfo &&
        //                fieldInfo.FieldType.Equals(typeof(T)))
        //            {
        //                returnValue = (T)fieldInfo.GetValue(source);
        //                return true;
        //            }
        //            break;

        //        // case Method
        //        case MemberTypes.Method:
        //            if (memberInfo is MethodInfo methodInfo && methodInfo.ReturnType.Equals(typeof(T)))
        //            {
        //                // パラメータ
        //                object[] parameters = null;

        //                // 引数無し
        //                if (arguments == null)
        //                {

        //                }

        //                // 拡張メソッド処理
        //                else if (memberInfo.GetCustomAttribute<ExtensionAttribute>() != null)
        //                {
        //                    arguments.TryParametersExtension(source, methodInfo.GetParameters(), out parameters);
        //                }

        //                // 通常メソッド処理
        //                else
        //                {
        //                    arguments.TryParameters(methodInfo.GetParameters(), out parameters);
        //                }

        //                try
        //                {
        //                    returnValue = (T)methodInfo.Invoke(source, parameters);
        //                    return true;
        //                }
        //                catch (Exception)
        //                {
        //                    // メソッドの実行失敗
        //                }
        //            }
        //            break;
        //    }

        //    // unmatch member.
        //    returnValue = default;
        //    return false;
        //}

        /// <summary>
        /// Create DObjectMember.
        /// </summary>
        /// <param name="dObjectMember">Dreated DObjectMember</param>
        /// <param name="source">Definition object</param>
        /// <param name="memberName">Member Name</param>
        /// <param name="arguments">Member arguments(Method Only)</param>
        /// <returns>Successful: true, Failed: false</returns>
        public static bool TrySetValue<T>(T value, [NotNull]object source, [NotNull]string memberName)
        {
            // get target menbers
            var staticMemberInfos = source.GetType().GetMember(memberName, PathHelper.Flags);

            // creating...
            foreach (var staticMemberInfo in staticMemberInfos)
                if (TrySetValue<T>(value, staticMemberInfo, source))
                    return true;

            return false;
        }

        /// <summary>
        /// メンバーの値を取得する
        /// </summary>
        /// <param name="returnValue">取得した値</param>
        /// <param name="memberInfo">メンバー情報</param>
        /// <param name="source">対象オブジェクト</param>
        /// <param name="arguments">引数（メソッドの場合）</param>
        /// <returns></returns>
        public static bool TrySetValue<T>(T value, MemberInfo memberInfo, object source)
        {
            switch (memberInfo.MemberType)
            {
                // case Property
                case MemberTypes.Property:
                    if (memberInfo is PropertyInfo propertyInfo && propertyInfo.PropertyType.Equals(typeof(T)))
                    {
                        propertyInfo.SetValue(source, value);
                        return true;
                    }
                    break;

                // case Field
                case MemberTypes.Field:
                    if (memberInfo is FieldInfo fieldInfo && fieldInfo.FieldType.Equals(typeof(T)))
                    {
                        fieldInfo.SetValue(source, value);
                        return true;
                    }
                    break;

                // case Method
                case MemberTypes.Method:
                    throw new InvalidProgramException("Member type miss match. (SetValue)");
            }
            return false;
        }

        /// <summary>
        /// Create PathValue.
        /// </summary>
        /// <param name="pathValue">Dreated DObjectMember</param>
        /// <param name="source">Definition object</param>
        /// <param name="memberName">Member Name</param>
        /// <param name="arguments">Member arguments(Method Only)</param>
        /// <returns>Successful: true, Failed: false</returns>
        public static bool TryGetPathValue(out DiveValue pathValue, object source, string memberName, PathMember pathMember = null)
        {
            // can't create.
            if (source == null || memberName == null)
            {
                pathValue = null;
                return false;
            }

            // Get normal members...
            foreach (var staticMemberInfo in source.GetType().GetMember(memberName, PathHelper.Flags))
                if (TryGetTargetPathValue(out pathValue, staticMemberInfo))
                    return true;

            // Get extension methods...
            foreach (var staticMemberInfo in ExtensionMethods.GetExtensionMethods(source, memberName))
                if (TryGetTargetPathValue(out pathValue, staticMemberInfo))
                    return true;

            // failed.
            pathValue = null;
            return false;

            bool TryGetTargetPathValue(out DiveValue pathValue, MemberInfo staticMemberInfo)
            {
                if (TryGetMemberValue(out object returnValue, staticMemberInfo, source, pathMember))
                {
                    pathValue = new DiveValue { Source = source, MemberInfo = staticMemberInfo, Value = returnValue };
                    return true;
                }
                
                else
                {
                    pathValue = null;
                    return false;
                }
            }
        }

        /// <summary>
        /// Get member value with Indexer
        /// </summary>
        /// <param name="returnValue">Result values</param>
        /// <param name="memberInfo">Target memver infomation</param>
        /// <param name="source">Source instance</param>
        /// <param name="pathMember">Target PathMember</param>
        /// <returns>true: successful., false: failed. </returns>
        public static bool TryGetMemberValue(out object returnValue, MemberInfo memberInfo, object source, PathMember pathMember = null)
        {
            // Get member value.
            if (_tryGetMemberValue(out object returnValue1, memberInfo, source, pathMember))
            {
                // TODO: hhh テスト
                //pathMember = null;

                // No indexer process.
                if (pathMember == null || pathMember.Indexers.Count == 0 || pathMember.Frame.Flags.HasFlag(DiveControlFlags.NoIndexer))
                {
                    returnValue = returnValue1;
                    return true;
                }

                // Try Indexer processing.
                else
                {
                    // Get KVType.
                    var kvTypeOfIndexers = pathMember.Indexers.KVType;

                    // Accept processing.
                    if (kvTypeOfIndexers.HasFlag(PathKeyValueType.ComplexError) == false)
                    {
                        // Member custom attribute<IndexerConverter>
                        var converters = IndexerConverterFactory.Get(memberInfo, pathMember.Indexers);

                        // Get target object.
                        var nativeValue = memberInfo.GetNativeValue(source);

                        // Select converter and try convert.
                        foreach (var converter in converters)
                            if (converter.TryConvert(pathMember, nativeValue, out returnValue))
                                return true;
                    }

                    // Failed indexer processing.
                    returnValue = null;
                    return false;
                }
            }

            // Failed, get member value.
            else
            {
                returnValue = null;
                return false;
            }
        }

        static bool _tryGetMemberValue(out object returnValue, MemberInfo memberInfo, object source, PathMember pathMember = null)
        {
            switch (memberInfo.MemberType)
            {
                // case Property
                case MemberTypes.Property:
                    if (memberInfo is PropertyInfo propertyInfo)
                    {
                        returnValue = propertyInfo.GetValue(source);
                        return true;
                    }
                    break;

                // case Field
                case MemberTypes.Field:
                    if (memberInfo is FieldInfo fieldInfo)
                    {
                        returnValue = fieldInfo.GetValue(source);
                        return true;
                    }
                    break;

                // case Method
                case MemberTypes.Method:
                    if (memberInfo is MethodInfo methodInfo && methodInfo.ReturnType.Equals(typeof(void)) == false)
                    {
                        // パラメータ
                        object[] parameters = null;

                        // Get arguments
                        PathArgumentCollection arguments = pathMember?.Arguments;
                        if (arguments == null || arguments.IsEmpty)
                        {

                        }

                        // 拡張メソッド処理
                        else if (memberInfo.GetCustomAttribute<ExtensionAttribute>() != null)
                        {
                            ParameterInfo[] parametersInfo = methodInfo.GetParameters();
                            if (arguments.Count != parametersInfo.Length - 1 || arguments.TryParametersExtension(source, parametersInfo, out parameters) == false)
                                break;
                        }

                        // 通常メソッド処理
                        else
                        {
                            ParameterInfo[] parametersInfo = methodInfo.GetParameters();
                            if (arguments.Count != parametersInfo.Length || arguments.TryParameters(parametersInfo, out parameters) == false)
                                break;
                        }

                        try
                        {
                            returnValue = methodInfo.Invoke(source, parameters);
                            return true;
                        }
                        catch (Exception)
                        {
                            // メソッドの実行失敗
                        }
                    }
                    break;
            }

            // unmatch member.
            returnValue = null;
            return false;
        }

        public static bool TrySetValue(MemberInfo memberInfo, object sourceObject, object destineValue)
        {
            switch (memberInfo.MemberType)
            {
                // case Property
                case MemberTypes.Property:
                    if (memberInfo is PropertyInfo propertyInfo)
                    {
                        propertyInfo.SetValue(sourceObject, destineValue);
                        return true;
                    }
                    break;

                // case Field
                case MemberTypes.Field:
                    if (memberInfo is FieldInfo fieldInfo)
                    {
                        fieldInfo.SetValue(sourceObject, destineValue);
                        return true;
                    }
                    break;

                // case Method
                case MemberTypes.Method:
                    throw new InvalidProgramException("Member type miss match. (SetValue)");
            }

            return false;
        }


        /// <summary>
        /// Create DMemberInfo.
        /// </summary>
        /// <param name="obj">Definition object</param>
        /// <param name="memberInfo">Dreated DMemberInfo</param>
        /// <returns>Successful: true, Failed: false</returns>
        public static MethodInfo GetMethodInfo(object obj, string memberName)
        {
            // get target menbers
            var staticMemberInfos = obj.GetType().GetMethods();

            // creating...
            foreach (var staticMemberInfo in staticMemberInfos)
                if (staticMemberInfo.Name == memberName)
                    return staticMemberInfo;

            return null;
        }

        /// <summary>
        /// メンバー名もしくは
        /// </summary>
        /// <param name="MemberName"></param>
        /// <returns></returns>
        public static DObjectValue GetStringToValue([NotNull]string memberName)
        {
            // 不明
            if (memberName == null || string.IsNullOrWhiteSpace(memberName))
                return DObjectValue.Unknown;

            var stringLower = memberName.ToLower();

            // bool型: false
            if (stringLower == "false" || stringLower == "!true")
                return DObjectValue.False;

            // bool型: true
            if (stringLower == "true" || stringLower == "!false")
                return DObjectValue.True;

            // Value
            if (stringLower[0] is '-' or '.' or (>= '0' and <= '9'))
                return new DObjectValue { Type = DValueType.Value, Value = stringLower };

            // MemberName
            return new DObjectValue { Type = DValueType.MemberName, Value = memberName };
        }
    }
}
