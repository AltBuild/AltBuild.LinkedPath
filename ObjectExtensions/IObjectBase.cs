using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace AltBuild.LinkedPath
{
    /// <summary>
    /// IObjectBase 
    /// </summary>
    public interface IObjectBase
    {
        /// <summary>
        /// Get definition object instance
        /// </summary>
        object DefinitionObject { get; }

        /// <summary>
        /// Get PathValue.
        /// </summary>
        /// <param name="pathMember"></param>
        /// <returns></returns>
        DiveValue ToDive(PathMember pathMember);

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
