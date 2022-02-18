using AltBuild.LinkedPath;
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Reflection;

namespace AltBuild.LinkedPath
{
    /// <summary>
    /// Indexer converter factory.
    /// </summary>
    public class IndexerConverterFactory
    {
        static readonly IndexerConverterAttribute[] _DefaultIndecerConverter = new[] {
            new IndexerConverterAttribute(typeof(DefaultKeysIndexerConverter)),         // Keys only       :Items[1,2,3]
            new IndexerConverterAttribute(typeof(DefaultKeyAndValueIndexerConverter)),  // Key and Value   :Items[Type.Id=[1,2,3]]
            new IndexerConverterAttribute(typeof(DefaultReturnValuesIndexerConverter))  // Return Value    :Items[=SelectItems()]
        };

        /// <summary>
        /// Cache per type.
        /// </summary>
        static ConcurrentDictionary<Type, IndexerConverterBase> _converters = new();

        public static IndexerConverterBase[] Get(MemberInfo memberInfo, PathIndexerCollection pathIndexers)
        {
            // Target KVType.
            var kvType = pathIndexers.KVType;

            // Get target Conveters.
            IndexerConverterAttribute[] converterAttrs = GetConverter(memberInfo);

            // KeyAndValue will use the default converter.
            if (converterAttrs == null)
                converterAttrs = _DefaultIndecerConverter;

            // Results.
            var results = new List<IndexerConverterBase>();

            // Search KVType.
            for (int i = 0; i < converterAttrs.Length; i++)
            {
                var atConverter = _converters.GetOrAdd(converterAttrs[i].ConverterType, o => Activator.CreateInstance(o) as IndexerConverterBase);
                if (atConverter.KVType == kvType)
                    results.Add(atConverter);
            }

            return results.ToArray();
        }

        static IndexerConverterAttribute[] GetConverter(MemberInfo memberInfo)
        {
            // Get attribute from member or type attributes.
            var indexerConverterAttributes = AttributeHelper.GetAttributesEx<IndexerConverterAttribute>(memberInfo);
            if (indexerConverterAttributes != null)
                return indexerConverterAttributes;

            return null;
        }
    }
}
