using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SqlRepositoryBase.Core.ContextBuilders;
using SqlRepositoryBase.Core.Models;
using SqlRepositoryBase.Core.Options;
using SqlRepositoryBase.Core.Repository;

namespace SqlRepositoryBase.Configuration.Extensions;

public static class AspNetServiceCollectionExtensions
{
    public static IServiceCollection ConfigureConnectionStringFromAppSettings(this IServiceCollection services, IConfigurationSection configurationSection)
    {
        services.Configure<AppSettingsDatabaseOptions>(configurationSection);
        services.AddTransient<IConnectionStringProvider>(
            serviceProvider =>
            {
                var options = serviceProvider.GetRequiredService<IOptions<AppSettingsDatabaseOptions>>();
                return new AppSettingsConnectionStringProvider(options);
            }
        );

        return services;
    }

    public static IServiceCollection ConfigureDbContextFactory(this IServiceCollection services, Func<string, DbContext> buildFunc)
    {
        services.AddTransient<IDbContextFactory>(
            serviceProvider =>
            {
                var connectionStringProvider = serviceProvider.GetRequiredService<IConnectionStringProvider>();
                return new DbContextFactory(connectionStringProvider, buildFunc);
            }
        );

        return services;
    }

    public static IServiceCollection ConfigurePostgreSql(this IServiceCollection services)
    {
        var types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(s => s.GetTypes()).ToArray();
        return services.ConfigureCommonRepositories(types).ConfigureVersionedRepositories(types);
    }

    private static IServiceCollection ConfigureCommonRepositories(this IServiceCollection services, IEnumerable<Type> types)
    {
        var sqlStorageElementTypes = types
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

    private static IServiceCollection ConfigureVersionedRepositories(this IServiceCollection services, IEnumerable<Type> types)
    {
        var sqlStorageElementTypes = types
                                     .Where(p => typeof(VersionedSqlStorageElement).IsAssignableFrom(p))
                                     .Where(p => p != typeof(VersionedSqlStorageElement));
        foreach (var sqlStorageElementType in sqlStorageElementTypes)
        {
            var genericSqlRepositoryInterfaceType = typeof(IVersionedSqlRepository<>).MakeGenericType(sqlStorageElementType);
            var genericSqlRepositoryImplementationType = typeof(VersionedSqlRepository<>).MakeGenericType(sqlStorageElementType);
            services.AddTransient(genericSqlRepositoryInterfaceType, genericSqlRepositoryImplementationType);
        }

        return services;
    }
}