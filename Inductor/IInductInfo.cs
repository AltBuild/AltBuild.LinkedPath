using AltBuild.LinkedPath.Parser;
using System;
using System.Reflection;

namespace AltBuild.LinkedPath
{
    /// <summary>
    /// Inductive value and infomation interface.
    /// </summary>
    public interface IInductInfo
    {
        /// <summary>
        /// Core frame.
        /// </summary>
        InductFrame Frame { get; }

        /// <summary>
        /// Reason PathMember
        /// </summary>
        PathMember PathMember { get; }

        /// <summary>
        /// Original member value
        /// </summary>
        object BaseObject { get; }

        /// <summary>
        /// Target member infomation
        /// </summary>
        MemberInfo MemberInfo { get; }

        /// <summary>
        /// Return value.
        /// </summary>
        object ReturnValue { get; }

        /// <summary>
        /// Return type.
        /// </summary>
        Type ReturnType { get; }

        /// <summary>
        /// ToString
        /// </summary>
        /// <returns></returns>
        string ToString();
    }
}
