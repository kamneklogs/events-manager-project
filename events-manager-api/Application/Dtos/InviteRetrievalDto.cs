
using AutoMapper;
using events_manager_api.Domain.Entities;

namespace events_manager_api.Application.Dtos;

public class InviteRetrievalDto
{
    public int Id { get; set; }
    public int EventId { get; set; } = default!;
    public string DeveloperEmail { get; set; } = default!;
    public string Status { get; set; } = default!;
}

public class InviteRetrievalDTOMapperProfile : Profile
{
    public InviteRetrievalDTOMapperProfile()
    {
        CreateMap<InviteEntity, InviteRetrievalDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.EventId, opt => opt.MapFrom(src => src.EventEntity.Id))
            .ForMember(dest => dest.DeveloperEmail, opt => opt.MapFrom(src => src.DeveloperEntity.Email))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));
    }
}