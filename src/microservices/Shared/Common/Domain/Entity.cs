namespace Common.Domain;

public abstract class Entity : IEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid CreatedBy { get; set; }
    public DateTime CreatedDate { get; set;  } 
}

public interface IEntity
{
    Guid Id { get; set; }
}