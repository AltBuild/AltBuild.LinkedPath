using AltBuild.BaseExtensions;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace AltBuild.LinkedPath
{
    /// <summary>
    /// KeyValuePair のリスト
    /// </summary>
    public partial class PathOptionCollection : List<PathOption>
    {
        /// <summary>
        /// この引数の関連パス情報（親）
        /// </summary>
        public PathMember Path { get; init; }

        public bool IsEmpty => Count == 0;

        internal PathOptionCollection() { }

        /// <summary>
        /// インデクサを生成
        /// </summary>
        /// <param name="line"></param>
        internal PathOptionCollection(string line)
        {
            PathHelper.CreateKeyValues(line, (key, value) =>
            {
                Add(new PathOption(key, value) { Parent = this });
            });
        }

        public StringBuilder ToStringBuilder(StringBuilder builder) => this.ToStringBuilder(builder: builder);
        public override string ToString() => this.ToStringBuilder().ToString();
    }
}
