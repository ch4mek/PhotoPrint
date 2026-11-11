using NetArchTest.Rules;

namespace Modsen.Portal.ArchitectureTests.Extensions;

internal static class TestResultExtensions
{
    public static string GetFailureTypes(this TestResult testResult)
    {
        return string.Join("; ", testResult.FailingTypes.Select(r => r.ReflectionType.FullName));
    }
}
