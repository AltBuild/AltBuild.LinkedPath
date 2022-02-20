using System;

namespace AltBuild.LinkedPath.Parser
{
    [Flags]
    public enum ChunkPartType
    {
        Empty = default,

        Parts = 0x_0000_0001,
        Path = 0x_0000_0002,
        Name = 0x_0000_0004,
        Into = 0x_0000_0008,
        Sub = 0x_0000_0010,

        Arguments = 0x_0000_0100,
        Indexers = 0x_0000_0200,
        Operator = 0x_0000_0400,
        Values = 0x_0000_0800,

        Attributes = 0x_0000_1000,
        Options = 0x_0000_2000,

        IntoName = Into | Name,
        AttributeName = Attributes | Name,
        MemberName = Path | Name,

        // Value (string base)
        String = 0x_0001_0000,
        Guid = 0x_0002_0000,

        // Value (number)
        Number = 0x_0010_0000,
        Decimal = 0x_0020_0000,

        // Value (native)
        Bool = 0x_0100_0000,
    }
}
