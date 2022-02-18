using System;
using System.Collections.Generic;
using System.Text;

namespace AltBuild.LinkedPath
{
    public static class RealNumberExtensions
    {
        /// <summary>
        /// ���̑��̐��l�������񉻃f�t�H���g����
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToRealString(object value)
        {
            if (value != null)
            {
                return _toRealString(value.ToString());
            }

            return "";
        }

        /// <summary>
        /// ������������
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        static string _toRealString(string line)
        {
            if (line.Length == 0)
                return "";

            // ����
            StringBuilder bild = new StringBuilder();

            // �}�C�i�X�̕�ԏ���
            if (line[0] == '-')
            {
                bild.Append('-');
                line = line.Substring(1);
            }

            // �����_�t���̏ꍇ
            int indexOfDot = line.IndexOf('.');
            if (indexOfDot >= 0)
            {
                line = line.TrimEnd('0').TrimEnd('.');
            }
            else
            {
                indexOfDot = line.Length;
            }

            // 4���ȏ�� �J���}�t��
            for (; ; )
            {
                if (indexOfDot < 4)
                {
                    bild.Append(line);
                    break;
                }
                else
                {
                    int mod = indexOfDot % 3;
                    if (mod == 0)
                        mod = 3;

                    if (mod > 0)
                    {
                        bild.Append(line.Substring(0, mod)).Append(',');
                        line = line.Substring(mod);
                        indexOfDot = indexOfDot - mod;
                    }
                }
            }

            // ����
            return bild.ToString();
        }
    }
}
