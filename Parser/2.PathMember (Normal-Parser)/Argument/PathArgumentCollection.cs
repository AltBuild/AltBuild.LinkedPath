using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace AltBuild.LinkedPath.Parser
{
    /// <summary>
    /// １つの名前（文字列）に対して、複数のフラグを保持する
    /// 
    /// 例１： DepertmentName:class=text-success,disabled
    ///        Name: DepertmentName
    ///        Option1: key=class, value=text-success
    ///        Option2: key=disabled, value=null
    /// 
    /// </summary>
    public class PathArgumentCollection : List<PathArgument>
    {
        /// <summary>
        /// この引数の関連パス情報（親）
        /// </summary>
        public PathMember Path { get; init; }

        /// <summary>
        /// 値のみのリスト（内部的には、キーのみ（値がnull）で判断）のリストを保持
        /// </summary>
        public PathDataCollection Values { get; } = new PathDataCollection();

        /// <summary>
        /// Is Empty
        /// </summary>
        public bool IsEmpty =>　Count == 0;

        internal PathArgumentCollection() { }

        internal PathArgumentCollection(string line)
        {
            InductHelper.CreateKeyValues(line, (key, value, bString) =>
            {
                // 値を値に保持
                if (key == null)
                    Add(new PathArgument(null, value, bString) { Parent = this });

                // キーを値に保持
                else if (value == null)
                    Add(new PathArgument(null, key, bString) { Parent = this });

                // キーと値
                else
                    Add(new PathArgument(key, value, bString) { Parent = this });
            });
        }

        public PathArgumentCollection(ChunkPartArguments arguments)
        {
            foreach (ChunkPart arg in arguments)
            {
                if (arg is ChunkPartValue partValue)
                {
                    var value = partValue.Value;
                    Type type = value?.GetType();
                    Add(new PathArgument(null, value.ToString(), (type == typeof(Guid) || type == typeof(string))));
                }
            }
        }

        /// <summary>
        /// 値のみのリストを取得する
        /// </summary>
        /// <returns></returns>
        object[] ToValueOnlys() =>
            (from x in this where x.Type == PathKeyValueType.Value select x.Results ?? (object)x.Line).ToArray();

        /// <summary>
        /// パラメーターマッチ
        /// </summary>
        /// <param name="parameterInfos"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public bool TryParameters(ParameterInfo[] parameterInfos, out object[] parameters) =>
            InductHelper.TryParameters(ToValueOnlys(), parameterInfos, out parameters);

        public bool TryParametersExtension(object source, ParameterInfo[] parameterInfos, out object[] parameters)
        {
            List<object> results = new();

            results.Add(source);
            results.AddRange(ToValueOnlys());

            return InductHelper.TryParameters(results.ToArray(), parameterInfos, out parameters);
        }


        public StringBuilder ToStringBuilder(StringBuilder builder)
        {
            // Create builder
            if (builder == null)
                builder = new StringBuilder();

            // Prefix
            builder.Append('(');

            // Contents
            for (int i = 0; i < Count; i++)
            {
                if (i > 0)
                    builder.Append(',');

                builder.Append(this[i].ToString());
            }

            // Suffix
            builder.Append(')');

            return builder;
        }
        public override string ToString() => ToStringBuilder(null).ToString();
    }
}
