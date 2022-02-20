using System;

namespace AltBuild.LinkedPath
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false)]
    public class CodeAttribute : System.Attribute
    {
        /// <summary>
        /// Empty object.
        /// </summary>
        public static readonly CodeAttribute Empty = new CodeAttribute();

        public string Name { get; init; }

        public CodeAttribute() { }

        public CodeAttribute(string name)
        {
            Name = name;
        }
    }
}
