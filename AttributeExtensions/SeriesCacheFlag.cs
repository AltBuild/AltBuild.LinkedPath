using System;

namespace AltBuild.LinkedPath
{
    [Flags]
    public enum SeriesCacheFlag
    {
        /// <summary>
        /// Automatic caching
        /// </summary>
        None = default,

        PerYear,

        PerMonth,

        PerDay,

        PerHour,

        PerMinute,

        PerSecond,

        PerMask,
    }
}
