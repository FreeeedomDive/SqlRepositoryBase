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

    /// <summary>
    ///     Read all entities from table
    /// </summary>
    public async Task<TStorageElement[]> ReadAllAsync()
    {
        return await storage.ToArrayAsync();
    }

    /// <summary>
    ///     Read entity by id
    /// </summary>
    /// <exception cref="SqlEntityNotFoundException">If there is no entities with this id, SqlEntityNotFoundException will be thrown</exception>
    public async Task<TStorageElement> ReadAsync(Guid id)
    {
        var result = await TryReadAsync(id);
        if (result == null)
        {
            throw new SqlEntityNotFoundException(id);
        }

        return result;
    }

    /// <summary>
    ///     Read all entities with id
    /// </summary>
    public async Task<TStorageElement[]> ReadAllAsync(Guid id)
    {
        return await FindAsync(x => x.Id == id);
    }

    /// <summary>
    ///     Read entities with given ids without preserving order
    /// </summary>
    public async Task<TStorageElement[]> ReadAsync(Guid[] ids)
    {
        return await storage.Where(x => ids.Contains(x.Id)).ToArrayAsync();
    }
    /// <summary>
    ///     Read entity by id
    /// </summary>
    /// <returns>Entity with given id or null if entity doesn't exist</returns>
    public async Task<TStorageElement?> TryReadAsync(Guid id)
    {
        return await storage.FirstOrDefaultAsync(x => x.Id == id);
    }

    /// <summary>
    ///     Read all entities by predicate
    /// </summary>
    public async Task<TStorageElement[]> FindAsync(Expression<Func<TStorageElement, bool>> predicate)
    {
        return await storage.Where(predicate).ToArrayAsync();
    }

    /// <summary>
    ///     Build custom query with IQueryable
    /// </summary>
    public IQueryable<TStorageElement> BuildCustomQuery()
    {
        return storage.AsQueryable();
    }

    /// <summary>
    ///     Execute operations with DbContext, all changes will be saved automatically
    /// </summary>
    public async Task ExecuteWithDatabaseContextAsync(Func<DbContext, Task> func)
    {
        await func(databaseContext);
        await databaseContext.SaveChangesAsync();
    }

    /// <summary>
    ///     Create a new entity
    /// </summary>
    public async Task CreateAsync(TStorageElement storageElement)
    {
        await storage.AddAsync(storageElement);
        await databaseContext.SaveChangesAsync();
    }

    /// <summary>
    ///     Create multiple entities
    /// </summary>
    public async Task CreateAsync(IEnumerable<TStorageElement> storageElements)
    {
        await storage.AddRangeAsync(storageElements);
        await databaseContext.SaveChangesAsync();
    }

    /// <summary>
    ///     Update entity by id
    /// </summary>
    public async Task UpdateAsync(Guid id, Action<TStorageElement> updateAction)
    {
        var @object = await storage.FirstAsync(x => x.Id == id);
        updateAction(@object);
        await databaseContext.SaveChangesAsync();
    }

    /// <summary>
    ///     Delete entities by ids
    /// </summary>
    public async Task DeleteAsync(params Guid[] ids)
    {
        var objects = await ReadAsync(ids);
        if (objects.Length == 0)
        {
            return;
        }

        await DeleteAsync(objects);
    }

    /// <summary>
    ///     Delete entities
    /// </summary>
    public async Task DeleteAsync(params TStorageElement[] storageElements)
    {
        storage.RemoveRange(storageElements);
        await databaseContext.SaveChangesAsync();
    }

    protected readonly DbContext databaseContext;
    protected readonly DbSet<TStorageElement> storage;
}