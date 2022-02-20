using System;

namespace AltBuild.LinkedPath
{
    public interface IObjectCopyControl
    {
        /// <summary>
        /// Current Depth
        /// </summary>
        int Depth { get; }

        /// <summary>
        /// Set destine GenDate
        /// </summary>
        DateTime? GenDate { get; }

        /// <summary>
        /// Target copy table flags.
        /// </summary>
        ECopyTargets Targets { get; }

        /// <summary>
        /// Copy operation flags.
        /// </summary>
        ECopyUse Use { get; }

        /// <summary>
        /// Change rule.
        /// </summary>
        InductiveRule ChangeRule { get; set; }

        /// <summary>
        /// Object copy cache.
        /// </summary>
        IObjectCopyCache CopyCache { get; }

        /// <summary>
        /// Targets copy member.
        /// </summary>
        object Predicate { get; }

        /// <summary>
        /// Clone control.
        /// </summary>
        /// <param name="offsetDepth"></param>
        /// <returns></returns>
        IObjectCopyControl Clone(int offsetDepth);

        /// <summary>
        /// Over Allowable Depth flag.
        /// </summary>
        /// <returns></returns>
        bool IsOverAllowableDepth();

        /// <summary>
        /// Is target.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        bool IsTargets(object obj, object initRecord = null);
    }
}
