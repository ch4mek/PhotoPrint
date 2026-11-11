using Microsoft.AspNetCore.OData.Query;
using MockQueryable.Moq;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.OData;
using Modsen.Portal.Inquiries.DataAccess.Npgsql;
using Modsen.Portal.Inquiries.DataAccess.Npgsql.Entities;
using Modsen.Portal.Inquiries.Requests.Inquiries.Core.CoreDto;
using Modsen.Portal.Inquiries.Requests.Inquiries.Core.Resolvers;
using Modsen.Portal.Inquiries.Requests.Inquiries.Request;
using Modsen.Portal.Inquiries.Requests.InquiriesPortalSettings;
using Modsen.Portal.Inquiries.Requests.InquiriesPortalSettings.CoreDto;
using Modsen.Portal.TimeTrack.Contract;
using Moq;

namespace Modsen.Portal.Inquiries.UnitTests.Inquiries.Requests;

public class GetInquiriesRequestTests
{
    private readonly Mock<IAuthenticatedUserContext> _userContextMock = new();
    private readonly Mock<IInquiriesDbContext> _inquiriesDbContextMock = new();
    private readonly Mock<IGetInquiriesCanBeApprovedAsSubstituteResolver> _getInquiriesCanBeApprovedAsSubstituteResolverMock = new();
    private readonly Mock<IInquiriesPortalSettingsAccessor> _inquiriesPortalSettingsAccessorMock = new();
    private readonly Mock<ITimeTrackContract> _timeTrackContractMock = new();
    private readonly GetInquiriesRequest _handler;
    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    private readonly InquiriesPortalSettingsDto portalSettings = new()
    {
        SalesDepartmentId =  "Sales"
    };

    public GetInquiriesRequestTests()
    {
        _handler = new GetInquiriesRequest(_userContextMock.Object, _inquiriesDbContextMock.Object,
            _getInquiriesCanBeApprovedAsSubstituteResolverMock.Object, _inquiriesPortalSettingsAccessorMock.Object, _timeTrackContractMock.Object);
    }

    [Fact]
    public async Task HandleWhenUserHasManageAllRequestsClaimShouldReturnAllInquiries()
    {
        // Arrange
        var queryParameters = new Dictionary<string, string?>
        {
            [nameof(ODataRawQueryOptions.Top)] = "10",
            [nameof(ODataRawQueryOptions.Skip)] = "0"
        };
        var queryOptions = ODataQueryOptionsMock.BuildMock<InquiryCoreDto>(queryParameters);
        var command = new GetInquiriesRequest.Query(queryOptions);

        var inquiries = CreateUserInquiryEntities();
        var mockDbSet = inquiries.BuildMockDbSet();
        _inquiriesDbContextMock.Setup(x => x.UserInquiries)
            .Returns(mockDbSet.Object);

        // Act
        SetupUserContext(hasManageRequests: true, hasManageAllRequests: true);
        var result = await _handler.Handle(command, _cancellationToken);

        // Assert
        result.Items.Should().HaveCount(3);
        result.Total.Should().Be(3);
        result.Page.Should().Be(1);
        result.PageSize.Should().Be(10);
    }

    [Fact]
    public async Task HandleWhenUserIsHeadOfDepartmentShouldFilterByDepartmentAndAccepts()
    {
        // Arrange
        var queryParameters = new Dictionary<string, string?>
        {
            [nameof(ODataRawQueryOptions.Top)] = "10",
            [nameof(ODataRawQueryOptions.Skip)] = "0"
        };
        var queryOptions = ODataQueryOptionsMock.BuildMock<InquiryCoreDto>(queryParameters);
        var command = new GetInquiriesRequest.Query(queryOptions);

        var inquiries = CreateUserInquiryEntities();
        var mockDbSet = inquiries.BuildMockDbSet();
        _inquiriesDbContextMock.Setup(x => x.UserInquiries)
            .Returns(mockDbSet.Object);

        SetupUserContext(hasManageRequests: true, hasManageAllRequests: false);
        _userContextMock.Setup(x => x.GetRoles()).Returns(["HeadOfDepartment"]);
        _userContextMock.Setup(x => x.DepartmentId).Returns("IT");
        _userContextMock.Setup(x => x.UserId).Returns(1);

        _getInquiriesCanBeApprovedAsSubstituteResolverMock
            .Setup(x => x.ResolveAsync(true, _cancellationToken))
            .ReturnsAsync([]);

        _inquiriesPortalSettingsAccessorMock.Setup(x => x.GetSettings(It.IsAny<CancellationToken>())).ReturnsAsync(portalSettings);

        // Act
        var result = await _handler.Handle(command, _cancellationToken);

        // Assert
        result.Items.Should().HaveCount(2);
        result.Items.Should().ContainSingle(x => x.Id == 1);
        result.Items.Should().ContainSingle(x => x.Id == 3);
    }

    [Fact]
    public async Task HandleWhenUserIsGroupManagerShouldFilterByResourceGroupAndAccepts()
    {
        // Arrange
        var queryParameters = new Dictionary<string, string?>
        {
            [nameof(ODataRawQueryOptions.Top)] = "10",
            [nameof(ODataRawQueryOptions.Skip)] = "0"
        };
        var queryOptions = ODataQueryOptionsMock.BuildMock<InquiryCoreDto>(queryParameters);
        var command = new GetInquiriesRequest.Query(queryOptions);

        var inquiries = CreateUserInquiryEntities();
        var mockDbSet = inquiries.BuildMockDbSet();
        _inquiriesDbContextMock.Setup(x => x.UserInquiries)
            .Returns(mockDbSet.Object);

        SetupUserContext(hasManageRequests: true, hasManageAllRequests: false);
        _userContextMock.Setup(x => x.GetRoles()).Returns(["GroupManager"]);
        _userContextMock.Setup(x => x.ResourceGroupId).Returns(100);
        _userContextMock.Setup(x => x.UserId).Returns(1);

        _getInquiriesCanBeApprovedAsSubstituteResolverMock
            .Setup(x => x.ResolveAsync(true, _cancellationToken))
            .ReturnsAsync([]);

        _inquiriesPortalSettingsAccessorMock.Setup(x => x.GetSettings(It.IsAny<CancellationToken>())).ReturnsAsync(portalSettings);

        // Act
        var result = await _handler.Handle(command, _cancellationToken);

        // Assert
        result.Items.Should().HaveCount(2);
        result.Items.Should().ContainSingle(x => x.Id == 1);
        result.Items.Should().ContainSingle(x => x.Id == 3);
    }

    [Fact]
    public async Task HandleWhenUserIsEmployeeShouldFilterByUserAccepts()
    {
        // Arrange
        var queryParameters = new Dictionary<string, string?>
        {
            [nameof(ODataRawQueryOptions.Top)] = "10",
            [nameof(ODataRawQueryOptions.Skip)] = "0"
        };
        var queryOptions = ODataQueryOptionsMock.BuildMock<InquiryCoreDto>(queryParameters);
        var command = new GetInquiriesRequest.Query(queryOptions);

        var inquiries = CreateUserInquiryEntities();
        var mockDbSet = inquiries.BuildMockDbSet();
        _inquiriesDbContextMock.Setup(x => x.UserInquiries)
            .Returns(mockDbSet.Object);

        SetupUserContext(hasManageRequests: true, hasManageAllRequests: false);
        _userContextMock.Setup(x => x.GetRoles()).Returns(["Employee"]);
        _userContextMock.Setup(x => x.UserId).Returns(1);

        _getInquiriesCanBeApprovedAsSubstituteResolverMock
            .Setup(x => x.ResolveAsync(true, _cancellationToken))
            .ReturnsAsync([]);

        _inquiriesPortalSettingsAccessorMock.Setup(x => x.GetSettings(It.IsAny<CancellationToken>())).ReturnsAsync(portalSettings);

        // Act
        var result = await _handler.Handle(command, _cancellationToken);

        // Assert
        result.Items.Should().ContainSingle();
        result.Items.First().Id.Should().Be(1);
    }

    [Fact]
    public async Task HandleWhenUserIsSystemAdministratorShouldFilterByUserAccepts()
    {
        // Arrange
        var queryParameters = new Dictionary<string, string?>
        {
            [nameof(ODataRawQueryOptions.Top)] = "10",
            [nameof(ODataRawQueryOptions.Skip)] = "0"
        };
        var queryOptions = ODataQueryOptionsMock.BuildMock<InquiryCoreDto>(queryParameters);
        var command = new GetInquiriesRequest.Query(queryOptions);

        var inquiries = CreateUserInquiryEntities();
        var mockDbSet = inquiries.BuildMockDbSet();
        _inquiriesDbContextMock.Setup(x => x.UserInquiries)
            .Returns(mockDbSet.Object);

        SetupUserContext(hasManageRequests: true, hasManageAllRequests: false);
        _userContextMock.Setup(x => x.GetRoles()).Returns(["SystemAdministrator"]);
        _userContextMock.Setup(x => x.UserId).Returns(1);
        _getInquiriesCanBeApprovedAsSubstituteResolverMock
            .Setup(x => x.ResolveAsync(true, _cancellationToken))
            .ReturnsAsync([]);

        _inquiriesPortalSettingsAccessorMock.Setup(x => x.GetSettings(It.IsAny<CancellationToken>())).ReturnsAsync(portalSettings);

        // Act
        var result = await _handler.Handle(command, _cancellationToken);

        // Assert
        result.Items.Should().ContainSingle();
        result.Items.First().Id.Should().Be(1);
    }

    [Fact]
    public async Task HandleWhenUserFromSalesShouldAddByProjectClients()
    {
        // Arrange
        var queryParameters = new Dictionary<string, string?>
        {
            [nameof(ODataRawQueryOptions.Top)] = "10",
            [nameof(ODataRawQueryOptions.Skip)] = "0"
        };
        var queryOptions = ODataQueryOptionsMock.BuildMock<InquiryCoreDto>(queryParameters);
        var command = new GetInquiriesRequest.Query(queryOptions);

        var inquiries = CreateUserInquiryEntities();
        var mockDbSet = inquiries.BuildMockDbSet();
        _inquiriesDbContextMock.Setup(x => x.UserInquiries)
            .Returns(mockDbSet.Object);

        SetupUserContext(hasManageRequests: true, hasManageAllRequests: false);
        _userContextMock.Setup(x => x.GetRoles()).Returns([PortalRoleNames.Employee]);
        _userContextMock.Setup(x => x.UserId).Returns(1);
        _userContextMock.Setup(x => x.IsFromDepartment(portalSettings.SalesDepartmentId)).Returns(true);
        _getInquiriesCanBeApprovedAsSubstituteResolverMock
            .Setup(x => x.ResolveAsync(true, _cancellationToken))
            .ReturnsAsync([]);

        _inquiriesPortalSettingsAccessorMock.Setup(x => x.GetSettings(It.IsAny<CancellationToken>())).ReturnsAsync(portalSettings);

        _timeTrackContractMock.Setup(x => x.GetUsersBySalesManagerId(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([2]);

        // Act
        var result = await _handler.Handle(command, _cancellationToken);

        // Assert
        result.Items.Count.Should().Be(2);
    }

    [Fact]
    public async Task HandleWhenNoInquiriesFoundShouldReturnEmptyPageableList()
    {
        // Arrange
        var queryParameters = new Dictionary<string, string?>
        {
            [nameof(ODataRawQueryOptions.Top)] = "10",
            [nameof(ODataRawQueryOptions.Skip)] = "0"
        };
        var queryOptions = ODataQueryOptionsMock.BuildMock<InquiryCoreDto>(queryParameters);
        var command = new GetInquiriesRequest.Query(queryOptions);

        var emptyInquiries = new List<UserInquiryEntity>();
        var mockDbSet = emptyInquiries.BuildMockDbSet();
        _inquiriesDbContextMock.Setup(x => x.UserInquiries)
            .Returns(mockDbSet.Object);

        SetupUserContext(hasManageRequests: true, hasManageAllRequests: true);

        // Act
        var result = await _handler.Handle(command, _cancellationToken);

        // Assert
        result.Items.Should().BeEmpty();
        result.Total.Should().Be(0);
    }

    [Fact]
    public async Task HandleWhenPaginationAppliedShouldReturnCorrectPage()
    {
        // Arrange
        var queryParameters = new Dictionary<string, string?>
        {
            [nameof(ODataRawQueryOptions.Top)] = "2",
            [nameof(ODataRawQueryOptions.Skip)] = "1"
        };
        var queryOptions = ODataQueryOptionsMock.BuildMock<InquiryCoreDto>(queryParameters);
        var command = new GetInquiriesRequest.Query(queryOptions);

        var inquiries = CreateUserInquiryEntities();
        var mockDbSet = inquiries.BuildMockDbSet();
        _inquiriesDbContextMock.Setup(x => x.UserInquiries)
            .Returns(mockDbSet.Object);

        SetupUserContext(hasManageRequests: true, hasManageAllRequests: true);

        // Act
        var result = await _handler.Handle(command, _cancellationToken);

        // Assert
        result.Items.Should().HaveCount(2);
        result.Total.Should().Be(3);
        result.Page.Should().Be(1);
        result.PageSize.Should().Be(2);
    }

    private void SetupUserContext(bool hasManageRequests, bool hasManageAllRequests)
    {
        _userContextMock.Setup(x => x.HasClaim(RequestsClaimTypeNames.ManageRequests.Name))
            .Returns(hasManageRequests);
        _userContextMock.Setup(x => x.HasClaim(RequestsClaimTypeNames.ManageAllRequests.Name))
            .Returns(hasManageAllRequests);
        _userContextMock.Setup(x => x.IsAuthenticated).Returns(true);
        _userContextMock.Setup(x => x.UserId).Returns(1);
    }

    private List<UserInquiryEntity> CreateUserInquiryEntities()
    {
        return
        [
            new UserInquiryEntity
            {
                Id = 1,
                UserId = 1,
                User = new InquiriesUserEntity { UserId = 1, DepartmentId = "IT", ResourceGroupId = 100,Email = "test",
                    FirstName = "test",
                    LastName = "test", },
                UserInquiryAccepts = [new() { Id = 1, AcceptUserId = 1 }],
                InquiryTemplate = new InquiryTemplateEntity { Id = 1, Name = "Template 1" }
            },

            new UserInquiryEntity
            {
                Id = 2,
                UserId = 2,
                User = new InquiriesUserEntity { UserId = 2, DepartmentId = "HR", ResourceGroupId = 200,Email = "test",
                    FirstName = "test",
                    LastName = "test", },
                UserInquiryAccepts = [new() { Id = 2, AcceptUserId = 3 }],
                InquiryTemplate = new InquiryTemplateEntity { Id = 2, Name = "Template 2" }
            },

            new UserInquiryEntity
            {
                Id = 3,
                UserId = 3,
                User = new InquiriesUserEntity { UserId = 3, DepartmentId = "IT", ResourceGroupId = 100,Email = "test",
                    FirstName = "test",
                    LastName = "test", },
                UserInquiryAccepts = [],
                InquiryTemplate = new InquiryTemplateEntity { Id = 3, Name = "Template 3" }
            }
        ];
    }
}
