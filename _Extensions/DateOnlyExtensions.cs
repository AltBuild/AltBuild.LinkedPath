using System;
using System.Globalization;
using System.Collections.Generic;

namespace AltBuild.LinkedPath.Extensions
{
    public static class DateOnlyExtensions
    {
        public static DateTime ToDateTime(this DateOnly? date, TimeOnly time)
        {
            if ((date is DateOnly dateOnly) == false)
                dateOnly = DateTime.Now.ToDateOnly();

            return new DateTime(dateOnly.Year, dateOnly.Month, dateOnly.Day, time.Hour, time.Minute, time.Second, time.Millisecond);
        }
    }
}
