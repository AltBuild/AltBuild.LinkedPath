namespace AltBuild.LinkedPath.Parser
{
    public class ChunkPartOptions : ChunkPart
    {
        const string ctrlChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz+-*/0123456789_;";

        public ChunkPartOptions() : base(ChunkPartType.Options)
        {
        }

        public override void Parse(ChunkParser parser)
        {
            while (parser.Read())
            {
                var atChar = parser.Current;

                // Add name char
                if (atChar > 0xff || ctrlChars.Contains(atChar))
                {
                    Line.Append(atChar);
                }

                else
                {
                    // Redo: 1Char
                    parser.Redo();
                    break;
                }
            }

            parser.Canopy = Parent;
        }
    }
}
