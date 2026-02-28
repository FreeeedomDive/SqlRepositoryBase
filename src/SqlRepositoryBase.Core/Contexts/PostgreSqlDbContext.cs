using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SqlRepositoryBase.Core.Options;

namespace SqlRepositoryBase.Core.Contexts;

public abstract class PostgreSqlDbContext : DbContext
{
    protected PostgreSqlDbContext(
        IConnectionStringProvider connectionStringProvider,
        IOptions<AppSettingsDatabaseOptions> options,
        ILogger<PostgreSqlDbContext> logger
    )
    {
        this.connectionStringProvider = connectionStringProvider;
        this.options = options;
        this.logger = logger;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (optionsBuilder.IsConfigured)
        {
            return;
        }

        var connectionString = connectionStringProvider.GetConnectionString();
        optionsBuilder.UseNpgsql(connectionString, builder =>
        {
            var migrationsAssembly = options.Value.MigrationsAssembly;
            if (!string.IsNullOrEmpty(migrationsAssembly))
            {
                builder.MigrationsAssembly(migrationsAssembly);
            }
        });

        if (options.Value.LogSql)
        {
            optionsBuilder.LogTo(x => logger.LogInformation(x), LogLevel.Information);
        }
    }

    private readonly IConnectionStringProvider connectionStringProvider;
    private readonly IOptions<AppSettingsDatabaseOptions> options;
    private readonly ILogger<PostgreSqlDbContext> logger;
}