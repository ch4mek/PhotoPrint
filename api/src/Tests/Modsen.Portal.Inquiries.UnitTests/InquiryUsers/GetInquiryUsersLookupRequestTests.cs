using MockQueryable.Moq;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Inquiries.DataAccess.Npgsql;
using Modsen.Portal.Inquiries.DataAccess.Npgsql.Entities;
using Modsen.Portal.Inquiries.Requests.Inquiries.Core.Resolvers;
using Modsen.Portal.Inquiries.Requests.InquiriesPortalSettings;
using Modsen.Portal.Inquiries.Requests.InquiriesPortalSettings.CoreDto;
using Modsen.Portal.Inquiries.Requests.InquiryUsers.Requests;
using Modsen.Portal.TimeTrack.Contract;
using Moq;

namespace Modsen.Portal.Inquiries.UnitTests.InquiryUsers;

public class GetInquiryUsersLookupRequestTests
{
    private readonly Mock<IAuthenticatedUserContext> _authenticatedUserContextMock = new();
    private readonly Mock<IInquiriesDbContext> _inquiriesDbContextMock = new();
    private readonly Mock<ITimeTrackContract> _timeTrackContractMock = new();
    private readonly Mock<IInquiriesPortalSettingsAccessor> _inquiriesPortalSettingsAccessorMock = new();
    private readonly Mock<IGetInquiriesCanBeApprovedAsSubstituteResolver> _getInquiriesCanBeApprovedAsSubstituteResolverMock = new();

    private readonly GetInquiryUsersLookupRequest _handler;
    private readonly CancellationToken _token = CancellationToken.None;

    private readonly InquiriesPortalSettingsDto portalSettings = new()
    {
        SalesDepartmentId = "Sales"
    };

    public GetInquiryUsersLookupRequestTests()
    {
        _handler = new GetInquiryUsersLookupRequest(
            _authenticatedUserContextMock.Object,
            _inquiriesDbContextMock.Object,
            _timeTrackContractMock.Object,
            _inquiriesPortalSettingsAccessorMock.Object,
            _getInquiriesCanBeApprovedAsSubstituteResolverMock.Object);

        _getInquiriesCanBeApprovedAsSubstituteResolverMock
            .Setup(x => x.ResolveAsync(It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);
        _inquiriesDbContextMock
            .Setup(x => x.UserInquiries)
            .Returns(new List<UserInquiryEntity>().BuildMockDbSet().Object);
    }

    [Fact]
    public async Task HandleWhenIsApprovalsTrueAndHeadCanManageAllRequestsAndSettingsReturnsAllUsers()
    {
        //Arrange
        const string departmentId = "Net";

        _authenticatedUserContextMock
            .Setup(context => context.HasClaim(RequestsClaimTypeNames.ManageRequests.Name))
            .Returns(true);

        _authenticatedUserContextMock
            .Setup(context => context.HasClaim(RequestsClaimTypeNames.ManageAllRequests.Name))
            .Returns(true);

        _authenticatedUserContextMock
            .Setup(context => context.HasClaim(AbsencesClaimTypeNames.ManageAbsenceSettings.Name))
            .Returns(true);

        _authenticatedUserContextMock
            .Setup(x => x.GetRoles())
            .Returns([PortalRoleNames.HeadOfDepartment]);

        _authenticatedUserContextMock
            .Setup(context => context.DepartmentId)
            .Returns(departmentId);

        _inquiriesDbContextMock
            .Setup(context => context.InquiriesUsers)
            .Returns(new List<InquiriesUserEntity>
            {
                new() { UserId = 1, IsActive = true, DepartmentId = departmentId , Email = "test",
                    FirstName = "test",
                    LastName = "test",},
                new() { UserId = 2, IsActive = true, DepartmentId = "AnotherDept", Email = "test",
                    FirstName = "test",
                    LastName = "test", }
            }.BuildMockDbSet().Object);

        var query = new GetInquiryUsersLookupRequest.Query(IsApprovals: true);

        //Act
        var result = await _handler.Handle(query, _token);

        //Assert
        result.Count.Should().Be(2);
        result[0].Id.Should().Be(1);
        result[1].Id.Should().Be(2);

    }

    [Fact]
    public async Task HandleWhenIsApprovalsFalseAndHeadCanManageAllRequestsAndSettingsReturnsAllUsers()
    {
        //Arrange
        const string departmentId = "Net";

        _authenticatedUserContextMock
            .Setup(context => context.HasClaim(RequestsClaimTypeNames.ManageRequests.Name))
            .Returns(true);

        _authenticatedUserContextMock
            .Setup(context => context.HasClaim(RequestsClaimTypeNames.ManageAllRequests.Name))
            .Returns(true);

        _authenticatedUserContextMock
            .Setup(context => context.HasClaim(AbsencesClaimTypeNames.ManageAbsenceSettings.Name))
            .Returns(true);

        _authenticatedUserContextMock
            .Setup(x => x.GetRoles())
            .Returns([PortalRoleNames.HeadOfDepartment]);

        _authenticatedUserContextMock
            .Setup(context => context.DepartmentId)
            .Returns(departmentId);

        _inquiriesDbContextMock
            .Setup(context => context.InquiriesUsers)
            .Returns(new List<InquiriesUserEntity>
            {
                new() { UserId = 1, IsActive = true, DepartmentId = departmentId , Email = "test",
                    FirstName = "test",
                    LastName = "test",},
                new() { UserId = 2, IsActive = true, DepartmentId = "AnotherDept", Email = "test",
                    FirstName = "test",
                    LastName = "test", }
            }.BuildMockDbSet().Object);

        var query = new GetInquiryUsersLookupRequest.Query(IsApprovals: false);

        //Act
        var result = await _handler.Handle(query, _token);

        //Assert
        result.Count.Should().Be(2);
        result[0].Id.Should().Be(1);
        result[1].Id.Should().Be(2);

    }

    [Fact]
    public async Task HandleWhenIsApprovalsTrueAndHeadCanManageRequestsAndSettingsReturnsAllUsers()
    {
        //Arrange
        const string departmentId = "Net";

        _authenticatedUserContextMock
            .Setup(context => context.HasClaim(RequestsClaimTypeNames.ManageRequests.Name))
            .Returns(true);

        _authenticatedUserContextMock
            .Setup(context => context.HasClaim(RequestsClaimTypeNames.ManageAllRequests.Name))
            .Returns(false);

        _authenticatedUserContextMock
            .Setup(context => context.HasClaim(AbsencesClaimTypeNames.ManageAbsenceSettings.Name))
            .Returns(true);

        _authenticatedUserContextMock
            .Setup(x => x.GetRoles())
            .Returns([PortalRoleNames.HeadOfDepartment]);

        _authenticatedUserContextMock
            .Setup(context => context.DepartmentId)
            .Returns(departmentId);

        _inquiriesDbContextMock
            .Setup(context => context.InquiriesUsers)
            .Returns(new List<InquiriesUserEntity>
            {
                new() { UserId = 1, IsActive = true, DepartmentId = departmentId , Email = "test",
                    FirstName = "test",
                    LastName = "test",},
                new() { UserId = 2, IsActive = true, DepartmentId = "AnotherDept", Email = "test",
                    FirstName = "test",
                    LastName = "test", }
            }.BuildMockDbSet().Object);

        var query = new GetInquiryUsersLookupRequest.Query(IsApprovals: true);

        //Act
        var result = await _handler.Handle(query, _token);

        //Assert
        result.Count.Should().Be(2);
        result[0].Id.Should().Be(1);
        result[1].Id.Should().Be(2);

    }

    [Fact]
    public async Task HandleWhenIsApprovalsFalseAndHeadCanManageRequestsAndSettingsReturnsOnlySubordinals()
    {
        //Arrange
        const string departmentId = "Net";

        _authenticatedUserContextMock
            .Setup(context => context.HasClaim(RequestsClaimTypeNames.ManageRequests.Name))
            .Returns(true);

        _authenticatedUserContextMock
            .Setup(context => context.HasClaim(RequestsClaimTypeNames.ManageAllRequests.Name))
            .Returns(false);

        _authenticatedUserContextMock
            .Setup(context => context.HasClaim(AbsencesClaimTypeNames.ManageAbsenceSettings.Name))
            .Returns(true);

        _authenticatedUserContextMock
            .Setup(x => x.GetRoles())
            .Returns([PortalRoleNames.HeadOfDepartment]);

        _authenticatedUserContextMock
            .Setup(context => context.DepartmentId)
            .Returns(departmentId);

        _inquiriesDbContextMock
            .Setup(context => context.InquiriesUsers)
            .Returns(new List<InquiriesUserEntity>
            {
                new() { UserId = 1, IsActive = true, DepartmentId = departmentId , Email = "test",
                    FirstName = "test",
                    LastName = "test",},
                new() { UserId = 2, IsActive = true, DepartmentId = "AnotherDept", Email = "test",
                    FirstName = "test",
                    LastName = "test", }
            }.BuildMockDbSet().Object);

        _inquiriesPortalSettingsAccessorMock.Setup(x => x.GetSettings(It.IsAny<CancellationToken>())).ReturnsAsync(portalSettings);

        _timeTrackContractMock.Setup(x => x.GetUsersBySalesManagerId(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        var query = new GetInquiryUsersLookupRequest.Query(IsApprovals: false);

        //Act
        var result = await _handler.Handle(query, _token);

        //Assert
        result.Count.Should().Be(1);
        result.FirstOrDefault()!.Id.Should().Be(1);

    }

    [Fact]
    public async Task HandleWhenIsApprovalsTrueAndHeadCanManageRequestsReturnsOnlySubordinals()
    {
        //Arrange
        const string departmentId = "Net";

        _authenticatedUserContextMock
            .Setup(context => context.HasClaim(RequestsClaimTypeNames.ManageRequests.Name))
            .Returns(true);

        _authenticatedUserContextMock
            .Setup(context => context.HasClaim(RequestsClaimTypeNames.ManageAllRequests.Name))
            .Returns(false);

        _authenticatedUserContextMock
            .Setup(context => context.HasClaim(AbsencesClaimTypeNames.ManageAbsenceSettings.Name))
            .Returns(false);

        _authenticatedUserContextMock
            .Setup(x => x.GetRoles())
            .Returns([PortalRoleNames.HeadOfDepartment]);

        _authenticatedUserContextMock
            .Setup(context => context.DepartmentId)
            .Returns(departmentId);

        _inquiriesDbContextMock
            .Setup(context => context.InquiriesUsers)
            .Returns(new List<InquiriesUserEntity>
            {
                new() { UserId = 1, IsActive = true, DepartmentId = departmentId , Email = "test",
                    FirstName = "test",
                    LastName = "test",},
                new() { UserId = 2, IsActive = true, DepartmentId = "AnotherDept", Email = "test",
                    FirstName = "test",
                    LastName = "test", }
            }.BuildMockDbSet().Object);

        _inquiriesPortalSettingsAccessorMock.Setup(x => x.GetSettings(It.IsAny<CancellationToken>())).ReturnsAsync(portalSettings);

        _timeTrackContractMock.Setup(x => x.GetUsersBySalesManagerId(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        var query = new GetInquiryUsersLookupRequest.Query(IsApprovals: true);

        //Act
        var result = await _handler.Handle(query, _token);

        //Assert
        result.Count.Should().Be(1);
        result.FirstOrDefault()!.Id.Should().Be(1);

    }

    [Fact]
    public async Task HandleWhenIsApprovalsFalseAndHeadCanManageRequestsReturnsOnlySubordinals()
    {
        //Arrange
        const string departmentId = "Net";

        _authenticatedUserContextMock
            .Setup(context => context.HasClaim(RequestsClaimTypeNames.ManageRequests.Name))
            .Returns(true);

        _authenticatedUserContextMock
            .Setup(context => context.HasClaim(RequestsClaimTypeNames.ManageAllRequests.Name))
            .Returns(false);

        _authenticatedUserContextMock
            .Setup(context => context.HasClaim(AbsencesClaimTypeNames.ManageAbsenceSettings.Name))
            .Returns(false);

        _authenticatedUserContextMock
            .Setup(x => x.GetRoles())
            .Returns([PortalRoleNames.HeadOfDepartment]);

        _authenticatedUserContextMock
            .Setup(context => context.DepartmentId)
            .Returns(departmentId);

        _inquiriesDbContextMock
            .Setup(context => context.InquiriesUsers)
            .Returns(new List<InquiriesUserEntity>
            {
                new() { UserId = 1, IsActive = true, DepartmentId = departmentId , Email = "test",
                    FirstName = "test",
                    LastName = "test",},
                new() { UserId = 2, IsActive = true, DepartmentId = "AnotherDept", Email = "test",
                    FirstName = "test",
                    LastName = "test", }
            }.BuildMockDbSet().Object);

        _inquiriesPortalSettingsAccessorMock.Setup(x => x.GetSettings(It.IsAny<CancellationToken>())).ReturnsAsync(portalSettings);

        _timeTrackContractMock.Setup(x => x.GetUsersBySalesManagerId(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        var query = new GetInquiryUsersLookupRequest.Query(IsApprovals: false);

        //Act
        var result = await _handler.Handle(query, _token);

        //Assert
        result.Count.Should().Be(1);
        result.FirstOrDefault()!.Id.Should().Be(1);

    }

    [Fact]
    public async Task HandleWhenIsApprovalsFalseAndHeadCanManageAllRequestsReturnsAllUsers()
    {
        //Arrange
        const string departmentId = "Net";

        _authenticatedUserContextMock
            .Setup(context => context.HasClaim(RequestsClaimTypeNames.ManageRequests.Name))
            .Returns(true);

        _authenticatedUserContextMock
            .Setup(context => context.HasClaim(RequestsClaimTypeNames.ManageAllRequests.Name))
            .Returns(true);

        _authenticatedUserContextMock
            .Setup(context => context.HasClaim(AbsencesClaimTypeNames.ManageAbsenceSettings.Name))
            .Returns(false);

        _authenticatedUserContextMock
            .Setup(x => x.GetRoles())
            .Returns([PortalRoleNames.HeadOfDepartment]);

        _authenticatedUserContextMock
            .Setup(context => context.DepartmentId)
            .Returns(departmentId);

        _inquiriesDbContextMock
            .Setup(context => context.InquiriesUsers)
            .Returns(new List<InquiriesUserEntity>
            {
                new() { UserId = 1, IsActive = true, DepartmentId = departmentId , Email = "test",
                    FirstName = "test",
                    LastName = "test",},
                new() { UserId = 2, IsActive = true, DepartmentId = "AnotherDept", Email = "test",
                    FirstName = "test",
                    LastName = "test", }
            }.BuildMockDbSet().Object);

        var query = new GetInquiryUsersLookupRequest.Query(IsApprovals: false);

        //Act
        var result = await _handler.Handle(query, _token);

        //Assert
        result.Count.Should().Be(2);
        result[0].Id.Should().Be(1);
        result[1].Id.Should().Be(2);

    }

    [Fact]
    public async Task HandleWhenIsApprovalsTrueAndHeadCanManageAllRequestsReturnsAllUsers()
    {
        //Arrange
        const string departmentId = "Net";

        _authenticatedUserContextMock
            .Setup(context => context.HasClaim(RequestsClaimTypeNames.ManageRequests.Name))
            .Returns(true);

        _authenticatedUserContextMock
            .Setup(context => context.HasClaim(RequestsClaimTypeNames.ManageAllRequests.Name))
            .Returns(true);

        _authenticatedUserContextMock
            .Setup(context => context.HasClaim(AbsencesClaimTypeNames.ManageAbsenceSettings.Name))
            .Returns(false);

        _authenticatedUserContextMock
            .Setup(x => x.GetRoles())
            .Returns([PortalRoleNames.HeadOfDepartment]);

        _authenticatedUserContextMock
            .Setup(context => context.DepartmentId)
            .Returns(departmentId);

        _inquiriesDbContextMock
            .Setup(context => context.InquiriesUsers)
            .Returns(new List<InquiriesUserEntity>
            {
                new() { UserId = 1, IsActive = true, DepartmentId = departmentId , Email = "test",
                    FirstName = "test",
                    LastName = "test",},
                new() { UserId = 2, IsActive = true, DepartmentId = "AnotherDept", Email = "test",
                    FirstName = "test",
                    LastName = "test", }
            }.BuildMockDbSet().Object);

        var query = new GetInquiryUsersLookupRequest.Query(IsApprovals: true);

        //Act
        var result = await _handler.Handle(query, _token);

        //Assert
        result.Count.Should().Be(2);
        result[0].Id.Should().Be(1);
        result[1].Id.Should().Be(2);

    }

    [Theory]
    [InlineData(true, true, true, PortalRoleNames.Ceo)]
    [InlineData(true, true, false, PortalRoleNames.Ceo)]
    [InlineData(true, false, false, PortalRoleNames.Ceo)]
    [InlineData(true, false, true, PortalRoleNames.Ceo)]
    [InlineData(true, true, true, PortalRoleNames.Cto)]
    [InlineData(true, true, false, PortalRoleNames.Cto)]
    [InlineData(true, false, false, PortalRoleNames.Cto)]
    [InlineData(true, false, true, PortalRoleNames.Cto)]
    public async Task HandleWhenIsApprovalsCeoOrCtoReturnsAllUsers(
        bool canManageRequests,
        bool canManageAllRequests,
        bool canManageAbsenceSettings,
        string role)
    {
        //Arrange
        const string departmentId = "Net";

        _authenticatedUserContextMock
            .Setup(context => context.HasClaim(RequestsClaimTypeNames.ManageRequests.Name))
            .Returns(canManageRequests);

        _authenticatedUserContextMock
            .Setup(context => context.HasClaim(RequestsClaimTypeNames.ManageAllRequests.Name))
            .Returns(canManageAllRequests);

        _authenticatedUserContextMock
            .Setup(context => context.HasClaim(AbsencesClaimTypeNames.ManageAbsenceSettings.Name))
            .Returns(canManageAbsenceSettings);

        _authenticatedUserContextMock
            .Setup(x => x.GetRoles())
            .Returns([role]);

        _inquiriesDbContextMock
            .Setup(context => context.InquiriesUsers)
            .Returns(new List<InquiriesUserEntity>
            {
                new() { UserId = 1, IsActive = true, DepartmentId = departmentId , Email = "test",
                    FirstName = "test",
                    LastName = "test",},
                new() { UserId = 2, IsActive = true, DepartmentId = "AnotherDept", Email = "test",
                    FirstName = "test",
                    LastName = "test", }
            }.BuildMockDbSet().Object);

        _inquiriesPortalSettingsAccessorMock.Setup(x => x.GetSettings(It.IsAny<CancellationToken>())).ReturnsAsync(portalSettings);

        _timeTrackContractMock.Setup(x => x.GetUsersBySalesManagerId(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        var query = new GetInquiryUsersLookupRequest.Query(IsApprovals: true);

        //Act
        var result = await _handler.Handle(query, _token);

        //Assert
        result.Count.Should().Be(2);
        result[0].Id.Should().Be(1);
        result[1].Id.Should().Be(2);

    }

    [Fact]
    public async Task HandleWhenIsApprovalsTrueAndGroupManagerCanManageAllRequestsAndSettingsReturnsAllUsers()
    {
        //Arrange
        const int resourceGroupId = 1;

        _authenticatedUserContextMock
            .Setup(context => context.HasClaim(RequestsClaimTypeNames.ManageRequests.Name))
            .Returns(true);

        _authenticatedUserContextMock
            .Setup(context => context.HasClaim(RequestsClaimTypeNames.ManageAllRequests.Name))
            .Returns(true);

        _authenticatedUserContextMock
            .Setup(context => context.HasClaim(AbsencesClaimTypeNames.ManageAbsenceSettings.Name))
            .Returns(true);

        _authenticatedUserContextMock
            .Setup(x => x.GetRoles())
            .Returns([PortalRoleNames.GroupManager]);

        _authenticatedUserContextMock
            .Setup(context => context.ResourceGroupId)
            .Returns(resourceGroupId);

        _inquiriesDbContextMock
            .Setup(context => context.InquiriesUsers)
            .Returns(new List<InquiriesUserEntity>
            {
                new() { UserId = 1, IsActive = true, ResourceGroupId = 1, Email = "test",
                    FirstName = "test",
                    LastName = "test", },
                new() { UserId = 2, IsActive = true, ResourceGroupId = 2, Email = "test",
                    FirstName = "test",
                    LastName = "test", }
            }.BuildMockDbSet().Object);

        var query = new GetInquiryUsersLookupRequest.Query(IsApprovals: true);

        //Act
        var result = await _handler.Handle(query, _token);

        //Assert
        result.Count.Should().Be(2);
        result[0].Id.Should().Be(1);
        result[1].Id.Should().Be(2);

    }

    [Fact]
    public async Task HandleWhenIsApprovalsFalseAndGroupManagerCanManageAllRequestsAndSettingsReturnsAllUsers()
    {
        //Arrange
        const int resourceGroupId = 1;

        _authenticatedUserContextMock
            .Setup(context => context.HasClaim(RequestsClaimTypeNames.ManageRequests.Name))
            .Returns(true);

        _authenticatedUserContextMock
            .Setup(context => context.HasClaim(RequestsClaimTypeNames.ManageAllRequests.Name))
            .Returns(true);

        _authenticatedUserContextMock
            .Setup(context => context.HasClaim(AbsencesClaimTypeNames.ManageAbsenceSettings.Name))
            .Returns(true);

        _authenticatedUserContextMock
            .Setup(x => x.GetRoles())
            .Returns([PortalRoleNames.GroupManager]);

        _authenticatedUserContextMock
            .Setup(context => context.ResourceGroupId)
            .Returns(resourceGroupId);

        _inquiriesDbContextMock
            .Setup(context => context.InquiriesUsers)
            .Returns(new List<InquiriesUserEntity>
            {
                new() { UserId = 1, IsActive = true, ResourceGroupId = 1 , Email = "test",
                    FirstName = "test",
                    LastName = "test",},
                new() { UserId = 2, IsActive = true, ResourceGroupId = 2 , Email = "test",
                    FirstName = "test",
                    LastName = "test",}
            }.BuildMockDbSet().Object);

        var query = new GetInquiryUsersLookupRequest.Query(IsApprovals: false);

        //Act
        var result = await _handler.Handle(query, _token);

        //Assert
        result.Count.Should().Be(2);
        result[0].Id.Should().Be(1);
        result[1].Id.Should().Be(2);

    }

    [Fact]
    public async Task HandleWhenIsApprovalsTrueAndGroupManagerCanManageRequestsAndSettingsReturnsAllUsers()
    {
        //Arrange
        const int resourceGroupId = 1;

        _authenticatedUserContextMock
            .Setup(context => context.HasClaim(RequestsClaimTypeNames.ManageRequests.Name))
            .Returns(true);

        _authenticatedUserContextMock
            .Setup(context => context.HasClaim(RequestsClaimTypeNames.ManageAllRequests.Name))
            .Returns(false);

        _authenticatedUserContextMock
            .Setup(context => context.HasClaim(AbsencesClaimTypeNames.ManageAbsenceSettings.Name))
            .Returns(true);

        _authenticatedUserContextMock
            .Setup(x => x.GetRoles())
            .Returns([PortalRoleNames.GroupManager]);

        _authenticatedUserContextMock
            .Setup(context => context.ResourceGroupId)
            .Returns(resourceGroupId);

        _inquiriesDbContextMock
            .Setup(context => context.InquiriesUsers)
            .Returns(new List<InquiriesUserEntity>
            {
                new() { UserId = 1, IsActive = true, ResourceGroupId = 1 , Email = "test",
                    FirstName = "test",
                    LastName = "test",},
                new() { UserId = 2, IsActive = true, ResourceGroupId = 2 , Email = "test",
                    FirstName = "test",
                    LastName = "test",}
            }.BuildMockDbSet().Object);

        var query = new GetInquiryUsersLookupRequest.Query(IsApprovals: true);

        //Act
        var result = await _handler.Handle(query, _token);

        //Assert
        result.Count.Should().Be(2);
        result[0].Id.Should().Be(1);
        result[1].Id.Should().Be(2);

    }

    [Fact]
    public async Task HandleWhenIsApprovalsFalseAndGroupManagerCanManageRequestsAndSettingsReturnsOnlySubordinals()
    {
        //Arrange
        const int resourceGroupId = 1;

        _authenticatedUserContextMock
            .Setup(context => context.HasClaim(RequestsClaimTypeNames.ManageRequests.Name))
            .Returns(true);

        _authenticatedUserContextMock
            .Setup(context => context.HasClaim(RequestsClaimTypeNames.ManageAllRequests.Name))
            .Returns(false);

        _authenticatedUserContextMock
            .Setup(context => context.HasClaim(AbsencesClaimTypeNames.ManageAbsenceSettings.Name))
            .Returns(true);

        _authenticatedUserContextMock
            .Setup(x => x.GetRoles())
            .Returns([PortalRoleNames.GroupManager]);

        _authenticatedUserContextMock
            .Setup(context => context.ResourceGroupId)
            .Returns(resourceGroupId);

        _inquiriesDbContextMock
            .Setup(context => context.InquiriesUsers)
            .Returns(new List<InquiriesUserEntity>
            {
                new() { UserId = 1, IsActive = true, ResourceGroupId = 1, Email = "test",
                    FirstName = "test",
                    LastName = "test", },
                new() { UserId = 2, IsActive = true, ResourceGroupId = 2, Email = "test",
                    FirstName = "test",
                    LastName = "test", }
            }.BuildMockDbSet().Object);

        _inquiriesPortalSettingsAccessorMock.Setup(x => x.GetSettings(It.IsAny<CancellationToken>())).ReturnsAsync(portalSettings);

        _timeTrackContractMock.Setup(x => x.GetUsersBySalesManagerId(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        var query = new GetInquiryUsersLookupRequest.Query(IsApprovals: false);

        //Act
        var result = await _handler.Handle(query, _token);

        //Assert
        result.Count.Should().Be(1);
        result.FirstOrDefault()!.Id.Should().Be(1);

    }

    [Fact]
    public async Task HandleWhenIsApprovalsTrueAndGroupManagerCanManageRequestsReturnsOnlySubordinals()
    {
        //Arrange
        const int resourceGroupId = 1;

        _authenticatedUserContextMock
            .Setup(context => context.HasClaim(RequestsClaimTypeNames.ManageRequests.Name))
            .Returns(true);

        _authenticatedUserContextMock
            .Setup(context => context.HasClaim(RequestsClaimTypeNames.ManageAllRequests.Name))
            .Returns(false);

        _authenticatedUserContextMock
            .Setup(context => context.HasClaim(AbsencesClaimTypeNames.ManageAbsenceSettings.Name))
            .Returns(false);

        _authenticatedUserContextMock
            .Setup(x => x.GetRoles())
            .Returns([PortalRoleNames.GroupManager]);

        _authenticatedUserContextMock
            .Setup(context => context.ResourceGroupId)
            .Returns(resourceGroupId);

        _inquiriesDbContextMock
            .Setup(context => context.InquiriesUsers)
            .Returns(new List<InquiriesUserEntity>
            {
                new() { UserId = 1, IsActive = true, ResourceGroupId = 1, Email = "test",
                    FirstName = "test",
                    LastName = "test",},
                new() { UserId = 2, IsActive = true, ResourceGroupId = 2, Email = "test",
                    FirstName = "test",
                    LastName = "test",}
            }.BuildMockDbSet().Object);

        _inquiriesPortalSettingsAccessorMock.Setup(x => x.GetSettings(It.IsAny<CancellationToken>())).ReturnsAsync(portalSettings);

        _timeTrackContractMock.Setup(x => x.GetUsersBySalesManagerId(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        var query = new GetInquiryUsersLookupRequest.Query(IsApprovals: true);

        //Act
        var result = await _handler.Handle(query, _token);

        //Assert
        result.Count.Should().Be(1);
        result.FirstOrDefault()!.Id.Should().Be(1);

    }

    [Fact]
    public async Task HandleWhenIsApprovalsFalseAndGroupManagerCanManageRequestsReturnsOnlySubordinals()
    {
        //Arrange
        const int resourceGroupId = 1;

        _authenticatedUserContextMock
            .Setup(context => context.HasClaim(RequestsClaimTypeNames.ManageRequests.Name))
            .Returns(true);

        _authenticatedUserContextMock
            .Setup(context => context.HasClaim(RequestsClaimTypeNames.ManageAllRequests.Name))
            .Returns(false);

        _authenticatedUserContextMock
            .Setup(context => context.HasClaim(AbsencesClaimTypeNames.ManageAbsenceSettings.Name))
            .Returns(false);

        _authenticatedUserContextMock
            .Setup(x => x.GetRoles())
            .Returns([PortalRoleNames.GroupManager]);

        _authenticatedUserContextMock
            .Setup(context => context.ResourceGroupId)
            .Returns(resourceGroupId);

        _inquiriesDbContextMock
            .Setup(context => context.InquiriesUsers)
            .Returns(new List<InquiriesUserEntity>
            {
                new() { UserId = 1, IsActive = true, ResourceGroupId = 1, Email = "test",
                    FirstName = "test",
                    LastName = "test", },
                new() { UserId = 2, IsActive = true, ResourceGroupId = 2, Email = "test",
                    FirstName = "test",
                    LastName = "test", }
            }.BuildMockDbSet().Object);

        _inquiriesPortalSettingsAccessorMock.Setup(x => x.GetSettings(It.IsAny<CancellationToken>())).ReturnsAsync(portalSettings);

        _timeTrackContractMock.Setup(x => x.GetUsersBySalesManagerId(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        var query = new GetInquiryUsersLookupRequest.Query(IsApprovals: false);

        //Act
        var result = await _handler.Handle(query, _token);

        //Assert
        result.Count.Should().Be(1);
        result.FirstOrDefault()!.Id.Should().Be(1);

    }

    [Fact]
    public async Task HandleWhenIsApprovalsFalseAndGroupManagerCanManageAllRequestsReturnsAllUsers()
    {
        //Arrange
        const int resourceGroupId = 1;

        _authenticatedUserContextMock
            .Setup(context => context.HasClaim(RequestsClaimTypeNames.ManageRequests.Name))
            .Returns(true);

        _authenticatedUserContextMock
            .Setup(context => context.HasClaim(RequestsClaimTypeNames.ManageAllRequests.Name))
            .Returns(true);

        _authenticatedUserContextMock
            .Setup(context => context.HasClaim(AbsencesClaimTypeNames.ManageAbsenceSettings.Name))
            .Returns(false);

        _authenticatedUserContextMock
            .Setup(x => x.GetRoles())
            .Returns([PortalRoleNames.GroupManager]);

        _authenticatedUserContextMock
            .Setup(context => context.ResourceGroupId)
            .Returns(resourceGroupId);

        _inquiriesDbContextMock
            .Setup(context => context.InquiriesUsers)
            .Returns(new List<InquiriesUserEntity>
            {
                new() { UserId = 1, IsActive = true, ResourceGroupId = 1, Email = "test",
                    FirstName = "test",
                    LastName = "test", },
                new() { UserId = 2, IsActive = true, ResourceGroupId = 2, Email = "test",
                    FirstName = "test",
                    LastName = "test", }
            }.BuildMockDbSet().Object);

        var query = new GetInquiryUsersLookupRequest.Query(IsApprovals: false);

        //Act
        var result = await _handler.Handle(query, _token);

        //Assert
        result.Count.Should().Be(2);
        result[0].Id.Should().Be(1);
        result[1].Id.Should().Be(2);

    }

    [Fact]
    public async Task HandleWhenIsApprovalsTrueAndGroupManagerCanManageAllRequestsReturnsAllUsers()
    {
        //Arrange
        const int resourceGroupId = 1;

        _authenticatedUserContextMock
            .Setup(context => context.HasClaim(RequestsClaimTypeNames.ManageRequests.Name))
            .Returns(true);

        _authenticatedUserContextMock
            .Setup(context => context.HasClaim(RequestsClaimTypeNames.ManageAllRequests.Name))
            .Returns(true);

        _authenticatedUserContextMock
            .Setup(context => context.HasClaim(AbsencesClaimTypeNames.ManageAbsenceSettings.Name))
            .Returns(false);

        _authenticatedUserContextMock
            .Setup(x => x.GetRoles())
            .Returns([PortalRoleNames.GroupManager]);

        _authenticatedUserContextMock
            .Setup(context => context.ResourceGroupId)
            .Returns(resourceGroupId);

        _inquiriesDbContextMock
            .Setup(context => context.InquiriesUsers)
            .Returns(new List<InquiriesUserEntity>
            {
                new() { UserId = 1, IsActive = true, ResourceGroupId = 1, Email = "test",
                    FirstName = "test",
                    LastName = "test", },
                new() { UserId = 2, IsActive = true, ResourceGroupId = 2, Email = "test",
                    FirstName = "test",
                    LastName = "test", }
            }.BuildMockDbSet().Object);

        var query = new GetInquiryUsersLookupRequest.Query(IsApprovals: true);

        //Act
        var result = await _handler.Handle(query, _token);

        //Assert
        result.Count.Should().Be(2);
        result[0].Id.Should().Be(1);
        result[1].Id.Should().Be(2);

    }

    [Fact]
    public async Task HandleWhenIsApprovalsTrueAndEmployeeCanManageAllRequestsAndSettingsReturnsAllUsers()
    {
        //Arrange
        _authenticatedUserContextMock
            .Setup(context => context.HasClaim(RequestsClaimTypeNames.ManageRequests.Name))
            .Returns(true);

        _authenticatedUserContextMock
            .Setup(context => context.HasClaim(RequestsClaimTypeNames.ManageAllRequests.Name))
            .Returns(true);

        _authenticatedUserContextMock
            .Setup(context => context.HasClaim(AbsencesClaimTypeNames.ManageAbsenceSettings.Name))
            .Returns(true);

        _authenticatedUserContextMock
            .Setup(x => x.GetRoles())
            .Returns([PortalRoleNames.Employee]);

        const int userId = 1;

        _authenticatedUserContextMock
            .Setup(context => context.UserId)
            .Returns(userId);

        _inquiriesDbContextMock
            .Setup(context => context.InquiriesUsers)
            .Returns(new List<InquiriesUserEntity>
            {
                new() { UserId = userId, IsActive = true, Email = "test",
                    FirstName = "test",
                    LastName = "test", },
                new() { UserId = 2, IsActive = true     , Email = "test",
                    FirstName = "test",
                    LastName = "test",  }
            }.BuildMockDbSet().Object);

        var query = new GetInquiryUsersLookupRequest.Query(IsApprovals: true);

        //Act
        var result = await _handler.Handle(query, _token);

        //Assert
        result.Count.Should().Be(2);
        result[0].Id.Should().Be(1);
        result[1].Id.Should().Be(2);

    }

    [Fact]
    public async Task HandleWhenIsApprovalsFalseAndEmployeeCanManageAllRequestsAndSettingsReturnsAllUsers()
    {
        //Arrange
        _authenticatedUserContextMock
            .Setup(context => context.HasClaim(RequestsClaimTypeNames.ManageRequests.Name))
            .Returns(true);

        _authenticatedUserContextMock
            .Setup(context => context.HasClaim(RequestsClaimTypeNames.ManageAllRequests.Name))
            .Returns(true);

        _authenticatedUserContextMock
            .Setup(context => context.HasClaim(AbsencesClaimTypeNames.ManageAbsenceSettings.Name))
            .Returns(true);

        _authenticatedUserContextMock
            .Setup(x => x.GetRoles())
            .Returns([PortalRoleNames.Employee]);

        const int userId = 1;

        _authenticatedUserContextMock
            .Setup(context => context.UserId)
            .Returns(userId);

        _inquiriesDbContextMock
            .Setup(context => context.InquiriesUsers)
            .Returns(new List<InquiriesUserEntity>
            {
                new() { UserId = userId, IsActive = true, Email = "test",
                    FirstName = "test",
                    LastName = "test", },
                new() { UserId = 2, IsActive = true     , Email = "test",
                    FirstName = "test",
                    LastName = "test",  }
            }.BuildMockDbSet().Object);

        var query = new GetInquiryUsersLookupRequest.Query(IsApprovals: false);

        //Act
        var result = await _handler.Handle(query, _token);

        //Assert
        result.Count.Should().Be(2);
        result[0].Id.Should().Be(1);
        result[1].Id.Should().Be(2);

    }

    [Fact]
    public async Task HandleWhenIsApprovalsTrueAndEmployeeCanManageRequestsAndSettingsReturnsAllUsers()
    {
        //Arrange
        _authenticatedUserContextMock
            .Setup(context => context.HasClaim(RequestsClaimTypeNames.ManageRequests.Name))
            .Returns(true);

        _authenticatedUserContextMock
            .Setup(context => context.HasClaim(RequestsClaimTypeNames.ManageAllRequests.Name))
            .Returns(false);

        _authenticatedUserContextMock
            .Setup(context => context.HasClaim(AbsencesClaimTypeNames.ManageAbsenceSettings.Name))
            .Returns(true);

        _authenticatedUserContextMock
            .Setup(x => x.GetRoles())
            .Returns([PortalRoleNames.Employee]);

        const int userId = 1;

        _authenticatedUserContextMock
            .Setup(context => context.UserId)
            .Returns(userId);

        _inquiriesDbContextMock
            .Setup(context => context.InquiriesUsers)
            .Returns(new List<InquiriesUserEntity>
            {
                new() { UserId = userId, IsActive = true, Email = "test",
                    FirstName = "test",
                    LastName = "test", },
                new() { UserId = 2, IsActive = true     , Email = "test",
                    FirstName = "test",
                    LastName = "test", }
            }.BuildMockDbSet().Object);

        var query = new GetInquiryUsersLookupRequest.Query(IsApprovals: true);

        //Act
        var result = await _handler.Handle(query, _token);

        //Assert
        result.Count.Should().Be(2);
        result[0].Id.Should().Be(1);
        result[1].Id.Should().Be(2);

    }

    [Fact]
    public async Task HandleWhenIsApprovalsFalseAndEmployeeCanManageRequestsAndSettingsReturnsOnlyEmployee()
    {
        //Arrange

        _authenticatedUserContextMock
            .Setup(context => context.HasClaim(RequestsClaimTypeNames.ManageRequests.Name))
            .Returns(true);

        _authenticatedUserContextMock
            .Setup(context => context.HasClaim(RequestsClaimTypeNames.ManageAllRequests.Name))
            .Returns(false);

        _authenticatedUserContextMock
            .Setup(context => context.HasClaim(AbsencesClaimTypeNames.ManageAbsenceSettings.Name))
            .Returns(true);

        _authenticatedUserContextMock
            .Setup(x => x.GetRoles())
            .Returns([PortalRoleNames.Employee]);

        const int userId = 1;

        _authenticatedUserContextMock
            .Setup(context => context.UserId)
            .Returns(userId);

        _inquiriesDbContextMock
            .Setup(context => context.InquiriesUsers)
            .Returns(new List<InquiriesUserEntity>
            {
                new() { UserId = userId, IsActive = true, Email = "test",
                    FirstName = "test",
                    LastName = "test", },
                new() { UserId = 2, IsActive = true     , Email = "test",
                    FirstName = "test",
                    LastName = "test",  }
            }.BuildMockDbSet().Object);

        _inquiriesPortalSettingsAccessorMock.Setup(x => x.GetSettings(It.IsAny<CancellationToken>())).ReturnsAsync(portalSettings);

        _timeTrackContractMock.Setup(x => x.GetUsersBySalesManagerId(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        var query = new GetInquiryUsersLookupRequest.Query(IsApprovals: false);

        //Act
        var result = await _handler.Handle(query, _token);

        //Assert
        result.Count.Should().Be(1);
        result.FirstOrDefault()!.Id.Should().Be(1);

    }

    [Fact]
    public async Task HandleWhenIsApprovalsTrueAndEmployeeCanManageRequestsReturnsOnlyEmployee()
    {
        //Arrange
        _authenticatedUserContextMock
            .Setup(context => context.HasClaim(RequestsClaimTypeNames.ManageRequests.Name))
            .Returns(true);

        _authenticatedUserContextMock
            .Setup(context => context.HasClaim(RequestsClaimTypeNames.ManageAllRequests.Name))
            .Returns(false);

        _authenticatedUserContextMock
            .Setup(context => context.HasClaim(AbsencesClaimTypeNames.ManageAbsenceSettings.Name))
            .Returns(false);

        _authenticatedUserContextMock
            .Setup(x => x.GetRoles())
            .Returns([PortalRoleNames.Employee]);

        const int userId = 1;

        _authenticatedUserContextMock
            .Setup(context => context.UserId)
            .Returns(userId);

        _inquiriesDbContextMock
            .Setup(context => context.InquiriesUsers)
            .Returns(new List<InquiriesUserEntity>
            {
                new() { UserId = userId, IsActive = true, Email = "test",
                    FirstName = "test",
                    LastName = "test", },
                new() { UserId = 2, IsActive = true     , Email = "test",
                    FirstName = "test",
                    LastName = "test",  }
            }.BuildMockDbSet().Object);

        _inquiriesPortalSettingsAccessorMock.Setup(x => x.GetSettings(It.IsAny<CancellationToken>())).ReturnsAsync(portalSettings);

        _timeTrackContractMock.Setup(x => x.GetUsersBySalesManagerId(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        var query = new GetInquiryUsersLookupRequest.Query(IsApprovals: true);

        //Act
        var result = await _handler.Handle(query, _token);

        //Assert
        result.Count.Should().Be(1);
        result.FirstOrDefault()!.Id.Should().Be(1);

    }

    [Fact]
    public async Task HandleWhenIsApprovalsFalseAndEmployeeCanManageRequestsReturnsOnlyEmployee()
    {
        //Arrange
        _authenticatedUserContextMock
            .Setup(context => context.HasClaim(RequestsClaimTypeNames.ManageRequests.Name))
            .Returns(true);

        _authenticatedUserContextMock
            .Setup(context => context.HasClaim(RequestsClaimTypeNames.ManageAllRequests.Name))
            .Returns(false);

        _authenticatedUserContextMock
            .Setup(context => context.HasClaim(AbsencesClaimTypeNames.ManageAbsenceSettings.Name))
            .Returns(false);

        _authenticatedUserContextMock
            .Setup(x => x.GetRoles())
            .Returns([PortalRoleNames.Employee]);

        const int userId = 1;

        _authenticatedUserContextMock
            .Setup(context => context.UserId)
            .Returns(userId);

        _inquiriesDbContextMock
            .Setup(context => context.InquiriesUsers)
            .Returns(new List<InquiriesUserEntity>
            {
                new() { UserId = userId, IsActive = true, Email = "test",
                    FirstName = "test",
                    LastName = "test", },
                new() { UserId = 2, IsActive = true     , Email = "test",
                    FirstName = "test",
                    LastName = "test",   }
            }.BuildMockDbSet().Object);

        _inquiriesPortalSettingsAccessorMock.Setup(x => x.GetSettings(It.IsAny<CancellationToken>())).ReturnsAsync(portalSettings);

        _timeTrackContractMock.Setup(x => x.GetUsersBySalesManagerId(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        var query = new GetInquiryUsersLookupRequest.Query(IsApprovals: false);

        //Act
        var result = await _handler.Handle(query, _token);

        //Assert
        result.Count.Should().Be(1);
        result.FirstOrDefault()!.Id.Should().Be(1);

    }

    [Fact]
    public async Task HandleWhenIsApprovalsFalseAndEmployeeCanManageAllRequestsReturnsAllUsers()
    {
        //Arrange
        _authenticatedUserContextMock
            .Setup(context => context.HasClaim(RequestsClaimTypeNames.ManageRequests.Name))
            .Returns(true);

        _authenticatedUserContextMock
            .Setup(context => context.HasClaim(RequestsClaimTypeNames.ManageAllRequests.Name))
            .Returns(true);

        _authenticatedUserContextMock
            .Setup(context => context.HasClaim(AbsencesClaimTypeNames.ManageAbsenceSettings.Name))
            .Returns(false);

        _authenticatedUserContextMock
            .Setup(x => x.GetRoles())
            .Returns([PortalRoleNames.Employee]);

        const int userId = 1;

        _authenticatedUserContextMock
            .Setup(context => context.UserId)
            .Returns(userId);

        _inquiriesDbContextMock
            .Setup(context => context.InquiriesUsers)
            .Returns(new List<InquiriesUserEntity>
            {
                new() { UserId = userId, IsActive = true, Email = "test",
                    FirstName = "test",
                    LastName = "test", },
                new() { UserId = 2, IsActive = true     , Email = "test",
                    FirstName = "test",
                    LastName = "test", }
            }.BuildMockDbSet().Object);

        var query = new GetInquiryUsersLookupRequest.Query(IsApprovals: false);

        //Act
        var result = await _handler.Handle(query, _token);

        //Assert
        result.Count.Should().Be(2);
        result[0].Id.Should().Be(1);
        result[1].Id.Should().Be(2);

    }

    [Fact]
    public async Task HandleWhenIsApprovalsTrueAndEmployeeCanManageAllRequestsReturnsAllUsers()
    {
        //Arrange
        _authenticatedUserContextMock
            .Setup(context => context.HasClaim(RequestsClaimTypeNames.ManageRequests.Name))
            .Returns(true);

        _authenticatedUserContextMock
            .Setup(context => context.HasClaim(RequestsClaimTypeNames.ManageAllRequests.Name))
            .Returns(true);

        _authenticatedUserContextMock
            .Setup(context => context.HasClaim(AbsencesClaimTypeNames.ManageAbsenceSettings.Name))
            .Returns(false);

        _authenticatedUserContextMock
            .Setup(x => x.GetRoles())
            .Returns([PortalRoleNames.Employee]);

        const int userId = 1;

        _authenticatedUserContextMock
            .Setup(context => context.UserId)
            .Returns(userId);

        _inquiriesDbContextMock
            .Setup(context => context.InquiriesUsers)
            .Returns(new List<InquiriesUserEntity>
            {
                new() { UserId = userId, IsActive = true , Email = "test",
                    FirstName = "test",
                    LastName = "test",},
                new() { UserId = 2, IsActive = true      , Email = "test",
                    FirstName = "test",
                    LastName = "test",}
            }.BuildMockDbSet().Object);

        var query = new GetInquiryUsersLookupRequest.Query(IsApprovals: true);

        //Act
        var result = await _handler.Handle(query, _token);

        //Assert
        result.Count.Should().Be(2);
        result[0].Id.Should().Be(1);
        result[1].Id.Should().Be(2);

    }

    [Fact]
    public async Task HandleWhenEmployeeCanManageRequestsReturnsOnlyEmployeeAndUsersWhoHaveThisEmployeeAsApprover()
    {
        //Arrange
        _authenticatedUserContextMock
            .Setup(context => context.HasClaim(RequestsClaimTypeNames.ManageRequests.Name))
            .Returns(true);

        _authenticatedUserContextMock
            .Setup(context => context.HasClaim(RequestsClaimTypeNames.ManageAllRequests.Name))
            .Returns(false);

        _authenticatedUserContextMock
            .Setup(context => context.HasClaim(AbsencesClaimTypeNames.ManageAbsenceSettings.Name))
            .Returns(false);

        _authenticatedUserContextMock
            .Setup(x => x.GetRoles())
            .Returns([PortalRoleNames.Employee]);

        const int userId = 1;

        _authenticatedUserContextMock
            .Setup(context => context.UserId)
            .Returns(userId);

        _inquiriesDbContextMock
            .Setup(context => context.InquiriesUsers)
            .Returns(new List<InquiriesUserEntity>
            {
                new() { UserId = userId, IsActive = true,Email = "test",
                    FirstName = "test",
                    LastName = "test", },
                new() { UserId = 3, IsActive = true,Email = "test",
                    FirstName = "test",
                    LastName = "test", },
                new()
                {
                    UserId = 2,
                    IsActive = true,
                    Email = "test",
                    FirstName = "test",
                    LastName = "test",
                    UserInquiries =
                    [
                        new UserInquiryEntity
                        {
                            UserInquiryAccepts = [new UserInquiryAcceptEntity { AcceptUserId = userId }]
                        }
                    ]
                }
            }.BuildMockDbSet().Object);

        _inquiriesPortalSettingsAccessorMock.Setup(x => x.GetSettings(It.IsAny<CancellationToken>())).ReturnsAsync(portalSettings);
        
        _timeTrackContractMock.Setup(x => x.GetUsersBySalesManagerId(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        var query = new GetInquiryUsersLookupRequest.Query(IsApprovals: false);

        //Act
        var result = await _handler.Handle(query, _token);

        //Assert
        result.Count.Should().Be(2);
        result[0].Id.Should().Be(1);
        result[1].Id.Should().Be(2);

    }

    [Fact]
    public async Task HandleWhenEmployeeIsFromSalesReturnsEmployeeAndUsersWhoAreOnProjectsOfSalesManagersClients()
    {
        //Arrange
        _authenticatedUserContextMock
            .Setup(context => context.HasClaim(RequestsClaimTypeNames.ManageRequests.Name))
            .Returns(true);

        _authenticatedUserContextMock
            .Setup(context => context.HasClaim(RequestsClaimTypeNames.ManageAllRequests.Name))
            .Returns(false);

        _authenticatedUserContextMock
            .Setup(context => context.HasClaim(AbsencesClaimTypeNames.ManageAbsenceSettings.Name))
            .Returns(false);

        _authenticatedUserContextMock
            .Setup(x => x.GetRoles())
            .Returns([PortalRoleNames.Employee]);

        _authenticatedUserContextMock
            .Setup(x => x.IsFromDepartment(portalSettings.SalesDepartmentId))
            .Returns(true);

        const int userId = 1;

        _authenticatedUserContextMock
            .Setup(context => context.UserId)
            .Returns(userId);

        _inquiriesDbContextMock
            .Setup(context => context.InquiriesUsers)
            .Returns(new List<InquiriesUserEntity>
            {
                new() { UserId = userId, IsActive = true,Email = "test",
                    FirstName = "test",
                    LastName = "test", },
                new() { UserId = 3, IsActive = true,Email = "test",
                    FirstName = "test",
                    LastName = "test", },
                new()
                {
                    UserId = 2,
                    IsActive = true,
                    Email = "test",
                    FirstName = "test",
                    LastName = "test",
                    UserInquiries =
                    [
                        new UserInquiryEntity
                        {
                            UserInquiryAccepts = [new UserInquiryAcceptEntity { AcceptUserId = userId }]
                        }
                    ]
                }
            }.BuildMockDbSet().Object);

        _inquiriesPortalSettingsAccessorMock.Setup(x => x.GetSettings(It.IsAny<CancellationToken>())).ReturnsAsync(portalSettings);

        _timeTrackContractMock.Setup(x => x.GetUsersBySalesManagerId(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([2]);

        var query = new GetInquiryUsersLookupRequest.Query(IsApprovals: false);

        //Act
        var result = await _handler.Handle(query, _token);

        //Assert
        result.Count.Should().Be(2);
        result[0].Id.Should().Be(1);
        result[1].Id.Should().Be(2);

    }
}
