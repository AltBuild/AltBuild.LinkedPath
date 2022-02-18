using AltBuild.BaseExtensions;
using AltBuild.LinkedPath;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace AltBuild.LinkedPath
{
    /// <summary>
    /// オブジェクトパスリスト
    /// （都度生成して処理する）
    /// </summary>
    public class DPathValueMap
    {
        /// <summary>
        /// パラメーターが変化した事を通知する
        /// </summary>
        public Action<DPathValue> OnChanged { get; set; }

        /// <summary>
        /// 用途が読み取り専用
        /// （キャッシュ構成等は行わない）
        /// </summary>
        public bool Readonly { get; set; }

        /// <summary>
        /// コピーチェック
        /// </summary>
        public bool UseCopyCheck { get; set; }

        /// <summary>
        /// トップアイテム
        /// </summary>
        public DPathValue Root { get; private set; }

        /// <summary>
        /// 値マップ（値の取得専用）
        /// </summary>
        public DPathValues TargetValues { get; } = new DPathValues();

        /// <summary>
        /// NativeValue の Get/Set 用
        /// PathのTrace
        /// </summary>
        public List<DiveValue> Traces { get; } = new List<DiveValue>();

        /// <summary>
        /// Native の Get/Set 用
        /// (Path の Native値）
        /// </summary>
        public DPathValue LastNativeValue { get; internal set; }

        /// <summary>
        /// 関連するPathMemberのフレームを取得する
        /// </summary>
        public PathFrame PathFrame { get; init; }

        /// <summary>
        /// 指定の深さ
        /// （デフォルト値： int.MinValue ）
        /// </summary>
        public int CurrentDepth { get; private set; } = int.MinValue;

        /// <summary>
        /// 深度のオフセット（RootのDepth値）
        /// </summary>
        public int DepthOffset { get; init; }

        /// <summary>
        /// 有効な深さを保持しているか？
        /// </summary>
        public bool ValidDepth =>
            CurrentDepth >= 0 && TargetValues.Count > 0 && TargetValues[0].Depths.Count > CurrentDepth;

        /// <summary>
        /// 最終パス情報
        /// </summary>
        public DiveValue LastTrace => Traces.Count > 0 ? Traces[^1] : null;

        /// <summary>
        /// 先頭パス情報
        /// </summary>
        public DiveValue FirstTrace => Traces.Count > 0 ? Traces[0] : null;

        /// <summary>
        /// 値の取得専用
        /// </summary>
        public DPathValue LastFromGetString
        {
            get
            {
                if (TargetValues.Count > 0)
                {
                    return TargetValues[0].TargetFromString;


                    //var value = TargetValues[0];
                    //if (value != null && value.StringValue == null && value.Source == null)
                    //    value = value.Parent;

                    //return value;
                }

                return null;
                //else
                //{
                //    return LastNativeValue;
                //}
            }
        }

        /// <summary>
        /// 値の設定専用
        /// </summary>
        public DPathValue LastFromSetString =>
            TargetValues.Count > 0 ? TargetValues[0].TargetFromCanWrite : null;

        /// <summary>
        /// 値の取得専用
        /// （Native Get/Set には使えない、Get/Set は ）
        /// </summary>
        public DPathValue LastFromPath =>
            TargetValues.Count > 0 ? TargetValues[0].TargetFromPath : LastNativeValue;

        /// <summary>
        /// 条件に合致する深度を走査する
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public bool SetDepth(Predicate<DPathValue> predicate)
        {
            if (CurrentDepth == int.MinValue && TargetValues.Count > 0)
                CurrentDepth = TargetValues[0].SetDepth(predicate);

            return (CurrentDepth >= 0);
        }

        /// <summary>
        /// １つのオブジェクトで
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="pathMember"></param>
        /// <param name="onChanged"></param>
        /// <returns></returns>
        public static DPathValueMap Create(object obj, PathMember pathMember, ILogBase logBase, bool isCopyCheck, Action<DPathValue> onChanged = null)
        {
            // フレームオブジェクト
            var map = new DPathValueMap { PathFrame = pathMember.Frame, UseCopyCheck = isCopyCheck, OnChanged = onChanged, DepthOffset = -1 };

            // ログをセット
            ((IPathFrameInner)map.PathFrame).SetLog(logBase);

            // ルート（階層）を作成
            var rootValue = map.Root = new DPathValue(obj, pathMember) { Frame = map, Items = new DPathValues() };

            // 子を走査
            rootValue._getPathValueMap(map);

            // フレームを返す
            return map;
        }

        public void InsertTargetValues(DPathValue beforeValue, DPathValue afterValue)
        {
            // 結果に追加
            var indexOfTargets = TargetValues.IndexOf(beforeValue);
            TargetValues.Insert(indexOfTargets + 1, afterValue);
        }


        ///// <summary>
        /////  前後のアタッチのチェック
        ///// </summary>
        //public void TestCode_CheckList()
        //{
        //    DPathValue parent = null;
        //    foreach (var value in TargetValues)
        //    {
        //        // Prev と その Next の不一致
        //        if (value.Prev != null)
        //        {
        //            if (value.Prev.Next != value)
        //                throw new InvalidProgramException();
        //        }

        //        // Next と その Prev の不一致
        //        if (value.Next != null)
        //        {
        //            if (value.Next.Prev != value)
        //                throw new InvalidProgramException();
        //        }

        //        // Parent の null チェック
        //        if (value.Parent == null)
        //            throw new InvalidProgramException();

        //        else if (parent == null)
        //            parent = value.Parent;
        //    }
        //}
    }
}
