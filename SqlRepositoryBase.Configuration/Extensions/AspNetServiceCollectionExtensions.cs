using Microsoft.Extensions.DependencyInjection;
using SqlRepositoryBase.Core.Models;
using SqlRepositoryBase.Core.Repository;

namespace SqlRepositoryBase.Configuration.Extensions;

public static class AspNetServiceCollectionExtensions
{
    public static IServiceCollection ConfigurePostgreSql(this IServiceCollection services)
    {
        var allTypes = AppDomain.CurrentDomain.GetAssemblies().SelectMany(s => s.GetTypes()).ToArray();

        var sqlStorageElementTypes = allTypes
            .Where(p => typeof(SqlStorageElement).IsAssignableFrom(p))
            .Where(p => p != typeof(SqlStorageElement));
        foreach (var sqlStorageElementType in sqlStorageElementTypes)
        {
            var genericSqlRepositoryInterfaceType = typeof(ISqlRepository<>).MakeGenericType(sqlStorageElementType);
            var genericSqlRepositoryImplementationType = typeof(SqlRepository<>).MakeGenericType(sqlStorageElementType);
            services.AddTransient(genericSqlRepositoryInterfaceType, genericSqlRepositoryImplementationType);
        }

        return services;
    }
}