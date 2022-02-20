namespace AltBuild.LinkedPath
{
    /// <summary>
    /// Object copy use.
    /// </summary>
    public enum ECopyUse
    {
        /// <summary>
        /// auto.
        /// </summary>
        Auto = default,

        /// <summary>
        /// Create new generation use.
        ///   Summary:
        ///     clone = source.Clone();
        ///     clone.Original = null;
        ///     clone.Rule = Writable;
        ///     //source.Original   Å©ïœçXÇ»Çµ
        /// </summary>
        NewGeneration,

        /// <summary>
        /// copy use
        ///   Summary:
        ///     clone = source.Clone();
        ///     clone.Original = source.Original;
        /// </summary>
        Duplicate,

        /// <summary>
        /// Create clone use.
        ///   Summary:
        ///     clone = source.Clone();
        ///     clone.Original = source;
        ///     source.Original = clone;
        /// </summary>
        Original,

        /// <summary>
        /// cache use clone (new object use)
        ///   Summary:
        ///     clone = source.Clone();
        ///     clone.Original = null;
        ///     clone.Rule &= ~DObjectRule.Writable;
        ///     source.Original = clone;
        /// </summary>
        Cache,

        /// <summary>
        /// cache use (update use)
        ///   Summary:
        /// </summary>
        CacheUpdate,
    }
}
