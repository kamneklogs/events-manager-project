using events_manager_api.Application.Dtos;
using events_manager_api.Domain.Entities;
using events_manager_api.Domain.UnitOfWork;
using AutoMapper;
using events_manager_api.Infrastructure.Clients;
using events_manager_api.Domain.Structs;
using events_manager_api.Domain.Enums;
using FluentValidation;
using events_manager_api.Common.Exceptions;
using System.Net;

namespace events_manager_api.Application.Services.Impl;

public class EventService : IEventService
{

    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IWeatherstackClient _weatherstackClient;
    private readonly IValidator<EventCreationDto> _eventValidator;
    private readonly ILogger<EventService> _logger;

    public EventService(IUnitOfWork unitOfWork, IMapper mapper, IWeatherstackClient weatherstackClient, ILogger<EventService> logger, IValidator<EventCreationDto> eventValidator)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _weatherstackClient = weatherstackClient;
        _logger = logger;
        _eventValidator = eventValidator;
    }

    public async Task<EventRetrievalDto> CreateEventAsync(EventCreationDto newEvent)
    {
        var validationResult = _eventValidator.Validate(newEvent);
        if (!validationResult.IsValid)
        {
            throw new WebApiException(HttpStatusCode.BadRequest, "Event information is invalid", validationResult.Errors);
        }

        EventEntity eventToCreate = _mapper.Map<EventEntity>(newEvent);

        if (EventTypeExtensions.FromId(newEvent.EventTypeId) == EventType.In_Person)
        {
            Location? location = await _weatherstackClient.GetLocationByCityNameAsync(newEvent.City!); // At this point, the city is not null 'cause we use the validator to check it before
            eventToCreate.Latitude = location!.Value.latitude;
            eventToCreate.Longitude = location!.Value.longitude;
        }

        _unitOfWork.EventRepository.Add(eventToCreate);
        await _unitOfWork.Complete();

        return _mapper.Map<EventRetrievalDto>(eventToCreate);
    }

    public EventRetrievalDto GetEventById(int id)
    {
        EventEntity? eventToGet = _unitOfWork.EventRepository.FindWhere(e => e.Id == id).FirstOrDefault();

        if (eventToGet == null)
        {
            throw new WebApiException(HttpStatusCode.NotFound, $"Event with id {id} not found");
        }

        return _mapper.Map<EventRetrievalDto>(eventToGet);
    }

    public async Task<ICollection<EventRetrievalDto>> GetEventsAsync()
    {
        var events = await _unitOfWork.EventRepository.GetAllAsync();
        return _mapper.Map<ICollection<EventRetrievalDto>>(events);
    }
}