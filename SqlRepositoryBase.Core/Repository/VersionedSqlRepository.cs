using Microsoft.EntityFrameworkCore;
using SqlRepositoryBase.Core.Exceptions;
using SqlRepositoryBase.Core.Models;
using SqlRepositoryBase.Core.Options;

namespace SqlRepositoryBase.Core.Repository;

public class VersionedSqlRepository<TVersionedStorageElement>
    : SqlRepository<TVersionedStorageElement>, IVersionedSqlRepository<TVersionedStorageElement>
    where TVersionedStorageElement : VersionedSqlStorageElement
{
    public VersionedSqlRepository(IConnectionStringProvider connectionStringProvider) : base(connectionStringProvider)
    {
    }

    public async Task ConcurrentUpdateAsync(Guid id, Action<TVersionedStorageElement> updateAction)
    {
        var (context, storage) = GetContextWithSet();
        var @object = await storage.FirstAsync(x => x.Id == id);
        var version = @object.Version;
        updateAction(@object);
        if (@object.Version != version)
        {
            throw new SqlConcurrentEntityUpdateException(id);
        }

        @object.Version++;

        await context.SaveChangesAsync();
    }
}