using System.Globalization;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using events_manager_api.Common.Exceptions;
using events_manager_api.Domain.Structs;
using events_manager_api.Infrastructure.Clients.Models;

namespace events_manager_api.Infrastructure.Clients.Impl;

public class WeatherstackClient : IWeatherstackClient
{
    private readonly HttpClient _httpClient;

    private readonly string baseParamsUrl;

    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public WeatherstackClient(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        baseParamsUrl = $"?access_key={configuration["Weatherstack:AccessKey"]}&query=";
    }



    public async Task<string?> GetTimeZoneIdByCityNameAsync(string cityName)
    {
        var content = await this.getPayloadResponseAsync(cityName);
        var weatherstackResponse = JsonSerializer.Deserialize<WeatherstackSuccessResponse>(content, _jsonOptions);

        return weatherstackResponse!.Location.Timezone_id;
    }

    public async Task<Domain.Structs.Location?> GetLocationByCityNameAsync(string cityName)
    {
        var content = await this.getPayloadResponseAsync(cityName);
        var weatherstackResponse = JsonSerializer.Deserialize<WeatherstackSuccessResponse>(content, _jsonOptions);

        var latitudeAsDouble = double.Parse(weatherstackResponse!.Location.Lat, CultureInfo.InvariantCulture);
        var longitudeAsDouble = double.Parse(weatherstackResponse!.Location.Lon, CultureInfo.InvariantCulture);

        return new Domain.Structs.Location(latitudeAsDouble, longitudeAsDouble);
    }

    private async Task<string> getPayloadResponseAsync(string cityName)
    {
        var response = await _httpClient.GetAsync(baseParamsUrl + cityName);

        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var weatherstackResponse = JsonSerializer.Deserialize<WeatherstackBaseResponse>(content, _jsonOptions);

        if (weatherstackResponse!.Success is false)
        {
            throw new WebApiException(
                HttpStatusCode.InternalServerError,
                $"Some internal services not working",
                JsonSerializer.Deserialize<WeatherstackFailedResponse>(content, _jsonOptions)
            );
        }

        return content;
    }
}