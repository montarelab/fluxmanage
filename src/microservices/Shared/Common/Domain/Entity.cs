namespace Common.Domain;

public abstract class Entity : IEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
}

public interface IEntity
{
    Guid Id { get; set; }
}