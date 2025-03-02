using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Infrastructure.Swagger;

public class DeleteQueryParameterOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        operation.Parameters.Add(new OpenApiParameter
        {
            Name = "id",
            In = ParameterLocation.Path,
            Required = false,
            Schema = new OpenApiSchema { Type = "string", Format = "uuid" }
        });
    }
}