using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Runtime.CompilerServices;
using System.Reflection;

namespace AltBuild.LinkedPath
{
    /// <summary>
    /// Default indexer converter class
    /// </summary>
    public class BasisArrayIndexerConverter : IndexerConverterBase
    {
        /// <summary>
        /// Accept KVType
        /// ( None is All accepts )
        /// </summary>
        public override PathKeyValueType KVType { get; } = PathKeyValueType.Key;

        public override bool TryConvert(PathMember pathMember, object source, out object destine)
        {
            destine = source;
            return true;
        }
    }
}
