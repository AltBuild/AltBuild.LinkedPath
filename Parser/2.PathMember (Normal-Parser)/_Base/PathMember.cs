using AltBuild.LinkedPath.Converters;
using System;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Security.AccessControl;

namespace AltBuild.LinkedPath.Parser
{
    /// <summary>
    ///  Holds one path information
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
        /// Indexers is Empty
        /// </summary>
        public bool IndexerIsEmpty => _indexerCollection == null || _indexerCollection.Count == 0;

        /// <summary>
        /// Arguments is Empty
        /// </summary>
        public bool ArgumentsIsEmpty => _argumentCollection == null || _argumentCollection.Count == 0;

        /// <summary>
        /// Options is Empty
        /// </summary>
        public bool OptionsIsEmpty => _optionCollection == null || _optionCollection.Count == 0;

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

        /// <summary>
        /// Get arguments
        /// </summary>
        public PathArgumentCollection Arguments
        {
            get => _argumentCollection ??= (Type == MemberTypes.Method ? new PathArgumentCollection { Path = this } : null);
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

        internal static PathMember Parse(ChunkPartPath atMember)
        {
            PathMember results = null;
            PathMember currentMember = null;

            if (atMember.StaticMarkup)
            {
                results = new PathMember(atMember.Line.ToString()) { Type = MemberTypes.Custom };
            }

            else
            {
                foreach (var atPart in atMember)
                {
                    if (atPart is ChunkPartAttributes attributes)
                    {
                        ((IPathFrameInner)results?.Frame)?.SetAttributes(attributes);
                    }

                    else
                    {
                        if (results == null)
                            results = currentMember = new PathMember(atPart, null);
                        else
                            currentMember = currentMember.SubMember = new PathMember(atPart, null);
                    }
                }
            }

            return results;
        }

        internal PathMember(ChunkPart beginPart, PathMember parent)
        {
            Parent = parent;
            Frame = parent?.Frame ?? new PathFrame { First = this };
            Name = (beginPart.Type.HasFlag(ChunkPartType.MemberName) || beginPart.Type.HasFlag(ChunkPartType.Number)) ? beginPart.Line.ToString() : "";

            if (beginPart is ChunkPartValues partValues)
            {
                _indexerCollection = new PathIndexerCollection(partValues) { Path = this };
            }

            else
            {
                foreach (var atPart in beginPart.Elements)
                {
                    if (atPart is ChunkPartIntoName atIntoName)
                        Child = new PathMember(atIntoName.First, this);

                    else if (atPart is ChunkPartName atName)
                        Child = new PathMember(atName, this);

                    else if (atPart is ChunkPartIndexers atIndexers)
                        _indexerCollection = new PathIndexerCollection(atIndexers) { Path = this };

                    else if (atPart is ChunkPartOptions atOptions)
                        _optionCollection = new PathOptionCollection(atOptions) { Path = this };

                    else if (atPart is ChunkPartArguments atArguments)
                        _argumentCollection = new PathArgumentCollection(atArguments) { Path = this };
                }
            }
        }

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

        public PathMember(string name) : this()
        {
            Name = name;
        }

        public string FullName => (Child != null ? $"{Name}.{Child.FullName}" : Name);

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
                if (atMember.Child != null)
                {
                    atMember = atMember.Child;

                    // To child.
                    bild.Append('.');
                }

                else
                {
                    atMember = atMember.First.SubMember;
                    if (atMember != null)
                    {
                        bild.Append('|');
                        continue;
                    }

                    break;
                }
            }

            // 属性を取得
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
            if (_argumentCollection != null)
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
