using AltBuild.BaseExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace AltBuild.LinkedPath
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
            PathHelper.CreateKeyValues(line, (key, value) =>
            {
                // 値を値に保持
                if (key == null)
                    Add(new PathArgument(null, value) { Parent = this });

                // キーを値に保持
                else if (value == null)
                    Add(new PathArgument(null, key) { Parent = this });

                // キーと値
                else
                    Add(new PathArgument(key, value) { Parent = this });
            });
        }

        /// <summary>
        /// 値のみのリストを取得する
        /// </summary>
        /// <returns></returns>
        object[] ToValueOnlys() =>
            (from x in this where x.Type == PathKeyValueType.Value select x.Results ?? (object)x.Value).ToArray();

        /// <summary>
        /// パラメーターマッチ
        /// </summary>
        /// <param name="parameterInfos"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public bool TryParameters(ParameterInfo[] parameterInfos, out object[] parameters) =>
            PathHelper.TryParameters(ToValueOnlys(), parameterInfos, out parameters);

        public bool TryParametersExtension(object source, ParameterInfo[] parameterInfos, out object[] parameters)
        {
            List<object> results = new();

            results.Add(source);
            results.AddRange(ToValueOnlys());

            return PathHelper.TryParameters(results.ToArray(), parameterInfos, out parameters);
        }


        public StringBuilder ToStringBuilder(StringBuilder builder)
        {
            // ビルダー構築
            if (builder == null)
                builder = new StringBuilder();

            if (IsEmpty == false)
            {
                // プレフィックス
                builder.Append('(');

                // インデクサ
                this.ToStringBuilder(chain: ",", builder: builder);

                // サフィックス
                builder.Append(')');
            }

            return builder;
        }
        public override string ToString() => ToStringBuilder(null).ToString();
    }
}
