using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace AltBuild.LinkedPath
{
    /// <summary>
    /// パスエレメントを生成
    ///  Exsample: var elements = PathElements.Parse("{Name} ({Ruby})");
    /// </summary>
    public class PathElements : List<PathElement>
    {
        /// <summary>
        /// Static type: short, int, long, decimal, float, double => 3-digit separated string
        /// IObjectBase => default string (long)
        /// </summary>
        public static PathElements Default => Parse("@");

        /// <summary>
        /// Static type: short, int, long, decimal, float, double => default
        /// IObjectBase => basis string
        /// </summary>
        public static PathElements Basis => Parse("@basis");

        /// <summary>
        /// Static type: short, int, long, decimal, float, double => default
        /// IObjectBase => short string
        /// </summary>
        public static PathElements Short => Parse("@short");

        /// <summary>
        /// パスエレメント生成
        /// </summary>
        /// <param name="line">エレメント文字列</param>
        /// <param name="isForceCreate">エレメントが無くても生成する</param>
        /// <returns></returns>
        public static PathElements Parse(string line, PathElementType defaultType = PathElementType.Format)
        {
            // スパン化
            var span = line.AsSpan();

            // 結果
            PathElements results = null;

            // ループ検証
            while (span.Length > 0)
            {
                var indexElementBegin = span.IndexOf('{');
                if (indexElementBegin >= 0)
                {
                    var indexElementEnd = span.IndexOf('}');
                    if (indexElementEnd >= 0)
                    {
                        // 事前の文字列を処理
                        if (indexElementBegin > 0)
                            Add(new PathElement(PathElementType.String, span.Slice(0, indexElementBegin)));

                        // エレメント部を取得
                        var element = span.Slice(indexElementBegin + 1, indexElementEnd - indexElementBegin - 1);

                        // 追加
                        Add(new PathElement(element));

                        // 続く
                        span = span.Slice(indexElementEnd + 1);
                        continue;
                    }
                }

                // Forced processing.
                if (span.Length > 0 && defaultType != PathElementType.None)
                {
                    // format processing.
                    if (results == null)
                        Add(new PathElement(defaultType, span));

                    // string processing.
                    else
                        Add(new PathElement(PathElementType.String, span));
                }
                // 終了
                break;
            }

            // 結果
            return results;

            // エレメントの追加
            void Add(PathElement element)
            {
                if (results == null)
                    results = new PathElements();

                results.Add(element);
            }
        }

        /// <summary>
        /// explicit cast method.
        /// </summary>
        /// <param name="line"></param>
        public static explicit operator PathElements(string line) => Parse(line);

        /// <summary>
        /// Initialize PathElements.
        /// </summary>
        /// <param name="pathMember"></param>
        public static explicit operator PathElements(PathMember pathMember)
        {
            var results = new PathElements();
            results.Add((PathElement)pathMember);
            return results;
        }

        /// <summary>
        /// Get format string of format only 
        /// </summary>
        /// <param name="format">Format string</param>
        /// <returns>true: success, false: failed</returns>
        public bool TryGetFormatOnlyElement(out string format)
        {
            if (Count == 1 && this[0].Type == PathElementType.Format)
            {
                format = this[0].Source;
                return true;
            }

            format = null;
            return false;
        }

        /// <summary>
        /// Parameter of Depth
        /// </summary>
        public int IndexOfDepth { get; set; } = 1;

        public PathElements Depth(int offsetDepth)
        {
            IndexOfDepth += offsetDepth;
            return this;
        }

        public PathElements Depth(PathElements pathElements)
        {
            IndexOfDepth += pathElements.IndexOfDepth;
            return this;
        }
    }
}
