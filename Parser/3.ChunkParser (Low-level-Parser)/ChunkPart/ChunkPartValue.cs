using System;

namespace AltBuild.LinkedPath.Parser
{
    public class ChunkPartValue : ChunkPart
    {
        public object Value { get; protected set; }

        public ChunkPartValue(ChunkPartType type) : base(type) { }

        public override void Parse(ChunkParser parser)
        {
            if (Type == ChunkPartType.String)
            {
                char markChar = Line[0];

                while (parser.Read())
                {
                    var atChar = parser.Current;
                    Line.Append(atChar);

                    if (atChar == markChar)
                        break;
                }

                var value = Line.ToString().Trim(markChar);
                Line.Clear().Append(value);

                if (Guid.TryParse(value, out Guid guid))
                {
                    Type = ChunkPartType.Guid;
                    Value = guid;
                }
                else
                {
                    Value = value;
                }
            }

            else if (Type == ChunkPartType.Bool)
            {
                var line = Line.ToString();
                if (bool.TryParse(line, out bool boolValue))
                    Value = boolValue;
            }

            else
            {
                while (parser.Read())
                {
                    var atChar = parser.Current;

                    // Target chars
                    if (atChar is (>= '0' and <= '9') or '-' or '+' or '.')
                    {
                        Line.Append(atChar);

                        if ((atChar is '.') && Type == ChunkPartType.Number)
                            Type = ChunkPartType.Decimal;
                    }

                    // Not covered
                    else
                    {
                        parser.Redo();
                        break;
                    }
                }

                var line = Line.ToString();

                if (int.TryParse(line, out int intValue))
                    Value = intValue;

                else if (decimal.TryParse(line, out decimal decimalValue))
                    Value = decimalValue;
            }

            parser.Canopy = Parent;
        }

        public override string ToString()
        {
            var type = Value?.GetType();

            if (type == typeof(string) || type == typeof(Guid))
            {
                return $"{base.ToString()} Value='{Value}'";
            }

            else
            {
                return $"{base.ToString()} Value={Value}";
            }
        }
    }
}
