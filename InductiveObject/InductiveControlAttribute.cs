using System;

namespace AltBuild.LinkedPath
{
    /// <summary>
    /// Attribute used in the control member of an Inductive Object.
    /// (Indicates a member that is not normally used by the user.)
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Method, AllowMultiple = false)]
    public class InductiveControlAttribute : System.Attribute
    {
    }
}
