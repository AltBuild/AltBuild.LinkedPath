namespace AltBuild.LinkedPath.Parser
{
    public class ChunkPartOperator : ChunkPart
    {
        public PathOperatorType Operator { get; init; }

        public ChunkPartOperator(PathOperatorType op) : base(ChunkPartType.Operator)
        {
            Operator = op;
        }

        public override string ToString() =>
            $"{base.ToString()} Operator={Operator}";
    }
}
