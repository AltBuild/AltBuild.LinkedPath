using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace AltBuild.LinkedPath
{
    /// <summary>
    /// �p�X�G�������g�𐶐�
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
        /// �p�X�G�������g����
        /// </summary>
        /// <param name="line">�G�������g������</param>
        /// <param name="isForceCreate">�G�������g�������Ă���������</param>
        /// <returns></returns>
        public static PathElements Parse(string line, PathElementType defaultType = PathElementType.Format)
        {
            // �X�p����
            var span = line.AsSpan();

            // ����
            PathElements results = null;

            // ���[�v����
            while (span.Length > 0)
            {
                var indexElementBegin = span.IndexOf('{');
                if (indexElementBegin >= 0)
                {
                    var indexElementEnd = span.IndexOf('}');
                    if (indexElementEnd >= 0)
                    {
                        // ���O�̕����������
                        if (indexElementBegin > 0)
                            Add(new PathElement(PathElementType.String, span.Slice(0, indexElementBegin)));

                        // �G�������g�����擾
                        var element = span.Slice(indexElementBegin + 1, indexElementEnd - indexElementBegin - 1);

                        // �ǉ�
                        Add(new PathElement(element));

                        // ����
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
                // �I��
                break;
            }

            // ����
            return results;

            // �G�������g�̒ǉ�
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
