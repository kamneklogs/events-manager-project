using AutoMapper;
using events_manager_api.Domain.Entities;
using events_manager_api.Domain.Enums;
using events_manager_api.Domain.Structs;

namespace events_manager_api.Application.Dtos;

public class EventRetrievalDto
{
    public int Id { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string Description { get; set; } = default!;
    public DateTime Date { get; set; } = default!;
    public string Type { get; set; } = default!;
    public string? City { get; set; }
    public string? Country { get; set; }
    public string? Location { get; set; }
}

public class EventRetrievalDTOMapperProfile : Profile
{
    public EventRetrievalDTOMapperProfile()
    {
        CreateMap<EventEntity, EventRetrievalDto>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.Date))
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.ToString()))
            .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.City))
            .ForMember(dest => dest.Country, opt => opt.MapFrom(src => src.Country))
            .ForMember(dest => dest.Location, opt => opt.MapFrom(src => new Location(src.Latitude, src.Longitude).ToString()));
    }
}