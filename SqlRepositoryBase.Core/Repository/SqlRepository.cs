using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using SqlRepositoryBase.Core.Exceptions;
using SqlRepositoryBase.Core.Models;

namespace SqlRepositoryBase.Core.Repository;

public class SqlRepository<TStorageElement> : ISqlRepository<TStorageElement> where TStorageElement : SqlStorageElement
{
    public SqlRepository(DbContext databaseContext)
    {
        this.databaseContext = databaseContext;
        storage = databaseContext.Set<TStorageElement>();
    }

    public async Task<TStorageElement[]> ReadAllAsync()
    {
        return await storage.ToArrayAsync();
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
        return await storage.Where(x => ids.Contains(x.Id)).ToArrayAsync();
    }

    public async Task<TStorageElement?> TryReadAsync(Guid id)
    {
        return await storage.FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<TStorageElement[]> FindAsync(Expression<Func<TStorageElement, bool>> predicate)
    {
        return await storage.Where(predicate).ToArrayAsync();
    }

    public IQueryable<TStorageElement> BuildCustomQuery()
    {
        return storage.AsQueryable();
    }

    public async Task ModifyDbSetAsync(Func<DbSet<TStorageElement>, Task> func)
    {
        await func(storage);
        await databaseContext.SaveChangesAsync();
    }

    public async Task CreateAsync(TStorageElement storageElement)
    {
        await storage.AddAsync(storageElement);
        await databaseContext.SaveChangesAsync();
    }

    public async Task CreateAsync(IEnumerable<TStorageElement> storageElements)
    {
        await storage.AddRangeAsync(storageElements);
        await databaseContext.SaveChangesAsync();
    }

    public async Task UpdateAsync(Guid id, Action<TStorageElement> updateAction)
    {
        var @object = await storage.FirstAsync(x => x.Id == id);
        updateAction(@object);
        await databaseContext.SaveChangesAsync();
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
        storage.RemoveRange(storageElements);
        await databaseContext.SaveChangesAsync();
    }

    protected readonly DbContext databaseContext;
    protected readonly DbSet<TStorageElement> storage;
}