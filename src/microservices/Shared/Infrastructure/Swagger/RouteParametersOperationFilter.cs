using System.Text.RegularExpressions;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Infrastructure.Swagger;

public partial class RouteParametersOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        if (context.ApiDescription.RelativePath == null) return;
        
        operation.Parameters ??= new List<OpenApiParameter>();

        var parameterNames = RouteVariableRegex()
            .Matches(context.ApiDescription.RelativePath)
            .Select(x => x.Groups[1].Value)
            .Where(paramName => operation.Parameters.All(p => p.Name != paramName))
            .Select(paramName => paramName.Split(':')[0]);

        foreach (var paramName in parameterNames)
        {
            operation.Parameters.Add(new OpenApiParameter
            {
                Name = paramName, 
                In = ParameterLocation.Path,
                Required = true,
                Schema = new OpenApiSchema { Type = "string", Format = "uuid" }
            });
        }
    }

    [GeneratedRegex(@"\{([^}]+)\}")]
    private static partial Regex RouteVariableRegex();
}