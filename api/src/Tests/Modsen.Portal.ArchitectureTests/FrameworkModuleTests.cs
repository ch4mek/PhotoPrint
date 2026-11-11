using System.Reflection;
using Modsen.Portal.Core.Utils.Extensions;

namespace Modsen.Portal.ArchitectureTests;

public class FrameworkModuleTests
{
    [Fact]
    public void FrameworkProjectsShouldReferenceOnlyToPortalProjects()
    {
        var assemblies = Assembly.GetExecutingAssembly()
            .GetReferencedAssemblies("Modsen.Portal.Core*.dll")
            .ToList();

        foreach (var assembly in assemblies)
        {
            var solutionAssemblies = assembly.GetReferencedAssemblies()
                .Where(x => x.FullName.Contains("Modsen."))
                .Where(x => !x.FullName.Contains("Modsen.Portal.Core"))
                .ToList();

            foreach (var reference in solutionAssemblies)
            {
                var hasIncorrectReference = reference.FullName.Contains("Modsen.Portal") &&
                    !reference.FullName.Contains("Modsen.Portal.Core");

                hasIncorrectReference.Should().BeFalse($"avoid references from Framework to '{reference.FullName}'");
            }
        }
    }
}
