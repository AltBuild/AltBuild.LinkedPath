using System;
using System.Globalization;
using System.Collections.Generic;

namespace AltBuild.LinkedPath.Extensions
{
    public static class TimeOnlyExtensions
    {
        public static DateTime ToDateTime(this TimeOnly time, DateOnly date)
        {
            return new DateTime(date.Year, date.Month, date.Day, time.Hour, time.Minute, time.Second, time.Millisecond);
        }

        public static DateTime ToDateTime(this TimeOnly? time, DateOnly date)
        {
            if (time is TimeOnly timeOnly)
                return new DateTime(date.Year, date.Month, date.Day, timeOnly.Hour, timeOnly.Minute, timeOnly.Second, timeOnly.Millisecond);

            else
                return new DateTime(date.Year, date.Month, date.Day);
        }
    }
}
