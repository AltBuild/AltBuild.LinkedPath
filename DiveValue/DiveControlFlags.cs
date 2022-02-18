using System;

namespace AltBuild.LinkedPath
{
    /// <summary>
    /// Results from object and PathMember
    /// </summary>
    [Flags]
    public enum DiveControlFlags
    {
        /// <summary>
        /// Process everything.
        /// </summary>
        FullProcess = 0,

        /// <summary>
        /// No indexer processing.
        /// </summary>
        NoIndexer = 0x0010,

        /// <summary>
        /// No descendant processing.
        /// </summary>
        NoDescendant = 0x0020,
    }
}
