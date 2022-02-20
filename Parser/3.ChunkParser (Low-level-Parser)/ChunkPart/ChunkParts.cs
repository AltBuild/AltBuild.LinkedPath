using System.Text;
using System.Collections.Generic;

namespace AltBuild.LinkedPath.Parser
{
    public class ChunkParts : ChunkPart
    {
        public ChunkParts() : base(ChunkPartType.Parts)
        {
        }

        internal void Trim()
        {
            if (Paths.Count > 0)
            {
                var lastItem = Paths[^1];
                if (lastItem.ElementsReady == false && string.IsNullOrWhiteSpace(lastItem.Line.ToString()))
                    Paths.RemoveAt(Paths.Count - 1);
            }
        }

        public List<ChunkPartPath> Paths { get; } = new();

        public ChunkPartPath CurrentMember => Paths.Count > 0 ? Paths[Paths.Count - 1] : null;

        public override string ToString()
        {
            var bild = new StringBuilder();

            foreach (var part in Elements)
                _toDescription(part);

            return bild.ToString();

            void _toDescription(ChunkPart atPart)
            {
                bild.Append($"{new string(' ', atPart.Depth)}{atPart} /{atPart.GetType().Name}\r\n");
                foreach (var part in atPart.Elements)
                    _toDescription(part);
            }
        }
    }
}
