namespace AltBuild.LinkedPath
{
    /// <summary>
    /// Rule cache.
    /// </summary>
    public interface IRuleCache
    {
        bool TryInclude(object obj);
    }
}
