using AltBuild.LinkedPath.Parser;
using System;

namespace AltBuild.LinkedPath.Converters
{
    /// <summary>
    /// Indexer converter interface
    /// </summary>
    public interface IIndexerConverter
    {
        PathKeyValueType KVType { get; }

        bool TryConvert(PathMember pathMember, object source, out object destineValue, out Type destineType);
    }
}
