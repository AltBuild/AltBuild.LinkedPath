using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Runtime.CompilerServices;
using System.Reflection;

namespace AltBuild.LinkedPath
{
    /// <summary>
    /// Default indexer converter class
    /// (CASE1: Value -> Items[=SelectItems()] ) ... Specifying a method)
    /// (CASE1: Value -> Items[=SelectItems]   ) ... Specifying properties)
    /// </summary>
    public class DefaultReturnValuesIndexerConverter : IndexerConverterBase
    {
        /// <summary>
        /// Accept KVType ( None is All accepts )
        /// </summary>
        public override PathKeyValueType KVType { get; } = PathKeyValueType.Value;

        public override bool TryConvert(PathMember pathMember, object source, out object destine)
        {
            // Target: enum type
            if (source is IEnumerable enumerable)
            {
                List<object> results = new();

                foreach (var item in enumerable)
                    if (GetMatchItem(item))
                        results.Add(item);

                destine = results.ToArray();
                return true;
            }

            // etc type
            else
            {
                if (GetMatchItem(source))
                {
                    destine = source;
                    return true;
                }
            }

            destine = null;
            return false;

            bool GetMatchItem(object item)
            {
                if (item is IPathMatch match)
                {
                    foreach (var indexer in pathMember.Indexers)
                        if (match.TryMatch(indexer) == false)
                            return false;

                    return true;
                }
                return false;
            }
        }
    }
}
