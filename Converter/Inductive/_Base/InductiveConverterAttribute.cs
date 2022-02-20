using System;

namespace AltBuild.LinkedPath.Converters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Property, AllowMultiple = true)]
    public class InductiveConverterAttribute : ConverterBaseAttribute
    {
        public InductiveConverterAttribute(Type converterType)
            : base(converterType)
        {
        }
    }
}
