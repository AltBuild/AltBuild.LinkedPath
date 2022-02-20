using System;

namespace AltBuild.LinkedPath
{
    /// <summary>
    /// Object rule
    /// </summary>
    [Flags]
    public enum InductiveRule
    {
        /// <summary>
        /// Unspecified.
        /// </summary>
        None = default,

        /// <summary>
        /// Object creating.
        /// </summary>
        Creating = 0x0001,

        /// <summary>
        /// Object building From datastore.
        /// </summary>
        Building = 0x0002,

        /// <summary>
        /// Object refreshing.
        /// </summary>
        Refreshing = 0x004,

        /// <summary>
        /// From the data store.
        /// </summary>
        Datastore = 0x0008,

        /// <summary>
        /// Updateable state to data store.
        /// </summary>
        Writable = 0x0010,

        /// <summary>
        /// Empty object.
        /// (Not edit, Not copy)
        /// </summary>
        Empty = 0x0020,

        /// <summary>
        /// Records for generation management.
        /// </summary>
        GenerationManage = 0x0040,

        /// <summary>
        /// Different generation (not modern) records.
        /// </summary>
        DifferentGeneration = 0x0100,

        /// <summary>
        /// Initialize Only. Exclude.
        /// </summary>
        Set = 0x4000,

        /// <summary>
        /// Initialize Only. Exclude.
        /// </summary>
        Exclude = 0x8000,
    }
}
