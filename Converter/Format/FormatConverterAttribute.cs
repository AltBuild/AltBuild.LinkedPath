using System;

namespace AltBuild.LinkedPath.Converters
{
    /// <summary>
    /// DatastoreModel converter attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = false)]
    public class FormatConverterAttribute : Attribute
    {
        public Type ConverterType { get; init; }

        public FormatConverterAttribute(Type converterType)
        {
            ConverterType = converterType;
        }
    }
}
