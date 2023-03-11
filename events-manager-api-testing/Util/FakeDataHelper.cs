using Bogus;
using events_manager_api.Application.Dtos;
using events_manager_api.Domain.Entities;
using events_manager_api.Domain.Enums;

namespace events_manager_api_testing.Util;

internal class FakeDataHelper
{
    public static Faker<DeveloperEntity> GenerateFakeDeveloperEntity()
    {
        return new Faker<DeveloperEntity>()
            .RuleFor(d => d.Email, f => f.Internet.Email())
            .RuleFor(d => d.Name, f => f.Name.FullName())
            .RuleFor(d => d.CurrentCity, f => f.Address.City())
            .RuleFor(d => d.PhoneNumber, f => f.Phone.PhoneNumber());
    }

    public static Faker<DeveloperDto> GenerateFakeDeveloperDto()
    {
        return new Faker<DeveloperDto>()
            .RuleFor(d => d.Email, f => f.Internet.Email())
            .RuleFor(d => d.Name, f => f.Name.FullName())
            .RuleFor(d => d.CurrentCity, f => f.Address.City())
            .RuleFor(d => d.PhoneNumber, f => f.Phone.PhoneNumber());
    }

    public static Faker<EventCreationDto> GenerateFakeEventCreationDtoForIn_PersonTypes()
    {
        return new Faker<EventCreationDto>()
            .RuleFor(d => d.Name, f => f.Name.FullName())
            .RuleFor(d => d.Description, f => f.Lorem.Sentence())
            .RuleFor(d => d.Date, f => f.Date.Future())
            .RuleFor(d => d.EventTypeId, f => (int)EventType.In_Person)
            .RuleFor(d => d.City, f => f.Address.City())
            .RuleFor(d => d.Country, f => f.Address.Country());
    }

    public static Faker<EventCreationDto> GenerateFakeEventCreationDtoForVirtualTypes()
    {
        return new Faker<EventCreationDto>()
            .RuleFor(d => d.Name, f => f.Name.FullName())
            .RuleFor(d => d.Description, f => f.Lorem.Sentence())
            .RuleFor(d => d.Date, f => f.Date.Future())
            .RuleFor(d => d.EventTypeId, f => (int)EventType.Virtual);
    }

    public static Faker<EventEntity> GenerateFakeEventEntityForIn_PersonTypes()
    {
        return new Faker<EventEntity>()
            .RuleFor(d => d.Name, f => f.Lorem.Sentence(3))
            .RuleFor(d => d.Description, f => f.Lorem.Sentence(10))
            .RuleFor(d => d.Date, f => f.Date.Future())
            .RuleFor(d => d.Type, f => EventType.In_Person)
            .RuleFor(d => d.City, f => f.Address.City())
            .RuleFor(d => d.Country, f => f.Address.Country())
            .RuleFor(d => d.Latitude, f => f.Address.Latitude())
            .RuleFor(d => d.Longitude, f => f.Address.Longitude());
    }

    public static Faker<EventEntity> GenerateFakeEventEntityForVirtualTypes()
    {
        return new Faker<EventEntity>()
            .RuleFor(d => d.Name, f => f.Lorem.Sentence(3))
            .RuleFor(d => d.Description, f => f.Lorem.Sentence(10))
            .RuleFor(d => d.Date, f => f.Date.Future())
            .RuleFor(d => d.Type, f => EventType.Virtual);
    }
}