using System.Linq.Expressions;
using SqlRepositoryBase.Core.Models;

namespace SqlRepositoryBase.Core.Repository;

public interface ISqlRepository<TStorageElement> where TStorageElement : SqlStorageElement
{
    Task<TStorageElement[]> ReadAllAsync();
    Task<TStorageElement> ReadAsync(Guid id);
    Task<TStorageElement[]> ReadAllAsync(Guid id);
    Task<TStorageElement[]> ReadAsync(Guid[] ids);
    Task<TStorageElement?> TryReadAsync(Guid id);
    Task<TStorageElement[]> FindAsync(Expression<Func<TStorageElement, bool>> predicate);
    IQueryable<TStorageElement> BuildCustomQuery();
    Task CreateAsync(TStorageElement storageElement);
    Task CreateAsync(IEnumerable<TStorageElement> storageElements);
    Task UpdateAsync(Guid id, Action<TStorageElement> updateAction);
    Task DeleteAsync(params Guid[] ids);
    Task DeleteAsync(params TStorageElement[] storageElements);
}