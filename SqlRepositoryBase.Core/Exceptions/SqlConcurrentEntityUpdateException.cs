namespace SqlRepositoryBase.Core.Exceptions;

public class SqlConcurrentEntityUpdateException : Exception
{
    public SqlConcurrentEntityUpdateException(Guid id)
        : base($"There was an attempt to concurrently update entity {id}")
    {
    }
}