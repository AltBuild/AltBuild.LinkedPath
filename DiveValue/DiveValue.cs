using System;
using System.Reflection;

namespace AltBuild.LinkedPath
{
    /// <summary>
    /// Results from object and PathMember
    /// </summary>
    public class DiveValue : IDiveValueInner
    {
        /// <summary>
        /// Reason PathMember
        /// </summary>
        public PathMember PathMember { get; init; }

        /// <summary>
        /// Original member value
        /// </summary>
        public object Source { get; init; }

        /// <summary>
        /// Return value.
        /// </summary>
        public object Value { get; init; }

        /// <summary>
        /// Reason member infomation
        /// </summary>
        public MemberInfo MemberInfo
        {
            get => _memberInfo ??= PathMember.GetMemberInfo(Source);
            init => _memberInfo = value;
        }
        MemberInfo _memberInfo = null;

        /// <summary>
        /// Log infomation.
        /// </summary>
        public DiveParameters Parameters { get; init; }

        /// <summary>
        /// Last Dive
        /// </summary>
        public DiveValue Last
        {
            get
            {
                var next = PathMember?.Child;
                if (next != null && PathMember.Frame.Flags.HasFlag(DiveControlFlags.NoDescendant) == false)
                    return next.FromDiveInner(Value).Last;
                else
                    return this;
            }
        }

        public DiveControlFlags Flags => PathMember?.Frame?.Flags ?? DiveControlFlags.FullProcess;

        public override string ToString() => Value.ToString(PathElements.Default);
    }
}
