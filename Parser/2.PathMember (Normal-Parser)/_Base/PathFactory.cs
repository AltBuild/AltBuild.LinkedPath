using System.Collections.Generic;
using System.Reflection;

namespace AltBuild.LinkedPath.Parser
{
    /// <summary>
    /// Parser class to parse link paths
    /// </summary>
    public static class PathFactory
    {
        /// <summary>
        ///  List of paths to be used statically, not parsed
        /// </summary>
        public static List<string> StaticMarkups { get; set; } = new List<string>();

        public static PathMemberCollection Parses(string line) => new PathMemberCollection(line);

        public static PathMember Parse(string line) => new PathMemberCollection(line).GetFirst();

        public static PathMember ParseOfLabel(string label) => _labeling(label);

        public static PathMember ParseOfFormat(string format) => _formatting(format);

        static PathMember _labeling(string label)
        {
            int indexOfSeparate = label.IndexOf(':');
            if (indexOfSeparate >= 0)
            {
                PathMember path = new PathMember(label.Substring(0, indexOfSeparate), null, null, null);
                ((IPathFrameInner)path.Frame).SetAttributes(label.Substring(indexOfSeparate + 1));
                return path;
            }
            else
            {
                return new PathMember(label, new PathFrame(), null, null);
            }
        }

        /// <summary>
        /// Create PathMember with formatting only.
        /// </summary>
        /// <param name="format"></param>
        /// <returns></returns>
        static PathMember _formatting(string format)
        {
            // Init PathMember.
            var pathMember = new PathMember();

            // Add format.
            pathMember.Frame.Attributes.Add(new PathAttribute("format", format));

            return pathMember;
        }
    }
}
