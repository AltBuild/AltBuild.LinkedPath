using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace AltBuild.LinkedPath.Parser
{
    /// <summary>
    /// Path string member list class.
    /// </summary>
    public class PathMemberCollection : IEnumerable<PathMember>
    {
        List<PathMember> items = new List<PathMember>();

        public PathMemberCollection() { }

        public PathMemberCollection(string line)
        {
            foreach (var partMember in ChunkParser.Parse(line).Paths)
                items.Add(PathMember.Parse(partMember));
        }

        public PathMember GetFirst()
        {
            if (items.Count > 0)
                return items[0];
            else
                return new PathMember();
        }

        public bool ExistsFix
        {
            get
            {
                foreach (var item in items)
                    if (item.Frame.Attributes.IsExist("prefix") || item.Frame.Attributes.IsExist("suffix"))
                        return true;

                return false;
            }
        }

        public void Add(PathMember path) => items.Add(path);

        public PathMember this[int index] => items[index];

        /// <summary>
        /// 配列の数
        /// </summary>
        public int Count => items.Count;

        public void Clear() => items.Clear();

        /// <summary>
        /// 文字列に変換
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            // 結果保持
            StringBuilder bild = new StringBuilder();

            foreach (var nameFlag in items)
                bild.Append(' ').Append(nameFlag.ToString());

            // 結果を出力
            return bild.ToString();
        }

        public IEnumerator<PathMember> GetEnumerator()
        {
            return ((IEnumerable<PathMember>)items).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<PathMember>)items).GetEnumerator();
        }
    }
}
