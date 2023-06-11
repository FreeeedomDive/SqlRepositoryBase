using Microsoft.EntityFrameworkCore;
using SqlRepositoryBase.Core.Exceptions;
using SqlRepositoryBase.Core.Models;

namespace SqlRepositoryBase.Core.Repository;

public class VersionedSqlRepository<TVersionedStorageElement>
    : SqlRepository<TVersionedStorageElement>, IVersionedSqlRepository<TVersionedStorageElement>
    where TVersionedStorageElement : VersionedSqlStorageElement
{
    public VersionedSqlRepository(DbContext databaseContext) : base(databaseContext)
    {
    }

    /// <summary>
    ///     Update an entity with concurrent check. You need to update entity version inside of updateAction. 
    /// </summary>
    /// <exception cref="SqlConcurrentEntityUpdateException">If provided version is different from the saved version at the beginning of the operation, SqlConcurrentEntityUpdateException will be thrown</exception>
    public async Task ConcurrentUpdateAsync(Guid id, Action<TVersionedStorageElement> updateAction)
    {
        var @object = await storage.FirstAsync(x => x.Id == id);
        var version = @object.Version;
        updateAction(@object);
        if (@object.Version != version)
        {
            throw new SqlConcurrentEntityUpdateException(id);
        }

        @object.Version++;

        await databaseContext.SaveChangesAsync();
    }
}