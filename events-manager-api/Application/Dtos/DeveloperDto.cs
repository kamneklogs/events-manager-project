using AutoMapper;
using events_manager_api.Domain.Entities;
using FluentValidation;

namespace events_manager_api.Application.Dtos;

public class DeveloperDto
{
    public string Email { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string CurrentCity { get; set; } = default!;
    public string PhoneNumber { get; set; } = default!;
}

public class DeveloperDtoValidator : AbstractValidator<DeveloperDto>
{
    public DeveloperDtoValidator()
    {
        RuleFor(request => request.Email)
            .NotEmpty()
            .WithMessage("Email is required.")
            .EmailAddress()
            .WithMessage("Email not valid.");

        RuleFor(request => request.Name)
            .NotEmpty()
            .WithMessage("Name of the developer is required.");

        RuleFor(request => request.CurrentCity)
            .NotEmpty()
            .WithMessage("Current city name is required.");

        RuleFor(request => request.PhoneNumber)
            .NotEmpty()
            .WithMessage("Phone number is required.");
    }
}

public class DeveloperDtoMapperProfile : Profile
{
    public DeveloperDtoMapperProfile()
    {
        CreateMap<DeveloperDto, DeveloperEntity>()
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.CurrentCity, opt => opt.MapFrom(src => src.CurrentCity))
            .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber));

        CreateMap<DeveloperEntity, DeveloperDto>()
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.CurrentCity, opt => opt.MapFrom(src => src.CurrentCity))
            .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber));
    }
}