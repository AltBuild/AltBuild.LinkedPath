using System;

namespace AltBuild.LinkedPath
{
    [AttributeUsage(AttributeTargets.Interface, AllowMultiple = false)]
    public class JsonConverterAttribute : System.Text.Json.Serialization.JsonConverterAttribute
    {
        public JsonConverterAttribute(Type converterType)
            : base(converterType)
        {
        }
    }
}
