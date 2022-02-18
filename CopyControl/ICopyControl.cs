using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;

namespace AltBuild.LinkedPath
{
    public interface ICopyControl
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
        RecordTargets Targets { get; }

        /// <summary>
        /// Copy operation flags.
        /// </summary>
        CopyUse Use { get; }

        /// <summary>
        /// Change rule.
        /// </summary>
        DObjectRule ChangeRule { get; set; }

        /// <summary>
        /// Object copy cache.
        /// </summary>
        ICopyCache CopyCache { get; }

        /// <summary>
        /// Targets copy member.
        /// </summary>
        object Predicate { get; }

        /// <summary>
        /// Clone control.
        /// </summary>
        /// <param name="offsetDepth"></param>
        /// <returns></returns>
        ICopyControl Clone(int offsetDepth);

        /// <summary>
        /// 許容深度オーバー
        /// </summary>
        /// <returns></returns>
        bool IsOverAllowableDepth();

        /// <summary>
        /// 操作対象か？
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        bool IsTargets(object obj, object initRecord = null);
    }
}
