using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Diagnostics;

namespace AltBuild.LinkedPath.Parser
{
    public class ChunkPart : IEnumerable<ChunkPart>
    {
        public ChunkPartType Type { get; internal set; }

        public ChunkPart Parent { get; internal set; }

        internal IList<ChunkPart> Elements => _elements ??= new List<ChunkPart>(0);
        List<ChunkPart> _elements;

        public bool ElementsReady => _elements != null && _elements.Count > 0;

        public StringBuilder Line { get; } = new();

        public string Source => Line.ToString();

        public int Depth => (Parent == null ? 0 : 1 + Parent.Depth);

        protected virtual string AcceptChars { get; }

        protected virtual string ValueChars { get; } = "0123456789-+.";

        public ChunkPart(ChunkPartType type)
        {
            Type = type;
        }

        public ChunkPart FullLine(StringBuilder builder, ChunkPartType types = ChunkPartType.Empty)
        {
            ChunkPart lastPart = null;

            if (types == ChunkPartType.Empty || types.HasFlag(Type))
            {
                builder.Append(Line);

                if (ElementsReady)
                {
                    foreach (var atPart in Elements)
                        lastPart = atPart.FullLine(builder, types);
                }

                return lastPart ?? this;
            }

            return null;
        }

        public ChunkPart First => ElementsReady ? _elements[0] : null;

        public ChunkPart Last => ElementsReady ? _elements[^1] : null;

        public bool TryGetFirst(out ChunkPart part, ChunkPartType types)
        {
            var first = First;

            if (types.HasFlag(first.Type))
            {
                part = first;
                return true;
            }

            else
            {
                part = default;
                return false;
            }
        }

        public bool TryGetFirst<T>(out T part)
        {
            if (First is T atFirst)
            {
                part = atFirst;
                return true;
            }

            else
            {
                part = default;
                return true;
            }
        }

        public bool TryGet<T>(out T part)
        {
            if (ElementsReady)
            {
                foreach (var atPart in Elements)
                {
                    if (atPart is T result)
                    {
                        part = result;
                        return true;
                    }
                }
            }

            part = default(T);
            return false;
        }

        public bool TryGetRoot(out ChunkPart root)
        {
            if (Parent == null)
            {
                root = this;
                return true;
            }
            else
            {
                return Parent.TryGetRoot(out root);
            }
        }

        public bool TryGetDescendants<T>(out T part)
        {
            if (ElementsReady)
            {
                foreach (var atPart in Elements)
                {
                    if (atPart is T result)
                    {
                        part = result;
                        return true;
                    }

                    else if (atPart.TryGetDescendants(out part))
                        return true;
                }
            }

            part = default(T);
            return false;
        }

        public bool TryGetAncestor<T>(out T part)
        {
            if (this is T result)
            {
                part = result;
                return true;
            }
            else if (Parent != null && Parent.TryGetAncestor(out part))
            {
                return true;
            }
            else
            {
                part = default(T);
                return false;
            }
        }

        public bool TryGetAncestor(ChunkPartType[] types, out ChunkPart part)
        {
            if (types.Contains(Type))
            {
                part = this;
                return true;
            }
            else if (Parent != null && Parent.TryGetAncestor(types, out part))
            {
                return true;
            }
            else
            {
                part = null;
                return false;
            }
        }
        public bool TryGetAncestor(char atChar, out ChunkPart part)
        {
            if (AcceptChars != null && AcceptChars.Contains(atChar))
            {
                part = this;
                return true;
            }
            else if (Parent != null && Parent.TryGetAncestor(atChar, out part))
            {
                return true;
            }
            else
            {
                part = null;
                return false;
            }
        }


        public virtual void Parse(ChunkParser parser)
        {
            while (parser.Canopy != null && parser.Read())
                parser.Canopy._parse(parser);
        }

        protected void _parse(ChunkParser parser)
        {
            // Get at char.
            char atChar = parser.Current;

            // ASCII <= 0x7f
            if (atChar <= 0x7f)
            {
                // Next to String
                if (atChar is '\'' or '\"')
                {
                    NextChildPart(new ChunkPartValue(ChunkPartType.String), parser);
                    return;
                }

                // Next to Value
                else if (atChar is (>= '0' and <= '9') or '-' or '+')
                {
                    if (TryGetAncestor(new[] { ChunkPartType.Attributes, ChunkPartType.Indexers, ChunkPartType.Arguments }, out ChunkPart atPart))
                    {
                        if (atPart is ChunkPartAttributes)
                        {
                            if (Type == ChunkPartType.Attributes)
                                NextChildPart(new ChunkPartAttributeFlag(ChunkPartType.String), parser);
                            else
                                NextChildPart(new ChunkPartAttributeValue(ChunkPartType.String), parser);

                            return;
                        }
                    }

                    // Other env.
                    NextChildPart(new ChunkPartValue(ChunkPartType.Number), parser);
                    return;
                }

                // Next into
                else if (atChar is '.')
                {
                    NextChildPart(new ChunkPartIntoName(), parser);
                    return;
                }

                else if ((atChar is 't') && parser.TryMatch("true"))
                {
                    NextChildPart(new ChunkPartValue(ChunkPartType.Bool), parser, "true");
                    return;
                }

                else if ((atChar is 'f') && parser.TryMatch("false"))
                {
                    NextChildPart(new ChunkPartValue(ChunkPartType.Bool), parser, "false");
                    return;
                }

                // Next to Sub member
                else if (atChar is '|')
                {
                    parser.CurrentMember.NextChildPart(CreatePartName(), parser, "");
                    return;
                }

                // Next to Attributes
                else if (atChar is ':')
                {
                    var part = parser.CurrentMember.Attributes = new ChunkPartAttributes();
                    parser.CurrentMember.NextChildPart(part, parser);
                    return;
                }

                // Next to ...
                else if (atChar is ',')
                {
                    if (TryGetAncestor(',', out ChunkPart part))
                    {
                        parser.Canopy = part;
                    }
                    else
                    {
                        parser.NextNewMember();
                    }
                    return;
                }

                // Next to Parent
                else if (atChar is ' ' or '\r' or '\n')
                {
                    if (Line.Length > 0)
                    {
                        if (Type != ChunkPartType.Values)
                            parser.NextNewMember();
                    }
                    return;
                }

                // Next to Indexers
                else if (atChar is '[')
                {
                    // Use Member to Indexers
                    if (Type == ChunkPartType.MemberName)
                    {
                        NextChildPart(new ChunkPartIndexers(), parser);
                    }

                    // Use Single to Values
                    else
                    {
                        NextChildPart(new ChunkPartValues(), parser);
                    }
                    return;
                }

                // Next to Parent.
                else if (atChar is ']')
                {
                    if (TryGetAncestor(new[] { ChunkPartType.Indexers, ChunkPartType.Values }, out ChunkPart part))
                    {
                        part.Line.Append(atChar);
                        parser.Canopy = part.Parent;
                    }
                    return;
                }

                // Next to Arguments
                else if (atChar is '(')
                {
                    NextChildPart(new ChunkPartArguments(), parser);
                    return;
                }

                // Next to Parent.
                else if (atChar is ')')
                {
                    if (TryGetAncestor(out ChunkPartArguments args))
                    {
                        args.Line.Append(atChar);
                        parser.Canopy = args.Parent;
                    }
                    return;
                }

                // Next to Operator "="
                else if (atChar is '=')
                {
                    NextChildPart(new ChunkPartOperator(PathOperatorType.Equal), parser);
                    return;
                }

                // '<'
                else if (atChar is '<')
                {
                    // Static Custom
                    if (Line.Length == 0 && this is ChunkPartPath thisPartPath)
                    {
                        foreach (var staticMarkup in PathFactory.StaticMarkups)
                        {
                            if (parser.TryMatch(staticMarkup))
                            {
                                parser.TryOffsetIndex(staticMarkup.Length - 1);
                                thisPartPath.StaticMarkup = true;
                                Line.Append(staticMarkup);
                                return;
                            }
                        }
                    }

                    // Next to Options '<='
                    if (parser.Next is '=')
                        NextChildPart(new ChunkPartOperator(PathOperatorType.SmallerOrEqual), parser, "<=");

                    // Next to Operator "<"
                    else
                        NextChildPart(new ChunkPartOperator(PathOperatorType.Smaller), parser);

                    return;
                }

                // '>'
                else if (atChar is '>')
                {
                    // Next to Options '>='
                    if (parser.Next is '=')
                        NextChildPart(new ChunkPartOperator(PathOperatorType.BiggerOrEqual), parser, ">=");

                    // Next to Operator ">"
                    else
                        NextChildPart(new ChunkPartOperator(PathOperatorType.Bigger), parser);

                    return;
                }

                // '!='
                else if ((atChar is '!') && parser.Next is '=')
                {
                    // Next to Operator "!="
                    NextChildPart(new ChunkPartOperator(PathOperatorType.Exclusion), parser, "!=");
                    return;
                }

                // Next to Options
                else if (atChar is '!')
                {
                    if (TryGetAncestor(out ChunkPartName partName))
                    {
                        partName.NextChildPart(partName.Options = new ChunkPartOptions(), parser);
                    }
                    else
                    {
                        partName = CreatePartName() as ChunkPartName;
                        Attach(partName);
                        partName.NextChildPart(partName.Options = new ChunkPartOptions(), parser);
                    }
                    return;
                }
            }

            // Other char...
            NextChildPart(CreatePartName(), parser);

            ChunkPart CreatePartName()
            {
                if (Type == ChunkPartType.Attributes)
                    return new ChunkPartAttributeName();

                else if (Type == ChunkPartType.Operator && Parent?.Type == ChunkPartType.AttributeName)
                    return new ChunkPartAttributeValue(ChunkPartType.String);

                else
                    return new ChunkPartMemberName();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void NextChildPart(ChunkPart part, ChunkParser parser)
        {
            // Set begin char.
            part.Line.Append(parser.Current);

            // Attach part with Next run.
            parser.SetNext(Attach(part));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void NextChildPart(ChunkPart part, ChunkParser parser, string atString)
        {
            // Seeker offset.
            if (atString.Length > 1)
                parser.TryOffsetIndex(atString.Length - 1);

            // Set begin string.
            if (string.IsNullOrEmpty(atString) == false)
                part.Line.Append(atString);

            // Attach part with Next run.
            parser.SetNext(Attach(part));
        }

        protected internal virtual T Attach<T>(T part) where T: ChunkPart
        {
            Debug.Assert(part != null);

            // Set parent (this)
            part.Parent = this;

            // Set child part.
            Elements.Add(part);

            // Return part.
            return part;
        }

        public override string ToString()
        {
            StringBuilder bild = new();

            bild.Append($"Line=\"{Line}\" Type={Type}");

            if (ElementsReady)
                bild.Append($" Items={Elements.Count}");

            return bild.ToString();
        }

        public IEnumerator<ChunkPart> GetEnumerator()
        {
            if (ElementsReady)
                return ((IEnumerable<ChunkPart>)Elements).GetEnumerator();
            else
                return Enumerable.Empty<ChunkPart>().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            if (ElementsReady)
                return ((IEnumerable)Elements).GetEnumerator();
            else
                return Enumerable.Empty<ChunkPart>().GetEnumerator();
        }
    }
}
