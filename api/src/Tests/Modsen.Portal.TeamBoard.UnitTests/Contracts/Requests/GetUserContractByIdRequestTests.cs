using MockQueryable.Moq;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Services.BusinessRules;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TeamBoard.Requests.Contracts.Core.BusinessRules;
using Modsen.Portal.TeamBoard.Requests.Contracts.Core.BusinessRules.Models;
using Modsen.Portal.TeamBoard.Requests.Contracts.Requests;
using Moq;

namespace Modsen.Portal.TeamBoard.UnitTests.Contracts.Requests;

public class GetUserContractByIdRequestTests
{
    private readonly GetUserContractByIdRequest _handler;
    private readonly Mock<ITeamBoardDbContext> _teamBoardDbContext = new();
    private readonly Mock<IIsUserHasPermissionsToViewContractsBusinessRule> _isUserHasPermissionsToViewContractsBusinessRule = new();
    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    public GetUserContractByIdRequestTests()
    {
        _handler = new GetUserContractByIdRequest(_teamBoardDbContext.Object,
            _isUserHasPermissionsToViewContractsBusinessRule.Object);
    }

    [Fact]
    public async Task HandleWhenUserContractDoesNotExistThrowEntityNotFound()
    {
        // Arrange
        var contractId = 123;
        var query = new GetUserContractByIdRequest.Query(contractId);

        _teamBoardDbContext.SetupGet(x => x.UserContracts)
            .Returns(Array.Empty<UserContract>().BuildMockDbSet().Object);

        // Act
        var act = () => _handler.Handle(query, _cancellationToken);

        // Assert
        await act.Should().ThrowAsync<EntityNotFoundException>();
    }

    [Fact]
    public async Task HandleWhenUserDoesNotExistThrowEntityConflict()
    {
        // Arrange
        var contractId = 123;
        var userId = 1234;

        var userContract = new UserContract
        {
            Id = contractId,
            UserId = userId,
        };

        _teamBoardDbContext.SetupGet(x => x.UserContracts)
            .Returns(new[] { userContract }.BuildMockDbSet().Object);

        _teamBoardDbContext.SetupGet(x => x.TeamBoardUsers)
            .Returns(Array.Empty<TeamBoardUser>().BuildMockDbSet().Object);

        var query = new GetUserContractByIdRequest.Query(contractId);

        // Act
        var act = () => _handler.Handle(query, _cancellationToken);

        // Assert
        await act.Should().ThrowAsync<EntityConflictException>();
    }

    [Fact]
    public async Task HandleWhenBusinessRuleFailedThrowActionNotAllowed()
    {
        // Arrange
        var contractId = 123;
        var userId = 1234;

        var userContract = new UserContract
        {
            Id = contractId,
            UserId = userId,
        };

        var user = new TeamBoardUser
        {
            UserId = userId,
            Email = "Email",
            FirstName = "FirstName",
            LastName = "LastName"
        };

        _teamBoardDbContext.SetupGet(x => x.UserContracts)
            .Returns(new[] { userContract }.BuildMockDbSet().Object);

        _teamBoardDbContext.SetupGet(x => x.TeamBoardUsers)
            .Returns(new[] { user }.BuildMockDbSet().Object);

        var failReason = "Failed";
        var model = new IsUserHasPermissionBusinessModel(userId, user.Position, user.DepartmentId,
            user.ResourceGroupId, user.IsActive);

        _isUserHasPermissionsToViewContractsBusinessRule.Setup(x => x.IsCorrespondsToBusiness(model, _cancellationToken))
            .ReturnsAsync(BusinessRuleResult.Fail(failReason));

        var query = new GetUserContractByIdRequest.Query(contractId);

        // Act
        var act = () => _handler.Handle(query, _cancellationToken);

        // Assert
        await act.Should().ThrowAsync<ActionNotAllowedException>().WithMessage(failReason);
    }
}
