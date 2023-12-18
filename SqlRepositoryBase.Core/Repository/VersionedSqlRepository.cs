using Microsoft.EntityFrameworkCore;
using SqlRepositoryBase.Core.ContextBuilders;
using SqlRepositoryBase.Core.Exceptions;
using SqlRepositoryBase.Core.Models;

namespace SqlRepositoryBase.Core.Repository;

public class VersionedSqlRepository<TVersionedStorageElement>
    : SqlRepository<TVersionedStorageElement>, IVersionedSqlRepository<TVersionedStorageElement>
    where TVersionedStorageElement : VersionedSqlStorageElement
{
    public VersionedSqlRepository(IDbContextFactory dbContextFactory) : base(dbContextFactory)
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