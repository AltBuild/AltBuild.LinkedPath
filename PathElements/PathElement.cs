using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Runtime.CompilerServices;

namespace AltBuild.LinkedPath
{
    /// <summary>
    /// �p�X���܂ޕ�������I�u�W�F�N�g��
    /// </summary>
    public class PathElement
    {
        /// <summary>
        /// �^�C�v��ێ�
        /// </summary>
        public PathElementType Type { get; }

        /// <summary>
        /// �\�[�X��ێ�
        /// </summary>
        public string Source
        {
            get => _source ??= PathMember?.ToString();
            private set => _source = value;
        }
        string _source;

        /// <summary>
        /// �p�X�����o�[��ێ�
        /// </summary>
        public PathMember PathMember
        {
            get
            {
                if (_pathMember == null)
                {
                    _pathMember = Type switch
                    {
                        PathElementType.Format => PathFactory.ParseOfFormat(Source),
                        PathElementType.Value => PathFactory.Parse(Source),
                        _ => throw new NotSupportedException()
                    };
                }

                return _pathMember;
            }
        }
        PathMember _pathMember;

        public PathElement(PathElementType type, PathMember pathMember)
        {
            // Set type.
            Type = type;

            // Set source.
            _pathMember = pathMember;
        }

        public PathElement(PathElementType type, string source)
        {
            // Set Type.
            Type = type;

            // Set Source.
            Source = InitializeSource(source);
        }

        public PathElement(PathElementType type, ReadOnlySpan<char> source)
        {
            // Set Type.
            Type = type;

            // Set Source.
            Source = InitializeSource(source);
        }

        public PathElement(ReadOnlySpan<char> source)
        {
            // Set Type.
            Type = source.StartsWith(stackalloc[] { '#' }) ? PathElementType.Parameter : PathElementType.Value;

            // Set Source.
            Source = InitializeSource(source);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        string InitializeSource(ReadOnlySpan<char> source)
        {
            // Set Source.
            return Type switch
            {
                PathElementType.Value => source.Trim(stackalloc[] { '\"', '\'' }).ToString(),
                _ => source.ToString(),
            };
        }

        /// <summary>
        /// Set source.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        string InitializeSource(string source)
        {
            return Type switch
            {
                PathElementType.Value => source?.Trim('\"', '\''),
                _ => source,
            };
        }
    }
}
