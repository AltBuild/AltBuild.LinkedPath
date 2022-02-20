namespace AltBuild.LinkedPath.Parser
{
    /// <summary>
    /// 引数情報
    /// </summary>
    public class PathIndexer : PathKeyValue
    {
        /// <summary>
        /// 複数の引数の親を保持
        /// </summary>
        public PathIndexerCollection Parent { get; init; }

        /// <summary>
        /// 
        /// </summary>
        public ChunkPartName Name { get; init; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsReady { get; private set; }

        /// <summary>
        /// 処理タイプを保持
        /// </summary>
        public PathIndexerMatchingType ProcessType { get; set; }

        /// <summary>
        /// キーと値をセット
        /// </summary>
        /// <param name="key"></param>
        /// <param name="stringValue"></param>
        public PathIndexer(PathOperatorType op, string key, string stringValue, bool isString) : base(key, stringValue, isString)
        {
            OperatorType = op;
        }

        /// <summary>
        /// キーと値をセット
        /// </summary>
        /// <param name="key"></param>
        /// <param name="stringValue"></param>
        public PathIndexer(PathOperatorType op, string key, object value) : base(key, value.ToString(), false)
        {
            OperatorType = op;
        }

        /// <summary>
        /// キーと値をセット
        /// </summary>
        /// <param name="key"></param>
        /// <param name="stringValue"></param>
        public PathIndexer(string key, object value) : base(key, value.ToString())
        {
            Results = new();
            Results.Add(new PathData { Data = value });
            IsReady = true;
        }

        /// <summary>
        /// InitValue が使えない場合に、事前にセットする
        /// </summary>
        /// <param name="value"></param>
        public void SetSourceValue(params object[] values)
        {
            if (IsReady == false && values.Length > 0)
                Results = new PathDataCollection(values[0]?.GetType());

            if (IsReady == false)
            {
                foreach (var value in values)
                    Results.Add(new PathData { Data = value });

                IsReady = true;
            }
        }

        public bool Match(object value, object obj)
        {
            if (value != null)
            {
                if (IsReady == false)
                {
                    Results = new PathDataCollection(value?.GetType(), Line, obj);
                    IsReady = true;
                }

                return Results.Contains(value);
            }

            // 比較対象が null の場合
            return false;
        }
    }
}
