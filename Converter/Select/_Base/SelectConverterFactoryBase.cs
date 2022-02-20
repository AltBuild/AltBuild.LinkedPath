using AltBuild.LinkedPath;
using AltBuild.LinkedPath.Extensions;
using AltBuild.LinkedPath.Parser;
using System;
using System.Collections.Concurrent;
using System.Reflection;
using System.Diagnostics.CodeAnalysis;
using System.Collections.Generic;

namespace AltBuild.LinkedPath.Converters
{
    /// <summary>
    /// DatastoreModel converter factory.
    /// </summary>
    public class SelectConverterFactoryBase
    {
        /// <summary>
        /// Cache per include conveter instance
        /// </summary>
        protected static List<SelectConverterBase> _includeConveters = new();

        /// <summary>
        /// Cache per converter instance
        /// </summary>
        static ConcurrentDictionary<Type, SelectConverterBase> _converters = new();

        static ConcurrentDictionary<Type, SelectConverterBase> _createSelectConveters = new();

        static ConcurrentDictionary<Type, SelectConverterBase> _cludeConverters = new();

        public static void IncludeConverter(params SelectConverterBase[] selectConverters) =>
            _includeConveters.AddRange(selectConverters);

        static (bool bSuccess, TValue value) TryConverter<TValue>(ISelectRequirementsBase requirements, ConcurrentDictionary<Type, SelectConverterBase> cacheConverters, Func<SelectConverterBase, (bool bSuccess, TValue value)> func)
        {
            // Get Converter selection type
            //  1. Generic type -> GenericTypeDefinition
            //  2. Other type -> Native member type
            Type type = requirements.InductInfo?.ReturnType;
            if (type.IsGenericType)
                type = type.GetGenericTypeDefinition();

            // Get converter from cache.
            if (cacheConverters.TryGetValue(type, out SelectConverterBase converter))
            {
                if (converter != null)
                    return func(converter);
            }

            // Search converter from attributes infomation.
            else
            {
                var memberInfo = requirements.InductInfo?.MemberInfo;
                foreach (var atConverter in Get(memberInfo))
                    if (TryConverter(atConverter, out (bool bSuccess, TValue value) results))
                        return results;
            }

            // DefaultConverter
            foreach (var atConverter in _includeConveters)
                if (TryConverter(atConverter, out (bool bSuccess, TValue value) results))
                    return results;

            // Converter not found.
            return (false, default);

            bool TryConverter(SelectConverterBase converter, out (bool bSuccess, TValue value) results)
            {
                results = func(converter);
                if (results.bSuccess)
                    cacheConverters.TryAdd(type, converter);

                return results.bSuccess;
            }
        }

        public static bool TryCreateSelects(ISelectRequirementsBase requirements, [MaybeNullWhen(false)] out ISelectItemsBase selects)
        {
            var results = TryConverter(requirements, _createSelectConveters, c =>
                (c.TryGetSelectItems(requirements, out ISelectItemsBase items), items));

            selects = results.value;
            return results.bSuccess;
        }

        public static bool TrySelect(ISelectRequirementsBase requirements, object value)
        {
            var results = TryConverter(requirements, _cludeConverters, c =>
                (c.TrySelect(requirements, value), true));

            return results.bSuccess;
        }

        public static bool TryInclude(ISelectRequirementsBase requirements, object value)
        {
            var results = TryConverter(requirements, _cludeConverters, c =>
                (c.TryInclude(requirements, value), true));

            return results.bSuccess;
        }

        public static bool TryExclude(ISelectRequirementsBase requirements, object value)
        {
            var results = TryConverter(requirements, _cludeConverters, c =>
                (c.TryExclude(requirements, value), true));

            return results.bSuccess;
        }

        public static SelectConverterBase[] Get(MemberInfo memberInfo)
        {
            if (AttributeExtensions.TryGetAttributesEx(memberInfo, out SelectConverterAttribute[] converterAttrs))
            {
                var results = new SelectConverterBase[converterAttrs.Length];

                for (int i = 0; i < converterAttrs.Length; i++)
                    results[i] = _converters.GetOrAdd(converterAttrs[i].ConverterType, o => Activator.CreateInstance(o) as SelectConverterBase);

                return results;
            }

            else
            {
                return Array.Empty<SelectConverterBase>();
            }
        }
    }
}
