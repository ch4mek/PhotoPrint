#pragma warning disable S1075

using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.OData;
using Microsoft.AspNetCore.OData.Extensions;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData.ModelBuilder;
using Modsen.Portal.Core.Utils.Extensions;

[assembly: InternalsVisibleTo("Modsen.Portal.Equipment.UnitTests")]
[assembly: InternalsVisibleTo("Modsen.Portal.Identity.UnitTests")]
[assembly: InternalsVisibleTo("Modsen.Portal.TimeTrack.UnitTests")]
[assembly: InternalsVisibleTo("Modsen.Portal.TeamBoard.UnitTests")]
[assembly: InternalsVisibleTo("Modsen.Portal.Notification.UnitTests")]
[assembly: InternalsVisibleTo("Modsen.Portal.Certificate.UnitTests")]
[assembly: InternalsVisibleTo("Modsen.Portal.Inquiries.UnitTests")]
[assembly: InternalsVisibleTo("Modsen.Portal.Cv.UnitTests")]
[assembly: InternalsVisibleTo("Modsen.Portal.Merch.UnitTests")]
[assembly: InternalsVisibleTo("Modsen.Portal.Attendance.UnitTests")]
namespace Modsen.Portal.Core.OData;

internal static class ODataQueryOptionsMock
{
    public static ODataQueryOptions<T> BuildMockWithKey<T>(
    Dictionary<string, string?> queryOptions,
    string keyPropertyName)
    where T : class
    {
        var edmModelBuilder = new ODataConventionModelBuilder();
        var entity = edmModelBuilder.EntitySet<T>(typeof(T).Name).EntityType;

        var keyProperty = typeof(T).GetProperty(keyPropertyName)
            ?? throw new ArgumentException($"Property '{keyPropertyName}' not found on type '{typeof(T).Name}'");

        var param = Expression.Parameter(typeof(T), "x");
        var body = Expression.Property(param, keyProperty);
        var lambdaType = typeof(Func<,>).MakeGenericType(typeof(T), keyProperty.PropertyType);
        var lambda = Expression.Lambda(lambdaType, body, param);

        var hasKeyMethod = typeof(EntityTypeConfiguration<T>)
            .GetMethods()
            .First(m => m.Name == "HasKey" && m.IsGenericMethod);

        var genericHasKey = hasKeyMethod.MakeGenericMethod(keyProperty.PropertyType);
        genericHasKey.Invoke(entity, [lambda]);

        var edmModel = edmModelBuilder.GetEdmModel();

        const string routeName = "odata";

        var request = CreateHttpRequest(
            CreateRequestUri(queryOptions),
            dataOptions => dataOptions.AddRouteComponents(routeName, edmModel));

        request.ODataFeature().Model = edmModel;
        request.ODataFeature().Path = [];
        request.ODataFeature().RoutePrefix = routeName;

        var context = new ODataQueryContext(edmModel, typeof(T), []);

        return new ODataQueryOptions<T>(context, request);
    }


    public static ODataQueryOptions<T> BuildMock<T>(Dictionary<string, string?> queryOptions)
        where T : class
    {
        var edmModelBuilder = new ODataConventionModelBuilder();
        edmModelBuilder.EntitySet<T>(typeof(T).Name);
        var edmModel = edmModelBuilder.GetEdmModel();

        const string routeName = "odata";

        var request = CreateHttpRequest(
            CreateRequestUri(queryOptions),
            dataOptions => dataOptions.AddRouteComponents(routeName, edmModel));

        request.ODataFeature().Model = edmModel;
        request.ODataFeature().Path = [];
        request.ODataFeature().RoutePrefix = routeName;

        var context = new ODataQueryContext(
            edmModel,
            typeof(T),
            []);

        return new ODataQueryOptions<T>(
            context,
            request);
    }

    private static Uri CreateRequestUri(Dictionary<string, string?> queryOptions)
    {
        ArgumentNullException.ThrowIfNull(queryOptions);

        var builder = new UriBuilder("https://localhost/api");
        var query = new QueryString();

        query = queryOptions
            .Where(queryOption => !queryOption.Value.IsNullOrEmpty())
            .Aggregate(query, (current, queryOption) => current.Add(queryOption.Key, queryOption.Value!));

        builder.Query = query.Value;

        return builder.Uri;
    }

    private static HttpRequest CreateHttpRequest(Uri requestUri, Action<ODataOptions> setupAction)
    {
        var context = new DefaultHttpContext();
        var request = context.Request;

        var services = new ServiceCollection();
        services.Configure(setupAction);
        context.RequestServices = services.BuildServiceProvider();

        request.Method = HttpMethods.Get;
        request.Scheme = requestUri.Scheme;
        request.Host = requestUri.IsDefaultPort
            ? new HostString(requestUri.Host)
            : new HostString(requestUri.Host, requestUri.Port);
        request.QueryString = new QueryString(requestUri.Query);
        request.Path = new PathString(requestUri.AbsolutePath);

        return request;
    }
}
