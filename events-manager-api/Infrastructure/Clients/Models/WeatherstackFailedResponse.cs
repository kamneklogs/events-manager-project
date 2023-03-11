namespace events_manager_api.Infrastructure.Clients.Models;

public class WeatherstackFailedResponse : WeatherstackBaseResponse
{
    public WeatherstackResponseError Error { get; set; } = default!;
}

public class WeatherstackResponseError
{
    public int Code { get; set; }
    public string Type { get; set; } = default!;
    public string Info { get; set; } = default!;
}