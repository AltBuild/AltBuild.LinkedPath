using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace AltBuild.LinkedPath
{
    public enum DValueType
    {
        /// <summary>
        /// ïsñæ
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// bool: bool: false, true
        /// number: byte, short, int, long: 0, 123, -123, 0x1234
        /// decimal: decimal, double, float: 10.1234, -.98765
        /// char: '*'
        /// </summary>
        Value,

        /// <summary>
        /// Position, Checked, User3 Åc
        /// </summary>
        MemberName,
    }
}
