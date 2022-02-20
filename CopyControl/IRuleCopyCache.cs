namespace AltBuild.LinkedPath
{
    /// <summary>
    /// Rule cache.
    /// </summary>
    public interface IRuleCopyCache
    {
        bool TryInclude(object obj);
    }
}
