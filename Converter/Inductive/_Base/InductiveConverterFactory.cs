using AltBuild.LinkedPath.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Reflection;
using System.Security.AccessControl;

namespace AltBuild.LinkedPath.Converters
{
    /// <summary>
    /// Model member converter factory.
    /// </summary>
    public static class InductiveConverterFactory
    {
        /// <summary>
        /// Holds default converter list
        /// </summary>
        static readonly InductiveConverterBase _defaultConverter = new DefaultInductiveConverter();

        /// <summary>
        /// Cache per converter instance
        /// </summary>
        static ConcurrentDictionary<Type, InductiveConverterBase> _converterTypes = new();

        /// <summary>
        /// Cache per target member use converter instance
        /// </summary>
        static ConcurrentDictionary<MemberInfo, InductiveConverters> _converterMembers = new();

        static ConcurrentDictionary<Type, InductiveConverterBase> _linkConverterBase = new();

        /// <summary>
        /// Cache per include conveter instance
        /// </summary>
        static List<InductiveConverterBase> _includeConveters = new();

        /// <summary>
        /// Get link value
        /// </summary>
        /// <param name="source">Source object</param>
        /// <param name="destine">Link value</param>
        /// <returns>True: Success, False: Failed</returns>
        public static bool TryGetLink(object sourceObject, Type sourceType, out object destineValue, out Type destineType)
        {
            // Get target type
            if (sourceType == null)
                sourceType = sourceObject?.GetType();

            if (sourceType != null)
            {
                // Get Link value
                if (_linkConverterBase.TryGetValue(sourceType, out InductiveConverterBase converter))
                {
                    if (converter != null)
                        return converter.TryGetLink(sourceObject, sourceType, out destineValue, out destineType);

                    destineType = sourceType;
                    destineValue = sourceObject;
                    return false;
                }

                // Search Converter and Link value.
                if (GetConverters(sourceType).TryGetLink(sourceObject, sourceType, out InductiveConverterBase conveter, out destineValue, out destineType))
                {
                    _linkConverterBase[sourceType] = conveter;
                    return true;
                }
                else
                {
                    _linkConverterBase[sourceType] = null;
                    destineValue = sourceObject;
                    return false;
                }
            }

            // Failed.
            destineType = sourceType;
            destineValue = null;
            return false;
        }

        public static void Include(params InductiveConverterBase[] inductiveConverters) =>
            _includeConveters.AddRange(inductiveConverters);

        /// <summary>
        /// Get type converter
        /// </summary>
        /// <typeparam name="T">Value type</typeparam>
        /// <param name="memberInfo">Target member infomation</param>
        /// <param name="converter">Target converter</param>
        /// <returns>True: Successful. False: Failed.</returns>
        public static bool TryGetConverter<T>(MemberInfo memberInfo, out InductiveConverterBase converter) =>
            TryGetConverter(typeof(T), memberInfo, out converter);

        /// <summary>
        /// Get type converter
        /// </summary>
        /// <typeparam name="T">Value type</typeparam>
        /// <param name="type">To type.</param>
        /// <param name="memberInfo">Target member infomation</param>
        /// <param name="converter">Target converter</param>
        /// <returns>True: Successful. False: Failed.</returns>
        public static bool TryGetConverter(Type type, MemberInfo memberInfo, out InductiveConverterBase converter)
        {
            // If member type and value match, returns default converter
            var memberType = memberInfo.GetReturnType();
            if (memberType.Equals(type) || type.Equals(Nullable.GetUnderlyingType(memberType)))
            {
                converter = _defaultConverter;
                return true;
            }

            // Returns processable converters
            else
            {
                // Get converter list
                var converters = _converterMembers.GetOrAdd(memberInfo, (m) => GetConverters(m));

                // Get a matching converter
                if (converters.TryGetConverter(type, memberInfo, out converter))
                {
                    return true;
                }

                // Include conveter ...
                if (_includeConveters.Count > 0)
                {
                    foreach (var atIncludeConveter in _includeConveters)
                    {
                        if (atIncludeConveter.CanConvert(type, null, memberInfo))
                        {
                            converter = atIncludeConveter;
                            return true;
                        }
                    }
                }

                // To string processing ...
                if (type.Equals(typeof(string)))
                {
                    converter = _defaultConverter;
                    return true;
                }
            }

            return (converter != null);
        }

        static InductiveConverters GetConverters(MemberInfo memberInfo)
        {
            // Get target converters
            if (AttributeExtensions.TryGetAttributesEx(memberInfo, out InductiveConverterAttribute[] converterAttrs))
            {
                // Create converter list
                var results = new InductiveConverterBase[converterAttrs.Length];

                for (int i = 0; i < converterAttrs.Length; i++)
                    results[i] = _converterTypes.GetOrAdd(converterAttrs[i].ConverterType, o => Activator.CreateInstance(o) as InductiveConverterBase);

                return new InductiveConverters(results);
            }

            else
            {
                return new InductiveConverters();
            }
        }
    }
}
