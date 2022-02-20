using AltBuild.LinkedPath;
using AltBuild.LinkedPath.Parser;
using System;
using System.Collections.Generic;

namespace AltBuild.LinkedPath.Converters
{
    /// <summary>
    /// Select converter interface.
    /// </summary>
    public interface ISelectRequirementsBase
    {
        ///// <summary>
        ///// Induct infomation
        ///// </summary>
        InductInfo InductInfo { get; }

        /// <summary>
        /// Text PathElements
        /// </summary>
        PathElements TextElements { get; }
    }
}
