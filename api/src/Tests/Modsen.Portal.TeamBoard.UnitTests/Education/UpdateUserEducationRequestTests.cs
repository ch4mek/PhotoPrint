using MockQueryable.Moq;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TeamBoard.Requests.Education.Core.BusinessRules;
using Modsen.Portal.TeamBoard.Requests.Education.Core.Calculators;
using Modsen.Portal.TeamBoard.Requests.Education.Requests;
using Modsen.Portal.TeamBoard.Requests.Education.Requests.RequestDto;
using Moq;

namespace Modsen.Portal.TeamBoard.UnitTests.Education
{
    public class UpdateUserEducationRequestTests
    {
        private readonly UpdateUserEducationRequest _handler;
        private readonly Mock<ICanUpdateUserEducationBusinessRule> _canUpdateEducationBusinessRuleMock = new();
        private readonly Mock<IAuthenticatedUserContext> _authenticatedUserContextMock = new();
        private readonly Mock<ITeamBoardDbContext> _teamBoardDbContextMock = new();
        private readonly Mock<IEducationLevelOrderWeightCalculator> _educationLevelOrderWeightCalculatorMock = new();
        private readonly Mock<IStudyFormOrderWeightCalculator> _studyFormOrderWeightCalculatorMock = new();
        private readonly CancellationToken _cancellationToken = CancellationToken.None;

        public UpdateUserEducationRequestTests()
        {
            _handler = new UpdateUserEducationRequest(_teamBoardDbContextMock.Object, _canUpdateEducationBusinessRuleMock.Object,
                _authenticatedUserContextMock.Object, _educationLevelOrderWeightCalculatorMock.Object,
                _studyFormOrderWeightCalculatorMock.Object);
        }

        [Fact]
        public async Task HandleWhenUserUpdatesThemselfThrowActionNotAllowedException()
        {
            // Arrange
            var initiatorId = 1;
            var education = new UserEducationEntity {
                Id = 1,
                UserId = initiatorId,
                EducationalInstitution = "EducationalInstitution",
                Faculty = "Faculty",
                Specialization = "Specialization"
            };

            _authenticatedUserContextMock
                .Setup(x => x.GetRequiredUserId())
                .Returns(initiatorId);

            _teamBoardDbContextMock.SetupGet(x => x.UserEducations)
            .Returns(new[] { education }.BuildMockDbSet().Object);

            var command = new UpdateUserEducationRequest.Command(It.IsAny<UpdateUserEducationDto>(), education.Id);

            // Act
            var act = () => _handler.Handle(command, _cancellationToken);

            // Assert
            await act.Should().ThrowAsync<ActionNotAllowedException>("You are not allowed to create your own education");
        }
    }
}
