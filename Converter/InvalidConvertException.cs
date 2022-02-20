using System;
using System.Runtime.Serialization;

namespace AltBuild.LinkedPath.Converters
{
    /// <summary>
    /// The exception that is thrown for invalid converting.
    /// </summary>
    public class InvalidConvertException : SystemException
    {
        public InvalidConvertException() { }
        protected InvalidConvertException(SerializationInfo info, StreamingContext context) : base(info, context) { }
        public InvalidConvertException(string message) : base(message) { }
        public InvalidConvertException(string message, Exception innerException) : base(message, innerException) { }
    }
}
