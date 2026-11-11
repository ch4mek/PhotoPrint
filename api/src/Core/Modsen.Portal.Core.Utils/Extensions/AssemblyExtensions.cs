using System.Reflection;
using System.Runtime.Loader;

namespace Modsen.Portal.Core.Utils.Extensions;

public static class AssemblyExtensions
{
    public static IEnumerable<Assembly> GetReferencedAssemblies(
        this Assembly assembly,
        string searchPattern,
        Func<IEnumerable<Assembly>, IEnumerable<Assembly>>? filter = null)
    {
        ArgumentNullException.ThrowIfNull(assembly);

        var assemblies = Directory
            .EnumerateFiles(Path.GetDirectoryName(assembly.Location)!, searchPattern)
            .Select(AssemblyLoadContext.Default.LoadFromAssemblyPath);

        return filter == null
            ? assemblies
            : filter(assemblies);
    }
}
