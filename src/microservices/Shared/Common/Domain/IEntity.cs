namespace Common.Domain;

public class IEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
}