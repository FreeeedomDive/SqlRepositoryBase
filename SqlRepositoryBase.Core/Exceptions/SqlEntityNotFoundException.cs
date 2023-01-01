namespace SqlRepositoryBase.Core.Exceptions;

public class SqlEntityNotFoundException : Exception
{
    public SqlEntityNotFoundException(Guid id): base($"Entity {id} not found")
    {
        EntityId = id;
    }
    
    public Guid EntityId { get; }
}