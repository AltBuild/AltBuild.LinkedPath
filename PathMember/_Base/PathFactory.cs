using AltBuild.BaseExtensions;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace AltBuild.LinkedPath
{
    /// <summary>
    /// リンクパスを解析
    ///
    ///   １）学校.学級('xxxx','yyyy')|Visible.学科[Gakka.ID='xxxx']:readonly,class='text-primary'
    ///       ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    ///       ↑(A)LinkedName                                ↑(B)                         → ':' で区切る（DLinkedPath:PathOption）
    /// 
    ///       ~~~~ ~~~~~~~~~~~~~~~~~~~ ~~~~~~~~~~~~~~~~~~~~~                               → '.' で区切る（DLinkedPathCollectionでリスト化）
    ///       ↑(1)    ↑(2)Method       ↑(3)Property
    ///
    ///
    /// １つの名前（文字列）に対して、複数のフラグを保持する
    /// 例１： DepertmentName:class=text-success,disabled
    ///        Name: DepertmentName
    ///        Option1: key=class, value=text-success
    ///        Option2: key=disabled, value=null
    /// 
    /// </summary>
    public static class PathFactory
    {
        public readonly static PreSufChars[] PreSuf = new[] {
            new PreSufChars { Pre = '\'', Suf = '\'' },
            new PreSufChars { Pre = '\"', Suf = '\"' },
            new PreSufChars { Pre = '[' , Suf = ']'  },
            new PreSufChars { Pre = '(' , Suf = ')'  }};

        enum ElementType
        {
            /// <summary>
            /// 未指定
            /// </summary>
            None = 0,

            /// <summary>
            /// パス名
            /// </summary>
            Name,

            /// <summary>
            /// 親パス
            /// </summary>
            Parent,

            /// <summary>
            /// 接頭辞
            /// </summary>
            Prefix,

            /// <summary>
            /// 接尾辞
            /// </summary>
            Suffix,
        }

        /// <summary>
        /// 静的に処理するタグを定義する
        /// </summary>
        public static List<string> StaticMarkups { get; set; } = new List<string>();

        public static PathMemberCollection Parses(string line) => _parses(line);

        public static PathMember Parse(string line) => _parses(line).GetFirst();

        public static PathMember ParseOfLabel(string label) => _labeling(label);

        public static PathMember ParseOfFormat(string format) => _formatting(format);

        static PathMemberCollection _parses(string line)
        {
            // Results
            PathMemberCollection results = new PathMemberCollection();

            if (line == null)
                return results;

            // Processing linkedPath
            PathFrame frame = null;
            PathMember parent = null;
            PathMember path = null;

            // processing state
            string name = null;
            string prefix = null;
            string suffix = null;

            // member
            int atIndex = 0;
            ElementType elementType = ElementType.None;

            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];

                if (c is '\'' or '\"')
                {
                    var indexOfEnd = line.IndexOf(c, i + 1, PreSuf);
                    if (indexOfEnd > 0)
                    {
                        i = indexOfEnd;
                        continue;
                    }
                    //else
                    //    throw new InvalidDataContractException();
                }

                else if (c == '{')
                {
                    SetElement(i, ElementType.Prefix);
                    elementType = ElementType.Name;
                    atIndex = i + 1;
                    continue;
                }

                else if (c == '}')
                {
                    SetElement(i);
                    elementType = ElementType.Suffix;
                    atIndex = i + 1;
                    continue;
                }

                else if (c == '[')
                {
                    ChainLinkedPath(i++);

                    var indexOfEnd = line.IndexOf(']', i, PreSuf);
                    if (indexOfEnd > 0 && path != null)
                    {
                        path.Type = MemberTypes.Property;
                        path.SetIndexers(line.Substring(i, indexOfEnd - i));
                        i = indexOfEnd;
                        continue;
                    }
                    //else
                    //    throw new InvalidDataContractException();
                }

                else if (c == '(')
                {
                    ChainLinkedPath(i++);

                    var indexOfEnd = line.IndexOf(')', i, PreSuf);
                    if (indexOfEnd > 0 && path != null)
                    {
                        path.Type = MemberTypes.Method;
                        path.SetArguments(line.Substring(i, indexOfEnd - i));
                        i = indexOfEnd;
                        continue;
                    }
                    //else
                        //throw new InvalidDataContractException();
                }

                // '!' is a Option delimiter
                else if (c == '!')
                {
                    ChainLinkedPath(i++, true);

                    var indexOfEnd = line.IndexOf('.', i, PreSuf);
                    if (indexOfEnd == -1)
                        indexOfEnd = line.Length;

                    if (indexOfEnd > 0 && path != null)
                    {
                        path.SetOptions(line.Substring(i, indexOfEnd - i));
                        i = indexOfEnd -1;
                        continue;
                    }
                }

                // '.' is a LinkedName delimiter
                else if (c == '.')
                {
                    // parent Mode
                    if (atIndex == i && (line.Length - 1 > i) && line[i + 1] == '.')
                    {
                        elementType = ElementType.Parent;
                        continue;
                    }

                    ChainLinkedPath(i, true);
                    atIndex = i + 1;
                    path = null;
                }

                // PathAttribute after ':'
                else if (c == ':')
                {
                    // Pre process
                    ChainLinkedPath(i, true);

                    // Set PathOption
                    ((IPathFrameInner)frame).SetAttributes(AddPathOption(), prefix, suffix);
                    prefix = suffix = null;

                    // At LinkedPath is Finished ... and next
                    atIndex = i + 1;
                    path = null;
                    parent = null;
                    frame = null;
                }

                // Separater
                else if (((int)c) is < 0x21 or 0x7f)
                {
                    // Pre process
                    FinishPath(i++);

                    // Skip the separater
                    while (line.Length > i && (((int)line[i]) is < 0x21 or 0x7f))
                        i++;

                    // indexOfBegin
                    atIndex = i--;
                    parent = null;
                    frame = null;
                }
                else
                {
                    // Part of the name
                    if (elementType == ElementType.None)
                        elementType = ElementType.Name;
                }

                string AddPathOption()
                {
                    int indexOfBegin = ++i;
                    char chainChar = '\0';

                    for (; i < line.Length; i++)
                    {
                        char c = line[i];
                        if (chainChar != '\0')
                        {
                            if (c == chainChar)
                                chainChar = '\0';

                            continue;
                        }
                        else if (c is '\'' or '\"')
                        {
                            chainChar = c;
                            continue;
                        }
                        else if (((int)c) is < 0x21 or 0x7f)
                        {
                            return line.Substring(indexOfBegin, i - indexOfBegin);
                        }
                    }
                    return line.Substring(indexOfBegin);
                }
            }

            // Pre process1
            FinishPath(line.Length);

            void FinishPath(int lastIndex)
            {
                ChainLinkedPath(lastIndex);

                if (prefix != null || suffix != null)
                {
                    ((IPathFrameInner)frame).SetAttributes("", prefix, suffix);
                    prefix = suffix = null;
                }
            }

            void SetElement(int lastIndex, ElementType type = ElementType.None)
            {
                // 処理タイプを取得
                if (type == ElementType.None && elementType != ElementType.None)
                    type = elementType;

                if (type != ElementType.None)
                {
                    // 対象文字数
                    var count = lastIndex - atIndex;
                    if (count > 0)
                    {
                        // 対象文字列
                        var elementString = line.Substring(atIndex, count);

                        // 対象にセット
                        if (type is ElementType.Name or ElementType.Parent)
                            name = elementString;

                        else if (type == ElementType.Prefix)
                            prefix = elementString.Trim(new[] { '\'', '\"' });

                        else if (type == ElementType.Suffix)
                            suffix = elementString.Trim(new[] { '\'', '\"' });
                    }

                    // reset type
                    elementType = ElementType.None;
                }
            }

            // Child Path
            void ChainLinkedPath(int lastIndex, bool isForce = false)
            {
                // 事前セット
                SetElement(lastIndex);

                if (!string.IsNullOrEmpty(name) || (path == null && isForce))
                {
                    // Create path.
                    path = new PathMember(name, frame, parent, null);

                    // CreateRoot
                    if (frame == null)
                        frame = path.Frame;

                    if (parent == null)
                        results.Add(path);
                    else
                        parent.SetChild(path);

                    parent = path;
                    name = null;
                }
            }

            // Results DLinkedPathCollection 
            return results;
        }

        static PathMember _labeling(string label)
        {
            int indexOfSeparate = label.IndexOf(':');
            if (indexOfSeparate >= 0)
            {
                PathMember path = new PathMember(label.Substring(0, indexOfSeparate), null, null, null);
                ((IPathFrameInner)path.Frame).SetAttributes(label.Substring(indexOfSeparate + 1));
                return path;
            }
            else
            {
                return new PathMember(label, new PathFrame(), null, null);
            }
        }

        /// <summary>
        /// Create PathMember with formatting only.
        /// </summary>
        /// <param name="format"></param>
        /// <returns></returns>
        static PathMember _formatting(string format)
        {
            // Init PathMember.
            var pathMember = new PathMember();

            // Add format.
            pathMember.Frame.Attributes.Add(new PathAttribute("format", format));

            return pathMember;
        }
    }
}
