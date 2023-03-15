using Moq;
using events_manager_api.Domain.UnitOfWork;
using events_manager_api.Application.Services;
using events_manager_api.Application.Services.Impl;
using events_manager_api.Domain.Entities;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using AutoMapper;
using FluentValidation;
using events_manager_api.Application.Dtos;
using events_manager_api.Common.Exceptions;
using events_manager_api_testing.Util;
using events_manager_api.Infrastructure.Clients;
using events_manager_api.Domain.Enums;
using events_manager_api.Domain.Structs;
using System.Linq.Expressions;

namespace events_manager_api_testing.Services;

[TestClass]
public class EventServiceTest
{

    private Mock<IUnitOfWork> _unitOfWorkMock = default!;
    private Mock<IMapper> _mapperMock = default!;
    private Mock<IWeatherstackClient> _weatherstackClientMock = default!;
    private Mock<ILogger<EventService>> _loggerMock = default!;
    // Mock validator
    private Mock<IValidator<EventCreationDto>> _validatorMock = default!;
    
    private IEventService _eventService = default!;

    [TestInitialize]
    public void Setup()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _mapperMock = new Mock<IMapper>();
        _weatherstackClientMock = new Mock<IWeatherstackClient>();
        _loggerMock = new Mock<ILogger<EventService>>();
        _validatorMock = new Mock<IValidator<EventCreationDto>>();
        _eventService = new EventService(_unitOfWorkMock.Object, _mapperMock.Object, _weatherstackClientMock.Object, _loggerMock.Object, _validatorMock.Object);
    }

    [TestMethod]
    public async Task CreateEventWithVirtualTypesAsyncShouldCreateEvent()
    {
        // Arrange

        var eventCreationDto = FakeDataHelper.GenerateFakeEventCreationDtoForVirtualTypes().Generate();

        var eventEntity = new EventEntity
        {
            Id = 1,
            Name = eventCreationDto.Name,
            Description = eventCreationDto.Description,
            Date = eventCreationDto.Date,
            Type = EventType.Virtual,
        };

        var eventCreatedDto = new EventRetrievalDto
        {
            Id = eventEntity.Id,
            Name = eventEntity.Name,
            Description = eventEntity.Description,
            Date = eventEntity.Date,
            Type = eventEntity.Type.ToString()
        };


        _validatorMock.Setup(v => v.Validate(eventCreationDto))
            .Returns(new FluentValidation.Results.ValidationResult());

        _mapperMock.Setup(m => m.Map<EventEntity>(eventCreationDto))
            .Returns(eventEntity);

        _unitOfWorkMock.Setup(u => u.EventRepository.Add(eventEntity));
        _unitOfWorkMock.Setup(u => u.Complete())
            .ReturnsAsync(true);

        _mapperMock.Setup(m => m.Map<EventRetrievalDto>(eventEntity))
            .Returns(eventCreatedDto);

        // Act
        var eventResult = await _eventService.CreateEventAsync(eventCreationDto);

        // Assert
        eventResult.Should().NotBeNull();
        eventResult.Name.Should().Be(eventCreationDto.Name);
        eventResult.Description.Should().Be(eventCreationDto.Description);
        eventResult.Date.Should().Be(eventCreationDto.Date);
        eventResult.Type.Should().Be(EventType.Virtual.ToString());

        _validatorMock.Verify(v => v.Validate(eventCreationDto), Times.Once);
        _mapperMock.Verify(m => m.Map<EventEntity>(eventCreationDto), Times.Once);
        _unitOfWorkMock.Verify(u => u.EventRepository.Add(eventEntity), Times.Once);
        _unitOfWorkMock.Verify(u => u.Complete(), Times.Once);
        _mapperMock.Verify(m => m.Map<EventRetrievalDto>(eventEntity), Times.Once);
    }

    [TestMethod]
    public async Task CreateEventAsyncShouldThrowExceptionWhenEventCreationDtoIsInvalid()
    {
        // Arrange
        var eventCreationDto = FakeDataHelper.GenerateFakeEventCreationDtoForVirtualTypes().Generate(); // Here we use the same fake data generator for virtual types because we don't care about the type of the event

        _validatorMock.Setup(v => v.Validate(eventCreationDto))
            .Returns(new FluentValidation.Results.ValidationResult(new List<FluentValidation.Results.ValidationFailure>
            {
                new FluentValidation.Results.ValidationFailure("Name", "Name is required")
            }));

        // Act
        var act = async () => await _eventService.CreateEventAsync(eventCreationDto);

        // Assert
        await act.Should().ThrowAsync<WebApiException>()
            .WithMessage("Event information is invalid");

        _validatorMock.Verify(v => v.Validate(eventCreationDto), Times.Once);
        _mapperMock.Verify(m => m.Map<EventEntity>(eventCreationDto), Times.Never);
        _unitOfWorkMock.Verify(u => u.EventRepository.Add(It.IsAny<EventEntity>()), Times.Never);
        _unitOfWorkMock.Verify(u => u.Complete(), Times.Never);
        _mapperMock.Verify(m => m.Map<EventRetrievalDto>(It.IsAny<EventEntity>()), Times.Never);
    }

    [TestMethod]
    public async Task CreateEventWithIn_PersonTypesAsyncShouldCreateEvent()
    {
        // Arrange

        var eventCreationDto = FakeDataHelper.GenerateFakeEventCreationDtoForIn_PersonTypes().Generate();

        var eventEntity = new EventEntity
        {
            Id = 1,
            Name = eventCreationDto.Name,
            Description = eventCreationDto.Description,
            Date = eventCreationDto.Date,
            Type = EventType.In_Person,
            City = eventCreationDto.City,
            Country = eventCreationDto.Country
        };

        var location = new Location
        {
            latitude = 1.0,
            longitude = 1.0
        };


        var eventCreatedDto = new EventRetrievalDto
        {
            Id = eventEntity.Id,
            Name = eventEntity.Name,
            Description = eventEntity.Description,
            Date = eventEntity.Date,
            Type = eventEntity.Type.ToString(),
            City = eventEntity.City,
            Country = eventEntity.Country,
            Location = location.ToString()

        };

        _validatorMock.Setup(v => v.Validate(eventCreationDto))
                .Returns(new FluentValidation.Results.ValidationResult());

        _mapperMock.Setup(m => m.Map<EventEntity>(eventCreationDto))
            .Returns(eventEntity);

        _weatherstackClientMock.Setup(w => w.GetLocationByCityNameAsync(eventCreationDto.City!))
            .ReturnsAsync(location);

        _unitOfWorkMock.Setup(u => u.EventRepository.Add(eventEntity));
        _unitOfWorkMock.Setup(u => u.Complete())
            .ReturnsAsync(true);

        _mapperMock.Setup(m => m.Map<EventRetrievalDto>(eventEntity))
                .Returns(eventCreatedDto);

        // Act
        var eventResult = await _eventService.CreateEventAsync(eventCreationDto);

        // Assert
        eventResult.Should().NotBeNull();
        eventResult.Name.Should().Be(eventCreationDto.Name);
        eventResult.Description.Should().Be(eventCreationDto.Description);
        eventResult.Date.Should().Be(eventCreationDto.Date);
        eventResult.Type.Should().Be(EventType.In_Person.ToString());
        eventResult.City.Should().Be(eventCreationDto.City);
        eventResult.Country.Should().Be(eventCreationDto.Country);
        eventResult.Location.Should().Be(location.ToString());

        _validatorMock.Verify(v => v.Validate(eventCreationDto), Times.Once);
        _mapperMock.Verify(m => m.Map<EventEntity>(eventCreationDto), Times.Once);
        _unitOfWorkMock.Verify(u => u.EventRepository.Add(eventEntity), Times.Once);
        _unitOfWorkMock.Verify(u => u.Complete(), Times.Once);
        _mapperMock.Verify(m => m.Map<EventRetrievalDto>(eventEntity), Times.Once);
    }

    [TestMethod]
    public void GetEventByIdShouldReturnEvent()
    {
        // Arrange
        var eventId = 1;

        var eventEntity = FakeDataHelper.GenerateFakeEventEntityForIn_PersonTypes(eventId).Generate();

        Location location = new Location
        {
            latitude = 1.0,
            longitude = 1.0
        };

        var eventRetrievalDto = new EventRetrievalDto
        {
            Id = eventEntity.Id,
            Name = eventEntity.Name,
            Description = eventEntity.Description,
            Date = eventEntity.Date,
            Type = eventEntity.Type.ToString(),
            City = eventEntity.City,
            Country = eventEntity.Country,
            Location = location.ToString()
        };

        _unitOfWorkMock.Setup(u => u.EventRepository.FindWhere(It.IsAny<Expression<Func<EventEntity, bool>>>()))
            .Returns(new List<EventEntity> { eventEntity }.AsQueryable());

        _mapperMock.Setup(m => m.Map<EventRetrievalDto>(eventEntity))
            .Returns(eventRetrievalDto);
        // Act
        var eventResult = _eventService.GetEventById(eventId);

        // Assert
        eventResult.Should().NotBeNull();
        eventResult.Id.Should().Be(eventRetrievalDto.Id);
        eventResult.Name.Should().Be(eventRetrievalDto.Name);
        eventResult.Description.Should().Be(eventRetrievalDto.Description);
        eventResult.Date.Should().Be(eventRetrievalDto.Date);
        eventResult.Type.Should().Be(eventRetrievalDto.Type);

        _unitOfWorkMock.Verify(u => u.EventRepository.FindWhere(It.IsAny<Expression<Func<EventEntity, bool>>>()), Times.Once);
        _mapperMock.Verify(m => m.Map<EventRetrievalDto>(eventEntity), Times.Once);
    }

    [TestMethod]
    public void GetEventByIdShouldThrowExceptionWhenEventDoesNotExist()
    {
        // Arrange
        var eventId = 1;

        _unitOfWorkMock.Setup(u => u.EventRepository.FindWhere(It.IsAny<Expression<Func<EventEntity, bool>>>()))
            .Returns(new List<EventEntity>().AsQueryable());

        // Act and Assert
        _eventService.Invoking(e => e.GetEventById(eventId))
            .Should().Throw<WebApiException>()
            .WithMessage($"Event with id {eventId} not found");

        _unitOfWorkMock.Verify(u => u.EventRepository.FindWhere(It.IsAny<Expression<Func<EventEntity, bool>>>()), Times.Once);
        _mapperMock.Verify(m => m.Map<EventRetrievalDto>(It.IsAny<EventEntity>()), Times.Never);
    }

    [TestMethod]
    public async Task GetEventsAsyncShouldReturnAllTheEvents()
    {
        // Arrange
        var eventEntities = FakeDataHelper.GenerateFakeEventEntityForIn_PersonTypes().Generate(2);

        var eventRetrievalDtos = new List<EventRetrievalDto>();

        foreach (var eventEntity in eventEntities)
        {
            Location location = new Location
            {
                latitude = 1.0,
                longitude = 1.0
            };

            var eventRetrievalDto = new EventRetrievalDto
            {
                Id = eventEntity.Id,
                Name = eventEntity.Name,
                Description = eventEntity.Description,
                Date = eventEntity.Date,
                Type = eventEntity.Type.ToString(),
                City = eventEntity.City,
                Country = eventEntity.Country,
                Location = location.ToString()
            };

            eventRetrievalDtos.Add(eventRetrievalDto);
        }

        _unitOfWorkMock.Setup(u => u.EventRepository.GetAllAsync())
            .ReturnsAsync(eventEntities);

        _mapperMock.Setup(m => m.Map<ICollection<EventRetrievalDto>>(eventEntities))
            .Returns(eventRetrievalDtos);

        // Act
        var eventsResult = await _eventService.GetEventsAsync();

        // Assert
        eventsResult.Should().NotBeNull();
        eventsResult.Should().HaveCount(2);

        _unitOfWorkMock.Verify(u => u.EventRepository.GetAllAsync(), Times.Once);
        _mapperMock.Verify(m => m.Map<IEnumerable<EventRetrievalDto>>(eventEntities), Times.Once);
    }

    [TestMethod]
    public async Task GetEventsAsyncShouldReturnEmptyListWhenNoEventsExist()
    {
        // Arrange
        _unitOfWorkMock.Setup(u => u.EventRepository.GetAllAsync())
            .ReturnsAsync(new List<EventEntity>());

        _mapperMock.Setup(m => m.Map<ICollection<EventRetrievalDto>>(It.IsAny<IEnumerable<EventEntity>>()))
            .Returns(new List<EventRetrievalDto>());

        // Act
        var eventsResult = await _eventService.GetEventsAsync();

        // Assert
        eventsResult.Should().NotBeNull();
        eventsResult.Should().BeEmpty();

        _unitOfWorkMock.Verify(u => u.EventRepository.GetAllAsync(), Times.Once);
        _mapperMock.Verify(m => m.Map<ICollection<EventRetrievalDto>>(It.IsAny<IEnumerable<EventEntity>>()), Times.Once);
    }
}