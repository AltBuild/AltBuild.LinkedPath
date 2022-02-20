using System;
using System.Text;

namespace AltBuild.LinkedPath.Parser
{
    public class ChunkPartName : ChunkPart
    {
        public ChunkPartName() : base(ChunkPartType.Name)
        {
        }

        public ChunkPartName(ChunkPartType type) : base(type)
        {
        }

        protected virtual string ControlChars => throw new InvalidProgramException();

        protected virtual bool IsAcceptOtherChars => true;

        public ChunkPartOptions Options { get; internal set; }

        public string FullName
        {
            get
            {
                StringBuilder builder = new();
                FullLine(builder, ChunkPartType.MemberName | ChunkPartType.IntoName | ChunkPartType.AttributeName);
                return builder.ToString();
            }
        }

        public string GetFullNameWithNext(out ChunkPart nextPart)
        {
            StringBuilder build = new();
            var lastPart = FullLine(build, ChunkPartType.MemberName | ChunkPartType.IntoName | ChunkPartType.AttributeName);
            nextPart = lastPart.First;
            return build.ToString();
        }

        public override void Parse(ChunkParser parser)
        {
            while (parser.Read())
            {
                var atChar = parser.Current;

                // Add name char
                if ((atChar > 0xff && IsAcceptOtherChars) || ControlChars.Contains(atChar))
                {
                    Line.Append(atChar);
                }

                // Etc break;
                else
                {
                    // Redo: 1Char
                    parser.Redo();
                    break;
                }
            }

            parser.Canopy = this;
        }
    }
}
