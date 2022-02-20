namespace AltBuild.LinkedPath.Parser
{
    public class ChunkPartIntoName : ChunkPartName
    {
        const string _ctrlChars = ".";

        protected override string ControlChars => _ctrlChars;

        protected override bool IsAcceptOtherChars => false;

        public ChunkPartIntoName() : base(ChunkPartType.IntoName) { }
    }
}
