using MockQueryable.Moq;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Cv;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Cv.DataAccess.Npgsql;
using Modsen.Portal.Cv.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.Cv.Requests.Cvs.Core.CoreDto;
using Modsen.Portal.Cv.Requests.Cvs.Requests;

namespace Modsen.Portal.Cv.UnitTests.Cvs.Requests;

public class ChangeCvStatusRequestTests
{
    private readonly Mock<ICvDbContext> _cvDbContextMock;
    private readonly Mock<IAuthenticatedUserContext> _authenticatedUserContextMock;
    private readonly ChangeCvStatusRequest _handler;
    private readonly CancellationToken _cancellationToken;

    public ChangeCvStatusRequestTests()
    {
        _cvDbContextMock = new Mock<ICvDbContext>();
        _authenticatedUserContextMock = new Mock<IAuthenticatedUserContext>();
        _cancellationToken = CancellationToken.None;

        _handler = new ChangeCvStatusRequest(
            _cvDbContextMock.Object,
            _authenticatedUserContextMock.Object);
    }

    [Fact]
    public async Task HandleWhenValidDataReturnsCvCoreDtoWithNewStatus()
    {
        var cvId = 1;
        var clientId = "1";
        var projectId = "1";

        // Arrange
        var command = new ChangeCvStatusRequest.Command
            (cvId,
            clientId,
            projectId,
            ClientName: null,
            ProjectName: null,
            CvStatus.Sent);

        var cvEntity = new CvEntity { Id = cvId, Status = CvStatus.Draft,
            UserFirstName = "test",
            UserLastName = "test",
            CvName = "test"
        };
        var cvProjectEntity = new CvProjectEntity { ProjectId = projectId, Name = "Project name" };
        var cvClientEntity = new CvClientEntity { ClientId = clientId, Name = "Client name" };

        var cv = new CvCoreDto
        {
            Id = command.Id,
            ClientId = command.ClientId,
            ProjectId = command.ProjectId,
            Status = CvStatus.Sent,
            UserFirstName = "test",
            UserLastName = "test",
            CvName = "test"
        };

        _authenticatedUserContextMock
            .Setup(x => x.GetRoles())
            .Returns([PortalRoleNames.Ceo]);

        _cvDbContextMock
            .Setup(x => x.Cvs)
            .Returns(new List<CvEntity>{ cvEntity }.BuildMockDbSet().Object);
        _cvDbContextMock
            .Setup(x => x.CvProjects)
            .Returns(new List<CvProjectEntity> { cvProjectEntity }.BuildMockDbSet().Object);
        _cvDbContextMock
            .Setup(x => x.CvClients)
            .Returns(new List<CvClientEntity> { cvClientEntity }.BuildMockDbSet().Object);

        // Act
        var result = await _handler.Handle(command, _cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(cv);

        _cvDbContextMock.Verify(x => x.SaveChangesAsync(_cancellationToken), Times.Once());
    }

    [Fact]
    public async Task HandleWhenCvNotFoundThrowEntityNotFoundException()
    {
        var cvId = 1;
        var clientId = "1";
        var projectId = "1";

        // Arrange
        var command = new ChangeCvStatusRequest.Command
            (cvId,
            clientId,
            projectId,
            ClientName: null,
            ProjectName: null,
            CvStatus.Draft);

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
    public async Task HandleWhenUserDoesNotHaveAccessThrowActionNotAllowedException(
        string role, int groupId, string departmentId)
    {
        var cvId = 1;
        var clientId = "1";
        var projectId = "1";
        var userId = 1;

        // Arrange
        var command = new ChangeCvStatusRequest.Command
            (cvId,
            clientId,
            projectId,
            ClientName: null,
            ProjectName: null,
            CvStatus.Draft);

        var cvUser = new CvUserEntity
        {
            UserId = userId,
            ResourceGroupId = groupId,
            DepartmentId = departmentId,
            FirstName = "test",
            LastName = "test",
            Email = "test@modsen-software.com"
        };
        var cvEntity = new CvEntity { Id = cvId, Status = CvStatus.Draft,
            UserFirstName = "test",
            UserLastName = "test",
            CvName = "test" };

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

    [Fact]
    public async Task HandleWhenCvSentThrowInvalidDataAppException()
    {
        var cvId = 1;
        var clientId = "1";
        var projectId = "1";

        // Arrange
        var command = new ChangeCvStatusRequest.Command
            (cvId,
            clientId,
            projectId,
            ClientName: null,
            ProjectName: null,
            CvStatus.Draft);

        var cvEntity = new CvEntity { Id = cvId, Status = CvStatus.Sent,
            UserFirstName = "test",
            UserLastName = "test",
            CvName = "test"
        };

        _authenticatedUserContextMock
            .Setup(x => x.GetRoles())
            .Returns([PortalRoleNames.Ceo]);

        _cvDbContextMock
            .Setup(x => x.Cvs)
            .Returns(new List<CvEntity> { cvEntity }.BuildMockDbSet().Object);

        // Act
        var result = () => _handler.Handle(
            command,
            _cancellationToken);

        // Assert
        await result.Should()
            .ThrowAsync<InvalidDataAppException>();
    }
}
