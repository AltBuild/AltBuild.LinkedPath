namespace AltBuild.LinkedPath.Parser
{
    /// <summary>
    /// 引数情報
    /// </summary>
    public class PathOption : PathKeyValue
    {
        /// <summary>
        /// 複数の引数の親を保持
        /// </summary>
        public PathOptionCollection Parent { get; init; }

        /// <summary>
        /// キーと値をセット
        /// </summary>
        /// <param name="key"></param>
        /// <param name="stringValue"></param>
        public PathOption(string key, string stringValue) : base(key, stringValue, true)
        {
        }

        /// <summary>
        /// ToStringを生成
        /// </summary>
        /// <returns></returns>
        public override string ToString() => $"!{base.ToString()}";
    }
}
