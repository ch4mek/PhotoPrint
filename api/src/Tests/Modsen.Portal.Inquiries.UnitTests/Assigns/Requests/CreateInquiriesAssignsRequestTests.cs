using MockQueryable.Moq;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Inquiries;
using Modsen.Portal.Inquiries.DataAccess.Npgsql;
using Modsen.Portal.Inquiries.DataAccess.Npgsql.Entities;
using Modsen.Portal.Inquiries.Requests.Agreements.Core;
using Modsen.Portal.Inquiries.Requests.Assigns.Core.Composers;
using Modsen.Portal.Inquiries.Requests.Assigns.Requests;
using Modsen.Portal.Inquiries.Requests.Inquiries.Core.Resolvers;
using Modsen.Portal.TeamBoard.Contract;
using Moq;

namespace Modsen.Portal.Inquiries.UnitTests.Assigns.Requests;

public class CreateInquiriesAssignsRequestTests
{
    private readonly Mock<IUserInquiryTemplateComposer> _inquiryTemplateComposerMock = new();
    private readonly Mock<IChangedAgreementTransactionComposer> _transactionsComposerMock = new();
    private readonly Mock<IInquiriesDbContext> _dbContextMock = new();
    private readonly Mock<ITeamBoardContract> _teamboardContract = new();
    private readonly Mock<IAuthenticatedUserContext> _authenticatedUserContextMock = new();
    private readonly Mock<IRejectInquiriesAfterUnassignResolver> _rejectInquiriesAfterUnassignResolverMock = new();
    private readonly Mock<IResetUserAgreementBalanceComposer> _resetUserAgreementBalanceComposerMock = new();


    private readonly CreateInquiriesAssignsRequest _handler;

    private readonly CancellationToken _token = CancellationToken.None;

    public CreateInquiriesAssignsRequestTests()
    {
        _handler = new CreateInquiriesAssignsRequest(
            _inquiryTemplateComposerMock.Object,
            _dbContextMock.Object,
            _transactionsComposerMock.Object,
            _authenticatedUserContextMock.Object,
            _rejectInquiriesAfterUnassignResolverMock.Object,
            _resetUserAgreementBalanceComposerMock.Object);
    }

    [Fact]
    public async Task HandleWhenNotAllUsersAreListedThrowsInvalidDataAppException()
    {
        //Arrange
        _dbContextMock
            .Setup(context => context.InquiriesUsers)
            .Returns(new List<InquiriesUserEntity>
            {
                new()
                {
                    UserId = 1,
                    IsActive = true,
                    Email = "test",
                    FirstName = "test",
                    LastName = "test",
                },
                new()
                {
                    UserId = 2,
                    IsActive = true,
                    Email = "test",
                    FirstName = "test",
                    LastName = "test",
                },
                new()
                {
                    UserId = 3,
                    IsActive = true,
                    Email = "test",
                    FirstName = "test",
                    LastName = "test",
                },
                new()
                {
                    UserId = 4,
                    IsActive = false,
                    Email = "test",
                    FirstName = "test",
                    LastName = "test",
                }
            }.BuildMockDbSet().Object);

        _teamboardContract.Setup(x => x.GetUsersHireDates(_token))
            .ReturnsAsync([
                new() { UserId = 5, HireDate = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(-1) }
            ]);
        var userIds = new[] { 1, 2, 3, 4 };

        var command = new CreateInquiriesAssignsRequest.Command(
            userIds, [], 1, null, DateOnly.FromDateTime(DateTime.Today));

        //Act
        var action = () => _handler.Handle(command, _token);

        //Assert
        await action.Should().ThrowAsync<EntityNotFoundException>("Not all users are exist");

    }

    [Fact]
    public async Task HandleWhenInquiryTemplateNotExistThrowsInvalidDataAppException()
    {
        //Arrange
        _dbContextMock
            .Setup(context => context.InquiriesUsers)
            .Returns(new List<InquiriesUserEntity>
            {
                new()
                {
                    UserId = 1,
                    IsActive = true,
                    Email = "test",
                    FirstName = "test",
                    LastName = "test",
                },
                new()
                {
                    UserId = 2,
                    IsActive = true,
                    Email = "test",
                    FirstName = "test",
                    LastName = "test",
                },
                new()
                {
                    UserId = 3,
                    IsActive = true,
                    Email = "test",
                    FirstName = "test",
                    LastName = "test",
                },
                new()
                {
                    UserId = 4,
                    IsActive = false,
                    Email = "test",
                    FirstName = "test",
                    LastName = "test",
                }
            }.BuildMockDbSet().Object);

        _dbContextMock
            .Setup(context => context.InquiryTemplates)
            .Returns(new List<InquiryTemplateEntity>().BuildMockDbSet().Object);

        _teamboardContract.Setup(x => x.GetUsersHireDates(_token))
            .ReturnsAsync([
                new() { UserId = 1, HireDate = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(-1) }
            ]);

        var userIds = new[] { 1, 2, 3 };

        var command = new CreateInquiriesAssignsRequest.Command(
            userIds, [], 1, null, DateOnly.FromDateTime(DateTime.Today));

        //Act
        var action = () => _handler.Handle(command, _token);

        //Assert
        await action.Should().ThrowAsync<EntityNotFoundException>("Inquiry template not found");

    }

    [Fact]
    public async Task HandleWhenRequestContainsAgreementIdButTemplateDoesNotContainAgreementsThrowsEntityNotFoundException()
    {
        //Arrange
        _dbContextMock
            .Setup(context => context.InquiriesUsers)
            .Returns(new List<InquiriesUserEntity>
            {
                new()
                {
                    UserId = 1,
                    IsActive = true,
                    Email = "test",
                    FirstName = "test",
                    LastName = "test",
                },
                new()
                {
                    UserId = 2,
                    IsActive = true,
                    Email = "test",
                    FirstName = "test",
                    LastName = "test",
                },
                new()
                {
                    UserId = 3,
                    IsActive = true,
                    Email = "test",
                    FirstName = "test",
                    LastName = "test",
                },
                new()
                {
                    UserId = 4,
                    IsActive = false,
                    Email = "test",
                    FirstName = "test",
                    LastName = "test",
                }
            }.BuildMockDbSet().Object);

        _dbContextMock
            .Setup(context => context.InquiryTemplates)
            .Returns(new List<InquiryTemplateEntity>
            {
                new() {
                    Id = 1,
                    Name = "test",
                    IsActive = true,
                    InquiryType = InquiryType.Vacation,
                    InquiryTemplateAgreements =
                    [
                        new() { Id = 10, }
                    ]
                }
            }.BuildMockDbSet().Object);

        _dbContextMock
            .Setup(context => context.InquiryTemplateAgreements)
            .Returns(new List<InquiryTemplateAgreementEntity>
            {
                new() {
                    Id = 10,
                }
            }.BuildMockDbSet().Object);

        _dbContextMock
            .Setup(context => context.UserInquiryTemplates)
            .Returns(new List<UserInquiryTemplateEntity>
            {
            }.BuildMockDbSet().Object);

        _inquiryTemplateComposerMock.Setup(x => x.Compose(It.IsAny<int[]>(), It.IsAny<DateOnly>(), It.IsAny<int>(), _token))
            .ReturnsAsync([]);

        _teamboardContract.Setup(x => x.GetUsersHireDates(_token))
            .ReturnsAsync([
                new() { UserId = 1, HireDate = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(-1) }
            ]);
        var userIds = new[] { 1, 2, 3 };

        var command = new CreateInquiriesAssignsRequest.Command(
            userIds, [], 1, 1, DateOnly.FromDateTime(DateTime.Today));

        //Act
        var action = () => _handler.Handle(command, _token);

        //Assert
        await action.Should().ThrowAsync<EntityNotFoundException>("Agreement template not found");

    }

    [Fact]
    public async Task HandleWhenInquiryWithAgreementAndRequestValidReturnsIds()
    {
        //Arrange
        var date = DateOnly.FromDateTime(DateTime.Today);

        _dbContextMock
            .Setup(context => context.InquiriesUsers)
            .Returns(new List<InquiriesUserEntity>
            {
                new ()
                {
                    UserId = 1,
                    IsActive = true,
                    Email = "test",
                    FirstName = "test",
                    LastName = "test",
                    HireDate = DateOnly.FromDateTime(DateTime.Today).AddDays(-10)
                },
                new ()
                {
                    UserId = 2,
                    IsActive = true,Email = "test",
                    FirstName = "test",
                    LastName = "test",
                    HireDate = DateOnly.FromDateTime(DateTime.Today).AddDays(-10)
                },
                new ()
                {
                    UserId = 3,
                    IsActive = true,Email = "test",
                    FirstName = "test",
                    LastName = "test",
                    HireDate = DateOnly.FromDateTime(DateTime.Today).AddDays(-10)
                },
                new ()
                {
                    UserId = 4,
                    IsActive = false,Email = "test",
                    FirstName = "test",
                    LastName = "test",
                    HireDate = DateOnly.FromDateTime(DateTime.Today).AddDays(-10)
                },
                new ()
                {
                    UserId = 5,
                    IsActive = true,Email = "test",
                    FirstName = "test",
                    LastName = "test",
                    HireDate = DateOnly.FromDateTime(DateTime.Today).AddDays(-10)
                },
            }.BuildMockDbSet().Object);

        _dbContextMock
            .Setup(context => context.InquiryTemplates)
            .Returns(new List<InquiryTemplateEntity>
            {
                new()
                {
                    Id = 1,
                    Name = "test",
                    IsActive = true,
                    InquiryType = InquiryType.Vacation,
                    InquiryTemplateAgreements = [new() { Id = 1 }]
                }
            }.BuildMockDbSet().Object);

        _dbContextMock
            .Setup(context => context.InquiryTemplateAgreements)
            .Returns(new List<InquiryTemplateAgreementEntity>
            {
                new()
                {
                    Id = 1,
                    Name = "test",
                    IsDefault = false,
                    Type = AgreementType.Unknown,
                    InquiryTemplateId = 1,
                    InquiryTemplate = new InquiryTemplateEntity
                    {
                        Id = 1,
                        IsActive = true,
                        Name = "test",
                    }

                }
            }.BuildMockDbSet().Object);

        _dbContextMock
            .Setup(context => context.UserInquiryTemplates)
            .Returns(new List<UserInquiryTemplateEntity>
            {
            }.BuildMockDbSet().Object);

        _dbContextMock
            .Setup(context => context.UserAgreements)
            .Returns(new List<UserAgreementEntity>
            {
                new()
                {
                    Id = 100,
                    UserId = 5,
                    InquiryAgreementTemplateId = 1,
                    DateFrom = date.AddDays(-10),
                    DateTo = null,
                    IsDeleted = false
                }
            }.BuildMockDbSet().Object);

        _dbContextMock
            .Setup(context => context.UserAgreementTransactions)
            .Returns(new List<UserAgreementTransactionEntity>
            {
            }.BuildMockDbSet().Object);

        _dbContextMock
            .Setup(context => context.UserInquiries)
            .Returns(new List<UserInquiryEntity>().BuildMockDbSet().Object);

        var userIds = new[] { 1, 2, 3 };

        var command = new CreateInquiriesAssignsRequest.Command(
            userIds, [5], 1, 1, date);

        //Act
        var result = await _handler.Handle(command, _token);

        //Assert
        result.AssignedUserInquiryOrAgreementIds.Count.Should().Be(3);
    }

    [Fact]
    public async Task HandleWhenRequestValidReturnsIds()
    {
        //Arrange
        var date = DateOnly.FromDateTime(DateTime.Today);

        _dbContextMock
            .Setup(context => context.InquiriesUsers)
            .Returns(new List<InquiriesUserEntity>
            {
                new ()
                {
                    UserId = 1,
                    IsActive = true,
                    Email = "test",
                    FirstName = "test",
                    LastName = "test",
                    HireDate = DateOnly.FromDateTime(DateTime.Today).AddDays(-10)
                },
                new ()
                {
                    UserId = 2,
                    IsActive = true,Email = "test",
                    FirstName = "test",
                    LastName = "test",
                    HireDate = DateOnly.FromDateTime(DateTime.Today).AddDays(-10)
                },
                new ()
                {
                    UserId = 3,
                    IsActive = true,Email = "test",
                    FirstName = "test",
                    LastName = "test",
                    HireDate = DateOnly.FromDateTime(DateTime.Today).AddDays(-10)
                },
                new ()
                {
                    UserId = 4,
                    IsActive = false,Email = "test",
                    FirstName = "test",
                    LastName = "test",
                    HireDate = DateOnly.FromDateTime(DateTime.Today).AddDays(-10)
                },
                new ()
                {
                    UserId = 5,
                    IsActive = true,Email = "test",
                    FirstName = "test",
                    LastName = "test",
                    HireDate = DateOnly.FromDateTime(DateTime.Today).AddDays(-10)
                },
            }.BuildMockDbSet().Object);

        _dbContextMock
            .Setup(context => context.InquiryTemplates)
            .Returns(new List<InquiryTemplateEntity>
            {
                new()
                {
                    Id = 1,
                    Name = "test",
                    IsActive = true,
                    InquiryType = InquiryType.SickLeave,
                }
            }.BuildMockDbSet().Object);

        var existingInquiryTemplateAssigns = new List<UserInquiryTemplateEntity>()
        {
            new ()
            {
                Id = 1,
                InquiryTemplateId = 1,
                InquiryTemplate = new InquiryTemplateEntity
                {
                    Id = 1,
                    Name = "test",
                    IsActive = true,
                    InquiryType = InquiryType.SickLeave,
                },
                AssignedToUserId = 1,
            },
            new ()
            {
                Id = 2,
                InquiryTemplateId = 1,
                InquiryTemplate = new InquiryTemplateEntity
                {
                    Id = 1,
                    Name = "test",
                    IsActive = true,
                    InquiryType = InquiryType.SickLeave,
                },
                AssignedToUserId = 2,
            },
            new ()
            {
                Id = 3,
                InquiryTemplateId = 1,
                InquiryTemplate = new InquiryTemplateEntity
                {
                    Id = 1,
                    Name = "test",
                    IsActive = true,
                    InquiryType = InquiryType.SickLeave,
                },
                AssignedToUserId = 3,
            },
            new ()
            {
                Id = 4,
                InquiryTemplateId = 1,
                InquiryTemplate = new InquiryTemplateEntity
                {
                    Id = 1,
                    Name = "test",
                    IsActive = true,
                    InquiryType = InquiryType.SickLeave,
                },
                AssignedToUserId = 5,
                DateFrom = date.AddDays(-10),
                DateTo = null,
            }
        };

        var composedInquiryTemplateAssigns = existingInquiryTemplateAssigns
            .Where(x => x.AssignedToUserId != 5)
            .ToList();

        _dbContextMock
            .Setup(context => context.UserInquiryTemplates)
            .Returns(existingInquiryTemplateAssigns.BuildMockDbSet().Object);

        _dbContextMock
            .Setup(context => context.UserAgreementTransactions)
            .Returns(new List<UserAgreementTransactionEntity>
            {
            }.BuildMockDbSet().Object);

        _dbContextMock
            .Setup(context => context.UserInquiries)
            .Returns(new List<UserInquiryEntity>().BuildMockDbSet().Object);

        _inquiryTemplateComposerMock
            .Setup(context => context.Compose(
                It.IsAny<int[]>(),
                It.IsAny<DateOnly>(),
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(composedInquiryTemplateAssigns);

        var userIds = new[] { 1, 2, 3 };

        var command = new CreateInquiriesAssignsRequest.Command(
            userIds, [5], 1, null, date);

        //Act
        var result = await _handler.Handle(command, _token);

        //Assert
        result.AssignedUserInquiryOrAgreementIds.Count.Should().Be(3);
    }

    [Fact]
    public async Task HandleWhenUserHasHrClaimAndAssignIdsProvidedThrowsActionNotAllowedException()
    {
        // Arrange
        _authenticatedUserContextMock
            .Setup(x => x.HasClaim(HrClaimTypeNames.ManageHrModule.Name))
            .Returns(true);

        var command = new CreateInquiriesAssignsRequest.Command(
            [1],
            [2],
            1,
            null,
            DateOnly.FromDateTime(DateTime.Today));

        // Act
        var action = () => _handler.Handle(command, _token);

        // Assert
        await action.Should().ThrowAsync<ActionNotAllowedException>()
            .WithMessage("HR users can only unassign users");
    }

    [Fact]
    public async Task HandleWhenUserHasHrAndAbsenceSettingsClaimAndAssignIdsProvidedShouldNotThrowActionNotAllowed()
    {
        // Arrange
        _authenticatedUserContextMock
            .Setup(x => x.HasClaim(HrClaimTypeNames.ManageHrModule.Name))
            .Returns(true);

        _authenticatedUserContextMock
            .Setup(x => x.HasClaim(AbsencesClaimTypeNames.ManageAbsenceSettings.Name))
            .Returns(true);

        var command = new CreateInquiriesAssignsRequest.Command(
            [1],
            [2],
            1,
            null,
            DateOnly.FromDateTime(DateTime.Today));

        // Act
        var action = () => _handler.Handle(command, _token);

        // Assert
        await action.Should().NotThrowAsync<ActionNotAllowedException>();
    }

    [Fact]
    public async Task HandleWhenUnassignedUserHasOpenInquiryShouldRejectInquiriesAfterUnassignResolver()
    {
        var date = DateOnly.FromDateTime(DateTime.Today);

        _dbContextMock.Setup(context => context.InquiriesUsers)
            .Returns(new List<InquiriesUserEntity>
            {
                new() { UserId = 1, IsActive = true, Email = "1", FirstName = "a", LastName = "a" },
                new() { UserId = 5, IsActive = true, Email = "b", FirstName = "b", LastName = "b" }
            }.BuildMockDbSet().Object);

        _dbContextMock.Setup(context => context.InquiryTemplates)
            .Returns(new List<InquiryTemplateEntity>
            {
                new() { Id = 1, Name = "test", IsActive = true, InquiryType = InquiryType.SickLeave }
            }.BuildMockDbSet().Object);

        _dbContextMock.Setup(context => context.UserInquiryTemplates)
            .Returns(new List<UserInquiryTemplateEntity>
            {
                new() { Id = 1, InquiryTemplateId = 1, AssignedToUserId = 5, DateFrom = date.AddDays(-10), DateTo = null }
            }.BuildMockDbSet().Object);

        _dbContextMock.Setup(context => context.UserAgreementTransactions)
            .Returns(new List<UserAgreementTransactionEntity>().BuildMockDbSet().Object);

        _inquiryTemplateComposerMock
            .Setup(context => context.Compose(It.IsAny<int[]>(), It.IsAny<DateOnly>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        var command = new CreateInquiriesAssignsRequest.Command([], [5], 1, null, date);

        await _handler.Handle(command, _token);

        _rejectInquiriesAfterUnassignResolverMock
            .Verify(x => x.ResolveAsync(It.IsAny<int[]>(), 1, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenNoUnassignedUsers_ShouldNotChangeInquiryStatuses()
    {
        var date = DateOnly.FromDateTime(DateTime.Today);

        _dbContextMock.Setup(context => context.InquiriesUsers)
            .Returns(new List<InquiriesUserEntity>
            {
                new() { UserId = 1, IsActive = true, Email = "a", FirstName = "a", LastName = "a" }
            }.BuildMockDbSet().Object);

        _dbContextMock.Setup(context => context.InquiryTemplates)
            .Returns(new List<InquiryTemplateEntity>
            {
                new() { Id = 1, Name = "test", IsActive = true, InquiryType = InquiryType.SickLeave }
            }.BuildMockDbSet().Object);

        _dbContextMock.Setup(context => context.UserInquiryTemplates)
            .Returns(new List<UserInquiryTemplateEntity>().BuildMockDbSet().Object);

        var inquiry = new UserInquiryEntity
        {
            Id = 200,
            UserId = 1,
            Status = UserInquiryStatus.Open,
            UserInquiryAccepts = [new UserInquiryAcceptEntity { Id = 20, UserInquiryId = 200, AcceptUserId = 101, Status = UserInquiryAcceptStatus.WaitingForApprove }]
        };

        _dbContextMock.Setup(context => context.UserInquiries)
            .Returns(new List<UserInquiryEntity> { inquiry }.BuildMockDbSet().Object);

        _dbContextMock.Setup(context => context.UserAgreementTransactions)
            .Returns(new List<UserAgreementTransactionEntity>().BuildMockDbSet().Object);

        _inquiryTemplateComposerMock
            .Setup(context => context.Compose(It.IsAny<int[]>(), It.IsAny<DateOnly>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        var command = new CreateInquiriesAssignsRequest.Command([], [], 1, null, date);

        await _handler.Handle(command, _token);

        inquiry.Status.Should().Be(UserInquiryStatus.Open);
        inquiry.UserInquiryAccepts.Single().Status.Should().Be(UserInquiryAcceptStatus.WaitingForApprove);
    }
}
