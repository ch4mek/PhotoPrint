using System.Text;
using MockQueryable.Moq;
using Modsen.Portal.Core;
using Modsen.Portal.Core.TimeTrack;
using Modsen.Portal.Identity.Contract;
using Modsen.Portal.Identity.Contract.Models;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TimeTrack.UseCases.Reports.Requests;
using Modsen.Portal.TimeTrack.UseCases.Services;
using Modsen.Portal.TimeTrack.UseCases.Services.Contracts;

namespace Modsen.Portal.TimeTrack.UnitTests.Composers;

public class ReportNameComposerTests
{
    private readonly Mock<ITimeTrackDbContext> _dbContextMock = new();
    private readonly CancellationToken _cancellationToken = CancellationToken.None;
    private readonly IReportNameComposer _reportNameComposer;
    private readonly Mock<IIdentityContract> _identityContract = new();

    public ReportNameComposerTests()
    {
        _reportNameComposer = new ReportNameComposer(_dbContextMock.Object, _identityContract.Object);
    }

    [Theory]
    [InlineData(DocumentType.Pdf)]
    [InlineData(DocumentType.Word)]
    [InlineData(DocumentType.Excel)]
    [InlineData(DocumentType.ExcelAccount)]
    public async Task ComposeForSingleFileAsyncWhenAllOptionsProvidedShouldReturnCorrectName(
        DocumentType documentType)
    {
        // Arrange
        var user = new TimeTrackUser
        {
            UserId = 1,
            FirstName = "UserFirstName",
            LastName = "UserLastName",
            Email = "test"
        };
        var userDbSetMock = new List<TimeTrackUser> { user }.BuildMockDbSet();

        var project = new Project
        {
            Id = 1,
            Name = "ProjectName",
        };
        var projectDbSetMock = new List<Project> { project }.BuildMockDbSet();

        var client = new Client
        {
            Id = 1,
            Name = "ClientName",
        };
        var clientDbSetMock = new List<Client> { client }.BuildMockDbSet();

        _dbContextMock.Setup(x => x.TimeTrackUsers)
            .Returns(userDbSetMock.Object);
        _dbContextMock.Setup(x => x.Projects)
            .Returns(projectDbSetMock.Object);
        _dbContextMock.Setup(x => x.Clients)
            .Returns(clientDbSetMock.Object);

        var year = 2025;
        int[] monthOfYear = [2, 7];
        var fileFormat = documentType.GetExtensionWithDot();

        var expectedFileName =
            $"{user.FirstName} {user.LastName}_{project.Name}_{client.Name}_Report_[01.02.2025-31.07.2025]{fileFormat}";

        // Act
        var fileName = await _reportNameComposer.ComposeForSingleFileAsync(
            user.UserId,
            project.Id,
            client.Id,
            year,
            monthOfYear,
            documentType,
            _cancellationToken);

        // Assert
        fileName.Should().Be(expectedFileName);
    }

    [Theory]
    [InlineData(DocumentType.Pdf)]
    [InlineData(DocumentType.Word)]
    [InlineData(DocumentType.Excel)]
    [InlineData(DocumentType.ExcelAccount)]
    public async Task ComposeForSingleFileAsyncWhenOptionIsNotProvidedShouldIgnoreThisOptions(
        DocumentType documentType)
    {
        // Arrange
        var userDbSetMock = new List<TimeTrackUser>().BuildMockDbSet();
        var projectDbSetMock = new List<Project>().BuildMockDbSet();
        var clientDbSetMock = new List<Client>().BuildMockDbSet();

        _dbContextMock.Setup(x => x.TimeTrackUsers)
            .Returns(userDbSetMock.Object);
        _dbContextMock.Setup(x => x.Projects)
            .Returns(projectDbSetMock.Object);
        _dbContextMock.Setup(x => x.Clients)
            .Returns(clientDbSetMock.Object);

        var fileFormat = documentType.GetExtensionWithDot();

        var expectedFileName = $"Report{fileFormat}";

        // Act
        var fileName = await _reportNameComposer.ComposeForSingleFileAsync(
            null,
            null,
            null,
            null,
            [],
            documentType,
            _cancellationToken);

        // Assert
        fileName.Should().Be(expectedFileName);
    }

    [Theory]
    [InlineData(DocumentType.Pdf)]
    [InlineData(DocumentType.Word)]
    [InlineData(DocumentType.Excel)]
    [InlineData(DocumentType.ExcelAccount)]
    public async Task ComposeForPerUserArchiveAsyncWhenAllOptionsProvidedShouldReturnCorrectName(
        DocumentType documentType)
    {
        // Arrange
        var project = new Project
        {
            Id = 1,
            Name = "ProjectName",
        };
        var projectDbSetMock = new List<Project> { project }
            .BuildMockDbSet();

        var client = new Client
        {
            Id = 1,
            Name = "ClientName",
        };
        var clientDbSetMock = new List<Client> { client }.BuildMockDbSet();

        var departmentId = "departmentId";
        var department = new DepartmentWithNameOutput(departmentId, "Department", "Department");

        var groupsSetMock = new List<TimeTrackResourceGroup>() {
            new TimeTrackResourceGroup {  ResourceGroupId = 1, ResourceGroupName =  "GroupName", IsActive = true }
        }.BuildMockDbSet();

        var group = groupsSetMock.Object.First();
        _dbContextMock.Setup(x => x.Projects)
            .Returns(projectDbSetMock.Object);
        _dbContextMock.Setup(x => x.Clients)
            .Returns(clientDbSetMock.Object);

        _dbContextMock.Setup(x => x.TimeTrackResourceGroups)
            .Returns(groupsSetMock.Object);

        _identityContract.Setup(x => x.GetDepartmentNames(_cancellationToken))
            .ReturnsAsync([department]);

        var year = 2025;
        int[] monthOfYear = [2, 7];
        var fileFormat = documentType.GetExtensionWithDot();

        var expectedFileName =
            $"{project.Name}_{client.Name}_{department.FullName}_{group.ResourceGroupName}_Report_[01.02.2025-31.07.2025]{fileFormat}.zip";

        var command = new ProcessReportRequest.Command(
            year,
            monthOfYear,
            null,
            departmentId,
            client.Id,
            null,
            project.Id,
            group.ResourceGroupId,
            TrackedTimeType.Billable,
            documentType,
            true);

        // Act
        var fileName = await _reportNameComposer.ComposeForPerUserArchiveAsync(command, _cancellationToken);

        // Assert
        fileName.Should().Be(expectedFileName);
    }

    [Theory]
    [InlineData(DocumentType.Pdf)]
    [InlineData(DocumentType.Word)]
    [InlineData(DocumentType.Excel)]
    [InlineData(DocumentType.ExcelAccount)]
    public async Task ComposeForPerUserArchiveAsyncWhenSomeOptionsAreNotProvededShouldReturnShouldIgnoreThisOptions(
        DocumentType documentType)
    {
        // Arrange
        var projectDbSetMock = new List<Project>().BuildMockDbSet();

        var clientDbSetMock = new List<Client>().BuildMockDbSet();

        var groupsSetMock = new List<TimeTrackResourceGroup>() {
                new TimeTrackResourceGroup {  ResourceGroupId = 1, ResourceGroupName =  "GroupName", IsActive = true }
            }.BuildMockDbSet();

        _dbContextMock.Setup(x => x.Projects)
            .Returns(projectDbSetMock.Object);
        _dbContextMock.Setup(x => x.Clients)
            .Returns(clientDbSetMock.Object);

        _dbContextMock.Setup(x => x.TimeTrackResourceGroups)
            .Returns(groupsSetMock.Object);


        var fileFormat = documentType.GetExtensionWithDot();

        var expectedFileName = $"Report{fileFormat}.zip";

        var command = new ProcessReportRequest.Command(
            null,
            [],
            null,
            null,
            null,
            null,
            null,
            null,
            TrackedTimeType.Billable,
            documentType,
            true);

        // Act
        var fileName = await _reportNameComposer.ComposeForPerUserArchiveAsync(command, _cancellationToken);

        // Assert
        fileName.Should().Be(expectedFileName);
    }

    [Theory]
    [InlineData(DocumentType.Word)]
    [InlineData(DocumentType.Excel)]
    [InlineData(DocumentType.ExcelAccount)]
    public async Task ComposeForSingleFileAsyncWhenOptionsHaveLongNamesShouldReturnTrimmedFilename(
        DocumentType documentType)
    {
        // Arrange
        var stringBuilder = new StringBuilder();

        for (var i = 1; i <= 23; i++)
        {
            stringBuilder.Append("Test");
        }

        var userName = stringBuilder.ToString();

        var user = new TimeTrackUser
        {
            UserId = 1,
            FirstName = userName,
            LastName = "ShouldBeTrimmed",
            Email = "test"
        };
        var userDbSetMock = new List<TimeTrackUser> { user }.BuildMockDbSet();
        var projectDbSetMock = new List<Project>().BuildMockDbSet();
        var clientDbSetMock = new List<Client>().BuildMockDbSet();

        _dbContextMock.Setup(x => x.TimeTrackUsers)
            .Returns(userDbSetMock.Object);
        _dbContextMock.Setup(x => x.Projects)
            .Returns(projectDbSetMock.Object);
        _dbContextMock.Setup(x => x.Clients)
            .Returns(clientDbSetMock.Object);

        var year = 2025;
        int[] monthOfYear = [2, 7];
        var fileFormat = documentType.GetExtensionWithDot();

        var expectedFileName =
            $"{user.FirstName}_Report_[01.02.2025-31.07.2025]{fileFormat}";

        // Act
        var fileName = await _reportNameComposer.ComposeForSingleFileAsync(
            user.UserId,
            null,
            null,
            year,
            monthOfYear,
            documentType,
            _cancellationToken);

        // Assert
        fileName.Should().Be(expectedFileName);
    }

    [Theory]
    [InlineData(DocumentType.Pdf)]
    [InlineData(DocumentType.Word)]
    [InlineData(DocumentType.Excel)]
    [InlineData(DocumentType.ExcelAccount)]
    public async Task ComposeForPerUserArchiveAsyncWhenGroupIdEqualZeroShouldReturnValidFilename(
        DocumentType documentType)
    {
        // Arrange
        var resourceGroupId = 0;
        var projectDbSetMock = new List<Project>().BuildMockDbSet();
        var clientDbSetMock = new List<Client>().BuildMockDbSet();        

        _dbContextMock.Setup(x => x.Projects)
            .Returns(projectDbSetMock.Object);
        _dbContextMock.Setup(x => x.Clients)
            .Returns(clientDbSetMock.Object);

        var fileFormat = documentType.GetExtensionWithDot();

        var expectedFileName = $"Report{fileFormat}.zip";

        var command = new ProcessReportRequest.Command(
            null,
            [],
            null,
            null,
            null,
            null,
            null,
            resourceGroupId,
            TrackedTimeType.Billable,
            documentType,
            true);

        // Act
        var fileName = await _reportNameComposer.ComposeForPerUserArchiveAsync(command, _cancellationToken);

        // Assert
        fileName.Should().Be(expectedFileName);
    }
}
