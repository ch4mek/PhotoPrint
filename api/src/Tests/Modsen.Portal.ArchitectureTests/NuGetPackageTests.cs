using System.Reflection;
using Modsen.Portal.Core.Utils.Extensions;

namespace Modsen.Portal.ArchitectureTests;

public class NuGetPackageTests
{
    [Fact]
    public void StimulsoftShouldBeSpecificVersion()
    {
        // Arrange
        var types = Assembly
            .GetExecutingAssembly()
            .GetReferencedAssemblies("Stimulsoft.Report.dll")
            .ToArray();

        // Act
        var reportDlls = types.Where(x => x.FullName?.Contains("Version=2025.1.2") == true).ToArray();

        // Assert
        reportDlls.Length.Should().Be(types.Length, "Stimulsoft should be v.2025.1.2 - paid licence");
    }

    [Fact]
    public void MassTransitShouldBeSpecificVersion()
    {
        // Arrange
        var types = Assembly
            .GetExecutingAssembly()
            .GetReferencedAssemblies("MassTransit*.dll")
            .ToArray();

        // Act
        var reportDlls = types.Where(x => x.FullName?.Contains("Version=8.") == true).ToArray();

        // Assert
        reportDlls.Length.Should().Be(types.Length, "MassTransit should be v.8.x - free licence");
    }

    [Fact]
    public void MediatRShouldBeSpecificVersion()
    {
        // Arrange
        var types = Assembly
            .GetExecutingAssembly()
            .GetReferencedAssemblies("MediatR.dll")
            .ToArray();

        // Act
        var reportDlls = types.Where(x => x.FullName?.Contains("Version=12.") == true).ToArray();

        // Assert
        reportDlls.Length.Should().Be(types.Length, "MediatR should be v.12.x - free licence");
    }
}
