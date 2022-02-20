using System;

namespace AltBuild.LinkedPath.Converters
{
    /// <summary>
    /// Format converter interface
    /// </summary>
    public interface IFormatConverter
    {
        Type SourceType { get; }

        Type DestineType { get; }

        bool TryConvert(string format, object source, out object destine);
    }
}
