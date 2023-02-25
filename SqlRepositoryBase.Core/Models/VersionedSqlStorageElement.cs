namespace SqlRepositoryBase.Core.Models;

public class VersionedSqlStorageElement : SqlStorageElement
{
    public long Version { get; set; }
}