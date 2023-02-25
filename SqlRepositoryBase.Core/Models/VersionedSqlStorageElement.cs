using System.ComponentModel.DataAnnotations.Schema;

namespace SqlRepositoryBase.Core.Models;

public class VersionedSqlStorageElement : SqlStorageElement
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Version { get; set; }
}