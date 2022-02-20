using System;
using System.Globalization;
using System.Collections.Generic;

namespace AltBuild.LinkedPath.Extensions
{
    public static class DateTimeExtensions
    {
        public static DateOnly ToDateOnly(this DateTime dateTime)
        {
            return new DateOnly(dateTime.Year, dateTime.Month, dateTime.Day);
        }

        public static DateOnly? ToDateOnly(this DateTime? dateTime)
        {
            if (dateTime == null)
                return default;

            else
                return new DateOnly(dateTime.Value.Year, dateTime.Value.Month, dateTime.Value.Day);
        }

        public static TimeOnly ToTimeOnly(this DateTime dateTime)
        {
            return new TimeOnly(dateTime.Hour, dateTime.Minute, dateTime.Second, dateTime.Millisecond);
        }

        public static TimeOnly? ToTimeOnly(this DateTime? dateTime)
        {
            if (dateTime == null)
                return default;

            else
                return new TimeOnly(dateTime.Value.Hour, dateTime.Value.Minute, dateTime.Value.Second, dateTime.Value.Millisecond);
        }
    }
}
