using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace AltBuild.LinkedPath
{
    /// <summary>
    /// Indexer processing helper class.
    /// </summary>
    public class PathIndexerPredicateBase : IPathIndexerPredicate
    {
        /// <summary>
        /// Interface of indexer processing.
        /// </summary>
        public static IPathIndexerPredicate Predicate
        {
            get => _predicate ??= new PathIndexerPredicateBase();
            set => _predicate = value;
        }
        static IPathIndexerPredicate _predicate;

        /// <summary>
        /// デフォルト処理
        /// </summary>
        /// <param name="indexers"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public Predicate<object> GetPredicate(PathIndexerCollection indexers, object keyNativeObject, object source)
        {
            Predicate<object> predicate = o => true;
            return predicate;
        }
    }
}
