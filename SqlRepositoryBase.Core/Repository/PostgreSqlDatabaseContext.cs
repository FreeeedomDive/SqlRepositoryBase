using Microsoft.EntityFrameworkCore;
using SqlRepositoryBase.Core.Options;

namespace SqlRepositoryBase.Core.Repository;

public class PostgreSqlDatabaseContext : DbContext
{
    public PostgreSqlDatabaseContext(IConnectionStringProvider connectionStringProvider)
    {
        this.connectionStringProvider = connectionStringProvider;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(connectionStringProvider.GetConnectionString());
    }

    private readonly IConnectionStringProvider connectionStringProvider;
}