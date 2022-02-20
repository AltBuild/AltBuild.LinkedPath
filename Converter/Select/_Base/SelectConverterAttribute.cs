using AltBuild.LinkedPath.Converters;
using System;

namespace AltBuild.LinkedPath.Converters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Property, AllowMultiple = true)]
    public class SelectConverterAttribute : ConverterBaseAttribute
    {
        public SelectConverterAttribute(Type converterType) : base(converterType)
        {
        }
    }
}
