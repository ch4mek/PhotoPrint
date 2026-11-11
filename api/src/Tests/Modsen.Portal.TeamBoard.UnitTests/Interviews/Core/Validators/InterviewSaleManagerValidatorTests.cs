using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TeamBoard.Requests.Interviews.Core.Validators;
using Modsen.Portal.TeamBoard.Requests.TeamBoardPortalSettings;
using Moq;

namespace Modsen.Portal.TeamBoard.UnitTests.Interviews.Core.Validators
{
    public class InterviewSaleManagerValidatorTests
    {
        private readonly Mock<IAuthenticatedUserContext> _authContextMock;
        private readonly Mock<ITeamBoardPortalSettingsAccessor> _teamBoardPortalSettingsAccessorMock;
        private readonly InterviewSaleManagerValidator _validator;

        public InterviewSaleManagerValidatorTests()
        {
            _authContextMock = new Mock<IAuthenticatedUserContext>();
            _teamBoardPortalSettingsAccessorMock = new Mock<ITeamBoardPortalSettingsAccessor>();
            _validator = new InterviewSaleManagerValidator(_authContextMock.Object, _teamBoardPortalSettingsAccessorMock.Object);
        }

        [Fact]
        public void ValidateShouldThrowArgumentNullExceptionWhenModelIsNull()
        {
            // Arrange
            InterviewSaleManagerValidator.Request? model = null;

            // Act
            var act = () => _validator.Validate(model, It.IsAny<CancellationToken>());

            // Assert
            act.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public void ValidateShouldThrowEntityNotFoundExceptionWhenSaleManagerIsNull()
        {
            // Arrange
            var model = new InterviewSaleManagerValidator.Request
            (
                SaleManagerId: 1,
                SaleManager: null
            );

            // Act
            var act = () => _validator.Validate(model, It.IsAny<CancellationToken>());

            // Assert
            act.Should().ThrowAsync<EntityNotFoundException>().WithMessage("Sale manager not found");
        }

        [Fact]
        public void ValidateShouldThrowInvalidDataAppExceptionWhenSaleManagerIsNotInSalesDepartment()
        {
            // Arrange
            var model = new InterviewSaleManagerValidator.Request
            (
                SaleManagerId: 1,
                SaleManager: new TeamBoardUser
                {
                    ResourceGroupId = 1,
                    DepartmentId = "Net",
                    FirstName = "Test",
                    LastName = "Test",
                    Email = "Test",
                    IsActive = true,
                }
            );

            // Act
            var act = () => _validator.Validate(model, It.IsAny<CancellationToken>());

            // Assert
            act.Should().ThrowAsync<InvalidDataAppException>().WithMessage("User is not Sale manager");
        }

        [Fact]
        public void ValidateShouldThrowInvalidDataAppExceptionWhenSalesEmployeeAssigningInterviewToAnotherSalesEmployee()
        {
            // Arrange
            var model = new InterviewSaleManagerValidator.Request
            (
                SaleManagerId: 1,
                SaleManager: new TeamBoardUser
                {
                    ResourceGroupId = 1,
                    DepartmentId = "Sales",
                    FirstName = "Test",
                    LastName = "Test",
                    Email = "Test",
                    IsActive = true,
                }
            );

            _authContextMock.Setup(x => x.IsManager).Returns(false);
            _authContextMock.Setup(x => x.DepartmentId).Returns("Sales");
            _authContextMock.Setup(x => x.GetRequiredUserId()).Returns((int)model.SaleManagerId!);

            // Act
            var act = () => _validator.Validate(model, It.IsAny<CancellationToken>());

            // Assert
            act.Should().ThrowAsync<InvalidDataAppException>()
                .WithMessage("Sales employees can only assign interviews to themselves and not to other Sale employees");
        }

        [Fact]
        public void ValidateShouldThrowInvalidDataAppExceptionWhenUserIsCeoAndAllowCeoCtoFalse()
        {
            // Arrange
            var model = new InterviewSaleManagerValidator.Request
            (
                SaleManagerId: 1,
                SaleManager: new TeamBoardUser
                {
                    ResourceGroupId = 1,
                    DepartmentId = "Sales",
                    FirstName = "Test",
                    LastName = "Test",
                    Email = "Test",
                    IsActive = true,
                    Position = "CEO"
                }
            );

            _authContextMock.Setup(x => x.IsManager).Returns(false);
            _authContextMock.Setup(x => x.GetRequiredUserId()).Returns((int)model.SaleManagerId!);

            // Act
            var act = () => _validator.Validate(model, It.IsAny<CancellationToken>());

            // Assert
            act.Should().ThrowAsync<InvalidDataAppException>()
                .WithMessage("User is not Sale manager");
        }

        [Fact]
        public void ValidateShouldNotThrowWhenUserIsCeoAndAllowCeoCtoTrue()
        {
            // Arrange
            var model = new InterviewSaleManagerValidator.Request
            (
                SaleManagerId: 1,
                SaleManager: new TeamBoardUser
                {
                    ResourceGroupId = 1,
                    DepartmentId = "Any",
                    FirstName = "Test",
                    LastName = "Test",
                    Email = "Test",
                    IsActive = true,
                    Position = "CEO"
                }
            );

            _authContextMock.Setup(x => x.IsManager).Returns(false);

            // Act
            var act = () => _validator.Validate(model, It.IsAny<CancellationToken>(), true);

            // Assert
            act.Should().NotThrowAsync();
        }

        [Fact]
        public void ValidateShouldNotThrowWhenUserIsCtoAndAllowCeoCtoTrue()
        {
            // Arrange
            var model = new InterviewSaleManagerValidator.Request
            (
                SaleManagerId: 1,
                SaleManager: new TeamBoardUser
                {
                    ResourceGroupId = 1,
                    DepartmentId = "Any",
                    FirstName = "Test",
                    LastName = "Test",
                    Email = "Test",
                    IsActive = true,
                    Position = "CTO"
                }
            );

            _authContextMock.Setup(x => x.IsManager).Returns(false);

            // Act
            var act = () => _validator.Validate(model, It.IsAny<CancellationToken>(), true);

            // Assert
            act.Should().NotThrowAsync();
        }

        [Fact]
        public void ValidateShouldThrowInvalidDataAppExceptionWhenUserIsNotCeoCtoSalesManagerAndAllowCeoCtoTrue()
        {
            // Arrange
            var model = new InterviewSaleManagerValidator.Request
            (
                SaleManagerId: 1,
                SaleManager: new TeamBoardUser
                {
                    ResourceGroupId = 1,
                    DepartmentId = "Any",
                    FirstName = "Test",
                    LastName = "Test",
                    Email = "Test",
                    IsActive = true,
                }
            );

            _authContextMock.Setup(x => x.IsManager).Returns(false);
            _authContextMock.Setup(x => x.GetRequiredUserId()).Returns((int)model.SaleManagerId!);

            // Act
            var act = () => _validator.Validate(model, It.IsAny<CancellationToken>(), true);

            // Assert
            act.Should().ThrowAsync<InvalidDataAppException>()
                .WithMessage("User is not Sale manager, CEO or CTO");
        }
    }
}
