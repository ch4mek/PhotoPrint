using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.Requests.Core.BusinessRules;
using Modsen.Portal.TeamBoard.Requests.Skills.UserSpecializations.Requests;
using Moq;

namespace Modsen.Portal.TeamBoard.UnitTests.UserSpecialization.GetUserSpecializationsByUserIdLookup;

public class GetUserSpecializationsByUserIdLookupTests
{
    private readonly Mock<ITeamBoardDbContext> _teamBoardDbContextMock = new();
    private readonly Mock<ICanTeamBoardUserBeRetrievedBusinessRule> _canTeamBoardUserBeRetrievedBusinessRuleMock = new();
    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    private readonly GetUserSpecializationsByUserIdRequest _handler;

    public GetUserSpecializationsByUserIdLookupTests()
    {
        _handler = new GetUserSpecializationsByUserIdRequest(
            _teamBoardDbContextMock.Object,
            _canTeamBoardUserBeRetrievedBusinessRuleMock.Object);
    }

    [Fact]
    public async Task HandleWhenUserCanNotBeRetrievedShouldThrowEntityNotFoundException()
    {
        // Arrange
        var userId = 1;

        _canTeamBoardUserBeRetrievedBusinessRuleMock
            .Setup(x => x.IsCorrespondsToBusiness(It.IsAny<int>(), _cancellationToken))
            .ReturnsAsync(false);

        // Act
        var result = async () => await _handler.Handle(
            new GetUserSpecializationsByUserIdRequest.Query(userId, "test"), _cancellationToken);

        //Assert
        await result.Should().ThrowAsync<EntityNotFoundException>();
    }
}
