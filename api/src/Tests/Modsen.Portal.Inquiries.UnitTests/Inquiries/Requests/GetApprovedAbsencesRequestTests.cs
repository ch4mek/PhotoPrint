using MockQueryable.Moq;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Inquiries;
using Modsen.Portal.Identity.Contract;
using Modsen.Portal.Identity.Contract.Models;
using Modsen.Portal.Inquiries.DataAccess.Npgsql;
using Modsen.Portal.Inquiries.DataAccess.Npgsql.Entities;
using Modsen.Portal.Inquiries.Requests.Inquiries.Request;
using Moq;

namespace Modsen.Portal.Inquiries.UnitTests.Inquiries.Requests;

public class GetApprovedAbsencesRequestTests
{
    private readonly Mock<IInquiriesDbContext> _inquiriesDbContextMock = new();
    private readonly Mock<IAuthenticatedUserContext> _authenticatedUserContextMock = new();
    private readonly Mock<IIdentityContract> _identityContractMock = new();
    private readonly CancellationToken _token = CancellationToken.None;

    private readonly GetApprovedAbsencesRequest _handler;

    public GetApprovedAbsencesRequestTests()
    {
        _handler = new GetApprovedAbsencesRequest(
            _inquiriesDbContextMock.Object,
            _authenticatedUserContextMock.Object,
            _identityContractMock.Object);
    }

    [Fact]
    public async Task Handle_WhenUserHasNoAllowedRole_ShouldThrowActionNotAllowedException()
    {
        // Arrange
        _authenticatedUserContextMock.Setup(x => x.GetRoles()).Returns([PortalRoleNames.Employee]);

        // Act
        var action = () => _handler.Handle(new GetApprovedAbsencesRequest.Query(), _token);

        // Assert
        await action.Should().ThrowAsync<ActionNotAllowedException>();
    }

    [Fact]
    public async Task Handle_WhenRoleIsCto_ShouldReturnAllAccessibleApprovedAbsences()
    {
        // Arrange
        _authenticatedUserContextMock.Setup(x => x.GetRoles()).Returns([PortalRoleNames.Cto]);

        _inquiriesDbContextMock.Setup(x => x.UserInquiries)
            .Returns(CreateInquiries().BuildMockDbSet().Object);

        _identityContractMock
            .Setup(x => x.GetUsersWithManagersAsync(It.IsAny<int[]>(), _token))
            .ReturnsAsync([
                new UserWithManagersDto { UserId = 10, DepartmentId = "IT", ResourceGroupId = 1 },
                new UserWithManagersDto { UserId = 11, DepartmentId = "IT", ResourceGroupId = 2 }
            ]);

        // Act
        var result = await _handler.Handle(new GetApprovedAbsencesRequest.Query(), _token);

        // Assert
        result.Should().HaveCount(2);
        result.Select(x => x.UserFullName).Should().Contain(["A A", "Б Б (B B)"]);
        result.Select(x => x.UserId).Should().BeEquivalentTo([10, 11]);
        result.Select(x => x.InquiryId).Should().BeEquivalentTo([100, 101]);
        result.Select(x => x.InquiryTemplateName).Should().BeEquivalentTo(["Vacation", "Own Expense"]);
    }

    [Fact]
    public async Task Handle_WhenRoleIsHeadOfDepartment_ShouldReturnOnlyUsersFromCurrentDepartment()
    {
        // Arrange
        _authenticatedUserContextMock.Setup(x => x.GetRoles()).Returns([PortalRoleNames.HeadOfDepartment]);
        _authenticatedUserContextMock.Setup(x => x.DepartmentId).Returns("IT");

        _inquiriesDbContextMock.Setup(x => x.UserInquiries)
            .Returns(CreateInquiries().BuildMockDbSet().Object);

        _identityContractMock
            .Setup(x => x.GetUsersWithManagersAsync(It.IsAny<int[]>(), _token))
            .ReturnsAsync([
                new UserWithManagersDto { UserId = 10, DepartmentId = "IT", ResourceGroupId = 1 },
                new UserWithManagersDto { UserId = 11, DepartmentId = "HR", ResourceGroupId = 2 }
            ]);

        // Act
        var result = await _handler.Handle(new GetApprovedAbsencesRequest.Query(), _token);

        // Assert
        result.Should().HaveCount(1);
        result[0].UserId.Should().Be(10);
        result[0].InquiryId.Should().Be(100);
        result[0].InquiryTemplateName.Should().Be("Vacation");
        result[0].UserFullName.Should().Be("A A");
    }

    [Fact]
    public async Task Handle_WhenRoleIsGroupManager_ShouldReturnOnlyUsersFromCurrentResourceGroup()
    {
        // Arrange
        _authenticatedUserContextMock.Setup(x => x.GetRoles()).Returns([PortalRoleNames.GroupManager]);
        _authenticatedUserContextMock.Setup(x => x.ResourceGroupId).Returns(2);

        _inquiriesDbContextMock.Setup(x => x.UserInquiries)
            .Returns(CreateInquiries().BuildMockDbSet().Object);

        _identityContractMock
            .Setup(x => x.GetUsersWithManagersAsync(It.IsAny<int[]>(), _token))
            .ReturnsAsync([
                new UserWithManagersDto { UserId = 10, DepartmentId = "IT", ResourceGroupId = 1 },
                new UserWithManagersDto { UserId = 11, DepartmentId = "HR", ResourceGroupId = 2 }
            ]);

        // Act
        var result = await _handler.Handle(new GetApprovedAbsencesRequest.Query(), _token);

        // Assert
        result.Should().HaveCount(1);
        result[0].UserId.Should().Be(11);
        result[0].InquiryId.Should().Be(101);
        result[0].InquiryTemplateName.Should().Be("Own Expense");
        result[0].UserFullName.Should().Be("Б Б (B B)");
    }

    [Fact]
    public async Task Handle_ShouldExcludeUsersMissingInActiveIdentityUsers()
    {
        // Arrange
        _authenticatedUserContextMock.Setup(x => x.GetRoles()).Returns([PortalRoleNames.Ceo]);

        _inquiriesDbContextMock.Setup(x => x.UserInquiries)
            .Returns(CreateInquiries().BuildMockDbSet().Object);

        _identityContractMock
            .Setup(x => x.GetUsersWithManagersAsync(It.IsAny<int[]>(), _token))
            .ReturnsAsync([
                new UserWithManagersDto { UserId = 10, DepartmentId = "IT", ResourceGroupId = 1 }
            ]);

        // Act
        var result = await _handler.Handle(new GetApprovedAbsencesRequest.Query(), _token);

        // Assert
        result.Should().HaveCount(1);
        result[0].UserId.Should().Be(10);
        result[0].InquiryId.Should().Be(100);
        result[0].InquiryTemplateName.Should().Be("Vacation");
        result[0].UserFullName.Should().Be("A A");
    }

    [Fact]
    public async Task Handle_WhenDepartmentIdProvided_ShouldReturnOnlyUsersFromRequestedDepartment()
    {
        // Arrange
        _authenticatedUserContextMock.Setup(x => x.GetRoles()).Returns([PortalRoleNames.Cto]);

        _inquiriesDbContextMock.Setup(x => x.UserInquiries)
            .Returns(CreateInquiries().BuildMockDbSet().Object);

        _identityContractMock
            .Setup(x => x.GetUsersWithManagersAsync(It.IsAny<int[]>(), _token))
            .ReturnsAsync([
                new UserWithManagersDto { UserId = 10, DepartmentId = "IT", ResourceGroupId = 1 },
                new UserWithManagersDto { UserId = 11, DepartmentId = "HR", ResourceGroupId = 2 }
            ]);

        // Act
        var result = await _handler.Handle(new GetApprovedAbsencesRequest.Query("HR"), _token);

        // Assert
        result.Should().HaveCount(1);
        result[0].UserId.Should().Be(11);
        result[0].InquiryId.Should().Be(101);
        result[0].InquiryTemplateName.Should().Be("Own Expense");
        result[0].UserFullName.Should().Be("Б Б (B B)");
    }

    private static List<UserInquiryEntity> CreateInquiries()
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        return
        [
            new UserInquiryEntity
            {
                Id = 100,
                UserId = 10,
                Status = UserInquiryStatus.Approved,
                DateFrom = today.AddDays(-2),
                DateTo = today.AddDays(2),
                User = new InquiriesUserEntity
                {
                    UserId = 10,
                    FirstName = "A",
                    LastName = "A",
                    Email = "a@modsen-software.com"
                },
                InquiryTemplate = new InquiryTemplateEntity
                {
                    Id = 1,
                    Name = "Vacation",
                    InquiryType = InquiryType.Vacation,
                    ActivityType = ActivityType.NonWorkingPaid
                }
            },
            new UserInquiryEntity
            {
                Id = 101,
                UserId = 11,
                Status = UserInquiryStatus.Approved,
                DateFrom = today.AddDays(-1),
                DateTo = today.AddDays(1),
                User = new InquiriesUserEntity
                {
                    UserId = 11,
                    FirstName = "B",
                    LastName = "B",
                    FirstNameRu = "Б",
                    LastNameRu = "Б",
                    Email = "b@modsen-software.com"
                },
                InquiryTemplate = new InquiryTemplateEntity
                {
                    Id = 2,
                    Name = "Own Expense",
                    InquiryType = InquiryType.OwnExpense,
                    ActivityType = ActivityType.NonWorkingUnpaid
                }
            },
            // Should be filtered out by status
            new UserInquiryEntity
            {
                Id = 102,
                UserId = 12,
                Status = UserInquiryStatus.Open,
                DateFrom = today.AddDays(-3),
                DateTo = today.AddDays(3),
                User = new InquiriesUserEntity
                {
                    UserId = 12,
                    FirstName = "C",
                    LastName = "C",
                    Email = "c@modsen-software.com"
                },
                InquiryTemplate = new InquiryTemplateEntity
                {
                    Id = 3,
                    Name = "Sick Leave",
                    InquiryType = InquiryType.SickLeave,
                    ActivityType = ActivityType.NonWorkingPaid
                }
            },
            // Should be filtered out by activity type
            new UserInquiryEntity
            {
                Id = 103,
                UserId = 13,
                Status = UserInquiryStatus.Approved,
                DateFrom = today.AddDays(-4),
                DateTo = today.AddDays(4),
                User = new InquiriesUserEntity
                {
                    UserId = 13,
                    FirstName = "D",
                    LastName = "D",
                    Email = "d@modsen-software.com"
                },
                InquiryTemplate = new InquiryTemplateEntity
                {
                    Id = 4,
                    Name = "Home Office",
                    InquiryType = InquiryType.HomeOffice,
                    ActivityType = ActivityType.WorkingPaid
                }
            }
        ];
    }
}
