using System;
using System.Reflection;

namespace AltBuild.LinkedPath
{
    /// <summary>
    /// Results from object and PathMember
    /// </summary>
    public interface IDiveValueInner
    {
        /// <summary>
        /// Original member value
        /// </summary>
        object Source { get; }

        /// <summary>
        /// Reason PathMember
        /// </summary>
        PathMember PathMember { get; }

        /// <summary>
        /// Reason member infomation
        /// </summary>
        MemberInfo MemberInfo { get; }

        /// <summary>
        /// Return value.
        /// </summary>
        object Value { get; }

        /// <summary>
        /// Processing parameters.
        /// </summary>
        public DiveParameters Parameters { get; }

        /// <summary>
        /// ToString
        /// </summary>
        /// <returns></returns>
        string ToString();
    }
}
