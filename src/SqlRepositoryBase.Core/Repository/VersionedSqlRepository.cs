using Microsoft.EntityFrameworkCore;
using SqlRepositoryBase.Core.Exceptions;
using SqlRepositoryBase.Core.Models;

namespace SqlRepositoryBase.Core.Repository;

public class VersionedSqlRepository<TVersionedStorageElement, TDbContext>
    : SqlRepository<TVersionedStorageElement, TDbContext>, IVersionedSqlRepository<TVersionedStorageElement>
    where TVersionedStorageElement : VersionedSqlStorageElement where TDbContext : DbContext
{
    public VersionedSqlRepository(IDbContextFactory<TDbContext> dbContextFactory) : base(dbContextFactory)
    {
    }

    public async Task ConcurrentUpdateAsync(Guid id, Action<TVersionedStorageElement> updateAction)
    {
        var (context, storage) = await GetContextWithSet();
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