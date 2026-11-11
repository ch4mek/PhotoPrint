using System.Reflection;
using Modsen.Portal.ArchitectureTests.Extensions;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Utils.Extensions;
using NetArchTest.Rules;

namespace Modsen.Portal.ArchitectureTests;

public class GeneralArchitectureTests
{
    [Fact]
    public void AllNamespacesShouldFollowAssemblyName()
    {
        var allAssemblies = Assembly.GetExecutingAssembly()
            .GetReferencedAssemblies("Modsen*.dll", x => x.ExcludeWhichNameEndsWith("Tests"));

        foreach (var assembly in allAssemblies)
        {
            var result = Types.InAssembly(assembly)
                .Should()
                .ResideInNamespace(assembly.GetName().Name)
                .GetResult();

            result.IsSuccessful
                .Should()
                .BeTrue($"""
                         All types should reside in namespace with assembly name.Types with errors :
                         {result.GetFailureTypes()}
                         """);
        }
    }

    [Fact]
    public void FrameworkAssembliesShouldNotDependOnAnyOtherAssemblies()
    {
        var frameworkAssemblies = Assembly.GetExecutingAssembly()
            .GetReferencedAssemblies("Modsen.Portal.Core*.dll", x => x.ExcludeWhichNameEndsWith("Tests"))
            .ToList();

        var otherAssemblies = Assembly.GetExecutingAssembly()
            .GetReferencedAssemblies("Modsen.*.dll", x => x.ExcludeWhichNameEndsWith("Tests"))
            .Except(frameworkAssemblies)
            .Except([ typeof(PortalClaimTypeNames).Assembly ]);

        var result = Types.InAssemblies(frameworkAssemblies)
            .ShouldNot()
            .HaveDependencyOnAny(otherAssemblies.GetNames())
            .GetResult();

        result.IsSuccessful
            .Should()
            .BeTrue($"""
                     Framework assemblies should not depend on any other assemblies.
                     Types with errors :
                     {result.GetFailureTypes()}
                     """);
    }
}
