using System;

namespace AltBuild.LinkedPath
{
    /// <summary>
    /// Attributes that indicate that the internal processing member is not covered.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Method, AllowMultiple = false)]
    public class InductiveIgnoreAttribute : System.Attribute
    {
        /// <summary>
        /// Empty object.
        /// </summary>
        public static readonly InductiveIgnoreAttribute Empty = new InductiveIgnoreAttribute { Ignore = false };

        public bool Ignore { get; init; } = true;
    }
}
