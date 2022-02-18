using AltBuild.LinkedPath;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace AltBuild.LinkedPath
{
    /// <summary>
    /// インデクサマッチタイプ
    /// </summary>
    public enum PathIndexerMatchingType
    {
        /// <summary>
        /// 未定義
        /// </summary>
        None = 0,

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
