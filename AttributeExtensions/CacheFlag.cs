using System;

namespace AltBuild.LinkedPath
{
    [Flags]
    public enum CacheFlag
    {
        /// <summary>
        /// Automatic caching
        /// </summary>
        None = default,

        /// <summary>
        /// Client and Server caching ignore
        /// </summary>
        IgnoreAll = ClientIgnore | ServerIgnore,

        /// <summary>
        /// Client caching ignore
        /// </summary>
        ClientIgnore = 0x0001,

        /// <summary>
        /// Server caching ignore
        /// </summary>
        ServerIgnore = 0x0002,
    }
}
