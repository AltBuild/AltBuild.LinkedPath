namespace AltBuild.LinkedPath
{
    /// <summary>
    ///  Object copy interface
    /// </summary>
    public interface IObjectCopy
    {
        /// <summary>
        /// Object copy.
        /// </summary>
        /// <param name="destine">Destine object</param>
        /// <param name="control">Copy control object</param>
        /// <param name="memberInfo">Target MemberInfo</param>
        /// <returns></returns>
        void Copy(object destine, IObjectCopyControl control, object memberInfo = null);

        /// <summary>
        /// Processing before copying.
        /// </summary>
        /// <param name="destine">Destine object</param>
        /// <param name="control">Copy control object</param>
        void SetFlagsBefore(object destine, IObjectCopyControl control);

        /// <summary>
        /// Processing after copying.
        /// </summary>
        /// <param name="destine">Destine object</param>
        /// <param name="control">Copy control object</param>
        void SetFlagsAfter(object destine, IObjectCopyControl control);

        /// <summary>
        /// Definition object
        /// </summary>
        object DefinitionObject { get; set; }

        /// <summary>
        /// Object rule.
        /// </summary>
        InductiveRule Rule { get; set; }
    }
}
