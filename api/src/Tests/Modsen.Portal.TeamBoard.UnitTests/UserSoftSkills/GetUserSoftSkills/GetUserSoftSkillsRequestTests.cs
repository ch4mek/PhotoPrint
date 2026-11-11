using Microsoft.AspNetCore.OData.Query;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.OData;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.OData;
using Modsen.Portal.TeamBoard.Requests.Core.DbQueries;
using Modsen.Portal.TeamBoard.Requests.Skills.UserSoftSkills.Requests;
using Moq;

namespace Modsen.Portal.TeamBoard.UnitTests.UserSoftSkills.GetUserSoftSkills;

public class GetUserSoftSkillsRequestTests
{
    private readonly Mock<IIsTeamBoardUserExistWithAccessControlDbQuery>
        _isTeamBoardUserPersonalInfoExistingByUserIdDbQuery = new();
    private readonly Mock<IAuthenticatedUserContext> _authenticatedUserContextMock = new();
    private readonly Mock<ITeamBoardDbContext> _teamBoardDbContextMock = new();

    private readonly CancellationToken _token = CancellationToken.None;

    private readonly GetUserSoftSkillsRequest _handler;

    public GetUserSoftSkillsRequestTests()
    {
        _handler = new GetUserSoftSkillsRequest(
            _authenticatedUserContextMock.Object,
            _isTeamBoardUserPersonalInfoExistingByUserIdDbQuery.Object,
            _teamBoardDbContextMock.Object);
    }

    [Fact]
    public async Task HandleWhenUserDoesNotExistShouldThrowEntityNotFoundException()
    {
        // Arrange
        var userId = 12345;
        var queryParameters = new Dictionary<string, string?>
        {
            [nameof(ODataRawQueryOptions.Top)] = "3",
            [nameof(ODataRawQueryOptions.Skip)] = "0"
        };
        var queryOptions = ODataQueryOptionsMock.BuildMock<OdpUserSoftSkill>(queryParameters);
        var request = new GetUserSoftSkillsRequest.Query(userId, queryOptions);

        _isTeamBoardUserPersonalInfoExistingByUserIdDbQuery
            .Setup(a => a.ExecuteAsync(userId, false, _token))
            .ReturnsAsync(false);

        _isTeamBoardUserPersonalInfoExistingByUserIdDbQuery
            .Setup(x => x.ExecuteAsync(userId, false, _token))
            .ReturnsAsync(false);

        // Act
        var result = async () => await _handler.Handle(request, _token);

        // Assert
        await result.Should().ThrowAsync<EntityNotFoundException>(PortalMessages.UserNotFound);
    }

    [Fact]
    public async Task HandleWhenEmployeePassAnotherUserIdShouldThrowActionNotAllowedException()
    {
        // Arrange
        var userId = 12345;
        var queryParameters = new Dictionary<string, string?>
        {
            [nameof(ODataRawQueryOptions.Top)] = "3",
            [nameof(ODataRawQueryOptions.Skip)] = "0"
        };
        var queryOptions = ODataQueryOptionsMock.BuildMock<OdpUserSoftSkill>(queryParameters);
        var request = new GetUserSoftSkillsRequest.Query(userId, queryOptions);

        _authenticatedUserContextMock
            .Setup(c => c.UserId)
            .Returns(userId + 10);

        _authenticatedUserContextMock
            .Setup(c => c.GetRoles())
            .Returns(["Employee"]);

        // Act
        var result = async () => await _handler.Handle(request, _token);

        // Assert
        await result.Should().ThrowAsync<ActionNotAllowedException>("You are not allowed to get another user soft-skills.");
    }
}
