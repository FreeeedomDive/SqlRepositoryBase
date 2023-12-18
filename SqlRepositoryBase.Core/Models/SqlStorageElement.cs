using System.ComponentModel.DataAnnotations;

namespace SqlRepositoryBase.Core.Models;

public class SqlStorageElement
{
    [Key] public Guid Id { get; set; }
}