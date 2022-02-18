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
    ///  オブジェクトをコピーする為のインターフェース
    /// </summary>
    public interface IObjectCopy
    {
        /// <summary>
        /// 既存のオブジェクトにメンバー値をコピーする
        /// </summary>
        /// <param name="destine">コピー先（既存オブジェクト）</param>
        /// <param name="control">コピーコントロール</param>
        /// <param name="memberInfo">対象のメンバー情報</param>
        /// <returns></returns>
        void Copy(object destine, ICopyControl control, object memberInfo = null);

        /// <summary>
        /// コピー前の処理
        /// </summary>
        /// <param name="destine">コピー先</param>
        /// <param name="control">コピーコントロール</param>
        void SetFlagsBefore(object destine, ICopyControl control);

        /// <summary>
        /// コピー後の処理
        /// </summary>
        /// <param name="destine">コピー先</param>
        /// <param name="control">コピーコントロール</param>
        void SetFlagsAfter(object destine, ICopyControl control);

        /// <summary>
        /// インスタンスソースオブジェクト
        /// </summary>
        object DefinitionObject { get; set; }

        /// <summary>
        /// データの状態
        /// </summary>
        DObjectRule Rule { get; set; }
    }
}
