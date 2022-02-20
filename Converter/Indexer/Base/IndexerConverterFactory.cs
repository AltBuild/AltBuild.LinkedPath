using AltBuild.LinkedPath.Extensions;
using AltBuild.LinkedPath.Parser;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;

namespace AltBuild.LinkedPath.Converters
{
    /// <summary>
    /// Indexer converter factory.
    /// </summary>
    public class IndexerConverterFactory
    {
        public static IndexerConverterBase DefaultIndexerConverter { get; } = new DefaultIndexerConverter();

        // Keys only       :Items[1,2,3]
        // Key and Value   :Items[Type.Id=[1,2,3]]
        // Return Value    :Items[=SelectItems()]
        static readonly IndexerConverterBase[] _defaultIndecerConverters = new[]
        {
            DefaultIndexerConverter
        };

        /// <summary>
        /// Cache per type.
        /// </summary>
        static ConcurrentDictionary<Type, IndexerConverterBase> _converters = new();

        public static IndexerConverterBase[] Get(MemberInfo memberInfo, PathIndexerCollection pathIndexers)
        {
            // Get target Conveters.
            if (AttributeExtensions.TryGetAttributesEx(memberInfo, out IndexerConverterAttribute[] converterAttrs))
            {
                // Target KVType.
                var kvType = pathIndexers.KVType;

                // Results.
                var results = new List<IndexerConverterBase>();

                // Search KVType.
                for (int i = 0; i < converterAttrs.Length; i++)
                {
                    var atConverter = _converters.GetOrAdd(converterAttrs[i].ConverterType, o => Activator.CreateInstance(o) as IndexerConverterBase);
                    if (atConverter.KVType == PathKeyValueType.None || atConverter.KVType == kvType)
                        results.Add(atConverter);
                }

                return results.ToArray();
            }

            // Get the default converter.
            else
            {
                return _defaultIndecerConverters;
            }
        }
    }
}
