using System;
using System.Text;

namespace AltBuild.LinkedPath.Parser
{
    public class ChunkPartAttributeValueBase : ChunkPartValue
    {
        public ChunkPartAttributeValueBase(ChunkPartType type) : base(type) { }

        public override void Parse(ChunkParser parser)
        {
            char bkChar = (parser.Current is '\'' or '\"') ? parser.Current : '\0'; 

            while (parser.Read())
            {
                var atChar = parser.Current;

                if (atChar == bkChar)
                {
                    break;
                }
                else if (atChar < 0xff && AcceptChars.Contains(atChar) == false)
                {
                    parser.Redo();
                    break;
                }

                Line.Append(atChar);
            }

            if (bkChar != '\0')
                Line.Remove(0, 1);

            Value = Line.ToString();

            if (TryGetAncestor(out ChunkPartAttributes attributes))
                parser.Canopy = attributes;
        }
    }
}
