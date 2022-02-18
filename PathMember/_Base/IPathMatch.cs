using AltBuild.BaseExtensions;
using AltBuild.LinkedPath;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace AltBuild.LinkedPath
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
