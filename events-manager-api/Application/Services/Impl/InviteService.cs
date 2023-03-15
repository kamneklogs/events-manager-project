using System.Net;
using AutoMapper;
using events_manager_api.Application.Dtos;
using events_manager_api.Common.Exceptions;
using events_manager_api.Domain.Entities;
using events_manager_api.Domain.Enums;
using events_manager_api.Domain.UnitOfWork;
using FluentValidation;

namespace events_manager_api.Application.Services.Impl;

public class InviteService : IInviteService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<InviteService> _logger;
    private readonly IValidator<SendInviteDto> _invitationValidator;

    public InviteService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<InviteService> logger, IValidator<SendInviteDto> invitationValidator)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
        _invitationValidator = invitationValidator;
    }

    public InviteRetrievalDto CreateInvite(int eventId, string developerEmail)
    {
        EventEntity? associatedEvent = _unitOfWork.EventRepository.FindWhere(e => e.Id == eventId).FirstOrDefault();
        DeveloperEntity? associatedDeveloper = _unitOfWork.DeveloperRepository.FindWhere(d => d.Email == developerEmail).FirstOrDefault();

        if (associatedEvent is null) // Is this necessary? The entity framework should throw an exception if the event does not exist
        {
            throw new WebApiException(HttpStatusCode.NotFound, $"The invite cannot be created because the event with id {eventId} does not exist");
        }

        if (associatedDeveloper is null) // Ditto as above
        {
            throw new WebApiException(HttpStatusCode.NotFound, $"The invite cannot be created because the developer with email {developerEmail} does not exist");
        }

        if (_unitOfWork.InviteRepository.FindWhere(i => i.EventEntity.Id == eventId && i.DeveloperEntity.Email == developerEmail, i => i.DeveloperEntity, i => i.EventEntity).FirstOrDefault() is not null)
        {
            throw new WebApiException(HttpStatusCode.BadRequest, $"The invite cannot be created because the developer with email {developerEmail} has already been invited to the event with id {eventId}");
        }

        var inviteToCreate = new InviteEntity
        {
            EventEntity = associatedEvent,
            DeveloperEntity = associatedDeveloper,
            Status = InviteResponseStatus.Pending
        };

        InviteEntity inviteCreated = _unitOfWork.InviteRepository.Add(inviteToCreate);

        return _mapper.Map<InviteRetrievalDto>(inviteCreated);
    }

    public async Task<ICollection<InviteRetrievalDto>> CreateInvitesAsync(SendInviteDto sendInviteDto)
    {
        var validationResult = await _invitationValidator.ValidateAsync(sendInviteDto);
        if (!validationResult.IsValid)
        {
            throw new WebApiException(HttpStatusCode.BadRequest, "The invite cannot be created because the information provided is invalid", validationResult.Errors);
        }

        List<InviteRetrievalDto> invitesCreated = new List<InviteRetrievalDto>();

        foreach (string email in sendInviteDto.DeveloperEmails)
        {
            invitesCreated.Add(this.CreateInvite(sendInviteDto.EventId, email));
        }

        await _unitOfWork.Complete(); // see added attr in EF

        return invitesCreated;
    }

    public Task<ICollection<InviteRetrievalDto>> GetInvitesByDeveloperEmailAndStatusAsync(string developerEmail, InviteResponseStatus status)
    {
        var invites = _unitOfWork.InviteRepository.FindWhere(i => i.DeveloperEntity.Email == developerEmail && i.Status == status);

        return Task.FromResult(_mapper.Map<ICollection<InviteRetrievalDto>>(invites));
    }

    public Task<ICollection<InviteRetrievalDto>> GetInvitesByEventIdAsync(int eventId)
    {
        var invites = _unitOfWork.InviteRepository.FindWhere(i => i.EventEntity.Id == eventId, i => i.DeveloperEntity, i => i.EventEntity);

        return Task.FromResult(_mapper.Map<ICollection<InviteRetrievalDto>>(invites));
    }

    public Task<ICollection<InviteRetrievalDto>> GetInvitesByEventIdAndStatusAsync(int eventId, InviteResponseStatus status)
    {
        var invites = _unitOfWork.InviteRepository.FindWhere(i => i.EventEntity.Id == eventId && i.Status == status, i => i.DeveloperEntity, i => i.EventEntity);

        return Task.FromResult(_mapper.Map<ICollection<InviteRetrievalDto>>(invites));
    }

    public async Task<InviteRetrievalDto> UpdateInviteStatusAsync(int eventId, int inviteId, InviteResponseStatus status)
    {
        InviteEntity? invite = _unitOfWork.InviteRepository.FindWhere(i => i.Id == inviteId, i => i.DeveloperEntity, i => i.EventEntity).FirstOrDefault();

        if (invite is not null)
        {
            if (invite.EventEntity.Id != eventId)
            {
                throw new WebApiException(HttpStatusCode.BadRequest, "Invite does not belong to the event");
            }

            invite.Status = status;

            _unitOfWork.InviteRepository.Update(invite);

            await _unitOfWork.Complete();

            return _mapper.Map<InviteRetrievalDto>(invite);
        }
        else
        {
            throw new WebApiException(HttpStatusCode.NotFound, "Invite not found");
        }
    }
}