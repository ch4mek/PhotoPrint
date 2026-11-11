using Modsen.Portal.Core;
using Modsen.Portal.TimeTrack.Domain;

namespace Modsen.Portal.TimeTrack.UnitTests.Domain;

public class ProjectTests
{
    [Fact]
    public void GetProjectTypeWhenProjectIsMarkedAsNotPublicAndBillableShouldReturnPrivate()
    {
        // Arrange
        var project = new Project
        {
            Id = 1,
            ClientId = 2,
            ManagerId = null,
            Color = "#000000",
            CreatedById = 5,
            IsActive = true,
            IsBillable = true,
            IsPublic = false,
            Name = "Test",
            ProjectUsers = [],
            TechStack = null,
            Industry = null,
            CloudServices = null,
            Architecture = null
        };

        // Act
        var result = project.GetProjectType();

        // Assert
        result.Should().Be(PortalProjectTypeNames.Private);
    }

    [Fact]
    public void GetProjectTypeWhenProjectIsMarkedAsNotPublicAndNotBillableShouldReturnPrivate()
    {
        // Arrange
        var project = new Project
        {
            Id = 1,
            ClientId = 2,
            ManagerId = null,
            Color = "#000000",
            CreatedById = 5,
            IsActive = true,
            IsBillable = false,
            IsPublic = false,
            Name = "Test",
            ProjectUsers = [],
            TechStack = null,
            Industry = null,
            CloudServices = null,
            Architecture = null
        };

        // Act
        var result = project.GetProjectType();

        // Assert
        result.Should().Be(PortalProjectTypeNames.Private);
    }

    [Fact]
    public void GetProjectTypeWhenProjectIsMarkedAsPublicShouldReturnPublic()
    {
        // Arrange
        var project = new Project
        {
            Id = 1,
            ClientId = 2,
            ManagerId = null,
            Color = "#000000",
            CreatedById = 5,
            IsActive = true,
            IsBillable = false,
            IsPublic = true,
            Name = "Test",
            ProjectUsers = [],
            TechStack = null,
            Industry = null,
            CloudServices = null,
            Architecture = null
        };

        // Act
        var result = project.GetProjectType();

        // Assert
        result.Should().Be(PortalProjectTypeNames.Public);
    }
}
