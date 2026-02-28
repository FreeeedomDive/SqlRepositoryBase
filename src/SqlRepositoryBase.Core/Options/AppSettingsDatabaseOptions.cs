namespace SqlRepositoryBase.Core.Options;

public class AppSettingsDatabaseOptions
{
    public string ConnectionString { get; set; }
    public bool LogSql { get; set; }
    public string? MigrationsAssembly { get; set; }
}