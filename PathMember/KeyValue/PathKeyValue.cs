using AltBuild.BaseExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AltBuild.LinkedPath
{
    /// <summary>
    /// キーと値：解析前（Key, StringValue）
    /// </summary>
    public partial class PathKeyValue
    {
        /// <summary>
        /// KVType
        /// </summary>
        public PathKeyValueType Type { get; }

        /// <summary>
        /// Key
        /// </summary>
        public string Key { get; }

        /// <summary>
        /// Value (string type, non processing value)
        /// </summary>
        public string Value { get; private set; }

        /// <summary>
        /// 解析結果の値
        /// </summary>
        public PathDataCollection Results { get; set; }

        public void AddValue(string line)
        {
            StringBuilder bild = new StringBuilder();

            // 既存値を追加
            if (string.IsNullOrWhiteSpace(Value) == false)
                bild.Append(Value);

            // 追加値を追加
            var split = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var atElement in split)
                bild.Chain(' ').Append(atElement);

            Value = bild.ToString();
        }

        public PathKeyValue(string key, string value)
        {
            // Exist Key.
            if (key != null)
            {
                // Exist Key & Value
                if (value != null)
                {
                    Key = key;
                    Value = value;
                    Type = PathKeyValueType.KeyAndValue;
                }

                else
                {
                    Key = key;
                    Type = PathKeyValueType.Key;
                }
            }

            // Key is null.
            else
            {
                // Exist Value.
                if (value != null)
                {
                    Value = value;
                    Type = PathKeyValueType.Value;
                }

                // Null.
                else
                {
                    Type = PathKeyValueType.None;
                }
            }
        }

        public override string ToString()
        {
            // No Key
            if (string.IsNullOrEmpty(Key))
            {
                // No Value
                if (string.IsNullOrEmpty(Value))
                    return "";

                // Value only
                else
                    return $"={Value}";
            }

            // Key
            else
            {
                // Key only
                if (string.IsNullOrEmpty(Value))
                    return $"{Key}";

                // Key and Value
                else
                    return $"{Key}={Value}";
            }
        }
    }
}
