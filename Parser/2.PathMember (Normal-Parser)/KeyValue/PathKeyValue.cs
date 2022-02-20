//using AltBuild.BaseExtensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace AltBuild.LinkedPath.Parser
{
    /// <summary>
    /// キーと値：解析前（Key, StringValue）
    /// </summary>
    public partial class PathKeyValue
    {
        /// <summary>
        /// Key Value Type
        /// </summary>
        public PathKeyValueType Type { get; }

        /// <summary>
        /// 演算方法
        /// </summary>
        public PathOperatorType OperatorType { get; protected set; } = PathOperatorType.Equal;

        /// <summary>
        /// Key
        /// </summary>
        public virtual string Key { get; }

        /// <summary>
        /// Value (string type, non processing value)
        /// </summary>
        public virtual string Line { get; private set; }

        /// <summary>
        /// Value is string
        /// </summary>
        public bool IsString { get; private set; }

        /// <summary>
        /// 解析結果の値
        /// </summary>
        public PathDataCollection Results { get; protected set; }

        public void AddValue(string line)
        {
            StringBuilder bild = new StringBuilder();

            // 既存値を追加
            if (string.IsNullOrWhiteSpace(Line) == false)
                bild.Append(Line);

            // 追加値を追加
            var split = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var atElement in split)
            {
                if (bild.Length > 0)
                    bild.Append(' ');

                bild.Append(atElement);
            }

            Line = bild.ToString();
        }

        void _setValue(object value, bool isString)
        {
            // Character string fixation
            if (isString)
            {
                IsString = true;
                Line = value?.ToString() ?? "";
            }

            // Value type is int, Guid, decimal, string, model, etc...
            // Results is null or un null.
            else
            {
                // Once held as a string
                if (value is string stringValue)
                {
                    Line = stringValue;
                }

                else if (value is IList iList)
                {
                    Results = new PathDataCollection(iList);
                }

                else
                {
                    Results = new PathDataCollection();
                    Results.Add(new PathData { Data = value });
                }
            }
        }

        public PathKeyValue(string key, object value, bool isString = false)
        {
            // Exist Key.
            if (key != null)
            {
                // Exist Key & Value
                if (value != null)
                {
                    Key = key;
                    _setValue(value, isString);
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
                    _setValue(value, isString);
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
                if (string.IsNullOrEmpty(Line))
                    return "";

                // Value only
                else if (IsString)
                    return $"'{Results?.ToString() ?? Line}'";
                else
                    return $"{Results?.ToString() ?? Line}";
            }

            // Key
            else
            {
                // Key only
                if (string.IsNullOrEmpty(Line))
                    return $"{Key}";

                // Key and Value
                else if (IsString)
                    return $"{Key}{OperatorType.GetOperatorCode()}'{Results?.ToString() ?? Line}'";

                else
                    return $"{Key}{OperatorType.GetOperatorCode()}{Results?.ToString() ?? Line}";
            }
        }
    }
}
