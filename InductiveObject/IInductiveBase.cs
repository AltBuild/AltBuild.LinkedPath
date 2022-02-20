using AltBuild.LinkedPath.Parser;

namespace AltBuild.LinkedPath
{
    /// <summary>
    /// IObjectBase 
    /// </summary>
    public interface IInductiveBase
    {
        /// <summary>
        /// Get definition object instance
        /// </summary>
        object DefinitionObject { get; }

        /// <summary>
        /// Get InductInfo.
        /// </summary>
        /// <param name="pathMember"></param>
        /// <param name="methods">Processing methods</param>
        /// <returns></returns>
        InductInfo Induct(PathMember pathMember, InductMethods methods = InductMethods.Normal);

        /// <summary>
        /// Get InductInfo.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="methods">Processing methods</param>
        /// <returns></returns>
        InductInfo Induct(string path, InductMethods methods = InductMethods.Normal);

        /// <summary>
        /// Get a string formatted with a path element.
        /// </summary>
        /// <param name="pathElements">Path element of a string.</param>
        /// <returns></returns>
        string ToString(string pathElements);

        /// <summary>
        /// Get a string formatted with a path element.
        /// </summary>
        /// <param name="pathElements">Path element.</param>
        /// <returns></returns>
        string ToString(PathElements pathElements);
    }
}
