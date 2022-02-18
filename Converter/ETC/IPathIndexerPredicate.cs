using System;

namespace AltBuild.LinkedPath
{
    /// <summary>
    /// Path indexer predicate
    /// </summary>
    public interface IPathIndexerPredicate
    {
        Predicate<object> GetPredicate(PathIndexerCollection indexers, object keyNativeObject, object source);
    }
}
