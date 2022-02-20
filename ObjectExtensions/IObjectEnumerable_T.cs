using System;

namespace AltBuild.LinkedPath
{
    /// <summary>
    ///  Use: DObject
    ///  difference:
    ///    IEnumerable       -> for general [GetFirst(false)]
    ///    IObjectEnumerable -> Support for copying [GetFirst(true) or GetFirst(false)]
    /// </summary>
    public interface IObjectEnumerable<out T> : IObjectEnumerable
    {
        new T[] ToExtract(int? index, DateTime? asOfGenDate, bool isCopyCheck);
    }
}
