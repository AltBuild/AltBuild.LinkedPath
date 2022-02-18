using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace AltBuild.LinkedPath
{
    /// <summary>
    /// Element type enumerate
    /// </summary>
    public enum PathElementType
    {
        /// <summary>
        /// none
        /// </summary>
        None = 0,

        /// <summary>
        /// static string element
        /// </summary>
        String = 0x0001,

        /// <summary>
        /// object value element
        /// </summary>
        Value = 0x0002,

        /// <summary>
        /// object paramenter element
        /// </summary>
        Parameter = 0x0004,

        /// <summary>
        /// environment format.
        /// </summary>
        Format = 0x0008,
    }
}
