using System;

namespace AltBuild.LinkedPath.Converters
{
    /// <summary>
    /// Converter basis attribute.
    /// </summary>
    public abstract class ConverterBaseAttribute : Attribute
    {
        public Type ConverterType { get; init; }

        public ConverterBaseAttribute(Type converterType)
        {
            ConverterType = converterType;
        }
    }
}
