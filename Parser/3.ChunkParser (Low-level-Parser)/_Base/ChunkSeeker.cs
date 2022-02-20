using System;
using System.Runtime.CompilerServices;

namespace AltBuild.LinkedPath.Parser
{
    public class ChunkSeeker
    {
        public string Line { get; }

        int Index = -1;

        public char Current => Index >= 0 ? Line[Index] : '\0';

        public char Next => IsNext ? Line[Index + 1] : '\0';

        public bool IsNext => Line.Length > (Index + 1);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Read()
        {
            if (Line.Length > Index + 1)
            {
                Index++;
                return true;
            }

            else
            {
                return false;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryOffsetIndex(int offset)
        {
            Index += offset;

            if (Index < -1)
            {
                Index = -1;
                return false;
            }

            else if (Line.Length <= Index)
            {
                Index = Line.Length - 1;
                return false;
            }

            return true;
        }

        public bool TryMatch(string value) =>
            (value != null && Line.Substring(Index, Math.Min(Line.Length - Index, value.Length)) == value);

        public int LimitedIndexOf(char searchChar, string endChars)
        {
            // Allowed character string
            string limitLine = Line.Substring(Index, Line.Length - Index);

            // Set limitter
            foreach (char c in endChars)
            {
                var index = limitLine.IndexOf(c);
                if (index >= 0)
                    limitLine = limitLine.Substring(0, index);
            }

            return limitLine.IndexOf(searchChar);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Redo()
        {
            Index--;
        }

        public ChunkSeeker(string line)
        {
            Line = line;
        }
    }
}
