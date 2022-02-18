using AltBuild.BaseExtensions;
using System;
using System.Linq;
using System.Reflection;
using System.Text;

namespace AltBuild.LinkedPath
{
    /// <summary>
    /// リンクパスを解析
    ///
    ///   １）学校.学級('xxxx','yyyy').学科[Gakka.ID='xxxx']:readonly,class='text-primary'
    ///       ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    ///       ↑(A)LinkedName                                ↑(B)                         → ':' で区切る（DLinkedPath:PathOption）
    /// 
    ///       ~~~~ ~~~~~~~~~~~~~~~~~~~ ~~~~~~~~~~~~~~~~~~~~~                               → '.' で区切る（DLinkedPathCollectionでリスト化）
    ///       ↑(1)    ↑(2)Method       ↑(3)Property
    /// </summary>
    public partial class PathMember
    {
        /// <summary>
        /// Object Member Name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Object Member Type.
        /// </summary>
        public MemberTypes Type { get; set; }

        /// <summary>
        /// Root Object.
        /// </summary>
        public PathFrame Frame { get; private set; }

        /// <summary>
        /// Object Sub member
        /// </summary>
        public PathMember SubMember { get; set; }

        /// <summary>
        /// Chain of parent.
        /// </summary>
        public PathMember Parent { get; private set; }

        /// <summary>
        /// Chain of child.
        /// </summary>
        public PathMember Child { get; private set; }

        /// <summary>
        /// Chain of Last.
        /// </summary>
        public PathMember Last => Child != null ? Child.Last : this;

        /// <summary>
        /// Chain of First.
        /// </summary>
        public PathMember First => Parent != null ? Parent.First : this;

        /// <summary>
        /// Depth Count.
        /// </summary>
        public int Depth
        {
            get
            {
                if (_depth == -1)
                {
                    if (Parent != null)
                        _depth = Parent.Depth + 1;
                    else
                        _depth = 0;
                }
                return _depth;
            }
        }
        int _depth = -1;

        /// <summary>
        /// Get static content
        /// </summary>
        /// <param name="content">content</param>
        /// <returns>true: success, false: failed</returns>
        public bool TryGetContent(out string content)
        {
            if (Type == MemberTypes.Custom)
            {
                content = Name;
                return true;
            }

            content = null;
            return false;
        }

        ///// <summary>
        ///// パス構造がシンプルであるか（シンプルに値が取得可能なパスであるか？）
        ///// 　影響のあるオプションが
        ///// 　→ Indexers: インデクサ
        ///// 　→ Arguments: 引数
        ///// 　
        ///// </summary>
        //public bool IsSimple => (_indexerCollection == null && _argumentCollection == null);

        /// <summary>
        /// Get arguments
        /// </summary>
        public PathArgumentCollection Arguments
        {
            get => _argumentCollection ??= new PathArgumentCollection { Path = this };
            private set => _argumentCollection = value;
        }
        PathArgumentCollection _argumentCollection;

        /// <summary>
        /// Get indexers
        /// </summary>
        public PathIndexerCollection Indexers
        {
            get => _indexerCollection ??= new PathIndexerCollection { Path = this };
            private set => _indexerCollection = value;
        }
        PathIndexerCollection _indexerCollection;

        /// <summary>
        /// Get options
        /// </summary>
        public PathOptionCollection Options
        {
            get => _optionCollection ??= new PathOptionCollection { Path = this };
            private set => _optionCollection = value;
        }
        PathOptionCollection _optionCollection;

        /// <summary>
        /// アーギュメント（文字列）をセット
        /// </summary>
        /// <param name="line"></param>
        internal void SetArguments(string line) => Arguments = new PathArgumentCollection(line) { Path = this };

        /// <summary>
        /// インデクサ（文字列）をセット
        /// </summary>
        /// <param name="line"></param>
        internal void SetIndexers(string line) => Indexers = new PathIndexerCollection(line) { Path = this };

        internal void SetOptions(string line) => Options = new PathOptionCollection(line) { Path = this };

        internal PathMember(string name, PathFrame frame, PathMember parent, PathMember child)
        {
            // フレームを生成＆保持
            Frame = frame != null ? frame : new PathFrame { First = this };

            // 親を保持
            Parent = parent;

            // 子を保持
            Child = child;

            // メンバー名を保持
            string[] names = name == null ? new string[] { null } : name.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            if (names.Length >= 1)
            {
                // メンバー名を保持
                Name = names[0];

                // メンバー名がスタティック名と同一なら、スタティックタグとして処理
                if (PathFactory.StaticMarkups.Find(t => t == names[0]) != null)
                    Type = MemberTypes.Custom;
            }

            // サブメンバーを構成
            if (names.Length >= 2)
            {
                PathMember face = this;
                for (int i = 1; i < names.Length; i++)
                    face = face.SubMember = new PathMember(names[i]);
            }
        }

        internal PathMember()
        {
            Frame = new PathFrame();
        }

        internal PathMember(string name) : this()
        {
            Name = name;
        }

        public string FullName => (Child != null ? $"{Name}.{Child.FullName}" : Name);

        public void SetChild(PathMember child) => Child = child;

        public T GetAttribute<T>(object obj)
        {
            foreach (var memberInfo in obj.GetType().GetMember(Name))
                if (memberInfo.TryGetAttribute(out T attribute))
                    return attribute;

            return default(T);
        }

        public bool TryGetAttribute<T>(object obj, out T attribute)
        {
            attribute = GetAttribute<T>(obj);
            return attribute != null;
        }

        public MemberInfo GetMemberInfo(object source)
        {
            if (source != null && string.IsNullOrWhiteSpace(Name) == false)
            {
                var memberInfos = source.GetType().GetMember(Name, PathHelper.Flags);
                if (memberInfos.Length > 0)
                    return memberInfos[0];
            }

            return null;
        }

        /// <summary>
        /// オブジェクト内のプロパティ値
        /// （プロパティのみサポート）
        /// </summary>
        /// <param name="source"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryGetPropertyValue<T>(object source, out T value)
        {
            if (string.IsNullOrWhiteSpace(Name) == false)
            {
                var propertyInfo = source.GetType().GetProperty(Name);
                if (propertyInfo != null && propertyInfo.GetValue(source) is T tValue)
                {
                    value = tValue;
                    return true;
                }
            }

            // 失敗
            value = default;
            return false;
        }

        /// <summary>
        /// Create DObjectMember.
        /// </summary>
        /// <param name="source">Definition object</param>
        /// <param name="dObjectMember">Created DMemberInfo</param>
        /// <returns>Successful: true, Failed: false</returns>
        public bool TryGetValue<T>(object source, out T value)
        {
            var propertyInfo = source.GetType().GetProperty(Name, PathHelper.Flags);
            if (propertyInfo != null)
            {
                // Index を考慮した値を取得
                var propertyValue = propertyInfo.GetValue(source);
                if (propertyValue != null && Indexers.Index.HasValue)
                {
                    // インデックス処理
                    if (propertyValue is IObjectIndexer objectIndexer)
                        propertyValue = objectIndexer[Indexers.Index.Value];
                }

                if (Child != null && string.IsNullOrWhiteSpace(Child.Name) == false)
                {
                    return Child.TryGetValue<T>(propertyValue, out value);
                }

                // 指定のタイプへのキャストを試みる
                else if (propertyValue is T atValue)
                {
                    value = atValue;
                    return true;
                }
            }

            value = default(T);
            return false;
        }

        /// <summary>
        /// Create DObjectMember.
        /// </summary>
        /// <param name="source">Definition object</param>
        /// <param name="dObjectMember">Created DMemberInfo</param>
        /// <returns>Successful: true, Failed: false</returns>
        public bool TrySetPropertyValue<T>(object source, T value)
        {
            var propertyInfo = source.GetType().GetProperty(Name, PathHelper.Flags);
            if (propertyInfo != null)
            {
                // Index を考慮した値を取得
                var propertyValue = propertyInfo.GetValue(source);
                if (propertyValue != null)
                {
                    if (Child != null && string.IsNullOrWhiteSpace(Child.Name) == false)
                    {
                        // インデックス処理
                        if (Indexers.Index.HasValue && propertyValue is IObjectIndexer objectIndexer)
                            propertyValue = objectIndexer[Indexers.Index.Value];

                        return Child.TrySetPropertyValue<T>(propertyValue, value);
                    }
                    else
                    {
                        if (Indexers.Index.HasValue && propertyValue is IObjectIndexer objectIndexer)
                        {
                            objectIndexer[Indexers.Index.Value] = value;
                            return true;
                        }
                        else
                        {
                            return PathHelper.TrySetValue<T>(value, source, Name);
                        }
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Create DiveValue.
        /// </summary>
        /// <param name="source">source object</param>
        /// <returns>DiveValue</returns>
        public DiveValue FromDiveInner(object source, DiveControlFlags flags)
        {
            ((IPathFrameInner)Frame).SetFlags(flags);
            return FromDiveInner(source);
        }

        /// <summary>
        /// Create DiveValue.
        /// </summary>
        /// <param name="source">source object</param>
        /// <returns>DiveValue</returns>
        public DiveValue FromDiveInner(object source)
        {
            object resultValue = null;

            if (source == null)
                Frame.Log.Add(new LogItem { Level = LogLevel.Warning, Name = "FromDiveInner", Message = "source is null." });

            if (Name == null)
                Frame.Log.Add(new LogItem { Level = LogLevel.Error, Name = "FromDiveInner", Message = "Name is null." });

            // Escalation to parent object.
            if (Name == ".")
            {
                if (Child != null && source is IObjectBase iObjectBase && iObjectBase.DefinitionObject != null)
                {
                    resultValue = iObjectBase.DefinitionObject;
                }
            }

            // Get member value.
            else if (PathHelper.TryGetPathValue(out DiveValue pathValue, source, Name, this))
            {
                Type = pathValue.MemberInfo.MemberType;
                resultValue = pathValue.Value;
            }

            // other error.
            else
            {
                Frame.Log.Add(new LogItem { Level = LogLevel.Error, Name = "FromDiveInner", Message = $"Name[{Name}] is not found." });
            }

            return new DiveValue { Source = source, PathMember = this, Value = resultValue };
        }

        /// <summary>
        /// フルパス名（オプションを除く）を取得
        /// </summary>
        /// <returns></returns>
        public string ToStringWithoutOptions() => _toStringWithoutOptions ??= _createString(true);
        string _toStringWithoutOptions;

        /// <summary>
        /// フルパス名を取得する
        /// </summary>
        /// <returns></returns>
        public override string ToString() => _toString ??= _createString();
        string _toString;

        string _createString(bool isDisableOptions = false)
        {
            // 結果
            var bild = new StringBuilder();

            // 処理対象
            var atMember = this;

            // 子孫を走査
            for (; ; )
            {
                // パス名の追加
                atMember.ToStringBuilder(bild, isDisableOptions);

                // 継続
                if ((atMember = atMember.Child) == null)
                    break;

                // 継続する時は '.' を追加
                bild.Append('.');
            }

            // 属性を取得
            if (isDisableOptions == false)
                bild.Append(Frame.Attributes.ToString());

            // 結果
            return bild.ToString();
        }

        public StringBuilder ToStringBuilder(StringBuilder builder = null, bool isDisableOptions = false)
        {
            if (builder == null)
                builder = new StringBuilder();

            // Name
            builder.Append(Name);

            // Arguments
            if (_argumentCollection != null && _argumentCollection.IsEmpty == false)
                builder.Append(_argumentCollection.ToString());

            // Indexers
            if (_indexerCollection != null && _indexerCollection.IsEmpty == false)
                builder.Append(_indexerCollection.ToString());

            // Options
            if (isDisableOptions == false)
                if (_optionCollection != null && _optionCollection.IsEmpty == false)
                    builder.Append(_optionCollection.ToString());

            return builder;
        }

        public static explicit operator PathElement(PathMember pathMember) =>
            new PathElement(PathElementType.Value, pathMember);
    }
}
