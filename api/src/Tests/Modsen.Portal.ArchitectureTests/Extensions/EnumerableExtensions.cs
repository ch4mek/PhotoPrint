using System.Reflection;

namespace Modsen.Portal.ArchitectureTests.Extensions;

internal static class EnumerableExtensions
{
    public static IEnumerable<Assembly> ExcludeWhichNameEndsWith(
        this IEnumerable<Assembly> source,
        string ending)
    {
        return source.Where(asm => !asm.GetName().Name?.EndsWith(ending, StringComparison.OrdinalIgnoreCase) == true);
    }

    public static string[] GetNames(this IEnumerable<Assembly> source)
    {
        return source.Select(asm => asm.GetName().Name).ToArray()!;
    }
}
