using events_manager_api.Infrastructure.Clients;
using events_manager_api.Infrastructure.Clients.Impl;
using Moq;
using FluentAssertions;
using System.Net;
using System.Text.Json;
using Moq.Protected;
using events_manager_api.Domain.Structs;
using events_manager_api.Common.Exceptions;

namespace events_manager_api_testing.Clients;

[TestClass]
public class WeatherstackClientTest
{
    private IWeatherstackClient _weatherstackClient = default!;
    private Mock<HttpMessageHandler> _handlerMock = default!;

    [TestInitialize]
    public void Setup()
    {
        _handlerMock = new Mock<HttpMessageHandler>();
        var httpClient = new HttpClient(_handlerMock.Object);

        var baseUrl = "https://api.weatherstack.com/";

        _weatherstackClient = new WeatherstackClient(httpClient, baseUrl, "NOT_A_REAL_ACCESS_KEY");
    }

    [TestMethod]
    public async Task GetLocationByCityNameAsyncShouldReturnLocation()
    {
        // Arrange
        var city = "Medellín";

        var weatherstackBaseResponse = new
        {
            Location = new
            {
                Name = city,
                Country = "Colombia",
                Lat = "2",
                Lon = "2",
            }
        };

        var expectedLocation = new Location(2, 2);

        var httpResponseMessage = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(JsonSerializer.Serialize(weatherstackBaseResponse, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            }))
        };

        _handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(httpResponseMessage);
        // Act
        var result = await _weatherstackClient.GetLocationByCityNameAsync(city);

        // Assert
        result.Should().BeEquivalentTo(expectedLocation);
    }

    [TestMethod]
    public async Task GetLocationByCityNameAsyncShouldThrowExceptionWhenResponseStatusCodeIsNotOk()
    {
        // Arrange
        var city = "Medellín";

        var weatherstackBaseResponse = new
        {
            Location = new
            {
                Name = city,
                Country = "Colombia",
                Lat = "2",
                Lon = "2",
            }
        };

        var expectedLocation = new Location(2, 2);

        var httpResponseMessage = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.InternalServerError, // This is not ok jaj
            Content = new StringContent(JsonSerializer.Serialize(weatherstackBaseResponse, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            }))
        };

        _handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(httpResponseMessage);

        // Act and Assert
        await _weatherstackClient
            .Invoking(async x => await x.GetLocationByCityNameAsync(city))
            .Should()
            .ThrowAsync<HttpRequestException>(); ;
    }

    [TestMethod]
    public async Task GetLocationByCityNameAsyncShouldThrowExceptionWhenResponseIsNotSuccess()
    {
        // Arrange
        var weatherstackErrorResponse = new
        {
            Success = false,
            Error = new
            {
                Code = 101,
                Type = "invalid_access_key",
                Info = "You have not supplied a valid API Access Key. [Technical Support: support@apilayer.com]"
            }
        };

        var httpResponseMessage = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(JsonSerializer.Serialize(weatherstackErrorResponse, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            }))
        };

        _handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(httpResponseMessage);

        // Act and Assert
        await _weatherstackClient
            .Invoking(async x => await x.GetLocationByCityNameAsync("Medellín"))
            .Should()
            .ThrowAsync<WebApiException>()
            .WithMessage("Some internal services not working");
    }
}