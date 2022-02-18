using System;

namespace AltBuild.LinkedPath
{
    /// <summary>
    /// DatastoreModel converter attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface, AllowMultiple = false)]
    public class FormatConverterAttribute : Attribute
    {
        public Type ConverterType { get; init; }

        public FormatConverterAttribute(Type converterType)
        {
            ConverterType = converterType;
        }
    }
}
