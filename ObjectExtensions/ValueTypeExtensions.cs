using System;
using System.Runtime.CompilerServices;

namespace AltBuild.LinkedPath
{
    /// <summary>
    /// Extended definition of Object.
    /// </summary>
    public static class ValueTypeExtensions
    {
        public static bool IsInteger(this ValueType value) =>
            value is Int32 || value is Int64 || value is Int16 ||
            value is Byte || value is SByte ||
            value is UInt32 || value is UInt64 || value is UInt16;

        public static bool IsFloat(this ValueType value) =>
            value is decimal || value is double || value is float;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNumeric(ValueType value) => IsInteger(value) || IsFloat(value);
    }
}
