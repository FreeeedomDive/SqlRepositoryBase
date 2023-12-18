using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using SqlRepositoryBase.Core.ContextBuilders;
using SqlRepositoryBase.Core.Exceptions;
using SqlRepositoryBase.Core.Models;

namespace SqlRepositoryBase.Core.Repository;

public class SqlRepository<TStorageElement> : ISqlRepository<TStorageElement> where TStorageElement : SqlStorageElement
{
    public SqlRepository(IDbContextFactory dbContextFactory)
    {
        this.dbContextFactory = dbContextFactory;
    }

    public async Task<TStorageElement[]> ReadAllAsync()
    {
        return await GetContextWithSet().Storage.ToArrayAsync();
    }

    public async Task<TStorageElement> ReadAsync(Guid id)
    {
        var result = await TryReadAsync(id);
        if (result == null)
        {
            throw new SqlEntityNotFoundException(id);
        }

        return result;
    }

    public async Task<TStorageElement[]> ReadAllAsync(Guid id)
    {
        return await FindAsync(x => x.Id == id);
    }

    public async Task<TStorageElement[]> ReadAsync(Guid[] ids)
    {
        return await GetContextWithSet().Storage.Where(x => ids.Contains(x.Id)).ToArrayAsync();
    }

    public async Task<TStorageElement?> TryReadAsync(Guid id)
    {
        return await GetContextWithSet().Storage.FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<TStorageElement[]> FindAsync(Expression<Func<TStorageElement, bool>> predicate)
    {
        return await GetContextWithSet().Storage.Where(predicate).ToArrayAsync();
    }

    public IQueryable<TStorageElement> BuildCustomQuery()
    {
        return GetContextWithSet().Storage.AsQueryable();
    }

    public async Task ModifyDbSetAsync(Func<DbSet<TStorageElement>, Task> func)
    {
        var (context, storage) = GetContextWithSet();
        await func(storage);
        await context.SaveChangesAsync();
    }

    public async Task CreateAsync(TStorageElement storageElement)
    {
        var (context, storage) = GetContextWithSet();
        await storage.AddAsync(storageElement);
        await context.SaveChangesAsync();
    }

    public async Task CreateAsync(IEnumerable<TStorageElement> storageElements)
    {
        var (context, storage) = GetContextWithSet();
        await storage.AddRangeAsync(storageElements);
        await context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Guid id, Action<TStorageElement> updateAction)
    {
        var (context, storage) = GetContextWithSet();
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
        var (context, storage) = GetContextWithSet();
        storage.RemoveRange(storageElements);
        await context.SaveChangesAsync();
    }

    protected (DbContext Context, DbSet<TStorageElement> Storage) GetContextWithSet()
    {
        var context = dbContextFactory.Build();
        var set = context.Set<TStorageElement>();
        return (context, set);
    }

    private readonly IDbContextFactory dbContextFactory;
}