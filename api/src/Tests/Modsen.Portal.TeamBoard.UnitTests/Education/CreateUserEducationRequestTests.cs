using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.Requests.Education.Core.BusinessRules;
using Modsen.Portal.TeamBoard.Requests.Education.Core.Calculators;
using Modsen.Portal.TeamBoard.Requests.Education.Requests;
using Modsen.Portal.TeamBoard.Requests.Education.Requests.RequestDto;
using Moq;

namespace Modsen.Portal.TeamBoard.UnitTests.Education
{
    public class CreateUserEducationRequestTests
    {
        private readonly CreateUserEducationRequest _handler;
        private readonly Mock<ICanUpdateUserEducationBusinessRule> _canCreateEducationBusinessRuleMock = new();
        private readonly Mock<IAuthenticatedUserContext> _authenticatedUserContextMock = new();
        private readonly Mock<ITeamBoardDbContext> _teamBoardDbContextMock = new();
        private readonly Mock<IEducationLevelOrderWeightCalculator> _educationLevelOrderWeightCalculatorMock = new();
        private readonly Mock<IStudyFormOrderWeightCalculator> _studyFormOrderWeightCalculatorMock = new();
        private readonly CancellationToken _cancellationToken = CancellationToken.None;

        public CreateUserEducationRequestTests()
        {
            _handler = new CreateUserEducationRequest(_canCreateEducationBusinessRuleMock.Object,
                _teamBoardDbContextMock.Object, _authenticatedUserContextMock.Object,
                _educationLevelOrderWeightCalculatorMock.Object, _studyFormOrderWeightCalculatorMock.Object);
        }

        [Fact]
        public async Task HandleWhenUserUpdatesThemselfThrowActionNotAllowedException()
        {
            // Arrange
            var initiatotId = 1;
            _authenticatedUserContextMock
                .Setup(x => x.GetRequiredUserId())
                .Returns(initiatotId);

            var dto = new CreateUserEducationDto {
                UserId = initiatotId,
                EducationalInstitution = "EducationalInstitution",
                Faculty = "Faculty",
                Specialization = "Specialization"
            };

            var command = new CreateUserEducationRequest.Command(dto);

            // Act
            var act = () => _handler.Handle(command, _cancellationToken);

            // Assert
            await act.Should().ThrowAsync<ActionNotAllowedException>("You are not allowed to create your own education");
        }
    }
}
