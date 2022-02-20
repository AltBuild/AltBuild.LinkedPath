using AltBuild.LinkedPath.Converters;
using AltBuild.LinkedPath.Parser;
using System;
using System.Diagnostics;

namespace AltBuild.LinkedPath
{
    /// <summary>
    /// Creating InductInfomation 
    /// </summary>
    public partial class InductInfo
    {
        /// <summary>
        /// Create InductInfo.
        /// </summary>
        /// <param name="pathMember">PathMember</param>
        /// <param name="sourceObject">source object</param>
        /// <param name="sourceType">source object</param>
        /// <returns>InductInfo</returns>
        public static InductInfo Create(PathMember pathMember, object sourceObject, Type sourceType = null, InductMethods methods = InductMethods.Normal)
        {
            var atType = sourceType ?? sourceObject?.GetType();
            var frame = new InductFrame(sourceObject, atType, methods);

            if (pathMember.Name == null && pathMember.Child == null)
                return new InductInfo { Frame = frame, BaseObject = sourceObject, ReturnType = atType, PathMember = pathMember, MemberInfo = atType };

            else
                return Create(frame, pathMember, sourceObject, atType).Last;
        }

        static InductInfo Create(InductFrame frame, PathMember pathMember, object baseObject, Type baseType = null)
        {
            // Escalation to parent object.
            if (pathMember.Name == ".")
            {
                if (pathMember.Child != null && baseObject is IInductiveBase iObjectBase && iObjectBase.DefinitionObject != null)
                    return new InductInfo { Frame = frame, BaseObject = baseObject, PathMember = pathMember, ReturnValue = iObjectBase.DefinitionObject };
            }

            // Get member value.
            else if (InductHelper.TryGetPathValue(out InductInfo inductInfo, baseObject, baseType, pathMember.Name, pathMember, frame))
            {
                pathMember.Type = inductInfo.MemberInfo.MemberType;
                return inductInfo;
            }

            frame.IsUnduct = true;
            return new InductInfo { Frame = frame, BaseObject = baseObject, ReturnType = baseType, PathMember = pathMember };
        }

        public static InductInfo CreateInner(InductFrame frame, PathMember pathMember, object baseObject, Type baseType = null, InductMethods methods = InductMethods.Normal)
        {
            var atType = baseType ?? baseObject?.GetType();

            if (frame == null)
                frame = new InductFrame(baseObject, atType, methods);

            // Get member value.
            if (InductHelper.TryGetPathValue(out InductInfo inductInfo, baseObject, atType, pathMember.Name, pathMember, frame))
            {
                pathMember.Type = inductInfo.MemberInfo.MemberType;
                return inductInfo;
            }

            frame.IsUnduct = true;
            return new InductInfo { Frame = frame, BaseObject = baseObject, ReturnType = baseType, PathMember = pathMember };
        }
    }
}
