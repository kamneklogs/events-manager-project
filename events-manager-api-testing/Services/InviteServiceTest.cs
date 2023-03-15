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
using events_manager_api.Domain.Enums;
using System.Linq.Expressions;

namespace events_manager_api_testing.Services;

[TestClass]
public class InviteServiceTest
{
    private Mock<IUnitOfWork> _unitOfWorkMock = default!;
    private Mock<IMapper> _mapperMock = default!;
    private Mock<ILogger<InviteService>> _loggerMock = default!;
    private Mock<IValidator<SendInviteDto>> _validatorMock = default!;

    private IInviteService _inviteService = default!;

    [TestInitialize]
    public void Setup()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _mapperMock = new Mock<IMapper>();
        _loggerMock = new Mock<ILogger<InviteService>>();
        _validatorMock = new Mock<IValidator<SendInviteDto>>();
        _inviteService = new InviteService(_unitOfWorkMock.Object, _mapperMock.Object, _loggerMock.Object, _validatorMock.Object);
    }

    [TestMethod]
    public void CreateInviteShouldCreateInvite()
    {
        // Arrange
        var eventId = 1;
        var developerEmail = "john.doe@test.com";

        var inviteId = 1;

        var inviteRetrievalDto = new InviteRetrievalDto
        {
            Id = inviteId,
            EventId = eventId,
            DeveloperEmail = developerEmail,
            Status = InviteResponseStatus.Pending.ToString()
        };


        var associatedEventEntity = FakeDataHelper.GenerateFakeEventEntityForIn_PersonTypes(eventId).Generate();
        var developerEntity = FakeDataHelper.GenerateFakeDeveloperEntity(developerEmail).Generate();

        var inviteCreated = new InviteEntity
        {
            Id = inviteId,
            EventEntity = associatedEventEntity,
            DeveloperEntity = developerEntity,
            Status = InviteResponseStatus.Pending
        };

        _unitOfWorkMock.Setup(u => u.EventRepository.FindWhere(It.IsAny<Expression<Func<EventEntity, bool>>>()))
            .Returns(new List<EventEntity> { associatedEventEntity }.AsQueryable());

        _unitOfWorkMock.Setup(u => u.DeveloperRepository.FindWhere(It.IsAny<Expression<Func<DeveloperEntity, bool>>>()))
            .Returns(new List<DeveloperEntity> { developerEntity }.AsQueryable());

        _unitOfWorkMock.Setup(u => u.InviteRepository.FindWhere(It.IsAny<Expression<Func<InviteEntity, bool>>>()))
            .Returns(new List<InviteEntity>().AsQueryable());

        _unitOfWorkMock.Setup(u => u.InviteRepository.Add(inviteCreated))
            .Returns(inviteCreated);

        _mapperMock.Setup(m => m.Map<InviteRetrievalDto>(It.IsAny<InviteEntity>()))
            .Returns(inviteRetrievalDto);

        // Act
        var inviteResult = _inviteService.CreateInvite(eventId, developerEmail);

        // Assert
        inviteResult.Should().BeEquivalentTo(inviteRetrievalDto);

        _unitOfWorkMock.Verify(u => u.EventRepository.FindWhere(It.IsAny<Expression<Func<EventEntity, bool>>>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.DeveloperRepository.FindWhere(It.IsAny<Expression<Func<DeveloperEntity, bool>>>()), Times.Once);
        _mapperMock.Verify(m => m.Map<InviteRetrievalDto>(It.IsAny<InviteEntity>()), Times.Once);
    }

    [TestMethod]
    public void CreateInviteShouldThrowExceptionWhenEventDoesNotExist()
    {
        // Arrange
        var eventId = 1;
        var developerEmail = "john.doe@test.com";

        _unitOfWorkMock.Setup(u => u.EventRepository.FindWhere(It.IsAny<Expression<Func<EventEntity, bool>>>()))
                    .Returns(new List<EventEntity>().AsQueryable());

        _unitOfWorkMock.Setup(u => u.DeveloperRepository.FindWhere(It.IsAny<Expression<Func<DeveloperEntity, bool>>>()))
            .Returns(new List<DeveloperEntity>().AsQueryable());

        // Act and Assert
        _inviteService.Invoking(i => i.CreateInvite(eventId, developerEmail))
            .Should().Throw<WebApiException>()
            .WithMessage($"The invite cannot be created because the event with id {eventId} does not exist");

        _unitOfWorkMock.Verify(u => u.EventRepository.FindWhere(It.IsAny<Expression<Func<EventEntity, bool>>>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.DeveloperRepository.FindWhere(It.IsAny<Expression<Func<DeveloperEntity, bool>>>()), Times.Once);
    }

    [TestMethod]
    public void CreateInviteShouldThrowExceptionWhenDeveloperDoesNotExist()
    {
        // Arrange
        var eventId = 1;
        var developerEmail = "john.doe@test.com";
        var inviteId = 1;

        var inviteRetrievalDto = new InviteRetrievalDto
        {
            Id = inviteId,
            EventId = eventId,
            DeveloperEmail = developerEmail,
            Status = InviteResponseStatus.Pending.ToString()
        };

        var associatedEventEntity = FakeDataHelper.GenerateFakeEventEntityForIn_PersonTypes(eventId).Generate();

        _unitOfWorkMock.Setup(u => u.EventRepository.FindWhere(It.IsAny<Expression<Func<EventEntity, bool>>>()))
                    .Returns(new List<EventEntity> { associatedEventEntity }.AsQueryable());

        _unitOfWorkMock.Setup(u => u.DeveloperRepository.FindWhere(It.IsAny<Expression<Func<DeveloperEntity, bool>>>()))
            .Returns(new List<DeveloperEntity>().AsQueryable());

        // Act and Assert
        _inviteService.Invoking(i => i.CreateInvite(eventId, developerEmail))
            .Should().Throw<WebApiException>()
            .WithMessage($"The invite cannot be created because the developer with email {developerEmail} does not exist");

        _unitOfWorkMock.Verify(u => u.EventRepository.FindWhere(It.IsAny<Expression<Func<EventEntity, bool>>>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.DeveloperRepository.FindWhere(It.IsAny<Expression<Func<DeveloperEntity, bool>>>()), Times.Once);
    }

    [TestMethod]
    public async Task UpdateInviteStatusAsyncShouldUpdateInviteStatus()
    {
        // Arrange
        var eventId = 1;
        var inviteId = 1;
        var developerEmail = "john.doe@test.com";

        var inviteStatus = InviteResponseStatus.Accepted;

        var inviteEntity = FakeDataHelper.GenerateFakeInviteEntity(inviteId, eventId, developerEmail).Generate();

        var inviteRetrievalDto = new InviteRetrievalDto
        {
            Id = inviteId,
            EventId = eventId,
            DeveloperEmail = developerEmail,
            Status = inviteStatus.ToString()
        };

        _unitOfWorkMock.Setup(u => u.InviteRepository.FindWhere(It.IsAny<Expression<Func<InviteEntity, bool>>>(), It.IsAny<Expression<Func<InviteEntity, object>>[]>()))
            .Returns(new List<InviteEntity> { inviteEntity }.AsQueryable());

        _unitOfWorkMock.Setup(u => u.InviteRepository.Update(inviteEntity));
        _unitOfWorkMock.Setup(u => u.Complete())
            .ReturnsAsync(true);

        _mapperMock.Setup(m => m.Map<InviteRetrievalDto>(inviteEntity))
            .Returns(inviteRetrievalDto);

        System.Console.WriteLine(inviteEntity);

        // Act
        await _inviteService.UpdateInviteStatusAsync(eventId, inviteId, inviteStatus);

        // Assert
        inviteEntity.Status.Should().Be(inviteStatus);

        _unitOfWorkMock.Verify(u => u.InviteRepository.FindWhere(It.IsAny<Expression<Func<InviteEntity, bool>>>(), It.IsAny<Expression<Func<InviteEntity, object>>[]>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.InviteRepository.Update(inviteEntity), Times.Once);
        _unitOfWorkMock.Verify(u => u.Complete(), Times.Once);
    }

    [TestMethod]
    public async Task UpdateInviteStatusAsyncShouldThrowExceptionWhenInviteDoesNotExist()
    {
        // Arrange
        var eventId = 1;
        var inviteId = 1;
        var inviteStatus = InviteResponseStatus.Accepted;

        _unitOfWorkMock.Setup(u => u.InviteRepository.FindWhere(It.IsAny<Expression<Func<InviteEntity, bool>>>(), It.IsAny<Expression<Func<InviteEntity, object>>[]>()))
            .Returns(new List<InviteEntity>().AsQueryable()); // No invite found

        // Act and Assert
        await _inviteService.Invoking(i => i.UpdateInviteStatusAsync(eventId, inviteId, inviteStatus))
            .Should().ThrowAsync<WebApiException>()
            .WithMessage("Invite not found");

        _unitOfWorkMock.Verify(u => u.InviteRepository.FindWhere(It.IsAny<Expression<Func<InviteEntity, bool>>>(), It.IsAny<Expression<Func<InviteEntity, object>>[]>()), Times.Once);
    }

    [TestMethod]
    public async Task UpdateInviteStatusAsyncShouldThrowExceptionWhenInviteBelongsToAnotherEvent()
    {
        // Arrange
        var eventId = 1;
        var anotherEventId = 2;
        var inviteId = 1;
        var inviteStatus = InviteResponseStatus.Accepted;
        var developerEmail = "john.doe@test.com";

        var inviteEntity = FakeDataHelper.GenerateFakeInviteEntity(inviteId, eventId, developerEmail, inviteStatus).Generate(); // Invite belongs to another event

        _unitOfWorkMock.Setup(u => u.InviteRepository.FindWhere(It.IsAny<Expression<Func<InviteEntity, bool>>>(), It.IsAny<Expression<Func<InviteEntity, object>>[]>()))
            .Returns(new List<InviteEntity> { inviteEntity }.AsQueryable());

        // Act and Assert
        await _inviteService.Invoking(i => i.UpdateInviteStatusAsync(anotherEventId, inviteId, inviteStatus))
            .Should().ThrowAsync<WebApiException>()
            .WithMessage("Invite does not belong to the event");

        _unitOfWorkMock.Verify(u => u.InviteRepository.FindWhere(It.IsAny<Expression<Func<InviteEntity, bool>>>(), It.IsAny<Expression<Func<InviteEntity, object>>[]>()), Times.Once);
    }
}