using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using SqlRepositoryBase.Core.Exceptions;
using SqlRepositoryBase.Core.Models;

namespace SqlRepositoryBase.Core.Repository;

public class SqlRepository<TStorageElement, TDbContext> : ISqlRepository<TStorageElement>
    where TStorageElement : SqlStorageElement
    where TDbContext : DbContext
{
    private readonly IDbContextFactory<TDbContext> factory;

    public SqlRepository(IDbContextFactory<TDbContext> factory)
    {
        this.factory = factory;
    }

    public async Task<TStorageElement[]> ReadAllAsync()
    {
        var storage = (await GetContextWithSet()).Storage;
        return await storage.ToArrayAsync();
    }

    public async Task<TStorageElement> ReadAsync(Guid id)
    {
        var result = await TryReadAsync(id);
        return result ?? throw new SqlEntityNotFoundException(id);
    }

    public async Task<TStorageElement[]> ReadAllAsync(Guid id)
    {
        return await FindAsync(x => x.Id == id);
    }

    public async Task<TStorageElement[]> ReadAsync(Guid[] ids)
    {
        var storage = (await GetContextWithSet()).Storage;
        return await storage.Where(x => ids.Contains(x.Id)).ToArrayAsync();
    }

    public async Task<TStorageElement?> TryReadAsync(Guid id)
    {
        var storage = (await GetContextWithSet()).Storage;
        return await storage.FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<TStorageElement[]> FindAsync(Expression<Func<TStorageElement, bool>> predicate)
    {
        var storage = (await GetContextWithSet()).Storage;
        return await storage.Where(predicate).ToArrayAsync();
    }

    public async Task<IQueryable<TStorageElement>> BuildCustomQueryAsync()
    {
        var storage = (await GetContextWithSet()).Storage;
        return storage.AsQueryable();
    }

    public async Task ModifyDbSetAsync(Func<DbSet<TStorageElement>, Task> func)
    {
        var (context, storage) = await GetContextWithSet();
        await func(storage);
        await context.SaveChangesAsync();
    }

    public async Task CreateAsync(TStorageElement storageElement)
    {
        var (context, storage) = await GetContextWithSet();
        await storage.AddAsync(storageElement);
        await context.SaveChangesAsync();
    }

    public async Task CreateAsync(IEnumerable<TStorageElement> storageElements)
    {
        var (context, storage) = await GetContextWithSet();
        await storage.AddRangeAsync(storageElements);
        await context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Guid id, Action<TStorageElement> updateAction)
    {
        var (context, storage) = await GetContextWithSet();
        var @object = await storage.FirstAsync(x => x.Id == id);
        updateAction(@object);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(params Guid[] ids)
    {
        var objects = await ReadAsync(ids);
        if (objects.Length == 0)
        {
            return;
        }

        await DeleteAsync(objects);
    }

    public async Task DeleteAsync(params TStorageElement[] storageElements)
    {
        var (context, storage) = await GetContextWithSet();
        storage.RemoveRange(storageElements);
        await context.SaveChangesAsync();
    }

    protected async Task<(DbContext Context, DbSet<TStorageElement> Storage)> GetContextWithSet()
    {
        var context = await factory.CreateDbContextAsync();
        var set = context.Set<TStorageElement>();
        return (context, set);
    }
}