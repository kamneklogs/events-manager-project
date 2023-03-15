using Bogus;
using events_manager_api.Application.Dtos;
using events_manager_api.Domain.Entities;
using events_manager_api.Domain.Enums;

namespace events_manager_api_testing.Util;

internal class FakeDataHelper
{
    public static Faker<DeveloperEntity> GenerateFakeDeveloperEntity(string? email = null)
    {
        var faker = new Faker<DeveloperEntity>();

        if (email != null)
        {
            faker.RuleFor(d => d.Email, f => email);
        }
        else
        {
            faker.RuleFor(d => d.Email, f => f.Internet.Email());
        }

        return faker
            .RuleFor(d => d.Name, f => f.Name.FullName())
            .RuleFor(d => d.CurrentCity, f => f.Address.City())
            .RuleFor(d => d.PhoneNumber, f => f.Phone.PhoneNumber());
    }

    public static Faker<DeveloperDto> GenerateFakeDeveloperDto()
    {
        return new Faker<DeveloperDto>()
            .RuleFor(d => d.Email, f => (f.IndexFaker + 1).ToString() + f.Internet.Email()) // Salt the email to avoid duplicates
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

    public static Faker<EventEntity> GenerateFakeEventEntityForIn_PersonTypes(int? id = null)
    {

        var fakeEntity = new Faker<EventEntity>();

        if (id.HasValue)
        {
            fakeEntity.RuleFor(d => d.Id, f => id.Value);
        }
        else
        {
            fakeEntity.RuleFor(d => d.Id, f => f.IndexFaker + 1);
        }

        return fakeEntity
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

    public static Faker<SendInviteDto> GenerateFakeSendInviteDto()
    {
        return new Faker<SendInviteDto>()
            .RuleFor(d => d.EventId, f => f.IndexFaker + 1)
            .RuleFor(d => d.DeveloperEmails, f => f.Random.WordsArray(f.Random.Number(1, 10)));
    }

    public static Faker<InviteRetrievalDto> GenerateFakeInviteRetrievalDto()
    {
        return new Faker<InviteRetrievalDto>()
            .RuleFor(d => d.Id, f => f.IndexFaker + 1)
            .RuleFor(d => d.EventId, f => f.IndexFaker + 1)
            .RuleFor(d => d.DeveloperEmail, f => f.Internet.Email())
            .RuleFor(d => d.Status, f => f.PickRandom<InviteResponseStatus>().ToString());
    }

    public static Faker<InviteEntity> GenerateFakeInviteEntity(int? inviteId, int? eventId, string? developerEmail, InviteResponseStatus? status = InviteResponseStatus.Pending)
    {
        var fakeEntity = new Faker<InviteEntity>();

        if (inviteId.HasValue)
        {
            fakeEntity.RuleFor(d => d.Id, f => inviteId.Value);
        }
        else
        {
            fakeEntity.RuleFor(d => d.Id, f => f.IndexFaker + 1);
        }

        if (eventId.HasValue)
        {
            fakeEntity.RuleFor(d => d.EventEntity, FakeDataHelper.GenerateFakeEventEntityForIn_PersonTypes(eventId.Value));
        }
        else
        {
            fakeEntity.RuleFor(d => d.EventEntity, FakeDataHelper.GenerateFakeEventEntityForIn_PersonTypes());
        }

        if (developerEmail != null)
        {
            fakeEntity.RuleFor(d => d.DeveloperEntity, FakeDataHelper.GenerateFakeDeveloperEntity(developerEmail));
        }
        else
        {
            fakeEntity.RuleFor(d => d.DeveloperEntity, FakeDataHelper.GenerateFakeDeveloperEntity());
        }

        if (status.HasValue)
        {
            fakeEntity.RuleFor(d => d.Status, f => status.Value);
        }
        else
        {
            fakeEntity.RuleFor(d => d.Status, f => f.PickRandom<InviteResponseStatus>());
        }

        return fakeEntity;
    }
}