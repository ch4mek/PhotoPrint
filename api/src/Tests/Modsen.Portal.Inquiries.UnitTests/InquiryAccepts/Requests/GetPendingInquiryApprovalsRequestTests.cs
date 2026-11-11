using MockQueryable.Moq;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Inquiries;
using Modsen.Portal.Inquiries.DataAccess.Npgsql;
using Modsen.Portal.Inquiries.DataAccess.Npgsql.Entities;
using Modsen.Portal.Inquiries.Requests.Inquiries.Core.BusinessRules;
using Modsen.Portal.Inquiries.Requests.Inquiries.Core.Resolvers;
using Modsen.Portal.Inquiries.Requests.InquiryAccepts.Requests;
using Modsen.Portal.TeamBoard.Contract;
using Moq;

namespace Modsen.Portal.Inquiries.UnitTests.InquiryAccepts.Requests;

public class GetPendingInquiryApprovalsRequestTests
{
    private readonly Mock<IInquiriesDbContext> _dbContextMock = new();
    private readonly Mock<IAuthenticatedUserContext> _authenticatedUserContextMock = new();

    private readonly Mock<IGetInquiriesCanBeApprovedAsSubstituteResolver>
        _getInquiriesCanBeApprovedAsSubstituteResolverMock = new();

    private readonly Mock<IHasUserAccessToInquiryBusinessRule> _hasUserAccessToInquiryBusinessRuleMock = new();
    private readonly Mock<ITeamBoardContract> _timeTrackContract = new();
    private readonly Mock<ISetUserInfoVisibilityResolver> _setUserInfoVisibilityResolverMock = new();
    private readonly GetPendingInquiryApprovalsRequest _handler;
    private readonly CancellationToken _token = CancellationToken.None;

    public GetPendingInquiryApprovalsRequestTests()
    {
        _getInquiriesCanBeApprovedAsSubstituteResolverMock
            .Setup(x => x.ResolveAsync(false, _token))
            .ReturnsAsync([]);

        _handler = new GetPendingInquiryApprovalsRequest(
            _dbContextMock.Object,
            _authenticatedUserContextMock.Object,
            _getInquiriesCanBeApprovedAsSubstituteResolverMock.Object,
            _timeTrackContract.Object);
    }

    [Fact]
    public async Task Handle_WhenUserHasOnlyEmployeeRole_ShouldThrowActionNotAllowedException()
    {
        // Arrange
        _authenticatedUserContextMock.Setup(x => x.GetRoles()).Returns([PortalRoleNames.Employee]);

        // Act
        var act = () => _handler.Handle(new GetPendingInquiryApprovalsRequest.Query(), _token);

        // Assert
        await act.Should().ThrowAsync<ActionNotAllowedException>();
    }

    [Fact]
    public async Task Handle_WhenUserHasOnlySystemAdministratorRole_ShouldThrowActionNotAllowedException()
    {
        // Arrange
        _authenticatedUserContextMock.Setup(x => x.GetRoles()).Returns([PortalRoleNames.SystemAdministrator]);

        // Act
        var act = () => _handler.Handle(new GetPendingInquiryApprovalsRequest.Query(), _token);

        // Assert
        await act.Should().ThrowAsync<ActionNotAllowedException>();
    }

    [Fact]
    public async Task Handle_WhenUserHasEmployeeAndCtoRoles_ShouldAllowAccess()
    {
        // Arrange
        _authenticatedUserContextMock.Setup(x => x.GetRoles())
            .Returns([PortalRoleNames.Employee, PortalRoleNames.Cto]);
        _authenticatedUserContextMock.Setup(x => x.GetRequiredUserId()).Returns(10);

        List<UserInquiryAcceptEntity> accepts =
        [
            new UserInquiryAcceptEntity
            {
                Id = 1, UserInquiryId = 501, AcceptUserId = 10, Status = UserInquiryAcceptStatus.WaitingForApprove
            }
        ];

        List<UserInquiryEntity> inquiries =
        [
            BuildInquiry(
                inquiryId: 501,
                userId: 100,
                dateFrom: new DateOnly(2026, 2, 10),
                lastName: "Doe",
                firstName: "John",
                lastNameRu: null,
                firstNameRu: null,
                inquiryType: InquiryType.Vacation,
                inquiryTemplateId: 55,
                inquiryTemplateName: "Vacation")
        ];

        SetupDbSets(accepts, inquiries);
        SetupCanUserBeRetrievedBusinessRule(inquiries);

        // Act
        var result = await _handler.Handle(new GetPendingInquiryApprovalsRequest.Query(), _token);

        // Assert
        result.Should().HaveCount(1);
        result[0].UserId.Should().Be(100);
        result[0].InquiryId.Should().Be(501);
        result[0].InquiryTemplateName.Should().Be("Vacation");
        result[0].UserFullName.Should().Be("Doe John");
        result[0].ResourceGroupId.Should().BeNull();
    }

    [Fact]
    public async Task Handle_ShouldReturnOnlyCurrentUserWaitingForApproveAndOrderByDateFrom()
    {
        // Arrange
        _authenticatedUserContextMock.Setup(x => x.GetRoles()).Returns([PortalRoleNames.Cto]);
        _authenticatedUserContextMock.Setup(x => x.GetRequiredUserId()).Returns(10);

        List<UserInquiryAcceptEntity> accepts =
        [
            // included (later date)
            new UserInquiryAcceptEntity
            {
                Id = 1, UserInquiryId = 1001, AcceptUserId = 10, Status = UserInquiryAcceptStatus.WaitingForApprove
            },
            // excluded: wrong status
            new UserInquiryAcceptEntity
            {
                Id = 2, UserInquiryId = 1002, AcceptUserId = 10, Status = UserInquiryAcceptStatus.Pending
            },
            // excluded: other user
            new UserInquiryAcceptEntity
            {
                Id = 3, UserInquiryId = 1003, AcceptUserId = 999, Status = UserInquiryAcceptStatus.WaitingForApprove
            },
            // included (earlier date)
            new UserInquiryAcceptEntity
            {
                Id = 4, UserInquiryId = 1004, AcceptUserId = 10, Status = UserInquiryAcceptStatus.WaitingForApprove
            }
        ];

        List<UserInquiryEntity> inquiries =
        [
            BuildInquiry(
                inquiryId: 1001,
                userId: 201,
                dateFrom: new DateOnly(2026, 3, 10),
                lastName: "Ivanov",
                firstName: "Ivan",
                lastNameRu: null,
                firstNameRu: null,
                inquiryType: InquiryType.SickLeave),
            BuildInquiry(
                inquiryId: 1002,
                userId: 202,
                dateFrom: new DateOnly(2026, 1, 1),
                lastName: "X",
                firstName: "Y",
                lastNameRu: null,
                firstNameRu: null,
                inquiryType: InquiryType.Vacation),
            BuildInquiry(
                inquiryId: 1003,
                userId: 203,
                dateFrom: new DateOnly(2026, 2, 1),
                lastName: "A",
                firstName: "B",
                lastNameRu: null,
                firstNameRu: null,
                inquiryType: InquiryType.SickDay),
            BuildInquiry(
                inquiryId: 1004,
                userId: 204,
                dateFrom: new DateOnly(2026, 2, 5),
                lastName: "Petrov",
                firstName: "Petr",
                lastNameRu: "Петров",
                firstNameRu: "Петр",
                inquiryType: InquiryType.Vacation)
        ];

        SetupDbSets(accepts, inquiries);
        SetupCanUserBeRetrievedBusinessRule(inquiries);

        // Act
        var result = await _handler.Handle(new GetPendingInquiryApprovalsRequest.Query(), _token);

        // Assert
        result.Should().HaveCount(2);
        result.Select(x => x.DateFrom).Should().ContainInOrder(
            new DateOnly(2026, 2, 5),
            new DateOnly(2026, 3, 10));

        result[0].UserFullName.Should().Be("Петров Петр (Petrov Petr)");
        result[1].UserFullName.Should().Be("Ivanov Ivan");
        result.Should().Match(x => x.All(c => c.ResourceGroupId.HasValue == false));
    }

    [Fact]
    public async Task Handle_WhenCurrentUserIsSubstituteForAbsentApprover_ShouldReturnWaitingRequest()
    {
        // Arrange
        _authenticatedUserContextMock.Setup(x => x.GetRoles()).Returns([PortalRoleNames.Cto]);
        _authenticatedUserContextMock.Setup(x => x.GetRequiredUserId()).Returns(777);
        _getInquiriesCanBeApprovedAsSubstituteResolverMock
            .Setup(x => x.ResolveAsync(false, _token))
            .ReturnsAsync([42]);

        List<UserInquiryEntity> inquiries =
        [
            BuildInquiry(
                inquiryId: 42,
                userId: 301,
                dateFrom: new DateOnly(2026, 4, 5),
                lastName: "Substitute",
                firstName: "Case",
                lastNameRu: null,
                firstNameRu: null,
                inquiryType: InquiryType.Vacation),
            BuildInquiry(
                inquiryId: 99,
                userId: 302,
                dateFrom: new DateOnly(2026, 4, 1),
                lastName: "Other",
                firstName: "Approver",
                lastNameRu: null,
                firstNameRu: null,
                inquiryType: InquiryType.SickLeave)
        ];

        List<UserInquiryAcceptEntity> accepts =
        [
            // included: current user is substitute for this inquiry id
            new UserInquiryAcceptEntity
            {
                Id = 1, UserInquiryId = 42, AcceptUserId = 10, Status = UserInquiryAcceptStatus.WaitingForApprove
            },
            // excluded: substitute id is not returned for this inquiry
            new UserInquiryAcceptEntity
            {
                Id = 2, UserInquiryId = 99, AcceptUserId = 11, Status = UserInquiryAcceptStatus.WaitingForApprove
            }
        ];

        SetupDbSets(accepts, inquiries);
        SetupCanUserBeRetrievedBusinessRule(inquiries);

        // Act
        var result = await _handler.Handle(new GetPendingInquiryApprovalsRequest.Query(), _token);

        // Assert
        result.Should().HaveCount(1);
        result[0].UserFullName.Should().Be("Substitute Case");
        result[0].ResourceGroupId.Should().BeNull();
    }

    [Fact]
    public async Task Handle_WhenSubstituteInquiryIsNotWaitingForApprove_ShouldNotReturnIt()
    {
        // Arrange
        _authenticatedUserContextMock.Setup(x => x.GetRoles()).Returns([PortalRoleNames.Cto]);
        _authenticatedUserContextMock.Setup(x => x.GetRequiredUserId()).Returns(777);
        _getInquiriesCanBeApprovedAsSubstituteResolverMock
            .Setup(x => x.ResolveAsync(false, _token))
            .ReturnsAsync([42]);

        List<UserInquiryAcceptEntity> accepts =
        [
            new UserInquiryAcceptEntity
            {
                Id = 1, UserInquiryId = 42, AcceptUserId = 10, Status = UserInquiryAcceptStatus.Pending
            }
        ];

        List<UserInquiryEntity> inquiries =
        [
            BuildInquiry(
                inquiryId: 42,
                userId: 303,
                dateFrom: new DateOnly(2026, 4, 5),
                lastName: "Substitute",
                firstName: "Case",
                lastNameRu: null,
                firstNameRu: null,
                inquiryType: InquiryType.Vacation)
        ];

        SetupDbSets(accepts, inquiries);
        SetupCanUserBeRetrievedBusinessRule(inquiries);

        // Act
        var result = await _handler.Handle(new GetPendingInquiryApprovalsRequest.Query(), _token);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_WhenDepartmentIdIsAll_ShouldReturnFromAllDepartments()
    {
        // Arrange
        _authenticatedUserContextMock.Setup(x => x.GetRoles()).Returns([PortalRoleNames.Cto]);
        _authenticatedUserContextMock.Setup(x => x.GetRequiredUserId()).Returns(10);

        List<UserInquiryAcceptEntity> accepts =
        [
            new UserInquiryAcceptEntity
            {
                Id = 1, AcceptUserId = 10, Status = UserInquiryAcceptStatus.WaitingForApprove, UserInquiryId = 101
            },
            new UserInquiryAcceptEntity
            {
                Id = 2, AcceptUserId = 10, Status = UserInquiryAcceptStatus.WaitingForApprove, UserInquiryId = 102
            }
        ];

        List<UserInquiryEntity> inquiries =
        [
            BuildInquiry(
                inquiryId: 101,
                userId: 1001,
                dateFrom: new DateOnly(2026, 2, 1),
                lastName: "Doe",
                firstName: "John",
                lastNameRu: null,
                firstNameRu: null,
                inquiryType: InquiryType.Vacation,
                inquiryTemplateId: 11,
                inquiryTemplateName: "Vacation",
                departmentId: "dep-1",
                resourceGroupId: 1),
            BuildInquiry(
                inquiryId: 102,
                userId: 1002,
                dateFrom: new DateOnly(2026, 2, 2),
                lastName: "Smith",
                firstName: "Jane",
                lastNameRu: null,
                firstNameRu: null,
                inquiryType: InquiryType.SickLeave,
                inquiryTemplateId: 12,
                inquiryTemplateName: "Sick leave",
                departmentId: "dep-2",
                resourceGroupId: 1)
        ];

        SetupDbSets(accepts, inquiries);
        SetupCanUserBeRetrievedBusinessRule(inquiries);

        // Act
        var result = await _handler.Handle(new GetPendingInquiryApprovalsRequest.Query(), _token);

        // Assert
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task Handle_WhenDepartmentIdProvided_ShouldReturnOnlyMatchingDepartment()
    {
        // Arrange
        _authenticatedUserContextMock.Setup(x => x.GetRoles()).Returns([PortalRoleNames.Cto]);
        _authenticatedUserContextMock.Setup(x => x.GetRequiredUserId()).Returns(10);

        List<UserInquiryAcceptEntity> accepts =
        [
            new UserInquiryAcceptEntity
            {
                Id = 1, AcceptUserId = 10, Status = UserInquiryAcceptStatus.WaitingForApprove, UserInquiryId = 101
            },
            new UserInquiryAcceptEntity
            {
                Id = 2, AcceptUserId = 10, Status = UserInquiryAcceptStatus.WaitingForApprove, UserInquiryId = 102
            }
        ];

        List<UserInquiryEntity> inquiries =
        [
            BuildInquiry(
                inquiryId: 101,
                userId: 1001,
                dateFrom: new DateOnly(2026, 2, 1),
                lastName: "Doe",
                firstName: "John",
                lastNameRu: null,
                firstNameRu: null,
                inquiryType: InquiryType.Vacation,
                inquiryTemplateId: 11,
                inquiryTemplateName: "Vacation",
                departmentId: "dep-1",
                resourceGroupId: 1),
            BuildInquiry(
                inquiryId: 102,
                userId: 1002,
                dateFrom: new DateOnly(2026, 2, 2),
                lastName: "Smith",
                firstName: "Jane",
                lastNameRu: null,
                firstNameRu: null,
                inquiryType: InquiryType.SickLeave,
                inquiryTemplateId: 12,
                inquiryTemplateName: "Sick leave",
                departmentId: "dep-2",
                resourceGroupId: 2)
        ];

        SetupDbSets(accepts, inquiries);
        SetupCanUserBeRetrievedBusinessRule(inquiries);

        // Act
        var result = await _handler.Handle(new GetPendingInquiryApprovalsRequest.Query("dep-2"), _token);

        // Assert
        result.Should().HaveCount(1);
        result[0].DepartmentId.Should().Be("dep-2");
        result[0].UserId.Should().Be(1002);
        result[0].ResourceGroupId.Should().Be(2);
    }

    [Fact]
    public async Task Handle_WhenUserHasOnlyRestrictedRoles_ShouldThrowActionNotAllowedException()
    {
        // Arrange
        _authenticatedUserContextMock.Setup(x => x.GetRoles())
            .Returns([PortalRoleNames.Employee, PortalRoleNames.SystemAdministrator]);

        // Act
        var act = () => _handler.Handle(new GetPendingInquiryApprovalsRequest.Query(), _token);

        // Assert
        await act.Should().ThrowAsync<ActionNotAllowedException>();
    }

    private void SetupDbSets(
        List<UserInquiryAcceptEntity> accepts,
        List<UserInquiryEntity> inquiries)
    {
        _dbContextMock.Setup(x => x.UserInquiryAccepts)
            .Returns(accepts.BuildMockDbSet().Object);

        _dbContextMock.Setup(x => x.UserInquiries)
            .Returns(inquiries.BuildMockDbSet().Object);
    }

    private UserInquiryEntity BuildInquiry(
        int inquiryId,
        int userId,
        DateOnly dateFrom,
        string lastName,
        string firstName,
        string? lastNameRu,
        string? firstNameRu,
        InquiryType inquiryType,
        int inquiryTemplateId = 1,
        string inquiryTemplateName = "Template",
        string? departmentId = null,
        int? resourceGroupId = null)
    {
        return new UserInquiryEntity
        {
            Id = inquiryId,
            UserId = userId,
            DateFrom = dateFrom,
            User = new InquiriesUserEntity
            {
                UserId = userId,
                FirstName = firstName,
                LastName = lastName,
                FirstNameRu = firstNameRu,
                LastNameRu = lastNameRu,
                DepartmentId = departmentId,
                ResourceGroupId = resourceGroupId,
                Email = "test@modsen-software.com"
            },
            InquiryTemplate = new InquiryTemplateEntity
            {
                Id = inquiryTemplateId, Name = inquiryTemplateName, InquiryType = inquiryType
            },
            InquiryTemplateId = inquiryTemplateId
        };
    }

    private void SetupCanUserBeRetrievedBusinessRule(ICollection<UserInquiryEntity> inquiries)
    {
        var dictionaryResult = inquiries
            .Select(x => x.UserId)
            .Distinct()
            .ToDictionary(keySelector: userId => userId, elementSelector: _ => true);

        _setUserInfoVisibilityResolverMock
            .Setup(x => x.GetVisibilityMapAsync(
                It.IsAny<int[]>(),
                It.IsAny<AccessOptions>(),
                It.IsAny<CancellationToken>())
            )
            .ReturnsAsync((int[] ids, AccessOptions _, CancellationToken __) =>
                ids.ToDictionary(x => x, x => true));

        _timeTrackContract
            .Setup(x => x.CanUsersBeRetrievedByUser(
                It.IsAny<int[]>(),
                It.IsAny<AccessOptions>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((int[] ids, AccessOptions _, CancellationToken __) =>
                ids.ToDictionary(x => x, x => true));
    }
}
