namespace AltBuild.LinkedPath
{
    /// <summary>
    /// Object initialization interface
    /// </summary>
    public interface IObjectFactory
    {
        void InitializeObject(ObjectInitializeParameters parameters);
    }
}
