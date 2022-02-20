using System;
using System.Collections.Concurrent;
using System.Text;
using System.Reflection;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace AltBuild.LinkedPath.Converters
{
    /// <summary>
    /// InductiveConverters base class.
    /// </summary>
    public class InductiveConverters
    {
        InductiveConverterBase[] _converterSources;

        ConcurrentDictionary<Type, InductiveConverterBase> _converterMappings = new();

        public InductiveConverters(InductiveConverterBase[] converters = null)
        {
            if (converters == null)
                _converterSources = Array.Empty<InductiveConverterBase>();

            else
                _converterSources = converters;
        }

        public bool TryGetConverter<T>(MemberInfo memberInfo, out InductiveConverterBase converter) =>
            TryGetConverter(typeof(T), memberInfo, out converter);

        public bool TryGetConverter(Type type, MemberInfo memberInfo, out InductiveConverterBase converter)
        {
            converter = _converterMappings.GetOrAdd(type, t =>
            {
                foreach (var converter in _converterSources)
                    if (converter.CanConvert(type, null, memberInfo))
                        return converter;

                return null;
            });
            return (converter != null);
        }

        public bool TryGetLink(object sourceObject, Type sourceType, [MaybeNullWhen(false)] out InductiveConverterBase converter, out object destineValue, [MaybeNullWhen(false)] out Type destineType)
        {
            foreach (var atConverter in _converterSources)
            {
                if (atConverter.TryGetLink(sourceObject, sourceType, out destineValue, out destineType))
                {
                    converter = atConverter;
                    return true;
                }
            }

            converter = default;
            destineValue = default;
            destineType = sourceObject?.GetType() ?? sourceType;

            return false;
        }
    }
}
