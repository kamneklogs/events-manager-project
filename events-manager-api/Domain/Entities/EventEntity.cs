using events_manager_api.Domain.Enums;

namespace events_manager_api.Domain.Entities;

public class EventEntity
{

    public int Id { get; set; }
    public string Name { get; set; } = default!;
    public string Description { get; set; } = default!;
    public DateTime Date { get; set; } = default!;
    public EventType Type { get; set; }
    public string? City { get; set; } 
    public string? Country { get; set; }

    public double Latitude { get; set; }
    public double Longitude { get; set; }

    public ICollection<InviteEntity> Invites { get; set; } = default!;
}