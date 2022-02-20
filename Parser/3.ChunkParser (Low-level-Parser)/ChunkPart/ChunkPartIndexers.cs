namespace AltBuild.LinkedPath.Parser
{
    public class ChunkPartIndexers : ChunkPart
    {
        protected override string AcceptChars { get; } = ",";

        public ChunkPartIndexers() : base(ChunkPartType.Indexers)
        {
        }
    }
}
