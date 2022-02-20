using System;
using System.Collections.Generic;

namespace AltBuild.LinkedPath.Converters
{
    /// <summary>
    /// Select item interface
    /// </summary>
    public interface ISelectItemBase
    {
        /// <summary>
        /// ラベル名
        /// </summary>
        string Text { get; }

        /// <summary>
        /// 識別IDを保持
        /// </summary>
        dynamic ID { get; }

        /// <summary>
        /// 値を保持（TextBoxなど）
        /// CheckBox などは別（Checked を使用）
        /// </summary>
        dynamic Value { get; }

        /// <summary>
        /// 元アイテム
        /// </summary>
        dynamic Source { get; set; }

        /// <summary>
        /// 付加情報
        /// </summary>
        object Tag { get; set; }
    }
}
