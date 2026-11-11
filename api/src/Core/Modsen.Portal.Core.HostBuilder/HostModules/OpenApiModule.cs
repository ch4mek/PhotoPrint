using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;
using Microsoft.OData;
using Modsen.Portal.Core.Utils.Modules;
using NJsonSchema;
using NSwag;
using NSwag.Generation.Processors;
using NSwag.Generation.Processors.Contexts;
using NSwag.Generation.Processors.Security;

namespace Modsen.Portal.Core.HostBuilder.HostModules;

internal sealed class OpenApiModule : RegistrationModule
{
    private readonly string _swaggerTitle;
    private const string s_defaultAuthenticationScheme = "Bearer";

    public OpenApiModule(string swaggerTitle)
    {
        _swaggerTitle = swaggerTitle;
    }

    public override void Load(IServiceCollection services)
    {
        services.AddOpenApiDocument(config =>
        {
            config.DocumentName = "v1";
            config.Title = _swaggerTitle;
            config.Version = "v1";
            config.Description = "Documentation of API";

            config.AddSecurity("JWT", new OpenApiSecurityScheme
            {
                Type = OpenApiSecuritySchemeType.ApiKey,
                Name = HeaderNames.Authorization,
                In = OpenApiSecurityApiKeyLocation.Header,
                Scheme = s_defaultAuthenticationScheme,
                Description = "Please, insert JSON Web Token: `Bearer ABC123...`",
            });

            config.OperationProcessors.Add(new AspNetCoreOperationSecurityScopeProcessor("JWT"));

            config.SchemaSettings.FlattenInheritanceHierarchy = false;

            
            config.SchemaSettings.GenerateAbstractSchemas = true;
            config.SchemaSettings.GenerateKnownTypes = true;


            config.OperationProcessors.Add(new TagByAreaNameOperationFilter());
            config.OperationProcessors.Add(new CustomHeaderSwaggerAttribute());


            config.OperationProcessors.Add(new ODataQueryParametersFilter());
            config.OperationProcessors.Add(new ODataMetadataFilter());
        });
    }

    private sealed class ODataQueryParametersFilter : IOperationProcessor
    {
        public bool Process(OperationProcessorContext context)
        {
            var operation = context.OperationDescription.Operation;

            if (context.Parameters.Keys.All(parameter => parameter.ParameterType.Name != typeof(ODataQueryOptions<>).Name))
            {
                return true;
            }

            var odataParameters = new List<OpenApiParameter>
            {
                GenerateODataParameter(
                    "Top",
                    true),
                GenerateODataParameter(
                    "Skip",
                    true),
                GenerateODataParameter(
                    "Orderby",
                    false),
                GenerateODataParameter(
                    "Filter",
                    false)
            };

            odataParameters.ForEach(x => operation.Parameters.Add(x));

            var oDataQueryOptionsApiParameter = context.Parameters.Keys.First(x => x.ParameterType.Name == typeof(ODataQueryOptions<>).Name);
            var removeParameter = context.Parameters[oDataQueryOptionsApiParameter];
            operation.Parameters.Remove(removeParameter);

            static OpenApiParameter GenerateODataParameter(
                string name,
                bool required)
            {
                return new OpenApiParameter
                {
                    Kind = OpenApiParameterKind.Query,
                    IsRequired = required,
                    Name = name,
                    Schema = new JsonSchema
                    {
                        Type = JsonObjectType.String
                    }
                };
            }
            return true;
        }
    }

    private sealed class ODataMetadataFilter : IOperationProcessor
    {
        public bool Process(OperationProcessorContext context)
        {
            var operation = context.OperationDescription.Operation;
            if (context.Parameters.Keys.All(parameter => parameter.ParameterType != typeof(ODataServiceDocument)))
            {
                return true;
            }

            operation.Parameters.Clear();
            return true;
        }
    }

    private sealed class TagByAreaNameOperationFilter : IOperationProcessor
    {
        public bool Process(OperationProcessorContext context)
        {
            var operation = context.OperationDescription.Operation;

            if (context.ControllerType != null)
            {

                var areaName = context.ControllerType
                    .GetCustomAttributes(typeof(AreaAttribute), inherit: true)
                    .OfType<AreaAttribute>()
                    .FirstOrDefault();

                operation.Tags = areaName != null
                    ? [areaName.RouteValue]
                    : [context.ControllerType.Name.Replace("Controller", "", StringComparison.InvariantCulture)];
            }

            return true;
        }
    }

    private sealed class CustomHeaderSwaggerAttribute : IOperationProcessor
    {
        public bool Process(OperationProcessorContext context)
        {
            var operation = context.OperationDescription.Operation;

            if (operation.Tags != null && operation.Tags.Any(x => x.Contains("[Recruitment]", StringComparison.InvariantCulture)))
            {
                operation.Parameters.Add(new OpenApiParameter
                {
                    Name = "X-RECRUITMENT-SERVICE-KEY",
                    Kind = OpenApiParameterKind.Header,
                    IsRequired = false,
                    Schema = new JsonSchema
                    {
                        Type = JsonObjectType.String
                    }
                });
            }

            return true;
        }
    }
}
