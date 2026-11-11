using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.Requests.Core.BusinessRules;
using Modsen.Portal.TeamBoard.Requests.UserSalary.Core.BusinessRules;
using Modsen.Portal.TeamBoard.Requests.UserSalary.Core.Encryptor;
using Modsen.Portal.TeamBoard.Requests.UserSalary.Core.Loaders;
using Modsen.Portal.TeamBoard.Requests.UserSalary.Requests;
using Modsen.Portal.TeamBoard.Requests.UserSalary.Requests.RequestDto;
using Moq;

namespace Modsen.Portal.TeamBoard.UnitTests.UserSalary.Requests
{
    public class UpdateUserSalaryRequestTests
    {
        private readonly UpdateUserSalaryRequest _handler;
        private readonly Mock<ICanTeamBoardUserBeRetrievedBusinessRule> _canTeamBoardUserBeRetrievedBusinessRuleMock = new();
        private readonly Mock<IIsSalaryFromDateValidBusinessRule> _isSalaryFromDateValidMock = new();
        private readonly Mock<ITeamBoardDbContext> _teamboardDbContextMock = new();
        private readonly Mock<ISalaryEncryptor> _salaryEncryptorMock = new();
        private readonly Mock<IUserSalaryLoader> _salaryLoaderMock = new();
        private readonly Mock<IAuthenticatedUserContext> _authenticatedUserContextMock = new();
        private readonly CancellationToken _cancellationToken = CancellationToken.None;

        public UpdateUserSalaryRequestTests()
        {
            _handler = new UpdateUserSalaryRequest(_canTeamBoardUserBeRetrievedBusinessRuleMock.Object,
                _isSalaryFromDateValidMock.Object, _teamboardDbContextMock.Object, _salaryEncryptorMock.Object,
                _salaryLoaderMock.Object, _authenticatedUserContextMock.Object);
        }

        [Fact]
        public async Task HandleWhenUserUpdatesThemselfThrowActionNotAllowedException()
        {
            // Arrange
            var initiatotId = 1;
            _authenticatedUserContextMock
                .Setup(x => x.GetRequiredUserId())
                .Returns(initiatotId);

            var dto = new UpdateSalaryDto(It.IsAny<int>(), initiatotId, It.IsAny<DateOnly>(), It.IsAny<decimal>());
            var command = new UpdateUserSalaryRequest.Command(dto);

            // Act
            var act = () => _handler.Handle(command, _cancellationToken);

            // Assert
            await act.Should().ThrowAsync<ActionNotAllowedException>("You are not allowed to update your own salary");
        }
    }
}
