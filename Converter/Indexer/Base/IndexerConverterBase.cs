using AltBuild.LinkedPath.Parser;
using System;

namespace AltBuild.LinkedPath.Converters
{
    /// <summary>
    /// Indexer converter basis class
    /// </summary>
    public class IndexerConverterBase : IIndexerConverter
    {
        /// <summary>
        /// Accept KVType ( None is All accepts )
        /// </summary>
        public virtual PathKeyValueType KVType { get; } = PathKeyValueType.None;

        public virtual bool TryConvert(PathMember pathMember, object source, out object destineValue, out Type destineType)
        {
            throw new NotImplementedException();
        }

        public virtual bool TryContains(PathMember pathMember, object source)
        {
            throw new NotImplementedException();
        }
    }
}
