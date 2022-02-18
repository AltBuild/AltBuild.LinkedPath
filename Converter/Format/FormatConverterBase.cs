using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Runtime.CompilerServices;

namespace AltBuild.LinkedPath
{
    /// <summary>
    /// Format with Type converter basis class
    /// </summary>
    public class FormatConverterBase : IFormatConverter
    {
        public virtual Type SourceType => null;

        public virtual Type DestineType => null;

        public virtual bool TryConvert(string format, object source, out object destine)
        {
            throw new NotImplementedException();
        }
    }
}
