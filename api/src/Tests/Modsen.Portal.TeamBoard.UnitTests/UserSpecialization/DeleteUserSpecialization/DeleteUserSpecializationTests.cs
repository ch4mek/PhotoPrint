using MockQueryable.Moq;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.Requests.Core.BusinessRules;
using Modsen.Portal.TeamBoard.Requests.Skills.UserSpecializations.Requests;
using Moq;

namespace Modsen.Portal.TeamBoard.UnitTests.UserSpecialization.DeleteUserSpecialization;

public class DeleteUserSpecializationTests
{
    private readonly Mock<ITeamBoardDbContext> _teamboardDbContextMock = new();
    private readonly Mock<ICanTeamBoardUserBeRetrievedBusinessRule> _canTeamBoardUserBeRetrievedBusinessRuleMock = new();

    private readonly DeleteUserSpecializationRequest _handler;
    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    public DeleteUserSpecializationTests()
    {
        _handler = new DeleteUserSpecializationRequest(
            _teamboardDbContextMock.Object,
            _canTeamBoardUserBeRetrievedBusinessRuleMock.Object);
    }

    [Fact]
    public async Task HandleWhenUserSpecializationDoesNotExistShouldThrowEntityNotFoundException()
    {
        // Arrange
        var command = new DeleteUserSpecializationRequest.Command(int.MaxValue);

        _teamboardDbContextMock
            .Setup(context => context.UserSpecializations)
            .Returns(new List<DataAccess.Npgsql.DatabaseEntities.UserSpecialization>().BuildMockDbSet().Object);

        // Act
        var result = () => _handler.Handle(command, _cancellationToken);

        // Assert
        await result.Should().ThrowAsync<EntityNotFoundException>(
            $"User specialization with id {command.Id} was not found");
    }

    [Fact]
    public async Task HandleWhenUserSpecializationIsPrimaryShouldThrowInvalidDataAppException()
    {
        // Arrange
        var command = new DeleteUserSpecializationRequest.Command(1);

        var userSpecialization = new DataAccess.Npgsql.DatabaseEntities.UserSpecialization
        {
            Id = 1,
            IsPrimary = true,
            SeniorityLevelId = "Senior =",
            SpecializationTemplateId = 1,
            UserId = 1
        };

        _teamboardDbContextMock
            .Setup(context => context.UserSpecializations)
            .Returns(new List<DataAccess.Npgsql.DatabaseEntities.UserSpecialization>()
            {
                userSpecialization
            }.BuildMockDbSet().Object);


        // Act
        var result = () => _handler.Handle(command, _cancellationToken);

        // Assert
        await result.Should().ThrowAsync<InvalidDataAppException>("User's primary specialization cannot be deleted");
    }
}
