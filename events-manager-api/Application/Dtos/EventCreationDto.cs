using AutoMapper;
using events_manager_api.Domain.Entities;
using events_manager_api.Domain.Enums;
//using events_manager_api.Domain.Structs;
using FluentValidation;

namespace events_manager_api.Application.Dtos;

public class EventCreationDto
{
    public string Name { get; set; } = default!;
    public string Description { get; set; } = default!;
    public DateTime Date { get; set; } = default!;
    public int EventTypeId { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }
}

public class EventCreationDTOValidator : AbstractValidator<EventCreationDto>
{
    public EventCreationDTOValidator()
    {
        RuleFor(request => request.Name)
            .NotEmpty()
            .WithMessage("Name of the developer is required.");

        RuleFor(request => request.Description)
            .NotEmpty()
            .WithMessage("Description is required.");


        RuleFor(request => request.Date)
            .NotEmpty()
            .WithMessage("Date and time is required.");

        RuleFor(request => request.EventTypeId)
            .NotEmpty()
            .WithMessage("Event type is required.");

        When(request => request.EventTypeId == ((int)EventType.In_Person), () =>
        {
            RuleFor(request => request.City)
                .NotEmpty()
                .WithMessage("City is required.");

            RuleFor(request => request.Country)
                .NotEmpty()
                .WithMessage("Country is required.");
        });
    }
}

public class EventCreationDTOMapperProfile : Profile
{
    public EventCreationDTOMapperProfile()
    {
        CreateMap<EventCreationDto, EventEntity>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.Date))
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.EventTypeId))
            .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.City))
            .ForMember(dest => dest.Country, opt => opt.MapFrom(src => src.Country));

        CreateMap<EventEntity, EventCreationDto>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.Date))
            .ForMember(dest => dest.EventTypeId, opt => opt.MapFrom(src => src.Type))
            .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.City))
            .ForMember(dest => dest.Country, opt => opt.MapFrom(src => src.Country));
    }
}