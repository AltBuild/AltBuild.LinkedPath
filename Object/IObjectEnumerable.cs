using AltBuild.BaseExtensions;
using AltBuild.LinkedPath;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace AltBuild.LinkedPath
{
    /// <summary>
    ///  (Use DObject)オブジェクト列挙用
    ///  Use: DObject
    ///  difference:
    ///    IEnumerable       -> for general [GetFirst(false)]
    ///    IObjectEnumerable -> Support for copying [GetFirst(true) or GetFirst(false)]
    /// </summary>
    public interface IObjectEnumerable
    {
        object[] ToExtract(int? index, bool isCopyCheck);
    }
}
