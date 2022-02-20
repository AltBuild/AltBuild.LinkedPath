using System;
using System.Globalization;
using System.Collections.Generic;

namespace AltBuild.LinkedPath.Extensions
{
    public static class DateTimeFormatExtensions
    {
        static Dictionary<string, IFormatProvider> FormatProvider { get; } = CreateFormatProvider();

        /// <summary>
        /// 国・言語により、DateTime.ToString を処理する
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, IFormatProvider> CreateFormatProvider()
        {
            var results = new Dictionary<string, IFormatProvider>();

            // 日本／日本語
            var culture = new CultureInfo("ja-JP", true);
            culture.DateTimeFormat.Calendar = new JapaneseCalendar();
            results.Add("JPN", culture);

            return results;
        }

        public static string ToString(this DateTime dateTime, string format, string code3)
        {
            try
            {
                if (dateTime == default)
                    return "";

                else if (FormatProvider.TryGetValue(code3, out IFormatProvider formatProvider))
                    return dateTime.ToString(format, formatProvider);

                else
                    return dateTime.ToString(format);
            }

            catch (Exception)
            {
                return "";
            }
        }

        public static bool TryStringToFormatProvider(string code3, out IFormatProvider formatProvider)
        {
            return FormatProvider.TryGetValue(code3, out formatProvider);
        }
    }
}
