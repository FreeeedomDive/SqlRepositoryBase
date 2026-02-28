using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SqlRepositoryBase.Core.Contexts;
using SqlRepositoryBase.Core.Models;
using SqlRepositoryBase.Core.Options;
using SqlRepositoryBase.Core.Repository;

namespace SqlRepositoryBase.Configuration.Extensions;

public static class AspNetServiceCollectionExtensions
{
    public static void ConfigurePostgreSql<TDbContext>(this IServiceCollection services, IConfigurationSection configurationSection)
        where TDbContext : PostgreSqlDbContext
    {
        services.Configure<AppSettingsDatabaseOptions>(configurationSection);
        services.AddTransient<IConnectionStringProvider>(serviceProvider =>
            {
                var options = serviceProvider.GetRequiredService<IOptions<AppSettingsDatabaseOptions>>();
                return new AppSettingsConnectionStringProvider(options);
            }
        );

        services.AddDbContextFactory<TDbContext>();
        
        var types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(s => s.GetTypes()).ToArray();
        services.ConfigureCommonRepositories<TDbContext>(types);
        services.ConfigureVersionedRepositories<TDbContext>(types);
    }

    private static void ConfigureCommonRepositories<TDbContext>(this IServiceCollection services, IEnumerable<Type> types)
        where TDbContext : PostgreSqlDbContext
    {
        var sqlStorageElementTypes = types
                                     .Where(p => typeof(SqlStorageElement).IsAssignableFrom(p))
                                     .Where(p => p != typeof(SqlStorageElement));
        foreach (var sqlStorageElementType in sqlStorageElementTypes)
        {
            var genericSqlRepositoryInterfaceType = typeof(ISqlRepository<>).MakeGenericType(sqlStorageElementType);
            var genericSqlRepositoryImplementationType = typeof(SqlRepository<,>).MakeGenericType(sqlStorageElementType, typeof(TDbContext));
            services.AddTransient(genericSqlRepositoryInterfaceType, genericSqlRepositoryImplementationType);
        }
    }

    private static void ConfigureVersionedRepositories<TDbContext>(this IServiceCollection services, IEnumerable<Type> types)
        where TDbContext : PostgreSqlDbContext
    {
        var sqlStorageElementTypes = types
                                     .Where(p => typeof(VersionedSqlStorageElement).IsAssignableFrom(p))
                                     .Where(p => p != typeof(VersionedSqlStorageElement));
        foreach (var sqlStorageElementType in sqlStorageElementTypes)
        {
            var genericSqlRepositoryInterfaceType = typeof(IVersionedSqlRepository<>).MakeGenericType(sqlStorageElementType);
            var genericSqlRepositoryImplementationType = typeof(VersionedSqlRepository<,>).MakeGenericType(sqlStorageElementType, typeof(TDbContext));
            services.AddTransient(genericSqlRepositoryInterfaceType, genericSqlRepositoryImplementationType);
        }
    }
}