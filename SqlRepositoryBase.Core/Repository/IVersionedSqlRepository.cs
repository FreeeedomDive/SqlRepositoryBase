using SqlRepositoryBase.Core.Models;

namespace SqlRepositoryBase.Core.Repository;

public interface IVersionedSqlRepository<TVersionedStorageElement> : ISqlRepository<TVersionedStorageElement>
    where TVersionedStorageElement : VersionedSqlStorageElement
{
    Task ConcurrentUpdateAsync(Guid id, Action<TVersionedStorageElement> updateAction);
}