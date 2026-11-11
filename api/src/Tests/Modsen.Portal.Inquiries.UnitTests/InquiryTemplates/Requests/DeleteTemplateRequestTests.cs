using MockQueryable.Moq;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Services;
using Modsen.Portal.Inquiries.DataAccess.Npgsql;
using Modsen.Portal.Inquiries.DataAccess.Npgsql.Entities;
using Modsen.Portal.Inquiries.Requests.InquiryTemplates.Requests;
using Moq;

namespace Modsen.Portal.Inquiries.UnitTests.InquiryTemplates.Requests;

public class DeleteTemplateRequestTests
{
    private readonly Mock<IInquiriesDbContext> _inquiriesDbContextMock = new();
    private readonly Mock<IClockService> _clockServiceMock = new();

    private readonly DeleteTemplateRequest _handler;
    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    public DeleteTemplateRequestTests()
    {
        _handler = new DeleteTemplateRequest(_inquiriesDbContextMock.Object, _clockServiceMock.Object);
    }

    [Fact]
    public async Task HandleWhenTemplateDoesNotExistShouldThrowEntityNotFoundException()
    {
        // Arrange
        var command = new DeleteTemplateRequest.Command(int.MaxValue);

        _inquiriesDbContextMock
            .Setup(context => context.InquiryTemplates)
            .Returns(new List<InquiryTemplateEntity>().BuildMockDbSet().Object);

        // Act
        var result = () => _handler.Handle(command, _cancellationToken);

        // Assert
        await result.Should().ThrowAsync<EntityNotFoundException>()
            .WithMessage(PortalMessages.InquiryTemplateNotFound);
    }

    [Fact]
    public async Task HandleWhenTemplateIsNotActiveShouldThrowEntityNotFoundException()
    {
        // Arrange
        var command = new DeleteTemplateRequest.Command(1);

        var template = new InquiryTemplateEntity
        {
            Id = 1,
            Name = "test",
            IsActive = false
        };

        _inquiriesDbContextMock
            .Setup(context => context.InquiryTemplates)
            .Returns(new List<InquiryTemplateEntity> { template }.BuildMockDbSet().Object);

        // Act
        var result = () => _handler.Handle(command, _cancellationToken);

        // Assert
        await result.Should().ThrowAsync<EntityNotFoundException>()
            .WithMessage(PortalMessages.InquiryTemplateNotFound);
    }

    [Fact]
    public async Task HandleWhenTemplateHasActiveAssignsShouldThrowEntityConflictException()
    {
        // Arrange
        var command = new DeleteTemplateRequest.Command(1);
        var today = new DateOnly(2024, 1, 15);

        var template = new InquiryTemplateEntity
        {
            Id = 1,
            IsActive = true,
            Name = "test",
            InquiryTemplateAgreements = []
        };

        var activeAssign = new UserInquiryTemplateEntity
        {
            Id = 1,
            InquiryTemplateId = 1,
            DateTo = today.AddDays(10) // будущая дата
        };

        _inquiriesDbContextMock
            .Setup(context => context.InquiryTemplates)
            .Returns(new List<InquiryTemplateEntity> { template }.BuildMockDbSet().Object);

        _inquiriesDbContextMock
            .Setup(context => context.UserInquiryTemplates)
            .Returns(new List<UserInquiryTemplateEntity> { activeAssign }.BuildMockDbSet().Object);

        _clockServiceMock
            .Setup(service => service.TodayUtc())
            .Returns(today);

        // Act
        var result = () => _handler.Handle(command, _cancellationToken);

        // Assert
        await result.Should().ThrowAsync<EntityConflictException>()
            .WithMessage("You cannot delete absence with assigned users");
    }

    [Fact]
    public async Task HandleWhenTemplateCanBeDeletedShouldReturnDtoAndDeactivate()
    {
        // Arrange
        var command = new DeleteTemplateRequest.Command(1);
        var today = new DateOnly(2024, 1, 15);

        var template = new InquiryTemplateEntity
        {
            Id = 1,
            Name = "Test Template",
            IsActive = true,
        };

        _inquiriesDbContextMock
            .Setup(context => context.InquiryTemplates)
            .Returns(new List<InquiryTemplateEntity> { template }.BuildMockDbSet().Object);

        _inquiriesDbContextMock
            .Setup(context => context.UserInquiryTemplates)
            .Returns(new List<UserInquiryTemplateEntity>().BuildMockDbSet().Object);

        _clockServiceMock
            .Setup(service => service.TodayUtc())
            .Returns(today);

        _inquiriesDbContextMock
            .Setup(context => context.SaveChangesAsync(_cancellationToken))
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, _cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(1);
        result.Name.Should().Be("Test Template");

        template.IsActive.Should().BeFalse();
        _inquiriesDbContextMock.Verify(context => context.SaveChangesAsync(_cancellationToken), Times.Once);
    }

    [Fact]
    public async Task HandleWhenNoActiveAssignsShouldDeleteSuccessfully()
    {
        // Arrange
        var command = new DeleteTemplateRequest.Command(1);
        var today = new DateOnly(2024, 1, 15);

        var template = new InquiryTemplateEntity()
        {
            Id = 1,
            IsActive = true,
            Name = "test",
            InquiryTemplateAgreements = []
        };

        _inquiriesDbContextMock
            .Setup(context => context.InquiryTemplates)
            .Returns(new List<InquiryTemplateEntity> { template }.BuildMockDbSet().Object);

        _inquiriesDbContextMock
            .Setup(context => context.UserInquiryTemplates)
            .Returns(new List<UserInquiryTemplateEntity>().BuildMockDbSet().Object); // пустой список назначений

        _clockServiceMock
            .Setup(service => service.TodayUtc())
            .Returns(today);

        _inquiriesDbContextMock
            .Setup(context => context.SaveChangesAsync(_cancellationToken))
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, _cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(1);
        template.IsActive.Should().BeFalse();
        _inquiriesDbContextMock.Verify(context => context.SaveChangesAsync(_cancellationToken), Times.Once);
    }
}
