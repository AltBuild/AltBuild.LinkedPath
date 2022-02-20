using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AltBuild.LinkedPath.Parser
{
    public class ChunkPartValues : ChunkPart, IEnumerable<ChunkPartValue>
    {
        protected override string AcceptChars { get; } = ",";

        public ChunkPartValues() : base(ChunkPartType.Values)
        {
        }

        public object[] GetValues()
        {
            var values = new List<object>(Elements.Count);

            if (ElementsReady)
                foreach (var part in Elements)
                    if (part is ChunkPartValue value)
                        values.Add(value.Value);

            return values.ToArray();
        }

        /// <summary>
        /// ElementPartValue のみを抽出する
        /// </summary>
        /// <returns></returns>
        public new IEnumerator<ChunkPartValue> GetEnumerator()
        {
            if (ElementsReady)
                foreach (var part in Elements)
                    if (part is ChunkPartValue value)
                        yield return value;
        }

        /// <summary>
        /// ElementPartValue のみを列挙する
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            if (ElementsReady)
                foreach (var part in Elements)
                    if (part is ChunkPartValue value)
                        yield return value;
        }
    }
}
