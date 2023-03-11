using System.Net;
using events_manager_api.Application.Dtos;
using events_manager_api.Application.Services;
using events_manager_api.Domain.Enums;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace events_manager_api.Application.Controllers;

[ApiController]
[Route("event")]
public class EventController : ControllerBase
{
    private readonly ILogger<DeveloperController> _logger;
    private readonly IEventService _eventService;
    private readonly IInviteService _inviteService;
    public EventController(ILogger<DeveloperController> logger,
        IEventService eventService,
        IInviteService inviteService)
    {
        _logger = logger;
        _eventService = eventService;
        _inviteService = inviteService;
    }

    [HttpGet]
    public async Task<IActionResult> GetEvents()
    {
        var events = await _eventService.GetEventsAsync();
        return Ok(events);
    }

    [HttpGet("{eventId}")]
    public IActionResult GetEvent(int eventId)
    {
        var eventResult = _eventService.GetEventById(eventId);
        return Ok(eventResult);
    }

    [ProducesResponseType(typeof(EventRetrievalDto), (int)HttpStatusCode.Created)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [HttpPost]
    public async Task<IActionResult> CreateEvent([FromBody] EventCreationDto eventCreationDto)
    {
        var eventResult = await _eventService.CreateEventAsync(eventCreationDto);
        return Ok(eventResult);
    }

    [ProducesResponseType(typeof(ICollection<InviteRetrievalDto>), (int)HttpStatusCode.Created)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [HttpPost("{eventId}/invitation")]
    public async Task<IActionResult> SendInvitation(int eventId, [FromBody] SendInviteDto sendInviteDto)
    {
        var eventResult = await _inviteService.CreateInvitesAsync(sendInviteDto);
        return Ok(eventResult);
    }

    [HttpGet("{eventId}/invitation")]
    public async Task<IActionResult> Search(int eventId)
    {
        var eventResult = await _inviteService.GetInvitesByEventIdAsync(eventId);
        return Ok(eventResult);
    }

    [ProducesResponseType(typeof(ICollection<InviteRetrievalDto>), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [HttpGet("{eventId}/invitation/accepted")]
    public async Task<IActionResult> GetAcceptedInvitations(int eventId)
    {
        var eventResult = await _inviteService.GetInvitesByEventIdAndStatusAsync(eventId, InviteResponseStatus.Accepted);
        return Ok(eventResult);
    }

    [HttpPut("{eventId}/invitation/{inviteId}/response")]
    public async Task<IActionResult> RespondToInvitation(int eventId /*Validate invite into this event*/, int inviteId, [FromBody] InviteResponseStatus status)
    {
        var eventResult = await _inviteService.UpdateInviteStatusAsync(eventId, inviteId, status);
        return Ok(eventResult);
    }
}