using MockQueryable.Moq;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Cv;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Cv.DataAccess.Npgsql;
using Modsen.Portal.Cv.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.Cv.Requests.Cvs.Requests;

namespace Modsen.Portal.Cv.UnitTests.Cvs.Requests;

public class DeleteCvRequestTests
{
    private readonly Mock<ICvDbContext> _cvDbContextMock;
    private readonly Mock<IAuthenticatedUserContext> _authenticatedUserContextMock;
    private readonly DeleteCvRequest _handler;
    private readonly CancellationToken _cancellationToken;

    public DeleteCvRequestTests()
    {
        _cvDbContextMock = new Mock<ICvDbContext>();
        _authenticatedUserContextMock = new Mock<IAuthenticatedUserContext>();
        _cancellationToken = CancellationToken.None;

        _handler = new DeleteCvRequest(
            _cvDbContextMock.Object,
            _authenticatedUserContextMock.Object);
    }

    [Fact]
    public async Task Handle_WhenValidData_DeleteCv()
    {
        var cvId = 1;

        // Arrange
        var command = new DeleteCvRequest.Command(cvId);
        var cvEntity = new CvEntity {Id = cvId, Status = CvStatus.Draft, UserFirstName = "test", UserLastName = "test", CvName = "test" };

        _authenticatedUserContextMock
            .Setup(x => x.GetRoles())
            .Returns([PortalRoleNames.Ceo]);

        _cvDbContextMock
            .Setup(x => x.Cvs)
            .Returns(new List<CvEntity> { cvEntity }.BuildMockDbSet().Object);

        // Act
        await _handler.Handle(command, _cancellationToken);

        // Assert
        _cvDbContextMock.Verify(x => x.SaveChangesAsync(_cancellationToken), Times.Once());
        cvEntity.IsDeleted.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_WhenCvNotFound_ThrowEntityNotFoundException()
    {
        var cvId = 1;

        // Arrange
        var command = new DeleteCvRequest.Command(cvId);

        _cvDbContextMock
            .Setup(x => x.Cvs)
            .Returns(new List<CvEntity>().BuildMockDbSet().Object);

        // Act
        var result = () => _handler.Handle(
            command,
            _cancellationToken);

        // Assert
        await result.Should()
            .ThrowAsync<EntityNotFoundException>()
            .WithMessage(PortalMessages.CvNotFound);
    }

    [Theory]
    [InlineData(PortalRoleNames.Employee, 1, "")]
    [InlineData(PortalRoleNames.SystemAdministrator, 1, "")]
    [InlineData(PortalRoleNames.GroupManager, 1, "")]
    [InlineData(PortalRoleNames.HeadOfDepartment, 1, ".Net")]
    public async Task Handle_WhenUserDoesNotHaveAccess_ThrowActionNotAllowedException(
        string role, int groupId, string departmentId)
    {
        var cvId = 1;

        // Arrange
        var command = new DeleteCvRequest.Command(cvId);
        var cvEntity = new CvEntity { Id = cvId,
            UserFirstName = "test",
            UserLastName = "test",
            CvName = "test"
        };

        _authenticatedUserContextMock
            .Setup(x => x.GetRoles())
            .Returns([role]);
        _authenticatedUserContextMock
            .Setup(x => x.ResourceGroupId)
            .Returns(groupId);
        _authenticatedUserContextMock
            .Setup(x => x.DepartmentId)
            .Returns(departmentId);

        _cvDbContextMock
            .Setup(x => x.Cvs)
            .Returns(new List<CvEntity> { cvEntity }.BuildMockDbSet().Object);

        // Act
        var result = () => _handler.Handle(
            command,
            _cancellationToken);

        // Assert
        await result.Should()
            .ThrowAsync<ActionNotAllowedException>();
    }
}
