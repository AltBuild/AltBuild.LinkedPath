using AltBuild.LinkedPath.Converters;
using AltBuild.LinkedPath.Parser;
using System;
using System.Reflection;
using System.Diagnostics;

namespace AltBuild.LinkedPath
{
    /// <summary>
    /// Results from object and PathMember
    /// </summary>
    public partial class InductInfo : IInductInfo
    {
        /// <summary>
        /// Core frame.
        /// </summary>
        public InductFrame Frame { get; init; }

        /// <summary>
        /// Reason PathMember
        /// </summary>
        public PathMember PathMember { get; init; }

        /// <summary>
        /// Original member value
        /// </summary>
        public object BaseObject { get; init; }

        /// <summary>
        /// Target member infomation
        /// </summary>
        public MemberInfo MemberInfo { get; init; }

        /// <summary>
        /// Return value.
        /// </summary>
        public object ReturnValue { get; init; }

        /// <summary>
        /// Return type.
        /// </summary>
        public Type ReturnType { get; init; }
    }
}
