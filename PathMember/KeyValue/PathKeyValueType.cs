using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AltBuild.LinkedPath
{
    /// <summary>
    /// KV type enum.
    /// </summary>
    [Flags]
    public enum PathKeyValueType
    {
        /// <summary>
        /// None
        /// </summary>
        None = 0x0000,

        /// <summary>
        /// Key only
        /// </summary>
        Key = 0x0001,

        /// <summary>
        /// Value only
        /// </summary>
        Value = 0x0002,
        
        /// <summary>
        /// Key and value
        /// </summary>
        KeyAndValue = Key | Value,

        /// <summary>
        /// Complex state (Error state)
        /// </summary>
        ComplexError = 0x1000,
    }
}
