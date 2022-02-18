using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Runtime.CompilerServices;
using System.Reflection;

namespace AltBuild.LinkedPath
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

        public virtual bool TryConvert(PathMember pathMember, object source, out object destine)
        {
            throw new NotImplementedException();
        }
    }
}
