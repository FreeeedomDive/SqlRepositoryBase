using Microsoft.EntityFrameworkCore;
using SqlRepositoryBase.Core.Options;

namespace SqlRepositoryBase.Core.ContextBuilders;

public class DbContextFactory : IDbContextFactory
{
    public DbContextFactory(
        IConnectionStringProvider connectionStringProvider,
        Func<string, DbContext> buildFunc
    )
    {
        this.connectionStringProvider = connectionStringProvider;
        this.buildFunc = buildFunc;
    }

    public DbContext Build()
    {
        var connectionString = connectionStringProvider.GetConnectionString();
        var context = buildFunc(connectionString);

        return context;
    }

    private readonly Func<string, DbContext> buildFunc;
    private readonly IConnectionStringProvider connectionStringProvider;
}