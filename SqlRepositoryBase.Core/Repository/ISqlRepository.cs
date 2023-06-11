using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using SqlRepositoryBase.Core.Exceptions;
using SqlRepositoryBase.Core.Models;

namespace SqlRepositoryBase.Core.Repository;

public interface ISqlRepository<TStorageElement> where TStorageElement : SqlStorageElement
{
    /// <summary>
    ///     Read all entities from table
    /// </summary>
    Task<TStorageElement[]> ReadAllAsync();

    /// <summary>
    ///     Read entity by id
    /// </summary>
    /// <exception cref="SqlEntityNotFoundException">If there is no entities with this id, SqlEntityNotFoundException will be thrown</exception>
    Task<TStorageElement> ReadAsync(Guid id);

    /// <summary>
    ///     Read all entities with id
    /// </summary>
    Task<TStorageElement[]> ReadAllAsync(Guid id);

    /// <summary>
    ///     Read entities with given ids without preserving order
    /// </summary>
    Task<TStorageElement[]> ReadAsync(Guid[] ids);

    /// <summary>
    ///     Read entity by id
    /// </summary>
    /// <returns>Entity with given id or null if entity doesn't exist</returns>
    Task<TStorageElement?> TryReadAsync(Guid id);

    /// <summary>
    ///     Read all entities by predicate
    /// </summary>
    Task<TStorageElement[]> FindAsync(Expression<Func<TStorageElement, bool>> predicate);

    /// <summary>
    ///     Build custom query with IQueryable
    /// </summary>
    IQueryable<TStorageElement> BuildCustomQuery();

    /// <summary>
    ///     Execute operations directly with DbSet, all changes will be saved automatically
    /// </summary>
    Task ModifyDbSetAsync(Func<DbSet<TStorageElement>, Task> func);

    /// <summary>
    ///     Create a new entity
    /// </summary>
    Task CreateAsync(TStorageElement storageElement);

    /// <summary>
    ///     Create multiple entities
    /// </summary>
    Task CreateAsync(IEnumerable<TStorageElement> storageElements);

    /// <summary>
    ///     Update entity by id
    /// </summary>
    Task UpdateAsync(Guid id, Action<TStorageElement> updateAction);

    /// <summary>
    ///     Delete entities by ids
    /// </summary>
    Task DeleteAsync(params Guid[] ids);

    /// <summary>
    ///     Delete entities
    /// </summary>
    Task DeleteAsync(params TStorageElement[] storageElements);
}