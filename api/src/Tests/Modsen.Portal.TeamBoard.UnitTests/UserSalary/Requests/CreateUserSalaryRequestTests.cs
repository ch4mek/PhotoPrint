using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.TeamBoard.Requests.Core.BusinessRules;
using Modsen.Portal.TeamBoard.Requests.TeamBoardPortalSettings;
using Modsen.Portal.TeamBoard.Requests.UserSalary.Core.BusinessRules;
using Modsen.Portal.TeamBoard.Requests.UserSalary.Core.Composers;
using Modsen.Portal.TeamBoard.Requests.UserSalary.Requests;
using Modsen.Portal.TeamBoard.Requests.UserSalary.Requests.RequestDto;
using Moq;

namespace Modsen.Portal.TeamBoard.UnitTests.UserSalary.Requests
{
    public class CreateUserSalaryRequestTests
    {
        private readonly CreateUserSalaryRequest _handler;
        private readonly Mock<ICanTeamBoardUserBeRetrievedBusinessRule> _canTeamBoardUserBeRetrievedBusinessRuleMock = new();
        private readonly Mock<ITeamBoardPortalSettingsAccessor> _teamBoardPortalSettingsAccessorMock = new();
        private readonly Mock<IAuthenticatedUserContext> _authenticatedUserContextMock = new();
        private readonly Mock<IIsSalaryFromDateValidBusinessRule> _isSalaryFromDateValidBusinessRuleMock = new();
        private readonly Mock<IUserSalaryComposer> _salaryComposerMock = new();
        private readonly CancellationToken _cancellationToken = CancellationToken.None;

        public CreateUserSalaryRequestTests()
        {
            _handler = new CreateUserSalaryRequest(_authenticatedUserContextMock.Object,
                _canTeamBoardUserBeRetrievedBusinessRuleMock.Object, _teamBoardPortalSettingsAccessorMock.Object,
                _isSalaryFromDateValidBusinessRuleMock.Object, _salaryComposerMock.Object);
        }

        [Fact]
        public async Task HandleWhenUserUpdatesThemselfThrowActionNotAllowedException()
        {
            // Arrange
            var initiatotId = 1;
            _authenticatedUserContextMock
                .Setup(x => x.GetRequiredUserId())
                .Returns(initiatotId);

            var dto = new CreateSalaryDto(initiatotId, It.IsAny<DateOnly>(), It.IsAny<decimal>(), null);
            var command = new CreateUserSalaryRequest.Command(dto, null);

            // Act
            var act = () => _handler.Handle(command, _cancellationToken);

            // Assert
            await act.Should().ThrowAsync<ActionNotAllowedException>("You are not allowed to create your own salary");
        }
    }
}
