using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;

namespace Modsen.Portal.Core.OData;

public static class EdmModelBuilder
{
    public static IEdmModel BuildWithoutExtraConfiguration<T>()
        where T : class
    {
        var builder = new ODataConventionModelBuilder();
        builder.EntitySet<T>(typeof(T).Name);

        return builder.GetEdmModel();
    }
}
