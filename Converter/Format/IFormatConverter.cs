using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Runtime.CompilerServices;

namespace AltBuild.LinkedPath
{
    /// <summary>
    /// インターフェース
    /// </summary>
    public interface IFormatConverter
    {
        Type SourceType { get; }

        Type DestineType { get; }

        bool TryConvert(string format, object source, out object destine);
    }
}
