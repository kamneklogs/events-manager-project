using System.Net;
using AutoMapper;
using events_manager_api.Application.Dtos;
using events_manager_api.Common.Exceptions;
using events_manager_api.Domain.Entities;
using events_manager_api.Domain.UnitOfWork;
using FluentValidation;
using FluentValidation.Results;

namespace events_manager_api.Application.Services.Impl;

public class DeveloperService : IDeveloperService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IValidator<DeveloperDto> _validator;

    private readonly ILogger<DeveloperService> _logger;

    public DeveloperService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<DeveloperService> logger, IValidator<DeveloperDto> validator)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
        _validator = validator;
    }

    public async Task<DeveloperDto> CreateDeveloperAsync(DeveloperDto developer)
    {

        ValidationResult validationResult = _validator.Validate(developer);

        if (!validationResult.IsValid)
        {
            throw new WebApiException(HttpStatusCode.BadRequest, "Developer information is invalid", validationResult.Errors);
        }

        _unitOfWork.DeveloperRepository.Add(_mapper.Map<DeveloperEntity>(developer));
        await _unitOfWork.Complete();

        return this.GetDeveloperByEmail(developer.Email);
    }

    public DeveloperDto GetDeveloperByEmail(string email)
    {
        var developer = _unitOfWork.DeveloperRepository.GetAsync(email).Result;

        if (developer == null)
        {
            throw new WebApiException(HttpStatusCode.NotFound, $"Developer with email {email} not found");
        }

        return _mapper.Map<DeveloperDto>(developer);
    }

    public Task<ICollection<DeveloperDto>> GetDevelopersAsync()
    {
        IEnumerable<DeveloperEntity> developers = _unitOfWork.DeveloperRepository.GetAllAsync().Result;
        return Task.FromResult(_mapper.Map<ICollection<DeveloperDto>>(developers));
    }
}