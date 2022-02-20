using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace AltBuild.LinkedPath.Parser
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
        public int? Index
        {
            get
            {
                if (_index == null)
                {
                    if (Count == 1 && this[0].Results != null && this[0].Results.Count == 1)
                    {
                        if (this[0].Results[0].Data is int atIndex)
                        {
                            return atIndex;
                        }
                    }
                }
                return _index;
            }
            set => _index = value;
        }
        int? _index;

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
            (Count == 0 && IndexReady == null && SelectItemsPropertyName == null && Index.HasValue == false);

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

        public PathIndexerCollection(ChunkPart atIndexers)
        {
            // 例1:  "=ABC", "=ABC()"
            // 例2:  "AccountID='xxx-xxx-xxx-xxx-xxx'"
            // 例3:  "Account.ID=['xxx-xxx-xxx-xxx-xxx', 'xxx-xxx-xxx-xxx-xxx', 'xxx-xxx-xxx-xxx-xxx']

            // Build members
            foreach (var atPart in atIndexers)
            {
                // Get full name.
                if (atPart is ChunkPartName namePart)
                {
                    var fullName = namePart.GetFullNameWithNext(out ChunkPart nextPart);
                    Create(fullName, namePart, nextPart);
                }
                else
                {
                    Create(null, null, atPart);
                }
            }

            void Create(string fullName, ChunkPartName atName, ChunkPart atPart)
            {
                // Get value(s)
                if (atPart is ChunkPartOperator partOperator)
                {
                    // Get Values
                    if (partOperator.TryGetDescendants(out ChunkPartValues partValues))
                    {
                        // Add Indexer
                        var pathIndexer = new PathIndexer(partOperator.Operator, fullName, partValues.Source) { Parent = this, Name = atName };
                        Add(pathIndexer);

                        // Set Values
                        // "Key=[xxx,xxx,xxx]"
                        pathIndexer.SetSourceValue(partValues.GetValues());
                    }

                    // Get Value
                    else if (atPart.TryGetDescendants(out ChunkPartValue partValue))
                    {
                        // "Key=Value"
                        // Add Indexer
                        var pathIndexer = new PathIndexer(partOperator.Operator, fullName, partValue.Value) { Parent = this, Name = atName };
                        Add(pathIndexer);

                        // Set Value
                        // "Key=123"
                        pathIndexer.SetSourceValue(partValue.Value);
                    }

                    // Call Property/Method
                    else if (partOperator.TryGetDescendants(out ChunkPartName partName))
                    {
                        // "=PropertyName(MethodName)"
                        if (string.IsNullOrEmpty(fullName))
                            SelectItemsPropertyName = partName.FullName;

                        // "Key=PropertyName(MethodName)"
                        else
                            Add(new PathIndexer(partOperator.Operator, fullName, partName.Source) { Parent = this, Name = atName });
                    }
                }

                // Get Value only. "[1]"
                else if (atPart is ChunkPartValue partValue)
                {
                    var indexer = new PathIndexer(null, partValue.Value) { Parent = this, Name = atName };
                    if (partValue.Type == ChunkPartType.Number && partValue.Value is int)
                    {
                        Add(indexer);
                    }
                }
            }
        }

        /// <summary>
        /// インデクサを生成
        /// </summary>
        /// <param name="line"></param>
        public PathIndexerCollection(string line) : this(ChunkParser.Parse(line)) { }

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
                {
                    builder.Append('=').Append(SelectItemsPropertyName);

                    if (Count > 0)
                        builder.Append(',');
                }

                // インデクサ
                if (Count > 0)
                {
                    for (int i = 0; i < Count; i++)
                    {
                        if (i > 0)
                            builder.Append(',');

                        builder.Append(this[i].ToString());
                    }
                }

                // インデックスを保有している場合
                else if (Index.HasValue)
                {
                    builder.Append(Index.Value);
                }

                // サフィックス
                builder.Append(']');
            }

            return builder;
        }

        public override string ToString() => ToStringBuilder(null).ToString();
    }
}
