using System;
using System.Text;

namespace AltBuild.LinkedPath.Parser
{
    public class ChunkPartAttributeValue : ChunkPartAttributeValueBase
    {
        protected override string AcceptChars { get; } = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789_-";

        public ChunkPartAttributeValue(ChunkPartType type) : base(type) { }
    }
}
