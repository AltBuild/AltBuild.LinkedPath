using System;

namespace AltBuild.LinkedPath
{
    /// <summary>
    ///  Use: DObject
    ///  difference:
    ///    IEnumerable       -> for general [GetFirst(false)]
    ///    IObjectEnumerable -> Support for copying and asof GenDate [GetFirst(true) or GetFirst(false)]
    /// </summary>
    public interface IObjectEnumerable
    {
        object[] ToExtract(int? index, DateTime? asOfGenDate, bool isCopyCheck);
    }
}
