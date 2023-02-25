using Microsoft.EntityFrameworkCore;
using SqlRepositoryBase.Core.Models;

namespace SqlRepositoryBase.Core.Extensions;

public static class ModelBuilderExtensions
{
    public static void CreateVersionForTable<T>(this ModelBuilder modelBuilder, string tableName)
        where T : VersionedSqlStorageElement
    {
        var versionSequenceName = $"{tableName}Version";
        modelBuilder.HasSequence<long>(versionSequenceName)
            .StartsAt(1)
            .IncrementsBy(1);
        modelBuilder.Entity<T>()
            .Property(o => o.Version)
            .HasDefaultValueSql($"NEXT VALUE FOR {versionSequenceName}");
    }
}