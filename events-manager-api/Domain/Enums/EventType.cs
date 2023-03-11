namespace events_manager_api.Domain.Enums;

public enum EventType
{
    Virtual = 1,
    In_Person = 2
}

static class EventTypeExtensions
{
    public static EventType FromId(int id)
    {
        return id switch
        {
            1 => EventType.Virtual,
            2 => EventType.In_Person,
            _ => throw new ArgumentException("Invalid developer type id")
        };
    }
}