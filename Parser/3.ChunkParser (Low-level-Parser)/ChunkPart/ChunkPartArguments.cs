namespace AltBuild.LinkedPath.Parser
{
    public class ChunkPartArguments : ChunkPart
    {
        protected override string AcceptChars { get; } = ",";

        public ChunkPartArguments() : base(ChunkPartType.Arguments)
        {
        }
    }
}
