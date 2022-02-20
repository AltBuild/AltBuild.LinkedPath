using System;

namespace AltBuild.LinkedPath
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class CacheAttribute : System.Attribute
    {
        /// <summary>
        /// Empty object.
        /// </summary>
        public static readonly CacheAttribute Empty = new CacheAttribute();

        public CacheFlag Flags { get; set; }

        public SeriesCacheFlag Series { get; set; }

        public int Mask { get; set; }

        public CacheAttribute() { }

        public CacheAttribute(CacheFlag flags)
        {
            Flags = flags;
        }
    }
}
