using events_manager_api.Domain.Enums;

namespace events_manager_api.Domain.Entities;

public class InviteEntity
{
    public int Id { get; set; }
    public EventEntity EventEntity { get; set; } = default!; //https://learn.microsoft.com/en-us/ef/core/modeling/relationships?tabs=fluent-api%2Cfluent-api-simple-key%2Csimple-key#no-foreign-key-property
    public DeveloperEntity DeveloperEntity { get; set; } = default!; //Ditto above

    public InviteResponseStatus Status { get; set; }

    //To string
    public override string ToString()
    {
        return $"Id: {Id}, EventId: {EventEntity.Id}, DeveloperEmail: {DeveloperEntity.Email}, Status: {Status}";
    }
}