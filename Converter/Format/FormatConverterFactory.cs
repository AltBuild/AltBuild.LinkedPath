using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Text;
using System.Threading;
using System.Runtime.CompilerServices;

namespace AltBuild.LinkedPath
{
    /// <summary>
    /// Format type converter
    /// </summary>
    public class FormatConverterFactory
    {
        static ConcurrentDictionary<Type, List<IFormatConverter>> destineStore = new();
        static ConcurrentDictionary<Type, List<IFormatConverter>> sourceStore = new();

        public static void Add(IFormatConverter converter)
        {
            if (converter.DestineType != null)
                destineStore.GetOrAdd(converter.DestineType, t => new List<IFormatConverter>()).Add(converter);

            else if (converter.SourceType != null)
                sourceStore.GetOrAdd(converter.SourceType, t => new List<IFormatConverter>()).Add(converter);
        }

        public static bool TryGetValueWithSourceType(Type sourceType, string format, object source, out object destine)
        {
            if (sourceStore.TryGetValue(sourceType, out var converters))
                foreach (var atConverter in converters)
                    if (atConverter.TryConvert(format, source, out destine))
                        return true;

            destine = null;
            return false;
        }

        public static bool TryGetConverterWithDestineType(Type destineType, string format, object source, out object destine)
        {
            if (destineStore.TryGetValue(destineType, out var converters))
                foreach (var atConverter in converters)
                    if (atConverter.TryConvert(format, source, out destine))
                        return true;

            destine = null;
            return false;
        }
    }
}
