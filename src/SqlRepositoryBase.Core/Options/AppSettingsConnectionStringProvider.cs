using Microsoft.Extensions.Options;

namespace SqlRepositoryBase.Core.Options;

public class AppSettingsConnectionStringProvider : IConnectionStringProvider
{
    public AppSettingsConnectionStringProvider(IOptions<AppSettingsDatabaseOptions> options)
    {
        this.options = options;
    }

    public string GetConnectionString()
    {
        return options.Value.ConnectionString;
    }

    private readonly IOptions<AppSettingsDatabaseOptions> options;
}