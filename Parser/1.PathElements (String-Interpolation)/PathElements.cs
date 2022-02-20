using System;
using System.Collections.Generic;

namespace AltBuild.LinkedPath.Parser
{
    /// <summary>
    /// パスエレメントを生成
    ///  Example: var elements = PathElements.Parse("{Name} ({Ruby})");
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
            // Null 
            if (line == null)
                return null;

            // To ReadOnlySpan<char>
            var span = line.AsSpan();

            // Results
            PathElements results = null;

            // Seek.
            while (span.Length > 0)
            {
                var indexElementBegin = span.IndexOf('{');
                if (indexElementBegin >= 0)
                {
                    var indexElementEnd = span.IndexOf('}');
                    if (indexElementEnd >= 0)
                    {
                        // Preprocessing.
                        if (indexElementBegin > 0)
                            Add(new PathElement(PathElementType.String, span.Slice(0, indexElementBegin)));

                        // Get element.
                        var element = span.Slice(indexElementBegin + 1, indexElementEnd - indexElementBegin - 1);

                        // Add
                        Add(new PathElement(element));

                        // continue
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

                // Finish.
                break;
            }

            // return
            return results;

            // Add Element
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
