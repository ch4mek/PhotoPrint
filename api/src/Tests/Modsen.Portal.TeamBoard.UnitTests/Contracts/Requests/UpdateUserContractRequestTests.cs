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

public class UpdateUserContractRequestTests
{
    private readonly UpdateUserContractRequest _handler;
    private readonly Mock<IAuthenticatedUserContext> _authenticatedUserContext = new();
    private readonly Mock<ITeamBoardDbContext> _teamBoardDbContext = new();
    private readonly Mock<IIsFireDateLaterThanHireDateDbQuery> _isFireDateLaterThanHireDateDbQuery = new();
    private readonly Mock<IIsUserHasPermissionsToMutateContractsBusinessRule> _isUserHasPermissionsToMutateContractsBusinessRule = new();
    private readonly Mock<ICommunicationBus> _communicationBus = new();
    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    public UpdateUserContractRequestTests()
    {
        _handler = new UpdateUserContractRequest(_authenticatedUserContext.Object, _teamBoardDbContext.Object,
            _isFireDateLaterThanHireDateDbQuery.Object, _isUserHasPermissionsToMutateContractsBusinessRule.Object,
            _communicationBus.Object);
    }

    [Fact]
    public async Task HandleWhenContractIsNotFoundThrowEntityNotFoundException()
    {
        // Arrange
        var initiatotId = 1;
        _authenticatedUserContext
            .Setup(x => x.GetRequiredUserId())
            .Returns(initiatotId);

        var contractId = 123;
        _teamBoardDbContext.SetupGet(x => x.UserContracts)
            .Returns(Array.Empty<UserContract>().BuildMockDbSet().Object);

        var command = new UpdateUserContractRequest.Command(contractId, It.IsAny<DateOnly>(), It.IsAny<DateOnly>(),
            It.IsAny<DateOnly>(), It.IsAny<FireType>(), It.IsAny<string>(), It.IsAny<string>());

        // Act
        var act = () => _handler.Handle(command, _cancellationToken);

        // Assert
        await act.Should().ThrowAsync<EntityNotFoundException>();
    }

    [Fact]
    public async Task HandleWhenUserIsNotFoundThrowEntityConflict()
    {
        // Arrange
        var initiatotId = 2;
        _authenticatedUserContext
            .Setup(x => x.GetRequiredUserId())
            .Returns(initiatotId);

        var contractId = 123;

        var userContract = new UserContract
        {
            UserId = 1,
            Id = contractId,
        };

        _teamBoardDbContext.SetupGet(x => x.UserContracts)
            .Returns(new[] { userContract }.BuildMockDbSet().Object);

        _teamBoardDbContext.SetupGet(x => x.TeamBoardUsers)
            .Returns(Array.Empty<TeamBoardUser>().BuildMockDbSet().Object);

        var command = new UpdateUserContractRequest.Command(contractId, It.IsAny<DateOnly>(), It.IsAny<DateOnly>(),
            It.IsAny<DateOnly>(), It.IsAny<FireType>(), It.IsAny<string>(), It.IsAny<string>());

        // Act
        var act = () => _handler.Handle(command, _cancellationToken);

        // Assert
        await act.Should().ThrowAsync<EntityConflictException>();
    }

    [Fact]
    public async Task HandleWhenBusinessRuleFailedThrowActionNotAllowed()
    {
        // Arrange
        var initiatotId = 1;
        _authenticatedUserContext
            .Setup(x => x.GetRequiredUserId())
            .Returns(initiatotId);

        var contractId = 123;
        var userId = 12;

        var userContract = new UserContract
        {
            UserId = userId,
            Id = contractId,
        };

        var user = new TeamBoardUser
        {
            UserId = userId,
            Email = "Email",
            FirstName = "FirstName",
            LastName = "LastName",
        };

        _teamBoardDbContext.SetupGet(x => x.UserContracts)
            .Returns(new[] { userContract }.BuildMockDbSet().Object);

        _teamBoardDbContext.SetupGet(x => x.TeamBoardUsers)
            .Returns(new[] { user }.BuildMockDbSet().Object);

        var command = new UpdateUserContractRequest.Command(contractId, It.IsAny<DateOnly>(), It.IsAny<DateOnly>(),
            It.IsAny<DateOnly>(), It.IsAny<FireType>(), It.IsAny<string>(), It.IsAny<string>());

        var model = new IsUserHasPermissionBusinessModel(user.UserId, user.Position, user.DepartmentId,
            user.ResourceGroupId, user.IsActive);
        var failReason = "Fail Reason";

        _isUserHasPermissionsToMutateContractsBusinessRule.Setup(x => x.IsCorrespondsToBusiness(model, _cancellationToken))
            .ReturnsAsync(BusinessRuleResult.Fail(failReason));

        // Act
        var act = () => _handler.Handle(command, _cancellationToken);

        // Assert
        await act.Should().ThrowAsync<ActionNotAllowedException>().WithMessage(failReason);
    }

    [Fact]
    public async Task HandleWhenUserUpdatesThemselfThrowActionNotAllowedException()
    {
        // Arrange
        var initiatotId = 1;
        _authenticatedUserContext
            .Setup(x => x.GetRequiredUserId())
            .Returns(initiatotId);

        var contractId = 123;

        var userContract = new UserContract
        {
            UserId = initiatotId,
            Id = contractId,
        };        

        _teamBoardDbContext.SetupGet(x => x.UserContracts)
            .Returns(new[] { userContract }.BuildMockDbSet().Object);        

        var command = new UpdateUserContractRequest.Command(contractId, It.IsAny<DateOnly>(), It.IsAny<DateOnly>(),
            It.IsAny<DateOnly>(), It.IsAny<FireType>(), It.IsAny<string>(), It.IsAny<string>());

        // Act
        var act = () => _handler.Handle(command, _cancellationToken);

        // Assert
        await act.Should().ThrowAsync<ActionNotAllowedException>("You are not allowed to edit your own contract");
    }
}
