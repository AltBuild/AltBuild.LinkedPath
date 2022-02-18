using System;

namespace AltBuild.LinkedPath
{
    /// <summary>
    /// レコードデータの状態
    /// </summary>
    [Flags]
    public enum DObjectRule
    {
        /// <summary>
        /// 未指定
        /// </summary>
        None = 0x0000,

        /// <summary>
        /// オブジェクト生成中
        /// </summary>
        Creating = 0x0001,

        /// <summary>
        /// 構築中（データストアから読込）
        /// </summary>
        Building = 0x0002,

        /// <summary>
        /// 更新中
        /// </summary>
        Refreshing = 0x004,

        /// <summary>
        /// 元はデータソースから
        /// </summary>
        Datastore = 0x0008,

        /// <summary>
        /// 通常はコピーオブジェクトに付くフラグ（データストへ更新する時には必要）
        /// キャッシュしないレコード（DModelBehavior.AutoCache または Master ではない）の場合は常に付く
        /// </summary>
        Writable = 0x0010,

        /// <summary>
        /// Empty object.
        /// (Not edit, Not copy)
        /// </summary>
        Empty = 0x0020,

        /// <summary>
        /// 世代管理レコードである事を示す
        /// </summary>
        GenerationManage = 0x0040,

        /// <summary>
        /// 異代レコードである事を示す
        /// （現代ではなく、古代（過去）か近代（未来）の情報である事を示す）
        /// </summary>
        DifferentGeneration = 0x0100,

        /// <summary>
        /// Initialize Only. Exclude.
        /// </summary>
        Set = 0x4000,

        /// <summary>
        /// Initialize Only. Exclude.
        /// </summary>
        Exclude = 0x8000,
    }
}
