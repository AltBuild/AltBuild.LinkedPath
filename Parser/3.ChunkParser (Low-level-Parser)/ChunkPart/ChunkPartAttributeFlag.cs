using System;
using System.Text;

namespace AltBuild.LinkedPath.Parser
{
    public class ChunkPartAttributeFlag : ChunkPartAttributeValueBase
    {
        protected override string AcceptChars { get; } = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789_-+#%$,";

        public ChunkPartAttributeFlag(ChunkPartType type) : base(type) { }
    }
}
