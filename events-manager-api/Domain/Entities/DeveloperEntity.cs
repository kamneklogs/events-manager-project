namespace events_manager_api.Domain.Entities;

public class DeveloperEntity
{
    public string Email { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string CurrentCity { get; set; } = default!;
    public string PhoneNumber { get; set; } = default!;

    public ICollection<InviteEntity> Invites { get; set; } = default!;

    public override string ToString()
    {
        return $"DeveloperEntity{{Email='{Email}', Name='{Name}', CurrentCity='{CurrentCity}', PhoneNumber='{PhoneNumber}'}}";
    }
}