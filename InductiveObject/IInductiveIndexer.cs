namespace AltBuild.LinkedPath
{
    /// <summary>
    /// Object indexer interface
    /// </summary>
    public interface IInductiveIndexer
    {
        object this[int index] { get; set; }

        int Count { get; }

        bool CanAdd { get; }

        object AddNewRecord(int? indexOfInsert = null);
    }
}
