using SqlRepositoryBase.Core.Exceptions;
using SqlRepositoryBase.Core.Models;

namespace SqlRepositoryBase.Core.Repository;

public interface IVersionedSqlRepository<TVersionedStorageElement> : ISqlRepository<TVersionedStorageElement>
    where TVersionedStorageElement : VersionedSqlStorageElement
{
    /// <summary>
    ///     Update an entity with concurrent check. You need to update entity version inside of updateAction.
    /// </summary>
    /// <exception cref="SqlConcurrentEntityUpdateException">
    ///     If provided version is different from the saved version at the beginning of the operation, SqlConcurrentEntityUpdateException
    ///     will be thrown
    /// </exception>
    Task ConcurrentUpdateAsync(Guid id, Action<TVersionedStorageElement> updateAction);
}