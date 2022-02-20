using System.Collections.Generic;
using System.Text;

namespace AltBuild.LinkedPath.Parser
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

        internal PathOptionCollection(ChunkPartOptions options)
        {
            InductHelper.CreateKeyValues(options.Line.ToString().TrimStart('!'), (key, value, bString) =>
            {
                Add(new PathOption(key, value) { Parent = this });
            });
        }

        /// <summary>
        /// インデクサを生成
        /// </summary>
        /// <param name="line"></param>
        internal PathOptionCollection(string line)
        {
            InductHelper.CreateKeyValues(line, (key, value, bString) =>
            {
                Add(new PathOption(key, value) { Parent = this });
            });
        }

        public StringBuilder ToStringBuilder(StringBuilder builder)
        {
            if (builder == null)
                builder = new StringBuilder();

            this.ForEach(o => builder.Append(o.ToString()));

            return builder;
        }

        public override string ToString() => ToStringBuilder(null).ToString();
    }
}
