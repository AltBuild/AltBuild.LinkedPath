using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AltBuild.LinkedPath.Parser
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

        public bool IsExist(string key)
        {
            var format = Find(o => o.Key == "format");
            if (format != null)
            {
                if (format.Line != null && format.Line.Contains(key))
                    return true;
            }

            return TryGetValue(key, out string optionValue);
        }

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
                value = results.Line;
                return true;
            }
        }

        /// <summary>
        /// 指定のキーの値を取得する
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string this[string key] => Find(o => o.Key == key)?.Line;

        public StringBuilder ToStringBuilder(StringBuilder builder)
        {
            if (builder == null)
                builder = new StringBuilder();

            if (Count > 0)
            {
                builder.Append(':');

                for (int i = 0; i < Count; i++)
                {
                    if (i > 0)
                        builder.Append(',');

                    builder.Append(this[i].ToString());
                }
            }

            return builder;
        }
        public override string ToString() => ToStringBuilder(null).ToString();

        public string ToString(PathKeyValueType keyValueType)
        {
            StringBuilder bild = new ();

            foreach (var kvItem in this)
            {
                if (kvItem.Type == keyValueType)
                {
                    if (bild.Length > 0)
                        bild.Append(',');

                    bild.Append(kvItem);
                }
            }

            return bild.ToString();
        }

        public PathAttributeCollection Add(string line)
        {
            // KeyValue を追加する
            InductHelper.CreateKeyValues(line, (key, value, bString) =>
            {
                // 結果
                List<string> classItems = new List<string>();

                // 既存値を分割
                PathAttribute pathAttribute = Find(o => o.Key == key);
                if (pathAttribute == null)
                    base.Add((pathAttribute = new PathAttribute(key, value, bString)));

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

        public PathAttributeCollection(ChunkPartAttributes attributes)
        {
            foreach (var atPart in attributes)
            {
                if (atPart is ChunkPartName partName)
                {
                    if (partName.TryGetDescendants(out ChunkPartOperator partOperator))
                    {
                        // ABC='123'
                        if (partOperator.TryGetDescendants(out ChunkPartValue partValue))
                            AddAttribute(partName.FullName, partValue.Value);

                        continue;
                    }
                    AddAttribute(partName.FullName, null);
                }

                else if (atPart is ChunkPartValue partValue)
                {
                    AddAttribute("format", partValue.Line.ToString());
                }
            }

            void AddAttribute(string name, object? value)
            {
                Add(new PathAttribute(
                    name,
                    value?.ToString(),
                    (value?.GetType() == typeof(string))));
            }
        }

        public static PathAttributeCollection Parse(string line) => new PathAttributeCollection(line);
    }
}
