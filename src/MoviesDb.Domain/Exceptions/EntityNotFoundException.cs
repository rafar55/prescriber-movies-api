namespace MoviesDb.Domain.Exceptions;

public class EntityNotFoundException : Exception
{
    public Guid EntityId { get; }
    public string Model { get; }

    public EntityNotFoundException(Guid entityId, string model) : base($"We didn't find a {model} with the id provided")
    {
        EntityId = entityId;
        Model = model;
    }
}
