using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace AltBuild.LinkedPath.Parser
{
    /// <summary>
    /// パス文字列を解析する
    /// パス文字列をキャッシュさせる為、パス文字列＝静的解析結果以外の動的データを保持してはならない。
    /// </summary>
    public class ChunkParser : ChunkSeeker
    {
        public ChunkParts Parts { get; init; }

        public ChunkPart Canopy { get; internal set; }

        public ChunkPartPath CurrentMember => Parts.CurrentMember;

        public ChunkParser(string line, ChunkParts root = null) : base(line)
        {
            Canopy = Parts = root ?? new ChunkParts();
        }

        public void SetNext(ChunkPart nextPart)
        {
            Canopy = nextPart;
            Canopy.Parse(this);
        }

        internal ChunkPartPath NextNewMember()
        {
            // Create member part.
            var pathPart = new ChunkPartPath() { Parent = Parts };

            // Add member part.
            Parts.Paths.Add(pathPart);

            // Extract and process one character at a time
            SetNext(Parts.Attach(pathPart));

            // Return member part.
            return pathPart;
        }

        public static ChunkParts Parse(string line)
        {
            // Create parse main
            var parser = new ChunkParser(line);

            if (string.IsNullOrWhiteSpace(line) == false)
            {
                // Next new member part.
                parser.NextNewMember();

                // Trim path.
                parser.Parts.Trim();
            }

            // Return root
            return parser.Parts;
        }
    }
}
