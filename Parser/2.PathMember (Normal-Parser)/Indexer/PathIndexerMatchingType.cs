namespace AltBuild.LinkedPath.Parser
{
    /// <summary>
    /// インデクサマッチタイプ
    /// </summary>
    public enum PathIndexerMatchingType
    {
        /// <summary>
        /// 未定義
        /// </summary>
        None = default,

        /// <summary>
        /// データソースマッチング
        /// </summary>
        DatastoreMatching = 0x0001,

        /// <summary>
        /// オブジェクトレベルでマッチング
        /// </summary>
        ObjectMemberMatching = 0x0002,
    }
}
