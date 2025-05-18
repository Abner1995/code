namespace Todo.Domain.Abstractions;

public interface IEntity
{
    object[] GetKeys();
}

public interface IEntity<Tkey> : IEntity
{
    Tkey Id { get; }
}
