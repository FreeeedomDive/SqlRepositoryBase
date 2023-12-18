using Microsoft.EntityFrameworkCore;

namespace SqlRepositoryBase.Core.ContextBuilders;

public abstract class PostgreSqlDbContext : DbContext
{
    protected PostgreSqlDbContext(string connectionString)
    {
        this.connectionString = connectionString;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(connectionString);
    }

    private readonly string connectionString;
}