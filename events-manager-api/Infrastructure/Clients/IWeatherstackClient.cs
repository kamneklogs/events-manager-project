using events_manager_api.Domain.Structs;

namespace events_manager_api.Infrastructure.Clients;

public interface IWeatherstackClient
{
    Task<Location?> GetLocationByCityNameAsync(string cityName);
}