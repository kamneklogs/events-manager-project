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
}