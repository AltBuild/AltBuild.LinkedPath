using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Runtime.CompilerServices;

namespace AltBuild.LinkedPath
{
    /// <summary>
    /// Indexer converter interface
    /// </summary>
    public interface IIndexerConverter
    {
        PathKeyValueType KVType { get; }

        bool TryConvert(PathMember pathMember, object source, out object destine);
    }
}
