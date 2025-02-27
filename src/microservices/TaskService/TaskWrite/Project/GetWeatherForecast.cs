using FastEndpoints;

namespace TaskWrite.Project;

public class GetWeatherForecast
{
    public record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
    {
        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
    }
    
    public record Response(WeatherForecast[] Forecast);

    public class Endpoint : EndpointWithoutRequest<Response>
    {
        public override void Configure()
        {
            Get("/weatherforecast");
            AllowAnonymous();
        }

        public override Task HandleAsync(CancellationToken ct)
        {
            var summaries = new[]
            {
                "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
            };
            
            var forecasts =  Enumerable.Range(1, 5).Select(index =>
                    new WeatherForecast
                    (
                        DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                        Random.Shared.Next(-20, 55),
                        summaries[Random.Shared.Next(summaries.Length)]
                    ))
                .ToArray();
            return SendAsync(new Response(forecasts), cancellation: ct);
        }
    }
    
 
}