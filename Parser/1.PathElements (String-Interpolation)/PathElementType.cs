namespace AltBuild.LinkedPath.Parser
{
    /// <summary>
    /// Element type enumerate
    /// </summary>
    public enum PathElementType
    {
        /// <summary>
        /// none
        /// </summary>
        None = default,

        /// <summary>
        /// static string element
        /// </summary>
        String = 0x0001,

        /// <summary>
        /// object value element
        /// </summary>
        Value = 0x0002,

        /// <summary>
        /// object paramenter element
        /// </summary>
        Parameter = 0x0004,

        /// <summary>
        /// environment format.
        /// </summary>
        Format = 0x0008,
    }
}
