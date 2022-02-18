using AltBuild.BaseExtensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AltBuild.LinkedPath
{
    /// <summary>
    /// Multiple data analysis and retention
    /// </summary>
    public partial class PathDataCollection : List<PathData>
    {
        /// <summary>
        /// Data type
        /// </summary>
        public Type Type { get; init; }

        public bool Contains(dynamic value) => this.Contains(o => o.Data == value);

        /// <summary>
        /// 抽出条件で使用する場合は変換先に型をマッチさせる為、destineType を指定する
        /// </summary>
        /// <param name="destineType"></param>
        public PathDataCollection(Type destineType)
        {
            Type = destineType;
        }

        /// <summary>
        /// Argumentsで使用する場合は destineTypeは無い
        /// </summary>
        public PathDataCollection()
        {
        }


        public PathDataCollection(Type destineType, string stringValues, object source)
        {
            if (stringValues == null)
                throw new InvalidProgramException("Error, PathValue is null.");

            // 型を保持
            Type = destineType;

            // リストの場合
            if (stringValues.StartsWith('[') && stringValues.EndsWith(']'))
            {
                // カンマ区切り
                var split = stringValues.Substring(1, stringValues.Length - 2).Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                split.ForEach(o => IncludeValue(o));
            }

            // １つの値の場合
            else
            {
                IncludeValue(stringValues);
            }

            // 値解析
            void IncludeValue(string value)
            {
                // 値を結果に保持
                if (TypeConverterExtensions.TryGet(Type, value, out object destineValue))
                {
                    Add(new PathData { Data = destineValue });
                }

                // メンバーの場合は戻り値を取得
                else if (PathHelper.TryGetPathValue(out DiveValue memberInfo, source, value))
                {
                    if (memberInfo.Value is IPathMatch pathMatch)
                        pathMatch.GetMatchValue().ForEach(o => Add(new PathData { Data = o }));
                }

                else
                    throw new InvalidProgramException("Error, PathValue is convert unmatch.");
            }
        }
    }
}
