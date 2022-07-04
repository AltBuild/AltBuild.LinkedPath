using AltBuild.LinkedPath.Converters;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AltBuild.LinkedPath.Parser
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

        public bool Contains(dynamic value)
        {
            foreach (var pathData in this)
                if (pathData.Data == value)
                    return true;

            return false;
        }

        public PathDataCollection()
        {
        }

        /// <summary>
        /// ���o�����Ŏg�p����ꍇ�͕ϊ���Ɍ^���}�b�`������ׁAdestineType ���w�肷��
        /// </summary>
        /// <param name="destineType"></param>
        public PathDataCollection(Type destineType)
        {
            Type = destineType;
        }

        public PathDataCollection(IList iList)
        {
            for (int i = 0; i < iList.Count; i++)
            {
                object value = iList[i];

                if (Type == null && value != null)
                    Type = value.GetType();

                Add(new PathData { Data = value });
            }
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

                foreach (var atSplit in split)
                    IncludeValue(atSplit);
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
                else if (InductHelper.TryGetPathValue(out InductInfo inductInfo, source, null, value))
                {
                    if (inductInfo.ReturnValue is IPathMatch pathMatch)
                    {
                        foreach (var data in pathMatch.GetMatchValue())
                            Add(new PathData { Data = data });
                    }
                }

                //
                else
                    throw new InvalidProgramException("Error, PathValue is convert unmatch.");
            }
        }

        public override string ToString()
        {
            var build = new StringBuilder();

            if (Count >= 2)
                build.Append('[');

            for (int i = 0; i < Count; i++)
            {
                PathData data = this[i];

                if ((data.Data is Guid guidData) || (data.Data is string stringData))
                {
                    build.Append(i > 0 ? ',' : null).Append('\'').Append(data.Data).Append('\'');
                }
                else
                {
                    build.Append(i > 0 ? ',' : null).Append(data.Data);
                }
            }

            if (Count >= 2)
                build.Append(']');

            return build.ToString();
        }
    }
}
