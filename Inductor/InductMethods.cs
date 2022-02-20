using System;

namespace AltBuild.LinkedPath
{
    /// <summary>
    /// Results from object and PathMember
    /// </summary>
    [Flags]
    public enum InductMethods
    {
        /// <summary>
        /// Process everything.
        /// </summary>
        Normal = default,

        /// <summary>
        /// No indexer processing.
        /// </summary>
        NoLastIndexer = 0x0010,

        /// <summary>
        /// No inductive converter.
        /// </summary>
        NoInductiveConverter = 0x0020,

        /// <summary>
        /// No descendant processing.
        /// </summary>
        NoDescendant = 0x0040,

        /// <summary>
        /// No inductive converter.
        /// </summary>
        SelectConverter = 0x0100,

        /// <summary>
        /// Execute the method even if the return value is void or Task.
        /// Usually used when executing in response to events such as Button click.
        /// </summary>
        InvokeMethod = 0x1000,
    }
}
