using AltBuild.LinkedPath.Converters;
using AltBuild.LinkedPath.Parser;
using System;
using System.Diagnostics;
using System.Text;
using System.Runtime.CompilerServices;

namespace AltBuild.LinkedPath
{
    /// <summary>
    /// Extended definition of Object.
    /// </summary>
    public static class ObjectExtensions
    {
        /// <summary>
        /// Get default value flag.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>true: default(obj), false: not default(objt)</returns>
        public static bool IsDefault(object obj)
        {
            if (obj == null)
                return true;

            else if (obj.GetType().IsValueType)
                return obj.Equals(Activator.CreateInstance(obj.GetType()));

            else
                return false;
        }

        public static bool TryInduct(this object obj, PathMember pathMember, out InductInfo inductInfo) =>
            !(inductInfo = Induct(obj, pathMember)).Frame.IsUnduct;


        public static bool TryInduct(this object obj, string path, out InductInfo inductInfo) =>
            !(inductInfo = Induct(obj, path)).Frame.IsUnduct;

        /// <summary>
        /// Get path value
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static InductInfo Induct(this object obj, string path, InductMethods methods = InductMethods.Normal) =>
            InductInfo.Create(PathFactory.Parse(path), obj, null, methods);

        /// <summary>
        /// Get path value
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="pathMember"></param>
        /// <returns></returns>
        public static InductInfo Induct(this object obj, PathMember pathMember, InductMethods methods = InductMethods.Normal) =>
            InductInfo.Create(pathMember, obj, null, methods);

        /// <summary>
        /// Extended ToString.
        /// </summary>
        /// <param name="obj">Target object</param>
        /// <param name="pathElements">String of path elements</param>
        /// <returns></returns>
        public static string ToString(this object obj, string pathElements) =>
            _createElementString(obj, PathElements.Parse(pathElements));

        /// <summary>
        /// Extended ToString.
        /// </summary>
        /// <param name="obj">Target object</param>
        /// <param name="pathElements">Path elements.</param>
        /// <returns></returns>
        public static string ToString(this object obj, PathElements pathElements) =>
            _createElementString(obj, pathElements);

        /// <summary>
        /// Create a string of elements.
        /// </summary>
        /// <param name="obj">Target object</param>
        /// <param name="pathElements">Path elements.</param>
        /// <returns></returns>
        internal static string _createElementString(object obj, PathElements pathElements)
        {
            // Format Priority
            //   1) obj is null
            //      1-1)                                     =>  null
            //
            //   2) obj is not null
            //      2-1) Search FormatConvertAttribute       =>  Select FormatConverter with source type and format
            //      2-2) Search obj.GetType() == sourceType  =>  Select FormatConverter with source typp and format

            // Return null.
            if (obj == null)
                return null;

            // Delegate to IObjectBase
            if (obj is IInductiveBase iObjectBase)
                return iObjectBase.ToString(pathElements);

            // Return default ToString method value.
            if (pathElements == null)
                return obj.ToString();

            // Create a string of PathElements
            if (pathElements.TryGetFormatOnlyElement(out string format))
            {
                if (FormatConverterFactory.TryGetConverterWithDestineType(typeof(string), format, obj, out object destine))
                    return (string)destine;

                else if (TryGetStringWithValueType(obj, format, out string result))
                    return result;

                return obj.ToString();
            }

            else
            {
                // Build strings
                var results = new StringBuilder();

                // Repeat the elements
                foreach (var element in pathElements)
                {
                    if (element.Type == PathElementType.String)
                    {
                        results.Append(element.Source);
                    }

                    else if (element.Type == PathElementType.Value)
                    {
                        var inductInfo = obj.Induct(element.PathMember);
                        if (inductInfo?.ReturnValue != null)
                            results.Append(inductInfo.ReturnValue);
                    }
                    else
                        throw new NotImplementedException("PathElementType error.");
                }

                // return
                return results.ToString();
            }
        }

        /// <summary>
        /// Supported Type:
        ///  enum, value type
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="pathElements"></param>
        /// <returns></returns>
        public static bool TryGetStringWithValueType(object obj, string format, out string result)
        {
            // Assert
            Debug.Assert(obj != null);

            // Get type
            Type type = obj.GetType();

            // Value type
            if (type.IsEnum)
            {
                result = obj.ToString();
                return true;
            }

            else if (type.IsValueType)
            {
                if (obj is DateTime dateTime)
                {
                    result = String.Format("{0:yyyy-MM-dd}", obj);
                    return true;
                }

                else if (format == null || format == "@")
                {
                    if (IsNumeric((ValueType)obj))
                        result = RealNumberExtensions.ToRealString(obj.ToString());

                    else
                        result = obj.ToString();

                    return true;
                }

                else if (format.StartsWith("@"))
                {
                    result = obj.ToString();
                    return true;
                }

                result = String.Format("{0:" + format + "}", obj);
                return true;
            }

            result = null;
            return false;
        }

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
