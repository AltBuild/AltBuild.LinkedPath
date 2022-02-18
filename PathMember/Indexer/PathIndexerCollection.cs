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
    public partial class PathIndexerCollection : List<PathIndexer>
    {
        /// <summary>
        /// この引数の関連パス情報（親）
        /// </summary>
        public PathMember Path { get; init; }

        /// <summary>
        /// リストから択一選択のインデックス値
        /// </summary>
        public int? Index { get; set; }

        /// <summary>
        /// パスにインデックス指示が有ったか？
        /// true: インデックスの準備が整った。 false:インデックス指示はあるが値が未定。 null:インデックス指示が無い
        /// </summary>
        public bool? IndexReady { get; set; }

        /// <summary>
        /// 選択肢指定の為のプロパティ名
        /// </summary>
        public string SelectItemsPropertyName { get; private set; }

        /// <summary>
        /// Is Empty
        /// </summary>
        public bool IsEmpty =>
            (Count == 0 && IndexReady == null && SelectItemsPropertyName == null);

        /// <summary>
        /// KVType
        /// </summary>
        public PathKeyValueType KVType
        {
            get
            {
                // Get KVType.
                return _kvType ??= (Count > 0 ? GetKVTypeAndCheck() : PathKeyValueType.None);

                // Get KVType and checking.
                PathKeyValueType GetKVTypeAndCheck()
                {
                    // First KVType.
                    var firstKVType = this[0].Type;

                    // Complex check.
                    for (int i = 1; i < Count; i++)
                        if (firstKVType != this[i].Type)
                            firstKVType |= PathKeyValueType.ComplexError;

                    return firstKVType;
                }
            }
        }
        PathKeyValueType? _kvType;

        internal PathIndexerCollection() { }

        /// <summary>
        /// インデクサを生成
        /// </summary>
        /// <param name="line"></param>
        public PathIndexerCollection(string line)
        {
            PathHelper.CreateKeyValues(line, (key, value) =>
            {
                // 選択肢指定の為のプロパティ名を指示
                if (key == null)
                {
                    SelectItemsPropertyName = value;
                }

                // Keys or KeyValues のどちらかにセット
                else
                {
                    // 値のみ
                    if (value == null)
                    {
                        // Set index value.
                        if (string.IsNullOrWhiteSpace(key) == false)
                        {
                            // インデックスをセット
                            if (int.TryParse(key, out int atIndex))
                            {
                                Index = atIndex;
                                IndexReady = true;
                            }

                            // 値のみをセット
                            else
                                Add(new PathIndexer(null, value) { Parent = this });
                        }

                        // インデックスフラグが準備中
                        else
                            IndexReady = false;
                    }

                    // キーと値
                    else
                    {
                        Add(new PathIndexer(key, value) { Parent = this });
                    }
                }
            });
        }

        /// <summary>
        /// 値のみのリストを取得する
        /// </summary>
        /// <returns></returns>
        object[] ToValueOnlys() =>
            (from x in this where x.Type == PathKeyValueType.Value select x.Results).ToArray();

        /// <summary>
        /// パラメーターマッチ
        /// </summary>
        /// <param name="parameterInfos"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public bool TryParameters(ParameterInfo[] parameterInfos, out object[] parameters) =>
            PathHelper.TryParameters(ToValueOnlys(), parameterInfos, out parameters);

        /// <summary>
        /// Create DObjectMember.
        /// </summary>
        /// <param name="obj">Definition object</param>
        /// <param name="memberInfo">Created DMemberInfo</param>
        /// <returns>Successful: true, Failed: false</returns>
        public bool TryGetReturnValue(object obj, out DiveValue memberInfo)
        {
            if (PathHelper.TryGetPathValue(out memberInfo, obj, Path.Name))
            {
                Path.Type = memberInfo.MemberInfo.MemberType;
                return true;
            }
            return false;
        }

        public StringBuilder ToStringBuilder(StringBuilder builder)
        {
            // ビルダー構築
            if (builder == null)
                builder = new StringBuilder();

            if (IsEmpty == false)
            {
                // プレフィックス
                builder.Append('[');

                // 選択肢メンバー名
                if (SelectItemsPropertyName != null)
                    builder.Append('=').Append(SelectItemsPropertyName);

                // インデックスを保有している場合
                if (Index.HasValue)
                    builder.Append(Index.Value);

                // インデクサ
                this.ToStringBuilder(chain: ",", builder: builder);

                // サフィックス
                builder.Append(']');
            }

            return builder;
        }
        public override string ToString() => ToStringBuilder(null).ToString();

        public bool TryParse(out object[] parameters)
        {
            throw new NotImplementedException();

            parameters = null;
            return false;
        }
    }
}
