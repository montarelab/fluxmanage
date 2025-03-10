using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace Infrastructure.Swagger;

public static class DependencyInjection
{
    public static IServiceCollection AddSwagger(this IServiceCollection services, string swaggerTitle)
    {
        return services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = swaggerTitle, Version = "v1" });
            c.OperationFilter<RouteParametersOperationFilter>();
        });
    }
    
    public static IApplicationBuilder UseSwaggerUtils(this IApplicationBuilder app)
    {
        return app
            .UseSwagger()
            .UseSwaggerUI();
    }
}