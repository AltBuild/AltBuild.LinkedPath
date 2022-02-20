namespace AltBuild.LinkedPath.Parser
{
    public class ChunkPartAttributes : ChunkPart
    {
        protected override string AcceptChars { get; } = ",";

        public ChunkPartAttributes() : base(ChunkPartType.Attributes) { }

        public override void Parse(ChunkParser parser)
        {
            //if (parser.LimitedIndexOf('=', " \r\n\t") >= 0)
            //{
                base.Parse(parser);
            //}

            // AttributeFlag
            //else
            //{
                // Next read...
                //parser.Read();
                //NextChildPart(new ChunkPartAttributeFlag(ChunkPartType.String), parser);
            //}
        }
    }
}