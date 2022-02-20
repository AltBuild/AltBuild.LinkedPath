namespace AltBuild.LinkedPath.Parser
{
    /// <summary>
    /// インデクサマッチタイプ
    /// </summary>
    public enum PathOperatorType
    {
        /// <summary>
        /// 未定義
        /// </summary>
        [Code("???")]
        Undefined = default,

        /// <summary>
        /// Empty
        /// </summary>
        [Code("")]
        None = 0x4000000,

        /// <summary>
        /// Operator '='
        /// </summary>
        [Code("=")]
        Equal = 0x0001,

        /// <summary>
        /// Operator key '<' value(s)
        /// </summary>
        [Code("<")]
        Smaller = 0x0002,

        /// <summary>
        /// Operator key '>' value(s)
        /// </summary>
        [Code(">")]
        Bigger = 0x0004,

        /// <summary>
        /// Operator key '<=' value(s)
        /// </summary>
        [Code("<=")]
        SmallerOrEqual = Smaller | Equal,

        /// <summary>
        /// Operator key '>=' value(s)
        /// </summary>
        [Code(">=")]
        BiggerOrEqual = Bigger | Equal,

        /// <summary>
        /// Operator key '!=' value(s)
        /// </summary>
        [Code("!=")]
        Exclusion = 0x0008,
    }
}
