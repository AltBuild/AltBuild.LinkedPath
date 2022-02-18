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
        /// ���o�����Ŏg�p����ꍇ�͕ϊ���Ɍ^���}�b�`������ׁAdestineType ���w�肷��
        /// </summary>
        /// <param name="destineType"></param>
        public PathDataCollection(Type destineType)
        {
            Type = destineType;
        }

        /// <summary>
        /// Arguments�Ŏg�p����ꍇ�� destineType�͖���
        /// </summary>
        public PathDataCollection()
        {
        }


        public PathDataCollection(Type destineType, string stringValues, object source)
        {
            if (stringValues == null)
                throw new InvalidProgramException("Error, PathValue is null.");

            // �^��ێ�
            Type = destineType;

            // ���X�g�̏ꍇ
            if (stringValues.StartsWith('[') && stringValues.EndsWith(']'))
            {
                // �J���}��؂�
                var split = stringValues.Substring(1, stringValues.Length - 2).Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                split.ForEach(o => IncludeValue(o));
            }

            // �P�̒l�̏ꍇ
            else
            {
                IncludeValue(stringValues);
            }

            // �l���
            void IncludeValue(string value)
            {
                // �l�����ʂɕێ�
                if (TypeConverterExtensions.TryGet(Type, value, out object destineValue))
                {
                    Add(new PathData { Data = destineValue });
                }

                // �����o�[�̏ꍇ�͖߂�l���擾
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
