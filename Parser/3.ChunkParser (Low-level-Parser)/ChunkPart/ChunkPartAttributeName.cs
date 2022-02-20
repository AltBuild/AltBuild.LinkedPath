using System;
using System.Text;

namespace AltBuild.LinkedPath.Parser
{
    public class ChunkPartAttributeName : ChunkPartName
    {
        const string _ctrlChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789_-";

        protected override string ControlChars => _ctrlChars;

        public ChunkPartAttributeName() : base(ChunkPartType.AttributeName) { }
    }
}
