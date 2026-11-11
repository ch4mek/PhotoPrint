using MockQueryable.Moq;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Inquiries;
using Modsen.Portal.Inquiries.DataAccess.Npgsql;
using Modsen.Portal.Inquiries.DataAccess.Npgsql.Entities;
using Modsen.Portal.Inquiries.Requests.Inquiries.Request;
using Moq;

namespace Modsen.Portal.Inquiries.UnitTests.Inquiries.Requests;

public class GetUserLeavesRequestTests
{
    private readonly Mock<IInquiriesDbContext> _inquiriesDbContext = new();
    private readonly Mock<IAuthenticatedUserContext> _authenticatedUserContext = new();

    private readonly GetUserLeavesRequest _handler;
    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    public GetUserLeavesRequestTests()
    {
        _handler = new GetUserLeavesRequest(
            _inquiriesDbContext.Object,
            _authenticatedUserContext.Object
        );

        _authenticatedUserContext.SetupUserContext(true, true);
    }

    [Theory]
    [InlineData(2025, 0)]
    [InlineData(2024, 5)]
    public async Task HandleCorrectShouldBeSuccess(int year, int expectedCount)
    {
        // Arrange
        var departmentId = "JS";
        _authenticatedUserContext.Setup(x => x.DepartmentId).Returns(departmentId);
        var userInquiries = CreateUserInquiryEntities(departmentId);

        _inquiriesDbContext.Setup(x => x.UserInquiries).Returns(userInquiries.BuildMockDbSet().Object);
        var query = new GetUserLeavesRequest.Query(10, 0, year, null, null);

        // Act
        var result = await _handler.Handle(query, _cancellationToken);

        // Assert
        result.Should().HaveCount(expectedCount);
    }

    [Fact]
    public async Task HandleIncorrectValuesShouldBeFailed()
    {
        // Arrange
        var query = new GetUserLeavesRequest.Query(-10, -5, -10, 15, null);
        var validator = new GetUserLeavesRequest.Validator();

        // Act
        var result = await validator.ValidateAsync(query, _cancellationToken);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().HaveCount(4);
    }

    private UserInquiryEntity[] CreateUserInquiryEntities(string departmentId)
    {
        var dateOnlyNow = DateOnly.Parse("01/01/2025");
        return Enumerable.Range(0, 5)
            .Select(_ => new UserInquiryEntity
            {
                DateFrom = dateOnlyNow.AddYears(-3),
                DateTo = dateOnlyNow.AddYears(-1),
                Status = UserInquiryStatus.Approved,
                UserId = InquiryRequestTestsHelper.AuthenticatedUserId,
                User = new InquiriesUserEntity
                {
                    UserId = InquiryRequestTestsHelper.AuthenticatedUserId,
                    Position = "Trainee",
                    FirstName = "First",
                    LastName = "Last",
                    Email = "Email",
                    DepartmentId = departmentId,
                }
            })
            .ToArray();
    }
}
