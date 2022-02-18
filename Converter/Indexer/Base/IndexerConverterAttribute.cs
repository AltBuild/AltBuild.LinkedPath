using System;

namespace AltBuild.LinkedPath
{
    /// <summary>
    /// Indexer converter attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Struct, AllowMultiple = true)]
    public class IndexerConverterAttribute : Attribute
    {
        public Type ConverterType { get; init; }

        public IndexerConverterAttribute(Type converterType)
        {
            ConverterType = converterType;
        }
    }
}
