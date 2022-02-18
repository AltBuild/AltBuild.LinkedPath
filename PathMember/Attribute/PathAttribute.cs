using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace AltBuild.LinkedPath
{
    /// <summary>
    /// 引数情報
    /// </summary>
    public class PathAttribute : PathKeyValue
    {
        /// <summary>
        /// 複数の引数の親を保持
        /// </summary>
        public PathAttributeCollection Parent { get; init; }

        /// <summary>
        /// キーと値をセット
        /// </summary>
        /// <param name="key"></param>
        /// <param name="stringValue"></param>
        public PathAttribute(string key, string stringValue) : base(key, stringValue)
        {
        }
    }
}
