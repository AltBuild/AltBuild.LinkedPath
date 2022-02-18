using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;

namespace AltBuild.LinkedPath
{
    /// <summary>
    /// Object copy use.
    /// </summary>
    public enum CopyUse
    {
        /// <summary>
        /// 自動操作
        /// </summary>
        Auto = 0,

        /// <summary>
        /// 新しい世代として複製する
        ///   Summary:
        ///     clone = source.Clone();
        ///     clone.Original = null;
        ///     clone.Rule = Writable;
        ///     //source.Original   ←変更なし
        /// </summary>
        NewGeneration,

        /// <summary>
        /// 複製
        ///   Summary:
        ///     clone = source.Clone();
        ///     clone.Original = source.Original;
        /// </summary>
        Duplicate,

        /// <summary>
        /// オリジナルのクローンを作成する
        ///   Summary:
        ///     clone = source.Clone();
        ///     clone.Original = source;
        ///     source.Original = clone;
        /// </summary>
        Original,

        /// <summary>
        /// キャッシュ用のクローンを作成する（新規キャッシュ用）
        ///   Summary:
        ///     clone = source.Clone();
        ///     clone.Original = null;
        ///     clone.Rule &= ~DObjectRule.Writable;
        /////     source.Original = clone;
        /// </summary>
        Cache,

        /// <summary>
        /// キャッシュ用のコピーをする（キャッシュ更新用）
        ///   Summary:
        /// </summary>
        CacheUpdate,
    }
}
