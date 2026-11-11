using System.Reflection;
using Modsen.Portal.ArchitectureTests.Extensions;
using Modsen.Portal.Core.Utils.Extensions;

namespace Modsen.Portal.ArchitectureTests;

public class CrossModuleReferencesTest
{
    [Fact]
    public void CrossModuleReferences()
    {
        var modules = new List<string>
        {
            "Attendance",
            "Cv",
            "Equipment",
            "Finance",
            "Identity",
            "Inquiries",
            "KnowledgeBase",
            "Notification",
            "Storage",
            "TeamBoard",
            "TimeTrack",
            "Utilities"
        };

        var countOfModules = Assembly
            .GetExecutingAssembly()
            .GetReferencedAssemblies("Modsen.Portal.*Controller*.dll")
            .Count();

        modules.Count.Should().Be(countOfModules, because: "Not all modules specified in list");

        TestCrossReferences(modules);
    }

    [Fact]
    public void InfrastructureCrossModuleReferences()
    {
        var modules = new List<string>
        {
            // "Agent",
            "BcseApi",
            "Csv",
            "Excel",
            "Jira",
            "Migrator",
            "ModsenApi",
            "StimulsoftReport",
            "Authentication",
            "BackgroundJob",
            "CloudStorage",
            "PowerAutomate",
        };

        TestCrossReferences(modules);
    }

    private void TestCrossReferences(List<string> modules)
    {
        foreach (var fromModule in modules)
        {
            var fromAssemblies = Assembly.GetExecutingAssembly()
                .GetReferencedAssemblies($"Modsen*{fromModule}*.dll", x => x.ExcludeWhichNameEndsWith("Contract"))
                .ToList();

            foreach (var toModule in modules)
            {
                if (fromModule == toModule)
                {
                    continue;
                }

                var toAssemblies = Assembly.GetExecutingAssembly()
                    .GetReferencedAssemblies($"Modsen*{toModule}*.dll", x => x.ExcludeWhichNameEndsWith("Contract"))
                    .ToList();

                foreach (var assembly in fromAssemblies)
                {
                    var referencedAssemblies = assembly.GetReferencedAssemblies();

                    var forbiddenReference = GetForbiddenReference(fromModule, toModule, referencedAssemblies, toAssemblies);

                    forbiddenReference
                        .Should()
                        .BeNull($"avoid cross-module reference from '{assembly.FullName}' to '{forbiddenReference?.FullName}'");
                }
            }
        }
    }

    private AssemblyName? GetForbiddenReference(string fromModule, string toModule, AssemblyName[] referencedAssemblies, List<Assembly> toAssemblies)
    {
        var forbiddenReference = referencedAssemblies.FirstOrDefault(reference =>
            toAssemblies.Any(toAssembly => reference.FullName.Contains(toAssembly.GetName().FullName)));


        if (forbiddenReference is not null)
        {
            //Migrator uses Outbox db context
            if (fromModule == "Migrator" &&
                forbiddenReference.FullName.Contains("Modsen.Portal.Outbox"))
            {
                return null;
            }

        }

        return forbiddenReference;
    }
}
