using FluentValidation;

namespace events_manager_api.Application.Dtos;

public class SendInviteDto
{
    public int EventId { get; set; }

    public ICollection<string> DeveloperEmails { get; set; } = default!;
}

public class SendInviteDtoValidator : AbstractValidator<SendInviteDto>
{
    public SendInviteDtoValidator()
    {
        RuleFor(request => request.EventId)
            .NotEmpty()
            .WithMessage("Event id is required.");
    }
}
