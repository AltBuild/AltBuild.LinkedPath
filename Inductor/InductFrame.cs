using AltBuild.LinkedPath.Converters;
using AltBuild.LinkedPath.Parser;
using System;
using System.Reflection;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics;

namespace AltBuild.LinkedPath
{
    /// <summary>
    /// Results from object and PathMember
    /// </summary>
    public class InductFrame
    {
        /// <summary>
        /// Original member value
        /// </summary>
        public object SourceObject { get; init; }

        /// <summary>
        /// Original member value
        /// </summary>
        public object SourceType { get; init; }

        /// <summary>
        /// Processing methods
        /// </summary>
        public InductMethods Methods { get; init; }

        /// <summary>
        /// Induct error
        /// </summary>
        public bool IsUnduct { get; internal set; }

        public InductFrame(object sourceObject, Type sourceType, InductMethods methods)
        {
            SourceObject = sourceObject;
            SourceType = sourceType;
            Methods = methods;
        }
    }
}
