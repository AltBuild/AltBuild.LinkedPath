using AltBuild.LinkedPath.Parser;
using System;
using System.Collections;
using System.Collections.Generic;

namespace AltBuild.LinkedPath.Converters
{
    /// <summary>
    /// Default indexer converter class
    /// 1)Key and Value
    ///   (CASE1: Key and Value -> Items[Id=[1,2,3]] )
    ///   
    /// 2) Value(s)
    ///   (CASE2: Value -> Items[=SelectItems()] ) ... Specifying a method)
    ///   (CASE3: Value -> Items[=SelectItems]   ) ... Specifying properties)
    ///   
    /// </summary>
    public class DefaultIndexerConverter : IndexerConverterBase
    {
        /// <summary>
        /// Accept KVType ( None is All accepts )
        /// </summary>
        public override PathKeyValueType KVType { get; } = PathKeyValueType.None;

        public override bool TryConvert(PathMember pathMember, object source, out object destineValue, out Type destineType)
        {
            // Result values
            List<object> results = new();

            int atIndex = -1;
            int? directIndex = pathMember.Indexers.Index;

            // Target: list type
            //   object[] source = new object[] { 1, 2, 3, 4 };
            //   object source = new List<int>(new int[]{ 1, 2, 3, 4, 5}); 
            if (source is IList list)
            {
                if (directIndex.HasValue)
                {
                    if (list.Count > directIndex.Value && directIndex.Value >= 0)
                    {
                        destineValue = list[directIndex.Value];
                        destineType = destineValue?.GetType();
                    }

                    else
                        destineValue = null;

                }
                else
                {
                    foreach (var item in list)
                        if (GetMatchItem(item))
                            break;

                    destineValue = GetDestine(results);
                }

                destineType = destineValue?.GetType();
                return true;
            }

            // Target: enum type
            else if (source is IEnumerable enumerable)
            {
                foreach (var item in enumerable)
                    if (GetMatchItem(item))
                        break;

                destineValue = GetDestine(results);
                destineType = destineValue?.GetType();
                return true;
            }

            // etc type
            else
            {
                GetMatchItem(source);

                destineValue = GetDestine(results);
                destineType = destineValue?.GetType();
                return true;
            }

            object GetDestine(List<object> results)
            {
                // not found
                if (results.Count == 0)
                    return null;

                // Select value
                else if (results.Count == 1)
                    return results[0];

                // Multiple values
                return results.ToArray();
            }

            bool GetMatchItem(object item)
            {
                // Increment
                atIndex++;

                // Direct Index
                if (directIndex.HasValue)
                {
                    if (directIndex.Value == atIndex)
                    {
                        results.Add(item);
                        return true;
                    }
                }

                // Matching
                else if (item is IPathMatch match)
                {
                    foreach (var indexer in pathMember.Indexers)
                        if (match.TryMatch(indexer) == false)
                            return false;

                    results.Add(item);
                }

                // Next.
                return false;
            }
        }
    }
}
