using AltBuild.BaseExtensions;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AltBuild.LinkedPath
{
    /// <summary>
    /// 属性のキーと値を複数保持（カンマ区切り= ','）
    /// 例１： class='text-success fw-bold',disabled
    /// </summary>
    public class PathAttributeCollection : List<PathAttribute>
    {
        /// <summary>
        /// 接頭辞
        /// </summary>
        public string Prefix { get; }

        /// <summary>
        /// 接尾辞
        /// </summary>
        public string Suffix { get; }

        public bool IsExist(string key) =>
            TryGetValue(key, out string optionValue);

        public bool Include(string key, string value)
        {
            var index = base.FindIndex(o => o.Key == key);
            if (index >= 0)
            {
                base.RemoveAt(index);
                return false;
            }
            else
            {
                base.Add(new PathAttribute(key, value));
                return true;
            }
        }

        public bool TryGetFormat(out string format)
        {
            // Get format only string
            if (Count == 1 && this[0].Type == PathKeyValueType.Key)
            {
                format = this[0].Key;
                return true;
            }

            // Get format value string
            return TryGetValue("format", out format);
        }

        /// <summary>
        /// 指定のキーを取得
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        public PathAttribute GetExists(IEnumerable<string> keys) =>
            this.FirstOrDefault(o => keys.Contains(o.Key));

        public bool IsExist(string key, string value) =>
            TryGetValue(key, out string optionValue) && optionValue.IndexOf(value) >= 0;

        public bool TryGetEnumFlags<T>(string key, out T enumValue)
            where T: struct, Enum
        {
            int target = default;
            if (TryGetValue(key, out string value))
            {
                foreach (var targetName in value.Split('|', StringSplitOptions.RemoveEmptyEntries))
                {
                    if (Enum.TryParse(targetName.Trim(), out T atValue))
                    {
                        dynamic dynValue = atValue;
                        int intValue = (int)dynValue;
                        target |= (int)dynValue;
                    }
                }
                enumValue = (T)Enum.ToObject(typeof(T), target);
                return true;
            }
            else
            {
                enumValue = default;
                return false;
            }
        }

        public T? GetEnum<T>(string key, T defaultValue = default)
            where T : struct, Enum
        {
            if (TryGetValue(key, out string value))
                if (Enum.TryParse(value, out T enumValue))
                    return enumValue;
                else
                    return defaultValue;

            return null;
        }

        public bool TryGetEnum<T>(string key, out T value)
            where T: struct, Enum
        {
            T? results = GetEnum<T>(key);
            value = results ?? default;
            return (results != null);
        }

        public bool TryGetValue(string key, out string value)
        {
            // 指定のキーを抽出
            var results = Find(o => o.Key == key);

            if (results == null)
            {
                value = null;
                return false;
            }
            else
            {
                value = results.Value;
                return true;
            }
        }

        /// <summary>
        /// 指定のキーの値を取得する
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string this[string key] => Find(o => o.Key == key)?.Value;

        public StringBuilder ToStringBuilder(StringBuilder builder) => this.ToStringBuilder(chain: ",", prefix: ":", builder: builder);
        public override string ToString() => this.ToStringBuilder(chain: ",", prefix: ":").ToString();

        public string ToString(PathKeyValueType keyValueType)
        {
            StringBuilder bild = new ();

            foreach (var kvItem in this)
                if (kvItem.Type == keyValueType)
                    bild.Chain(',').Append(kvItem.ToString());

            return bild.ToString();
        }

        public PathAttributeCollection Add(string line)
        {
            // KeyValue を追加する
            PathHelper.CreateKeyValues(line, (key, value) =>
            {
                // 結果
                List<string> classItems = new List<string>();

                // 既存値を分割
                PathAttribute pathAttribute = Find(o => o.Key == key);
                if (pathAttribute == null)
                    base.Add((pathAttribute = new PathAttribute(key, value)));

                else
                    pathAttribute.AddValue(value);
            });

            // 自身を返す
            return this;
        }

        public PathAttributeCollection() { }

        public PathAttributeCollection(string line, string prefix = null, string suffix = null)
        {
            // 元情報
            Prefix = prefix;
            Suffix = suffix;

            // Create keyValues
            Add(line);
        }

        public static PathAttributeCollection Parse(string line) => new PathAttributeCollection(line);
    }
}
