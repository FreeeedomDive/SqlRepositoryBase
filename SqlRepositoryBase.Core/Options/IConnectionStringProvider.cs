namespace SqlRepositoryBase.Core.Options;

public interface IConnectionStringProvider
{
    string GetConnectionString();
}