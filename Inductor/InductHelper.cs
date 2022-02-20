using AltBuild.LinkedPath.Converters;
using AltBuild.LinkedPath.Parser;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace AltBuild.LinkedPath
{
    // 関連の補助クラス
    public static class InductHelper
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
        public static void CreateKeyValues(string line, Action<string, string, bool> action)
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
                // Get Operator
                //var ilop = OperatorItem.GetOperation(optionKV);
                int indexOfSplit = optionKV.IndexOf('=');
                if (indexOfSplit < 0)
                {
                    action(optionKV.Trim(), null, false);
                }

                else
                {
                    var key = optionKV.Substring(0, indexOfSplit).Trim();

                    var value = optionKV.Substring(indexOfSplit + 1).Trim();
                    var strValue = value.Trim('\'', '\"');
                    bool bString = (strValue.Length + 2) == value.Length;

                    if (string.IsNullOrEmpty(key))
                        action(null, strValue, bString);

                    else
                        action(key, strValue, bString);
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

        /// <summary>
        /// Create PathValue.
        /// </summary>
        /// <param name="inductInfo">Out InductInfo.</param>
        /// <param name="sourceObject">Source object.</param>
        /// <param name="sourceType">Source type.</param>
        /// <param name="memberName">TargetMember name</param>
        /// <param name="frame">InductFrame object</param>
        /// <returns>Successful: true, Failed: false</returns>
        public static bool TryGetPathValue(out InductInfo inductInfo, object sourceObject, Type sourceType, string memberName, PathMember pathMember = null, InductFrame frame = null)
        {
            // can't create.
            if (memberName == null || (sourceObject == null && sourceType == null))
            {
                inductInfo = null;
                return false;
            }

            // Try Get inner value.
            if ((frame == null || frame.Methods.HasFlag(InductMethods.NoInductiveConverter) == false) &&
                InductiveConverterFactory.TryGetLink(sourceObject, sourceType, out object destineValue, out Type destineType))
            {
                sourceObject = destineValue;
                sourceType = destineType;
            }
            else
            {
                sourceType = sourceObject?.GetType();
            }

            // Get value
            if (sourceType != null)
            {
                // Get normal members...
                foreach (var memberInfo in sourceType.GetMember(memberName, InductHelper.Flags))
                    if (TryGetTargetPathValue(memberInfo, out inductInfo))
                        return true;

                // Get extension methods...
                foreach (var memberInfo in ExtensionMethods.GetExtensionMethods(sourceType, memberName))
                    if (TryGetTargetPathValue(memberInfo, out inductInfo))
                        return true;
            }

            // failed.
            inductInfo = null;
            return false;

            bool TryGetTargetPathValue(MemberInfo memberInfo, out InductInfo inductInfo)
            {
                if (TryGetValue_with_Indexer(out object returnValue, out Type returnType, memberInfo, sourceObject, pathMember, frame))
                {
                    inductInfo = new InductInfo { Frame = frame, PathMember = pathMember, BaseObject = sourceObject, MemberInfo = memberInfo, ReturnValue = returnValue, ReturnType = returnType };
                    return true;
                }
                
                else
                {
                    inductInfo = null;
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
        public static bool TryGetValue_with_Indexer(out object returnValue, out Type returnType, MemberInfo memberInfo, object source, PathMember pathMember = null, InductFrame frame = null)
        {
            // Get member value.
            if (TryGetValue(out object atReturnValue, out Type atReturnType, memberInfo, source, pathMember, frame))
            {
                // No indexer process.
                if (pathMember == null || pathMember.Indexers.Count == 0 || (pathMember.Child == null && frame != null && frame.Methods.HasFlag(InductMethods.NoLastIndexer)))
                {
                    returnValue = atReturnValue;
                    returnType = atReturnType;
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
                        if (converters.Length > 0)
                        {
                            // Get target object.
                            //var nativeValue = memberInfo.GetValue(source);

                            // Select converter and try convert.
                            foreach (var converter in converters)
                                //if (converter.TryConvert(pathMember, nativeValue, out returnValue, out returnType))
                                if (converter.TryConvert(pathMember, atReturnValue, out returnValue, out returnType))
                                    return true;
                        }
                    }

                    // Failed indexer processing.
                    returnValue = null;
                    returnType = null;
                    return false;
                }
            }

            // Failed, get member value.
            else
            {
                returnValue = null;
                returnType = null;
                return false;
            }
        }

        public static bool TryGetValue(out object returnValue, out Type returnType, MemberInfo memberInfo, object sourceObject, PathMember pathMember, InductFrame frame)
        {
            switch (memberInfo.MemberType)
            {
                // case Property
                case MemberTypes.Property:
                    if (memberInfo is PropertyInfo propertyInfo)
                    {
                        if (sourceObject != null)
                        {
                            returnValue = propertyInfo.GetValue(sourceObject);
                            returnType = returnValue?.GetType() ?? propertyInfo.PropertyType;
                        }
                        else
                        {
                            returnValue = null;
                            returnType = propertyInfo.PropertyType;
                        }
                        return true;
                    }
                    break;

                // case Field
                case MemberTypes.Field:
                    if (memberInfo is FieldInfo fieldInfo)
                    {
                        if (sourceObject != null)
                        {
                            returnValue = fieldInfo.GetValue(sourceObject);
                            returnType = returnValue?.GetType() ?? fieldInfo.FieldType;
                        }
                        else
                        {
                            returnValue = null;
                            returnType = fieldInfo.FieldType;
                        }
                        return true;
                    }
                    break;

                // case Method
                case MemberTypes.Method:
                    if (memberInfo is MethodInfo methodInfo)
                    {
                        // Case: source != null 
                        if (sourceObject != null)
                        {
                            // パラメータ
                            object[] parameters = null;

                            // Get arguments
                            var arguments = pathMember?.Arguments;
                            if (arguments == null || arguments.IsEmpty)
                            {
                                ParameterInfo[] parametersInfo = methodInfo.GetParameters();
                                if (parametersInfo.Length > 0)
                                    break;
                            }

                            // 拡張メソッド処理
                            else if (memberInfo.GetCustomAttribute<ExtensionAttribute>() != null)
                            {
                                ParameterInfo[] parametersInfo = methodInfo.GetParameters();
                                if (arguments.Count != parametersInfo.Length - 1 || arguments.TryParametersExtension(sourceObject, parametersInfo, out parameters) == false)
                                    break;
                            }

                            // 通常メソッド処理
                            else
                            {
                                ParameterInfo[] parametersInfo = methodInfo.GetParameters();
                                if (arguments.Count != parametersInfo.Length || arguments.TryParameters(parametersInfo, out parameters) == false)
                                    break;
                            }

                            // Call Method (return: void)
                            returnType = methodInfo.ReturnType;
                            if (returnType.Equals(typeof(void)) || returnType.Equals(typeof(System.Threading.Tasks.Task)))
                            {
                                if (frame != null && frame.Methods.HasFlag(InductMethods.InvokeMethod))
                                    returnValue = methodInfo.Invoke(sourceObject, parameters);

                                else
                                    returnValue = null;

                                return true;
                            }

                            else
                            {
                                try
                                {
                                    returnValue = methodInfo.Invoke(sourceObject, parameters);
                                    return true;
                                }
                                catch (Exception)
                                {
                                    // メソッドの実行失敗
                                }
                            }
                        }

                        // Case: source == null
                        else
                        {
                            returnValue = null;
                            returnType = methodInfo.ReturnType;
                            return true;
                        }
                    }
                    break;
            }

            // unmatch member.
            returnValue = null;
            returnType = null;
            return false;
        }

        /// <summary>
        /// Create DMemberInfo.
        /// </summary>
        /// <param name="obj">Definition object</param>
        /// <param name="memberInfo">Dreated DMemberInfo</param>
        /// <returns>Successful: true, Failed: false</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGetMethod(object obj, string memberName, out MethodInfo methodInfo) =>
            (methodInfo = obj.GetType().GetMethod(memberName, Flags)) != null;


        ///// <summary>
        ///// Create DObjectMember.
        ///// </summary>
        ///// <param name="value">Dreated DObjectMember</param>
        ///// <param name="source">Definition object</param>
        ///// <param name="memberName">Member Name</param>
        ///// <returns>Successful: true, Failed: false</returns>
        //public static bool TrySetValue<T>(T value, [NotNull]object source, [NotNull]string memberName)
        //{
        //    // get target menbers
        //    var staticMemberInfos = source.GetType().GetMember(memberName, InductHelper.Flags);

        //    // creating...
        //    foreach (var staticMemberInfo in staticMemberInfos)
        //        if (TrySetValue<T>(value, source, staticMemberInfo))
        //            return true;

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
        //public static bool TrySetValue<T>(T value, object source, MemberInfo memberInfo)
        //{
        //    switch (memberInfo.MemberType)
        //    {
        //        // case Property
        //        case MemberTypes.Property:
        //            if (memberInfo is PropertyInfo propertyInfo && propertyInfo.PropertyType.Equals(typeof(T)))
        //            {
        //                propertyInfo.SetValue(source, value);
        //                return true;
        //            }
        //            break;

        //        // case Field
        //        case MemberTypes.Field:
        //            if (memberInfo is FieldInfo fieldInfo && fieldInfo.FieldType.Equals(typeof(T)))
        //            {
        //                fieldInfo.SetValue(source, value);
        //                return true;
        //            }
        //            break;

        //        // case Method
        //        case MemberTypes.Method:
        //            throw new InvalidProgramException("Member type miss match. (SetValue)");
        //    }
        //    return false;
        //}
    }
}
