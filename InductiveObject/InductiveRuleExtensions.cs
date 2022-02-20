using System;
using System.Runtime.CompilerServices;

namespace AltBuild.LinkedPath
{
    /// <summary>
    /// The role of the object.
    /// </summary>
    public static class InductiveRuleExtensions
    {
        /// <summary>
        /// Is uncopied original.
        /// </summary>
        /// <param name="rule"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsOriginal(this InductiveRule rule) =>
            rule.HasFlag(InductiveRule.Writable) == false;

        /// <summary>
        ///  Is original obtained from data store.
        /// </summary>
        /// <param name="rule"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsDatastoreOriginal(this InductiveRule rule) =>
            (rule.HasFlag(InductiveRule.Datastore) && rule.HasFlag(InductiveRule.Writable) == false);

        /// <summary>
        /// Is writable object.
        /// </summary>
        /// <param name="rule"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsWritable(this InductiveRule rule) => rule.HasFlag(InductiveRule.Writable);

        /// <summary>
        /// Is writable object.
        /// </summary>
        /// <param name="rule"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsGenerationManage(this InductiveRule rule) => rule.HasFlag(InductiveRule.GenerationManage);

        /// <summary>
        /// Rule mixer
        /// </summary>
        /// <param name="paramRule">set/exclude + flags</param>
        /// <param name="sourceRule">source flags</param>
        /// <returns>destine flags</returns>
        public static InductiveRule GetMixRule(InductiveRule paramRule, InductiveRule sourceRule)
        {
            if (paramRule.HasFlag(InductiveRule.Set))
            {
                if (sourceRule.HasFlag(InductiveRule.Datastore))
                    return paramRule & ~InductiveRule.Set;
                else
                    return paramRule & ~(InductiveRule.Set | InductiveRule.Datastore);
            }

            else if (paramRule.HasFlag(InductiveRule.Exclude))
                return sourceRule & ~(paramRule & ~InductiveRule.Exclude);

            else
                return paramRule | sourceRule;
        }
    }
}
