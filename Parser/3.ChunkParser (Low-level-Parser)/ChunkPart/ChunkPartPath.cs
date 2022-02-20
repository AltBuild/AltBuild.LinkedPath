using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace AltBuild.LinkedPath.Parser
{
    public class ChunkPartPath : ChunkPart
    {
        public ChunkPartAttributes Attributes { get; internal set; }

        public List<ChunkPartName> Names { get; } = new List<ChunkPartName>();

        public ChunkPartName FirstName => Names.Count > 0 ? Names[0] : null;

        public bool StaticMarkup { get; internal set; }

        public ChunkPartPath() : base(ChunkPartType.Path)
        {
        }

        protected internal override T Attach<T>(T part)
        {
            // Set attach part
            base.Attach(part);

            // Add name.
            if (part is ChunkPartName atName)
                Names.Add(atName);

            // Return part.
            return part;
        }
    }
}
