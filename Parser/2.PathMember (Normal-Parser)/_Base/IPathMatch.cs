using System.Collections.Generic;

namespace AltBuild.LinkedPath.Parser
{
    /// <summary>
    /// オブジェクトと値のマッチ情報を取得する
    /// </summary>
    public interface IPathMatch
    {
        bool TryMatch(PathIndexer pathIndexer);

        IEnumerable<dynamic> GetMatchValue();
    }
}
