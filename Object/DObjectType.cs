using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace AltBuild.LinkedPath
{
    [Flags]
    public enum DObjectType
    {
        None = 0x0000,

        /// <summary>
        /// 列挙処理できない
        /// </summary>
        NotEnumerable = 0x0001,


    }
}
