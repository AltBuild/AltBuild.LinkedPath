using System.Text;

namespace AltBuild.LinkedPath.Converters
{
    public static class RealNumberExtensions
    {
        /// <summary>
        /// other number to 3dot strings.
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
        /// 数字→文字列化
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        static string _toRealString(string line)
        {
            if (line.Length == 0)
                return "";

            // 結果
            StringBuilder bild = new StringBuilder();

            // マイナスの補間処理
            if (line[0] == '-')
            {
                bild.Append('-');
                line = line.Substring(1);
            }

            // 小数点付きの場合
            int indexOfDot = line.IndexOf('.');
            if (indexOfDot >= 0)
            {
                line = line.TrimEnd('0').TrimEnd('.');
            }
            else
            {
                indexOfDot = line.Length;
            }

            // 4桁以上は カンマ付け
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

            // 結果
            return bild.ToString();
        }
    }
}
