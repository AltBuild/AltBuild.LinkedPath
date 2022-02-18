using AltBuild.BaseExtensions;
using AltBuild.LinkedPath;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Reflection;

namespace AltBuild.LinkedPath
{
    /// <summary>
    /// オブジェクトパス情報
    /// （都度生成して処理）
    /// 
    /// 1. (object)Source + (DLinkedPath)Path => DPathValue
    /// 2. Source + Path => Native => +(Index, Arguments) => Value, ID
    /// 
    ///   --SPM--
    ///   Source: Source Object（元のオブジェクト）
    ///   Path:   PathMember（文字列のパス情報）
    ///   Member: Return Value of member （メンバーの戻り値）
    ///
    ///   Depths: 自身より親側のみ取得可
    /// 
    /// 
    /// |                                                        | Targets (list)    |
    ///  Depth:0         Depth:1             Depth:2              Depth:3 (Only on the parent side than yourself) 
    /// ------------------------------------------------------------------------------
    /// |               |                   |          さ        |         た        |
    /// |               |          か       |--------------------|-------------------|
    /// |               |                   |          し        |         ち        |
    /// |       あ      |-------------------|--------------------|-------------------|
    /// |               |                   |          す        |         つ        |
    /// |               |          き       |--------------------|-------------------|
    /// |               |                   |          せ        |         て        |
    /// ------------------------------------------------------------------------------
    /// 
    /// </summary>
    public class DPathValue
    {
        /// <summary>
        /// オーナー
        /// </summary>
        public DPathValueMap Frame { get; internal set; }

        /// <summary>
        /// 直前の値
        /// （同じ親の中で）
        /// </summary>
        public DPathValue Prev { get; internal set; }

        /// <summary>
        /// 直後の値
        /// （同じ親の中で）
        /// </summary>
        public DPathValue Next { get; internal set; }

        /// <summary>
        /// 親を保持
        /// Parent <- this
        /// </summary>
        public DPathValue Parent { get; internal set; }

        /// <summary>
        /// 子孫を保持
        /// </summary>
        public DPathValues Items { get; internal set; }

        /// <summary>
        /// 元のオブジェクト
        /// </summary>
        public object Source { get; protected set; }

        /// <summary>
        /// StaticValue
        /// (No formatting process, etc.)
        /// </summary>
        public string StaticValue { get; set; }

        /// <summary>
        /// タグ情報を保持
        /// </summary>
        public object Tag { get; set; }

        /// <summary>
        /// バリュー属性を保持
        /// </summary>
        public PathAttributeCollection Attributes => _attributes ??= new PathAttributeCollection();
        PathAttributeCollection _attributes;

        /// <summary>
        /// 現在のパス（参照するメンバー情報）
        /// </summary>
        public PathMember Path { get; protected set; }

        /// <summary>
        /// 元のパスを取得する
        /// （親を遡る）
        /// </summary>
        public PathMember SourcePathMember
        {
            get
            {
                if (Path != null)
                    return Path;

                else if (Parent != null)
                    return Parent.SourcePathMember;

                return null;
            }
        }

        /// <summary>
        /// メンバー値を保持
        /// </summary>
        public DiveValue PathValue => _pathValue ??= Path.FromDiveInner(Source);
        DiveValue _pathValue;

        public int IndexOf(DPathValue value) => Items.IndexOf(value);

        /// <summary>
        /// 指定位置にアイテムを挿入する
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        public void Insert(int index, DPathValue value)
        {
            Items.Insert(index, value);
        }

        public void Refresh(object source)
        {
            Source = source;
            _pathValue = null;
        }

        public void Refresh()
        {
            _pathValue = null;
        }

        /// <summary>
        /// 元オブジェクトに直近のプロパティ情報
        /// </summary>
        public MemberInfo MemberInfo
        {
            get => _memberInfo ??= GetMemberInfo();
            private set => _memberInfo = value;
        }
        MemberInfo _memberInfo;

        ///// <summary>
        ///// 次の値を取得
        ///// </summary>
        ///// <returns></returns>
        //public DPathValueLocation NextLocation =>
        //    _nextLocation ??= DPathValueLocation.GetNextLocation(this);

        //DPathValueLocation _nextLocation;

        /// <summary>
        /// 前の値
        /// （同じ親に前の値が無い場合は、更に前の親へ引き継ぐ）
        /// </summary>
        public DPathValue PrevWithSequential
        {
            get
            {
                // Get Next.
                if (Prev != null)
                    return Prev;

                // From Parent.
                if (Parent != null)
                {
                    var parentValue = Parent.PrevWithSequential;
                    if (parentValue != null && parentValue.Items.Count > 0)
                        return parentValue.Items[^1];
                }

                // End of the Value.
                return null;
            }
        }

        /// <summary>
        /// 次の値
        /// （同じ親に次の値が無い場合は、次の親へ引き継ぐ）
        /// </summary>
        public DPathValue NextWithSequential
        {
            get
            {
                // Get Next.
                if (Next != null)
                    return Next;

                // From Parent.
                if (Parent != null)
                {
                    var parentValue = Parent.NextWithSequential;
                    if (parentValue != null && parentValue.Items.Count > 0)
                        return parentValue.Items[0];
                }

                // End of the Value.
                return null;
            }
        }

        /// <summary>
        /// 親ベースでの次（の同じDepthの値）
        /// </summary>
        public DPathValue NextParential
        {
            get
            {
                if (Parent != null)
                {
                    if (Parent.Items.Count <= 1)
                        return Parent.NextParential;

                    var nextParent = Parent.NextWithSequential;
                    if (nextParent?.Items != null && nextParent.Items.Count > 0)
                        return nextParent.Items[0];
                }

                return null;
            }
        }

        /// <summary>
        /// 前の列を列挙する
        /// （自身は返さない）
        /// </summary>
        public IEnumerable<DPathValue> PrevValues
        {
            get
            {
                if (Parent != null)
                {
                    int indexOfItems = IndexOfItems;
                    for (int i = 0; i < indexOfItems; i++)
                        yield return Parent.Items[i];
                }
            }
        }

        /// <summary>
        /// 後の列を列挙する
        /// （自身は返さない）
        /// </summary>
        public IEnumerable<DPathValue> NextValues
        {
            get
            {
                if (Parent != null)
                {
                    int indexOfItems = IndexOfItems;
                    for (int i = indexOfItems + 1; i < Parent.Items.Count; i++)
                        yield return Parent.Items[i];
                }
            }
        }

        /// <summary>
        /// 最終深度のアイテム数を取得
        /// </summary>
        public int CountOfLastItems => Items?.Sum(o => o.CountOfLastItems) ?? 1;

        /// <summary>
        /// 深さを取得
        /// </summary>
        public int IndexOfDepth => Parent?.IndexOfDepth + 1 ?? Frame.DepthOffset;

        /// <summary>
        /// 列挙位置を取得
        /// （直前の親ベース）
        /// </summary>
        public int IndexOfItems => Parent?.Items?.IndexOf(this) ?? 0;

        /// <summary>
        /// 指定の深さの親を取得
        /// </summary>
        /// <param name="indexOfDepth"></param>
        /// <returns></returns>
        public DPathValue GetParentOf(int indexOfDepth) =>
            (IndexOfDepth > indexOfDepth) ? Parent.GetParentOf(indexOfDepth) : this;

        /// <summary>
        /// 列挙位置を取得
        /// （指定の深度ベース）
        /// </summary>
        /// <param name="indexOfTargetDepth"></param>
        /// <returns></returns>
        public int IndexOf(int indexOfTargetDepth) =>
             _indexOf(indexOfTargetDepth) + IndexOfItems;

        /// <summary>
        /// 指定の深度（indexOfTargetDepth）ベースの末端アイテム数を返す
        /// </summary>
        /// <param name="indexOfTargetDepth"></param>
        /// <returns></returns>
        int _indexOf(int indexOfTargetDepth)
        {
            // indexOfTargetDepth まで親を遡る
            if (IndexOfDepth > indexOfTargetDepth)
                return Parent?._indexOf(indexOfTargetDepth) ?? 0;

            // 指定の親まで遡ったら、0 ～ 自身の直前(Prev) の CountOfLastItems の和を求める
            else
                return PrevValues.Sum(o => o.CountOfLastItems);
        }

        /// <summary>
        /// 指定の深度（indexOfDepth）のアイテム位置（IndexOfSequentialItems）を取得する
        /// </summary>
        /// <param name="indexOfDepth"></param>
        /// <returns></returns>
        public int IndexOfSequentialItems(int indexOfDepth)
        {
            // 指定の深さの親を取得
            var targetValue = GetParentOf(indexOfDepth);

            // 直近親ベースの自身の Index を取得
            int indexOfSequentialItems = targetValue.IndexOfItems;

            // 直近親の Prev を取得し、順次、子孫数を加算する
            for (var atPrev = targetValue.Parent?.Prev; atPrev != null ; atPrev = atPrev.Prev)
                indexOfSequentialItems += atPrev.Items.Count;

            // 結果
            return indexOfSequentialItems;
        }
            

        /// <summary>
        /// パラメーターが変化した事を通知する
        /// </summary>
        public void OnChanged(bool isUpdateChild = true)
        {
            // 子孫を更新
            if (isUpdateChild && Items != null)
                foreach (var item in Items)
                    item.Refresh(PathValue.Value);

            // 更新された事を通知
            if (Frame?.OnChanged != null)
                Frame.OnChanged(this);
        }

        /// <summary>
        /// 親から子のリストを取得
        /// （自身[this]より親側のみ取得可。子は取得不可）
        /// </summary>
        public DPathValues Depths => GetParentToChildList();

        public DPathValue DepthValue
        {
            get
            {
                if (Frame.CurrentDepth >= 0 && Frame.CurrentDepth < Depths.Count)
                    return Depths[Frame.CurrentDepth];

                else
                    return null;
            }
        }

        /// <summary>
        /// 対象がメソッドの場合に メソッドを呼び出す
        /// </summary>
        public bool InvokeMethod()
        {
            var objectType = Source?.GetType();
            var memberInfos = objectType?.GetMember(Path.Name, PathHelper.Flags);
            if (memberInfos != null && memberInfos.Length > 0 && memberInfos[0] is MethodInfo methodInfo)
            {
                // パラメーター生成
                var parameterInfos = methodInfo.GetParameters();
                object[] parameters = new object[parameterInfos.Length];

                for (int i = 0; i < parameterInfos.Length; i++)
                {
                    var type = parameterInfos[i].ParameterType;
                    if (this.GetType().GetInterfaces().Contains(type))
                        parameters[i] = this;
                }

                methodInfo.Invoke(Source, parameters);
                return true;
            }

            return false;
        }

        public int SetDepth(Predicate<DPathValue> predicate)
        {
            // Depth を走査
            for (int i = 0; i < Depths.Count; i++)
                if (predicate(Depths[i]))
                    return i;

            // Depth設定できず
            return -1;
        }

        public PathType PathType => _pathType ??= MemberInfo.GetPathType();
        PathType _pathType;

        /// <summary>
        /// 表示用の対象を取得
        /// （最終地の自身を返す）
        /// 
        ///   Target
        ///   必要なケース：
        ///     → ToString用 → 最終DPathValue
        ///     → 値の Get/Set 用 → CanWrite = true の位置
        /// 
        /// </summary>
        public DPathValue TargetFromString => this;

        /// <summary>
        /// 値の書き込みが出来る最終地を取得
        /// </summary>
        public DPathValue TargetFromCanWrite
        {
            get
            {
                // 値の書き込みが出来ればそれを返す
                if (PathValue?.MemberInfo?.CanWrite() ?? false)
                    return this;

                // 親に遡る
                else if (Parent != null)
                    return Parent.TargetFromCanWrite;

                // 書き込みが出来る親は居ない
                else
                    return null;
            }
        }

        /// <summary>
        /// 最終パスを保持する対象を取得
        /// </summary>
        public DPathValue TargetFromPath
        {
            get
            {
                if (Path != null && string.IsNullOrWhiteSpace(Path.Name) == false)
                    return this;

                if (Parent != null)
                    return Parent.TargetFromPath;

                return this;
            }
        }

        /// <summary>
        /// Create DPathValues.
        /// </summary>
        /// <param name="results"></param>
        internal void _getPathValueMap(DPathValueMap results)
        {
            // 自身が末端なら
            if (ValueItself)
            {
                // オーナーにセット
                Frame = results;

                // 末端に追加
                results.TargetValues.Attach(this);
            }

            // 名無しの場合自身を配列として処理
            else if (Path?.Name == null)
            {
                if (Source is Array array)
                    foreach (object obj in array)
                        SetValue(obj);

                else if (Source is IList list)
                    foreach (object obj in list)
                        SetValue(obj);

                else if (Source is ICollection collection)
                    foreach (object obj in collection)
                        SetValue(obj);

                else if (Source is IEnumerable enumerable)
                    foreach (object obj in enumerable)
                        SetValue(obj);

                void SetValue(object obj)
                {
                    // 値を作成
                    var atValue = new DPathValue(obj, Path.Child) { Frame = results, Parent = this, Items = new DPathValues() };

                    // 親にアタッチ
                    Items.Attach(atValue);

                    // 子を走査
                    atValue._getPathValueMap(results);
                }
            }

            // 通常パス処理
            else
            {
                // 条件を取得
                var predicate = Path?.Indexers?.IsEmpty == false ? PathIndexerPredicateBase.Predicate.GetPredicate(Path.Indexers, PathValue.Value, Source) : null;

                // トレースに追加
                results.Traces.Add(PathValue);

                // LastNative を保持
                if (Path.Child == null && results.LastNativeValue == null)
                    results.LastNativeValue = this;

                // １）親の取得の場合
                // ２）string型の場合（→ここで処理しないと、charの列挙になってしまう為）
                if (Path.Name == "." || PathValue.Value is string)
                {
                    BuildPathValue(PathValue.Value);
                }

                // ３）DObject列挙型の場合
                else if (PathValue.Value is IObjectEnumerable iDEnumerable)
                {
                    foreach (var item in iDEnumerable.ToExtract(Path.Indexers.Index, Frame.UseCopyCheck))
                        BuildPathValue(item);
                }

                // ４）汎用列挙型の場合
                else if (PathValue.Value is IEnumerable enumerable)
                {
                    // 初期化
                    int enumIndex = 0;
                    int? index = Path.Indexers.Index;

                    // 列挙走査
                    foreach (var item in enumerable)
                    {
                        // インデックス指定の場合
                        if (index.HasValue == false || enumIndex == index.Value)
                            BuildPathValue(item);

                        // インクリメント
                        enumIndex++;
                    }
                }

                // ４）その他の場合
                else
                {
                    BuildPathValue(PathValue.Value);
                }


                // PathValue を取得
                void BuildPathValue(object source)
                {
                    if (predicate == null || predicate(source))
                    {
                        // Get next path.
                        var nextPath = Path.Child;

                        // パスバリューを取得
                        var pathValue = new DPathValue(source, nextPath) {
                            Frame = results,
                            Parent = this,
                            Items = (nextPath != null ? new DPathValues() : null)
                        };

                        // 親にアタッチ
                        Items.Attach(pathValue);

                        // 子が有れば走査
                        if (nextPath != null)
                            pathValue._getPathValueMap(results);

                        // 無ければ 追加
                        else
                            results.TargetValues.Attach(pathValue);
                    }
                }
            }
        }

        /// <summary>
        /// リスト値の場合のIndex情報
        /// </summary>
        public int? Index
        {
            get
            {
                return Path?.Indexers?.Index;
            }
            set
            {
                // パスにインデックスフラグ立っている場合のみセットできる
                if (value != null)
                {
                    if (Path != null)
                        Path.Indexers.Index = value;

                    else
                        throw new InvalidProgramException();
                }
            }
        }

        /// <summary>
        /// リンク情報は空
        /// </summary>
        public bool ValueItself => (Path == null && MemberInfo == null);

        public DPathValue(object source, PathMember path, MemberInfo memberInfo = null, DPathValueMap frame = null)
        {
            Source = source;
            Path = path;
            Frame = frame;

            if (memberInfo != null)
            {
                MemberInfo = memberInfo;
                if (path == null)
                    Path = PathFactory.Parse(memberInfo.Name);
            }
        }

        public DPathValue Clone() =>
            new DPathValue(Source, Path, MemberInfo, Frame) { Parent = Parent, StaticValue  = StaticValue, Tag = Tag, Items = new DPathValues() };

        public void InsertParent(DPathValue afterValue)
        {
            // 自身がシングルバリューなら、コピーを作成して親にアタッチ
            if ((Items == null || Items.Count == 1) && Parent != null)
            {
                // クローンを作成
                var copyValue = Clone();

                // クローン親と子をアタッチ
                copyValue.Items.Add(afterValue);
                afterValue.Parent = copyValue;

                // 親にインサート
                Parent.InsertAfter(copyValue);
            }
        }

        public void InsertAfter(DPathValue afterValue)
        {
            //      prev  -
            // this 
            //      3) next  ↓
            //      1) prev  ↑
            // after
            //      2) next  ↓
            //      2) prev  ↑
            // after2
            //      next -

            afterValue.Prev = this;
            afterValue.Next = this.Next;
            this.Next = afterValue;
            if (afterValue.Next != null)
                afterValue.Next.Prev = afterValue;

            if (Parent != null)
            {
                // 後続として親にセット
                // 自身がシングルバリューなら、コピーを作成して親にアタッチ
                if (Parent.Items == null || Parent.Items.Count == 1)
                {
                    // 親をセット
                    afterValue.Parent = Parent.Clone();
                    afterValue.Parent.Items.Add(afterValue);
                    Parent.InsertAfter(afterValue.Parent);
                }

                else
                {
                    // 親をセット
                    afterValue.Parent = Parent;

                    var atIndex = Parent.IndexOf(this);
                    Parent.Insert(atIndex + 1, afterValue);
                }
            }
        }

        /// <summary>
        /// 値を直設定する（イベント対応）
        /// </summary>
        public object NativePathValue
        {
            get => MemberInfo?.GetNativeValue(Source, Path);
            set
            {
                MemberInfo?.SetNativeValue(Source, value);
                OnChanged();
            }
        }

        /// <summary>
        /// 値を直設定する（イベントを発生しない）
        /// </summary>
        /// <param name="value"></param>
        public void SetNativePathValueWithoutEvent(object value)
        {
            MemberInfo?.SetNativeValue(Source, value);
        }

        /// <summary>
        /// ■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■
        ///   NativeType の値を取得（３種の１つ）
        ///     1 (NativeValue):  何も加工しない値そのもの
        ///     2 (Value)      :  値もしくは対象のレコードを取得する
        ///     3 (ID)         :  ID値を操作して取得
        /// 
        ///     値取得３セット（NativeValue, Value, ID）の内の NativeValue
        ///     （プロパティ／メソッド／フィールドに対応）
        ///     
        /// ■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■
        /// </summary>
        public Type SafeReturnType
        {
            get
            {
                // 値そのものを保持している場合
                if (MemberInfo == null)
                {
                    if (Source == null)
                        return typeof(object);
                    else
                        return Source.GetType();
                }
                else
                    return MemberInfo.GetReturnType();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        MemberInfo GetMemberInfo()
        {
            if (Path?.Name != null && Source != null)
            {
                // プロパティかメソッドか
                PathType dType = new PathType(Source.GetType());
                var memberInfos = dType.UnderlyingType.GetMember(Path.Name, PathHelper.Flags);
                if (memberInfos != null && memberInfos.Length >= 1)
                {
                    foreach (var memberInfo in memberInfos)
                    {
                        // Method
                        if (memberInfo.MemberType == MemberTypes.Method)
                        {
                            Path.Type = memberInfo.MemberType;
                            if (memberInfo is MethodInfo methodInfo)
                            {
                                if (Path.Arguments.TryParameters(methodInfo.GetParameters(), out object[] parameters))
                                    return methodInfo;
                            }
                        }

                        // Property / Field
                        else
                        {
                            return memberInfo;
                        }
                    }
                }
            }

            return null;
        }

        public bool TryTraceBack(Predicate<DPathValue> predicate, out DPathValue pathValue)
        {
            if (predicate(this))
            {
                pathValue = this;
                return true;
            }

            if (Parent != null)
                return Parent.TryTraceBack(predicate, out pathValue);

            pathValue = null;
            return false;
        }

        /// <summary>
        /// DPathValue の 親から子へのリストを取得する
        /// </summary>
        /// <returns></returns>
        DPathValues GetParentToChildList()
        {
            var results = new DPathValues();
            _parentToChildList(results);
            return results;
        }

        void _parentToChildList(DPathValues results)
        {
            if (Parent != null)
                Parent._parentToChildList(results);

            results.Add(this);
        }

        /// <summary>
        /// DPathValue の 子（自身）から親へのリストを取得する
        /// </summary>
        /// <returns></returns>
        public DPathValues GetChildToParentList()
        {
            var results = new DPathValues();
            _childToParentList(results);
            return results;
        }

        void _childToParentList(DPathValues results)
        {
            results.Add(this);

            if (Parent != null)
                Parent._childToParentList(results);
        }

        /// <summary>
        /// DPathValue の親から子へのインデックスリストを取得する
        /// </summary>
        /// <returns></returns>
        public DPathValueIndexes GetIndexOfParentToChild()
        {
            var results = new DPathValueIndexes();
            _getIndexOfParentToChild(results);
            return results;
        }

        void _getIndexOfParentToChild(IPathValueIndexes results)
        {
            if (Parent != null)
                Parent._getIndexOfParentToChild(results);

            results.Add(IndexOfItems);
        }

        public override string ToString()
        {
            // 1) Get static value.
            if (StaticValue != null)
                return StaticValue;

            // 2) Source is null to ""
            else if (Source == null)
                return "";

            // 3) Get IObjectBase to String
            else if (Source is IObjectBase dObjectBase)
                return Source.ToString();

            // その他
            else
            {
                PathElements pathElements = null;

                // value = default<T>
                if (Source.IsDefault())
                {
                    if (SourcePathMember?.Frame?.Attributes?.IsExist("defaultHide", "true") ?? false)
                        return "";
                }

                // Format
                else if (SourcePathMember?.Frame?.Attributes.TryGetFormat(out string format) ?? false)
                {
                    pathElements = PathElements.Parse(format);
                }

                return ObjectExtensions.ToString(Source, pathElements ?? PathElements.Default);
                //return ToStringConverterBase.Converter.ToString(this, Source);
            }
        }

        public string ToString(string elements)
        {
            if (Source is IObjectBase dObjectBase)
                return dObjectBase.ToString(elements);

            else if (Source != null)
                return Source.ToString();

            else
                return "";
        }
    }
}
