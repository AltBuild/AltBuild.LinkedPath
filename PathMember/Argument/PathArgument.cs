using AltBuild.BaseExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace AltBuild.LinkedPath
{
    /// <summary>
    /// メソッド専用の１つの引数を表現
    /// </summary>
    public class PathArgument : PathKeyValue
    {
        public PathArgumentCollection Parent { get; init; }

        /// <summary>
        /// キーと値をセット
        /// </summary>
        /// <param name="key"></param>
        /// <param name="stringValue"></param>
        public PathArgument(string key, string stringValue) : base(key, stringValue)
        {
        }
    }
}
