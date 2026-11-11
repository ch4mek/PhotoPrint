using Modsen.Portal.Core;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TeamBoard.Requests.Interviews.Core.Validators;
using Modsen.Portal.TeamBoard.Requests.TeamBoardPortalSettings;
using Moq;

namespace Modsen.Portal.TeamBoard.UnitTests.Interviews.Core.Validators
{
    public class InterviewUsersValidatorTests
    {
        private readonly Mock<IAuthenticatedUserContext> _authContextMock;
        private readonly Mock<ITeamBoardPortalSettingsAccessor> _teamBoardPortalSettingsAccessor;
        private readonly InterviewUsersValidator _validator;

        public InterviewUsersValidatorTests()
        {
            _authContextMock = new Mock<IAuthenticatedUserContext>();
            _teamBoardPortalSettingsAccessor = new Mock<ITeamBoardPortalSettingsAccessor>();
            _validator = new InterviewUsersValidator(_authContextMock.Object, _teamBoardPortalSettingsAccessor.Object);
        }

        [Fact]
        public void ValidateShouldThrowArgumentNullExceptionWhenModelIsNull()
        {
            // Arrange
            InterviewUsersValidator.Request? model = null;

            // Act
            var act = () => _validator.Validate(model, It.IsAny<CancellationToken>());

            // Assert
            act.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public void ValidateShouldThrowEntityNotFoundExceptionWhenUserIsNull()
        {
            // Arrange
            var model = new InterviewUsersValidator.Request
            (
                User: null
            );

            // Act
            var act = () => _validator.Validate(model, It.IsAny<CancellationToken>());

            // Assert
            act.Should().ThrowAsync<EntityNotFoundException>().WithMessage(PortalMessages.UserNotFound);
        }

        [Fact]
        public void ValidateShouldNotThrowWhenUserHasManageAllInterviewsClaim()
        {
            // Arrange
            var model = new InterviewUsersValidator.Request
            (
                User: new TeamBoardUser
                {
                    ResourceGroupId = 1,
                    DepartmentId = "Net",
                    FirstName = "Test",
                    LastName = "Test",
                    Email = "Test",
                    IsActive = true,
                }
            );

            _authContextMock.Setup(x => x.HasClaim(StaffingClaimTypeNames.ManageAllInterviews.Name)).Returns(true);

            // Act
            var act = () => _validator.Validate(model, It.IsAny<CancellationToken>());

            // Assert
            act.Should().NotThrowAsync();
        }

        [Fact]
        public void ValidateShouldThrowActionNotAllowedExceptionWhenUserIsGroupManagerAndDifferentResourceGroup()
        {
            // Arrange
            var model = new InterviewUsersValidator.Request
            (
                User: new TeamBoardUser
                {
                    ResourceGroupId = 1,
                    DepartmentId = "Net",
                    FirstName = "Test",
                    LastName = "Test",
                    Email = "Test",
                    IsActive = true,
                }
            );

            _authContextMock.Setup(x => x.HasClaim(StaffingClaimTypeNames.ManageAllInterviews.Name)).Returns(false);
            _authContextMock.Setup(x => x.HasClaim(StaffingClaimTypeNames.ManageInterviews.Name)).Returns(true);
            _authContextMock.Setup(x => x.GetRoles()).Returns([PortalRoleNames.GroupManager]);
            _authContextMock.Setup(x => x.ResourceGroupId).Returns(2);

            // Act
            var act = () => _validator.Validate(model, It.IsAny<CancellationToken>());

            // Assert
            act.Should().ThrowAsync<ActionNotAllowedException>().WithMessage("You don't have access to user from another group");
        }

        [Fact]
        public void ValidateShouldThrowActionNotAllowedExceptionWhenUserIsHeadOfDepartmentAndDifferentDepartment()
        {
            // Arrange
            var model = new InterviewUsersValidator.Request
            (
                User: new TeamBoardUser
                {
                    ResourceGroupId = 1,
                    DepartmentId = "Net",
                    FirstName = "Test",
                    LastName = "Test",
                    Email = "Test",
                    IsActive = true,
                }
            );

            _authContextMock.Setup(x => x.HasClaim(StaffingClaimTypeNames.ManageAllInterviews.Name)).Returns(false);
            _authContextMock.Setup(x => x.HasClaim(StaffingClaimTypeNames.ManageInterviews.Name)).Returns(true);
            _authContextMock.Setup(x => x.GetRoles()).Returns([PortalRoleNames.HeadOfDepartment]);
            _authContextMock.Setup(x => x.DepartmentId).Returns("Java");

            // Act
            var act = () => _validator.Validate(model, It.IsAny<CancellationToken>());

            // Assert
            act.Should().ThrowAsync<ActionNotAllowedException>().WithMessage("You don't have access to user from another department");
        }
    }
}
