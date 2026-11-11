using MockQueryable.Moq;
using Modsen.Portal.Core.Communication;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Services.BusinessRules;
using Modsen.Portal.Core.TeamBoard;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TeamBoard.Requests.Contracts.Core.BusinessRules;
using Modsen.Portal.TeamBoard.Requests.Contracts.Core.BusinessRules.Models;
using Modsen.Portal.TeamBoard.Requests.Contracts.Requests;
using Modsen.Portal.TeamBoard.Requests.Core.DbQueries;
using Moq;

namespace Modsen.Portal.TeamBoard.UnitTests.Contracts.Requests;

public class CreateUserContractRequestTests
{
    private readonly CreateUserContractRequest _handler;
    private readonly Mock<ITeamBoardDbContext> _teamBoardDbContext = new();
    private readonly Mock<IAuthenticatedUserContext> _authenticatedUserContext = new();
    private readonly Mock<IIsFireDateLaterThanHireDateDbQuery> _isFireDateLaterThanHireDateDbQuery = new();
    private readonly Mock<IIsUserHasPermissionsToMutateContractsBusinessRule> _isUserHasPermissionsToMutateContractsBusinessRule = new();
    private readonly Mock<ICommunicationBus> _communicationBus = new();
    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    public CreateUserContractRequestTests()
    {
        _handler = new CreateUserContractRequest(_teamBoardDbContext.Object, _isFireDateLaterThanHireDateDbQuery.Object,
            _authenticatedUserContext.Object, _isUserHasPermissionsToMutateContractsBusinessRule.Object, _communicationBus.Object);
    }

    [Fact]
    public async Task HandleWhenUserDoesNotExistThrowEntityNotFound()
    {
        // Arrange
        var userId = 123;
        _teamBoardDbContext.SetupGet(x => x.TeamBoardUsers).Returns(Array.Empty<TeamBoardUser>().BuildMockDbSet().Object);
        var command = new CreateUserContractRequest.Command(userId, It.IsAny<DateOnly>(), It.IsAny<DateOnly>(),
            It.IsAny<DateOnly>(), It.IsAny<FireType>(), It.IsAny<string>(), It.IsAny<string>());

        // Act
        var act = () => _handler.Handle(command, _cancellationToken);

        // Assert
        await act.Should().ThrowAsync<EntityNotFoundException>();
    }

    [Fact]
    public async Task HandleWhenBusinessRuleFailedThrowActionNotAllowed()
    {
        // Arrange
        var userId = 123;

        var user = new TeamBoardUser
        {
            UserId = userId,
            Email = "email",
            FirstName = "FirstName",
            LastName = "LastName",
            IsActive = true
        };

        _teamBoardDbContext.SetupGet(x => x.TeamBoardUsers)
            .Returns(new[] { user }.BuildMockDbSet().Object);

        var bRuleModel = new IsUserHasPermissionBusinessModel(userId, null, null, null, true);
        var failMessage = "FAILED";

        _isUserHasPermissionsToMutateContractsBusinessRule.Setup(x => x.IsCorrespondsToBusiness(bRuleModel, _cancellationToken))
            .ReturnsAsync(BusinessRuleResult.Fail(failMessage));

        var command = new CreateUserContractRequest.Command(userId, It.IsAny<DateOnly>(), It.IsAny<DateOnly>(),
            It.IsAny<DateOnly>(), It.IsAny<FireType>(), It.IsAny<string>(), It.IsAny<string>());

        // Act
        var act = () => _handler.Handle(command, _cancellationToken);

        // Assert
        await act.Should().ThrowAsync<ActionNotAllowedException>().WithMessage(failMessage);
    }
}
