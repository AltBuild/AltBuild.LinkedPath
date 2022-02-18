using System;
using System.Runtime.CompilerServices;

namespace AltBuild.LinkedPath
{
    /// <summary>
    /// The role of the object.
    /// </summary>
    public static class DObjectRuleExtensions
    {
        /// <summary>
        /// Is uncopied original.
        /// </summary>
        /// <param name="rule"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsOriginal(this DObjectRule rule) =>
            rule.HasFlag(DObjectRule.Writable) == false;

        /// <summary>
        ///  Is original obtained from data store.
        /// </summary>
        /// <param name="rule"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsDatastoreOriginal(this DObjectRule rule) =>
            (rule.HasFlag(DObjectRule.Datastore) && rule.HasFlag(DObjectRule.Writable) == false);

        /// <summary>
        /// Is writable object.
        /// </summary>
        /// <param name="rule"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsWritable(this DObjectRule rule) => rule.HasFlag(DObjectRule.Writable);

        /// <summary>
        /// Is writable object.
        /// </summary>
        /// <param name="rule"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsGenerationManage(this DObjectRule rule) => rule.HasFlag(DObjectRule.GenerationManage);

        /// <summary>
        /// Rule mixer
        /// </summary>
        /// <param name="paramRule">set/exclude + flags</param>
        /// <param name="sourceRule">source flags</param>
        /// <returns>destine flags</returns>
        public static DObjectRule GetMixRule(DObjectRule paramRule, DObjectRule sourceRule)
        {
            if (paramRule.HasFlag(DObjectRule.Set))
            {
                if (sourceRule.HasFlag(DObjectRule.Datastore))
                    return paramRule & ~DObjectRule.Set;
                else
                    return paramRule & ~(DObjectRule.Set | DObjectRule.Datastore);
            }

            else if (paramRule.HasFlag(DObjectRule.Exclude))
                return sourceRule & ~(paramRule & ~DObjectRule.Exclude);

            else
                return paramRule | sourceRule;
        }
    }
}
