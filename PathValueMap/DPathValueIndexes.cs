using AltBuild.BaseExtensions;
using AltBuild.LinkedPath;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Reflection;

namespace AltBuild.LinkedPath
{
    public class DPathValueIndexes : IReadOnlyList<int>, IPathValueIndexes
    {
        List<int> Indexes = new List<int>();

        /// <summary>
        /// 変更されたインデクセス深度を取得する
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int GetIndexOfChangeDepth(DPathValueIndexes other)
        {
            var count = Math.Min(Count, other.Count);

            for (int i = 0; i < count; i++)
                if (this[i] != other[i])
                    return i;

            // 全て一致
            return -1;
        }


        public int this[int index] => ((IReadOnlyList<int>)Indexes)[index];

        public int Count => Indexes.Count;

        int IPathValueIndexes.Count => Indexes.Count;

        void IPathValueIndexes.Add(int index) => Indexes.Add(index);

        public IEnumerator<int> GetEnumerator()
        {
            return ((IEnumerable<int>)Indexes).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)Indexes).GetEnumerator();
        }
    }
}
