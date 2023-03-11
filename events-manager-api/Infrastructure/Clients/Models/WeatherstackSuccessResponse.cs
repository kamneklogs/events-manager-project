namespace events_manager_api.Infrastructure.Clients.Models;

public class WeatherstackSuccessResponse : WeatherstackBaseResponse
{
    public Location Location { get; set; } = default!;
}

public class Location
{
    public string Name { get; set; } = default!;
    public string Country { get; set; } = default!;
    public string Lat { get; set; } = default!;
    public string Lon { get; set; } = default!;
    public string Timezone_id { get; set; } = default!;
}