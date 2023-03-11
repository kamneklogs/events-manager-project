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
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace events_manager_api_testing.Services;

[TestClass]
public class DeveloperServiceTest
{

    private Mock<IUnitOfWork> _unitOfWorkMock = default!;
    private Mock<IMapper> _mapperMock = default!;
    private Mock<ILogger<DeveloperService>> _loggerMock = default!;
    // Mock validator
    private Mock<IValidator<DeveloperDto>> _validatorMock = default!;
    private IDeveloperService _developerService = default!;


    [TestInitialize]
    public void Setup()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _mapperMock = new Mock<IMapper>();
        _loggerMock = new Mock<ILogger<DeveloperService>>();
        _validatorMock = new Mock<IValidator<DeveloperDto>>();
        _developerService = new DeveloperService(_unitOfWorkMock.Object, _mapperMock.Object, _loggerMock.Object, _validatorMock.Object);
    }

    [TestMethod]
    public void GetDeveloperByEmailShouldReturnDeveloper()
    {
        // Arrange

        var developerEntity = FakeDataHelper.GenerateFakeDeveloperEntity().Generate();

        _unitOfWorkMock.Setup(u => u.DeveloperRepository.GetAsync(developerEntity.Email))
            .ReturnsAsync(developerEntity);

        _mapperMock.Setup(m => m.Map<DeveloperDto>(developerEntity))
            .Returns(new DeveloperDto
            {
                Email = developerEntity.Email,
                Name = developerEntity.Name,
                CurrentCity = developerEntity.CurrentCity,
                PhoneNumber = developerEntity.PhoneNumber
            });

        // Act
        var developerResult = _developerService.GetDeveloperByEmail(developerEntity.Email);

        // Assert
        developerResult.Should().NotBeNull();

        developerResult.Email.Should().Be(developerEntity.Email);
        developerResult.Name.Should().Be(developerEntity.Name);
        developerResult.CurrentCity.Should().Be(developerEntity.CurrentCity);
        developerResult.PhoneNumber.Should().Be(developerEntity.PhoneNumber);

        _unitOfWorkMock.Verify(u => u.DeveloperRepository.GetAsync(developerEntity.Email), Times.Once);
        _mapperMock.Verify(m => m.Map<DeveloperDto>(developerEntity), Times.Once);
    }

    [TestMethod]
    public void GetDeveloperByEmailShouldThrowExceptionWhenDeveloperNotFound()
    {
        // Arrange
        var email = "john.doe@test.com";
        DeveloperEntity? developerEntity = null;


        _unitOfWorkMock.Setup(u => u.DeveloperRepository.GetAsync(email))
            .ReturnsAsync((DeveloperEntity)developerEntity!);


        // Act and Assert
        _developerService.Invoking(d => d.GetDeveloperByEmail(email))
            .Should().Throw<WebApiException>()
            .WithMessage($"Developer with email {email} not found");

        _unitOfWorkMock.Verify(u => u.DeveloperRepository.GetAsync(email), Times.Once);
    }

    [TestMethod]
    public async Task GetDevelopersAsyncShouldReturnDevelopers()
    {
        // Arrange
        var developers = FakeDataHelper.GenerateFakeDeveloperEntity().Generate(1);

        _unitOfWorkMock.Setup(u => u.DeveloperRepository.GetAllAsync())
            .ReturnsAsync(developers);

        _mapperMock.Setup(m => m.Map<ICollection<DeveloperDto>>(developers)).Returns(new List<DeveloperDto>
        {
            new DeveloperDto
            {
                Email = developers.First().Email,
                Name = developers.First().Name,
                CurrentCity = developers.First().CurrentCity,
                PhoneNumber = developers.First().PhoneNumber
            }
        });

        // Act
        var developersResult = await _developerService.GetDevelopersAsync();

        // Assert
        developersResult.Should().NotBeNull();
        developersResult.Should().HaveCount(developers.Count);

        var developer = developersResult.First();

        developer.Email.Should().Be(developers.First().Email);
        developer.Name.Should().Be(developers.First().Name);
        developer.CurrentCity.Should().Be(developers.First().CurrentCity);
        developer.PhoneNumber.Should().Be(developers.First().PhoneNumber);

        _unitOfWorkMock.Verify(u => u.DeveloperRepository.GetAllAsync(), Times.Once);
        _mapperMock.Verify(m => m.Map<ICollection<DeveloperDto>>(developers), Times.Once);
    } 
/*
    [TestMethod]
    public void CreateDeveloperAsyncShouldCreateDeveloper()
    {
        // Arrange
        var developerDto = FakeDataHelper.GenerateFakeDeveloperDto().Generate();

        var developerEntity = new DeveloperEntity
        {
            Email = developerDto.Email,
            Name = developerDto.Name,
            CurrentCity = developerDto.CurrentCity,
            PhoneNumber = developerDto.PhoneNumber
        };


       FluentValidation.Results.ValidationResult validationResult = new FluentValidation.Results.alidationResult();
       validationResult.IsValid = true;
        _validatorMock.Setup(v => v.Validate(developerDto, default))
            .Returns(validationResult);

        _mapperMock.Setup(m => m.Map<DeveloperEntity>(developerDto))
            .Returns(developerEntity);

        _unitOfWorkMock.Setup(u => u.DeveloperRepository.Add(developerEntity))
            .Returns((EntityEntry<DeveloperEntity>)null!); // We don't care about the return value

        _unitOfWorkMock.Setup(u => u.DeveloperRepository.GetAsync(developerEntity.Email))
            .ReturnsAsync(developerEntity);

        _mapperMock.Setup(m => m.Map<DeveloperDto>(developerEntity))
            .Returns(developerDto);

        // Act
        var devCreated = _developerService.CreateDeveloperAsync(developerDto).Result;



        // Assert
        devCreated.Should().NotBeNull();
        devCreated.Email.Should().Be(developerDto.Email);
    } */
}