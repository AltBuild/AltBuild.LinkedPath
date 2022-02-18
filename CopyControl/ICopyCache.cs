using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;

namespace AltBuild.LinkedPath
{
    public interface ICopyCache
    {
        /// <summary>
        ///  指定の型とIDを持つインスタンスを取得する
        /// </summary>
        /// <param name="type">生成タイプ</param>
        /// <param name="id">生成ID</param>
        /// <param name="value">生成インスタンス</param>
        /// <returns>false: get cache, true: get create</returns>
        bool TryGetOrCreate(Type type, object id, out object value);

        /// <summary>
        ///  インスタンスを取得する
        /// </summary>
        /// <param name="source">生成元</param>
        /// <param name="value">生成先</param>
        /// <returns>false: get cache, true: get create</returns>
        bool TryGetOrCreate(object source, out object value);

        /// <summary>
        ///  インスタンスを取得する
        /// </summary>
        /// <param name="source">生成元</param>
        /// <param name="func">  </param>
        /// <param name="value">生成先</param>
        /// <returns></returns>
        bool TryGetOrCreate(object source, Func<object> func, out object value);
    }
}
