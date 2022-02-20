using System;

namespace AltBuild.LinkedPath.Converters
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
