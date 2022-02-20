using System;

namespace AltBuild.LinkedPath
{
    [Flags]
    public enum ECopyTargets
    {
        /// <summary>
        /// Not edit (Default)
        /// </summary>
        None = default,

        /// <summary>
        /// Up to primary record
        /// </summary>
        First = 0x0001,

        /// <summary>
        /// Up to secondary record
        /// </summary>
        Second = 0x0002,

        /// <summary>
        /// Data tables are covered
        /// </summary>
        Data = 0x1000,

        /// <summary>
        /// Master table is covered
        /// </summary>
        Master = 0x2000,

        /// <summary>
        /// Up to primary record with Data tables.
        /// </summary>
        FirstData = First | Data,

        /// <summary>
        /// Up to primary record with Master tables.
        /// </summary>
        FirstMaster = First | Master,

        /// <summary>
        /// Up to secondary record with Data tables.
        /// </summary>
        SecondData = Second | Data,

        /// <summary>
        /// Up to secondary record with Master tables.
        /// </summary>
        SecondMaster = Second | Master,

        /// <summary>
        /// All record with Master and Data tables.
        /// </summary>
        All = Data | Master,

        /// <summary>
        /// Auto mode
        /// </summary>
        Auto = 0x800000,
    }
}
