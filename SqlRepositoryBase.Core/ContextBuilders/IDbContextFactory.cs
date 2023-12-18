using Microsoft.EntityFrameworkCore;

namespace SqlRepositoryBase.Core.ContextBuilders;

public interface IDbContextFactory
{
    DbContext Build();
}