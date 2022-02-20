using System;

namespace AltBuild.LinkedPath.Converters
{
    /// <summary>
    /// Indexer converter attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Struct, AllowMultiple = true)]
    public class IndexerConverterAttribute : ConverterBaseAttribute
    {
        public IndexerConverterAttribute(Type converterType) : base(converterType)
        {
        }
    }
}
